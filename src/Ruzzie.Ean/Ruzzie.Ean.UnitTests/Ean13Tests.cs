using FluentAssertions;
using NUnit.Framework;

namespace Ruzzie.Ean.UnitTests
{
    [TestFixture]
    public class Ean13Tests
    {
        [TestCase("9644626547849", 9644626547849)]
        [TestCase("9789490433024", 9789490433024)]
        public void Ok(string codeToValidate, long expected)
        {
            //Act
            var success = Ean13.TryParse(codeToValidate, out var ean13);

            //Assert
            success.Should().Be(Ean13.ResultCode.Success);
            ean13.Ean13Code.Should().Be(expected);

        }

        [TestCase("9644626547849", 9)]
        [TestCase("9789490433024", 4)]
        public void CheckSumMatches(string codeToValidate, int expectedCheckSum)
        {
            //Act
            var success = Ean13.TryParse(codeToValidate, out var ean13);

            //Assert
            success.Should().Be(Ean13.ResultCode.Success);
            ean13.Checksum.Should().Be(expectedCheckSum);
        }

        [Test]
        public void ChecksumFailed()
        {
            var codeToValidate = "9789028414245";
            var success = Ean13.TryParse(codeToValidate, out _);

            success.Should().Be(Ean13.ResultCode.InvalidChecksum);
        }

        [Test]
        public void InvalidNumberOfDigits()
        {
            var codeToValidate = "978902841424";
            var success = Ean13.TryParse(codeToValidate, out _);

            success.Should().Be(Ean13.ResultCode.InvalidNumberOfDigits);
        }

        [TestCase("1861972717",9781861972712)]
        [TestCase("9020254677",9789020254679)]
        [TestCase("059600351X",9780596003517)]
        [TestCase("059600351A",9780596003517)]
        [TestCase("9063251345",9789063251345)]
        public void FromISBN10StrTest(string isbn10, long expected)
        {
            Ean13.TryConvertFomISBN10(isbn10, out var ean13).Should().Be(Ean13.ResultCode.Success);
            ean13.Ean13Code.Should().Be(expected);
        }

        [TestCase("1861972717",9781861972712)]
        [TestCase("9020254677",9789020254679)]
        [TestCase("059600351X",9780596003517)]
        [TestCase("0306406152", 9780306406157L)]
        [TestCase("9020254677", 9789020254679L)]
        [TestCase("9063251343", 9789063251345L)]
        public void FromISBN10Test(string isbn10Str, long expected)
        {
            ISBN10.TryParse(isbn10Str, out var isbn10);
            Ean13.TryConvertFomISBN10(isbn10, out var ean13).Should().Be(Ean13.ResultCode.Success);
            ean13.Ean13Code.Should().Be(expected);
        }
    }
}