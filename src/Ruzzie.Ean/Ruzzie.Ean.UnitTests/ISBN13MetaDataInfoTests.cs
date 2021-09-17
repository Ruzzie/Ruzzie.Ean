using System;
using System.Collections.Generic;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using Ruzzie.Ean.ISBN;

namespace Ruzzie.Ean.UnitTests
{
    [TestFixture]
    public class ISBN13MetaDataInfoTests
    {
        private readonly IReadOnlyDictionary<string, PrefixTree> _ruleTrees;

        public ISBN13MetaDataInfoTests()
        {
            var isbnRangeMessage = ISBNRangeMessage.LoadFromFile(TestContext.CurrentContext.TestDirectory + "\\ISBNRangeMessage.xml");

            _ruleTrees = PrefixTreeBuilder.BuildFromISBNRangeMessage(isbnRangeMessage);
        }

        [Test]
        public void TryParseSmokeTest()
        {
            //Arrange
            Ean13.TryParse("9789013004403", out var ean13);
            //Act
            var success = ISBN13.TryParse(_ruleTrees, ean13, out var metadataOpt);
            //Assert
            success.Should().BeTrue();
            var metadata = metadataOpt.UnwrapOr(new ISBN13.Metadata(ean13, "978"));
            metadata.MainPrefix.Should().Be("978");
            metadata.CountryAgency.Should().Be("Netherlands");
            metadata.CountryGroupCode.Should().Be("90");
            metadata.PublisherCode.Should().Be("13");
            metadata.TitleCode.Should().Be("00440");
        }

        [TestCase("9789013004403", true)]
        [TestCase("9789021819648", true)]
        [TestCase("9789023469568", true)]
        [TestCase("9789012020992", true)]
        [TestCase("9789460540417", true)]
        [TestCase("9789490433024", true)]
        [TestCase("9789462785281", true)]
        [TestCase("9780077159009", true)]
        [TestCase("9781000232134", true)]
        [TestCase("9781001232133", true)]
        [TestCase("9789071886140", true)]
        [TestCase("9782216115372	", true)]
        public void TryParseTestCases(string validIsbn13, bool hasTitleCode)
        {
            //Arrange
            Ean13.TryParse(validIsbn13, out var ean13).Should().Be(Ean13.ResultCode.Success);
            //Act
            var success = ISBN13.TryParse(_ruleTrees, ean13, out var metadataOpt);
            //Assert
            success.Should().BeTrue();
            metadataOpt.For(
                () => Assert.Fail($"Expected Some metadata for {validIsbn13}."),
                metadata =>
                {
                    Console.WriteLine(metadata.ToString());
                    string.IsNullOrWhiteSpace(metadata.TitleCode).Should().Be(!hasTitleCode);
                });
        }

        [Test]
        public void MetaDataToStringSmokeTest()
        {
            //Arrange
            Ean13.TryParse("9789490433024", out var ean13);
            ISBN13.TryParse(_ruleTrees, ean13, out var metadataOpt).Should().BeTrue();
            //Act
            var formattedString = metadataOpt.UnwrapOr(default).ToString();
            //Assert
            formattedString.Should().Be("978-94-90433-02-4");
        }

        [Test]
        public void MetaDataToStringIsEmptyOnDefault()
        {
            ISBN13.Metadata x = default;
            x.ToString().Should().Be("");
        }

        [FsCheck.NUnit.Property]
        public void TryParsePropertyTest(NonEmptyString input)
        {
            if (Ean13.TryParse(input.Item, out var ean13) == Ean13.ResultCode.Success)
            {
                var success = ISBN13.TryParse(_ruleTrees, ean13, out var metadataOpt);
                if (success)
                {
                    metadataOpt.IsSome().Should().BeTrue();
                }
                else
                {
                    metadataOpt.IsNone().Should().BeTrue();
                }
            }
        }
    }
}