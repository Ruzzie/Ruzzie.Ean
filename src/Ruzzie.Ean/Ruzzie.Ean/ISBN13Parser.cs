using System;
using System.Collections.Generic;
using Ruzzie.Ean.ISBN;
using ParseResult = Ruzzie.Common.Types.Result<Ruzzie.Common.Types.Err<Ruzzie.Ean.ISBNParseErrorKind>, Ruzzie.Ean.ISBN13.Metadata>;
using ParseError = Ruzzie.Common.Types.Err<Ruzzie.Ean.ISBNParseErrorKind>;

namespace Ruzzie.Ean
{
    public class ISBN13Parser : IISBNParser
    {
        private readonly IReadOnlyDictionary<string, PrefixTree> _prefixTrees;

        public ISBN13Parser(ISBNRangeMessage rangeMessage)
        {
            _prefixTrees = PrefixTreeBuilder.BuildFromISBNRangeMessage(rangeMessage);
        }

        public ParseResult Parse(long input)
        {
            return Parse(input.ToString());
        }

        public ParseResult Parse(string originalInput)
        {
            if (string.IsNullOrWhiteSpace(originalInput))
            {
                return new ParseError(
                    $"Input {originalInput} is not a valid EAN13 number: {originalInput}",
                    ISBNParseErrorKind.InvalidEan);
            }

            if (originalInput.Length == 10)
            {
                //Try ISBN10
                var isbn10ParseResult = ISBN10.TryParse(originalInput, out var isbn10);
                if (isbn10ParseResult != ISBN10.ResultCode.Success)
                {
                    return new ParseError(
                        $"Input {originalInput} is not a valid ISBN10 number: {isbn10ParseResult.ToString()}",
                        ISBNParseErrorKind.InvalidISBN10);
                }

                //Input is ISBN10, now convert to EAN13
                var convertResult = Ean13.TryConvertFomISBN10(isbn10, out var converted);
                if (convertResult != Ean13.ResultCode.Success)
                {
                    return new ParseError(
                        $"Could not convert ISBN10 {originalInput} to EAN13: {isbn10ParseResult.ToString()}",
                        ISBNParseErrorKind.InvalidISBN10);
                }

                return Parse(converted, originalInput);
            }

            //Assume EAN13 input
            var ean13ParseResult = Ean13.TryParse(originalInput, out var ean13);

            if (ean13ParseResult != Ean13.ResultCode.Success)
            {
                return new ParseError(
                    $"Input {originalInput} is not a valid EAN13 number: {ean13ParseResult.ToString()}",
                    ISBNParseErrorKind.InvalidEan);
            }

            return Parse(ean13, originalInput);
        }

        private ParseResult Parse(Ean13 ean13, string originalInput)
        {
            if (!ISBN13.TryParse(_prefixTrees, ean13, out var metadataOption))
            {
                return new ParseError(
                    $"Ean: {originalInput}, is not a valid ISBN13 number.",
                    ISBNParseErrorKind.InvalidISBN13);
            }

            return metadataOption.Match<ParseResult>(
                () => new ParseError(
                    $"No metadata found for: {originalInput}, it probably contains some unknown or invalid ranges.",
                    ISBNParseErrorKind.NoMetadataFound),
                some => some
            );
        }
    }
}