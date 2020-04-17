using System;
using FluentAssertions;
using NUnit.Framework;
using Ruzzie.Common.Types;
using Ruzzie.Ean.ISBN;

namespace Ruzzie.Ean.UnitTests
{
    [TestFixture]
    public class ISBN13ParserTests
    {
        private readonly IISBNParser _parser;

        public ISBN13ParserTests()
        {
            var loadResult = ISBNRangeMessage.LoadFromEmbeddedResource();
            _parser = new ISBN13Parser(loadResult.Unwrap());
        }

        [Test]
        public void OkTest()
        {
            //Act
            var parseResult = _parser.Parse(9789013004403);

            //Assert
            ISBN13.Metadata isbn13Metadata = parseResult.Unwrap();
            isbn13Metadata.TitleCode.Should().Be("00440");
        }

        [Test]
        public void ParseValidISBN10Test()
        {
            //Act
            var parseResult = _parser.Parse("0306406152");

            //Assert
            parseResult.Unwrap().TitleCode.Should().Be("40615");
        }

        [Test]
        public void ParseValidISBN102Test()
        {
            //Act
            var parseResult = _parser.Parse("904390001X");

            //Assert
            var metadata = parseResult.Unwrap();
            Console.WriteLine(metadata.Ean.Ean13Code);
            metadata.Ean.Ean13Code.Should().Be(9789043900010);
        }

        //Quadra

        [Test]
        public void ParseInvalidISBN10Test()
        {
            //Act
            var parseResult = _parser.Parse("90284142AA");

            //Assert
            parseResult.UnwrapError().ErrorKind.Should().Be(ISBNParseErrorKind.InvalidISBN10);
        }

        [Test]
        public void ValidEanButInvalidISBN13()
        {
            //Act
            var parseResult = _parser.Parse(6925281930744);

            //Assert
            parseResult.UnwrapError().ErrorKind.Should().Be(ISBNParseErrorKind.InvalidISBN13);
        }

        [Test]
        public void ErrInvalidEan()
        {
            _parser.Parse("invalid-ean").UnwrapError().ErrorKind.Should().Be(ISBNParseErrorKind.InvalidEan);
        }
    }
}