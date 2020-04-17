using System;
using NUnit.Framework;
using Ruzzie.Common.Types;
using Ruzzie.Ean.ISBN;

namespace Ruzzie.Ean.UnitTests
{
    [TestFixture]
    public class Examples
    {
        [Test]
        public void ValidateEan13()
        {
            // Parse a Ean13 input
            var validInput = "5053083195861";
            if (Ean13.TryParse(validInput, out var ean13) == Ean13.ResultCode.Success)
            {
                Console.WriteLine($"Code: {ean13.Ean13Code} Checksum: {ean13.Checksum}");
            }

            // Ean13 with an invalid checksum
            var invalidChecksumInput = "5053083195860";

            var parseResultCode = Ean13.TryParse(invalidChecksumInput, out ean13);
            Console.WriteLine($"Parse result for {invalidChecksumInput} is {parseResultCode}");
            //: Parse result for 5053083195860 is InvalidChecksum

            //All result cases
            switch (parseResultCode)
            {
                case Ean13.ResultCode.InvalidNumberOfDigits:
                    break;
                case Ean13.ResultCode.InvalidChecksum:
                    break;
                case Ean13.ResultCode.InvalidNotAValidNumber:
                    break;
                case Ean13.ResultCode.Invalid:
                    break;
                case Ean13.ResultCode.Success:
                    break;
            }

            //Also see Ean13Tests for more examples
        }

        [Test]
        public void ISBN10Examples()
        {
            //Validate an ISBN10
            if (ISBN10.TryParse("0306406152", out var isbn10) == ISBN10.ResultCode.Success)
            {
                Console.WriteLine($"Code: {isbn10.ISBN10Code} Checksum: {isbn10.Checksum}");
                //: Code: 0306406152 Checksum: 2
            }

            // ISBN10 with an invalid checksum
            var invalidChecksumInput = "0306406150";

            var parseResultCode = ISBN10.TryParse(invalidChecksumInput, out isbn10);
            Console.WriteLine($"Parse result for {invalidChecksumInput} is {parseResultCode}");
            //: Parse result for 0306406150 is InvalidChecksum
        }

        [Test]
        public void ConvertISBN10ToEan13Examples()
        {
            //Convert to Ean13 directly
            if (Ean13.TryConvertFomISBN10("0306406152", out var ean13) == Ean13.ResultCode.Success)
            {
                Console.WriteLine($"Convert ISBN10 from string; Ean13: {ean13.Ean13Code} Checksum: {ean13.Checksum}");
            }

            //Pass an ISBN10 struct
            if (ISBN10.TryParse("0306406152", out var isbn10) == ISBN10.ResultCode.Success)
            {
                if (Ean13.TryConvertFomISBN10(isbn10, out ean13) == Ean13.ResultCode.Success)
                {
                    Console.WriteLine($"Convert ISBN10 from struct; Ean13: {ean13.Ean13Code} Checksum: {ean13.Checksum}");
                }
            }
        }

        [Test]
        public void ISBN13FromISBN10Examples()
        {
            var loadResult = ISBNRangeMessage.LoadFromEmbeddedResource();
            var isbn13Parser = new ISBN13Parser(loadResult.Unwrap());

            var isbn10Str = "0306406152";
            Console.WriteLine(isbn13Parser.Parse(isbn10Str).Unwrap());
            //: 978-0-306-40615-7

            //Invalid ISBN10
            isbn13Parser.Parse("0306406199").MapErr(err =>
            {
                Console.WriteLine($"{err.Message}, {err.ErrorKind}");
                //: Input 0306406199 is not a valid ISBN10 number: InvalidChecksum, InvalidISBN10
                return false;
            });
        }

        [Test]
        public void ISBN13Examples()
        {
            //Validate and get metadata from an ISBN-13
            var loadResult = ISBNRangeMessage.LoadFromEmbeddedResource();
            var isbn13Parser = new ISBN13Parser(loadResult.Unwrap());

            var parseResult = isbn13Parser.Parse("9780671704278");

            //Unwrap (throws exception when result is an error)
            ISBN13.Metadata isbn13Metadata = parseResult.Unwrap();
            Console.WriteLine(isbn13Metadata.ToString());
            //: 978-0-671-70427-8

            //match: handle Err or Ok case
            parseResult.Match(
                err =>
                {
                    //This is executed when the result is an error
                    switch (err.ErrorKind)
                    {
                        case ISBNParseErrorKind.InvalidEan:
                            break;
                        case ISBNParseErrorKind.NoMetadataFound:
                            break;
                        case ISBNParseErrorKind.InvalidISBN13:
                            break;
                        case ISBNParseErrorKind.InvalidISBN10:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return err.Message;
                },
                metadata =>
                {

                    //This is executed when the result is Ok
                    Console.WriteLine($"ISBN13: {metadata.ToString()}, Country: {metadata.CountryAgency}");
                    //: ISBN13: 978-0-671-70427-8, Country: English language

                    //An isbn13 is a valid EAN13
                    var ean13 = metadata.Ean;
                    Console.WriteLine($"Code: {ean13.Ean13Code} Checksum: {ean13.Checksum}");
                    //: Code: 9780671704278 Checksum: 8

                    return metadata.ToString();
                });

            //See: ISBN13MetaDataInfoTests for more examples
        }
    }
}