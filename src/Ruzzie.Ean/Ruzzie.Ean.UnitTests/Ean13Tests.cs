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
    }
}