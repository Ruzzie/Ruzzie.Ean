using FluentAssertions;
using FsCheck;
using NUnit.Framework;

namespace Ruzzie.Ean.UnitTests
{
    [TestFixture]
    public class ISBN10Tests
    {
        [FsCheck.NUnit.Property]
        public void TryParsePropertyTestNoExceptions(NonEmptyString input)
        {
            ISBN10.TryParse(input.Get, out _);
        }

        [TestCase("0306406152", "0306406152", ISBN10.ResultCode.Success)]
        [TestCase("9020254677", "9020254677", ISBN10.ResultCode.Success)]
        [TestCase("9063251343", "9063251343", ISBN10.ResultCode.Success)]
        [TestCase("9028414231", "9028414231", ISBN10.ResultCode.Success)]
        [TestCase("059600351X", "059600351X", ISBN10.ResultCode.Success)]

        [TestCase("1000000001518", "", ISBN10.ResultCode.InvalidNumberOfDigits)]
        [TestCase("9789028414242", "",ISBN10.ResultCode.InvalidNumberOfDigits)]
        [TestCase("90284142AA", "",ISBN10.ResultCode.InvalidNotAValidNumber)]
        [TestCase("902841423A","", ISBN10.ResultCode.InvalidChecksum)]
        [TestCase("9063251345", "",ISBN10.ResultCode.InvalidChecksum)]

        [TestCase("", "",ISBN10.ResultCode.Invalid)]
        [TestCase("abcdefghij", "",ISBN10.ResultCode.InvalidNotAValidNumber)]
        [TestCase("123", "",ISBN10.ResultCode.InvalidNumberOfDigits)]
        public void TryParseTests(string input, string expected, ISBN10.ResultCode resultCode)
        {
            var result = ISBN10.TryParse(input, out var isbn10);
            result.Should().Be(resultCode);
            if (resultCode == ISBN10.ResultCode.Success)
            {
                isbn10.ISBN10Code.Should().Be(expected);
            }
            else
            {
                isbn10.Should().Be(ISBN10.Empty);
            }
        }
    }
}