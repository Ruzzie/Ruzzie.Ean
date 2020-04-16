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

        public ParseResult Parse(long ean)
        {
            return Parse(ean.ToString());
        }

        public ParseResult Parse(string ean)
        {
            var ean13ParseResult = Ean13.TryParse(ean, out var ean13);

            if (ean13ParseResult != Ean13.ResultCode.Success)
            {
                return new ParseError(
                    $"Input {ean} is not a valid EAN13 number: {ean13ParseResult.ToString()}",
                    ISBNParseErrorKind.InvalidEan);
            }

            if (!ISBN13.TryParse(_prefixTrees, ean13, out var metadataOption))
            {
                return new ParseError(
                    $"Ean: {ean}, is not a valid ISBN13 number.",
                    ISBNParseErrorKind.InvalidISBN13);
            }

            return metadataOption.Match<ParseResult>(
                () => new ParseError(
                    $"No metadata found for: {ean}, it probably contains some unknown or invalid ranges.",
                    ISBNParseErrorKind.NoMetadataFound),

                some => some
            );
        }
    }
}