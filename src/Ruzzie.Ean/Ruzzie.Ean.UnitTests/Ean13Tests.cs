using System;
using FluentAssertions;
using NUnit.Framework;

namespace Ruzzie.Ean.UnitTests;

[TestFixture]
public class Ean13Tests
{
    [TestCase("9644626547849", 9644626547849)]
    [TestCase("9789490433024", 9789490433024)]
    [TestCase("9789063252700", 9789063252700)]
    [TestCase("9789063500870", 9789063500870L)]
    [TestCase("9789071886140", 9789071886140)]
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
        var success        = Ean13.TryParse(codeToValidate, out _);

        success.Should().Be(Ean13.ResultCode.InvalidChecksum);
    }

    [Test]
    public void InvalidNumberOfDigits()
    {
        var codeToValidate = "978902841424";
        var success        = Ean13.TryParse(codeToValidate, out _);

        success.Should().Be(Ean13.ResultCode.InvalidNumberOfDigits);
    }

    [TestCase("1861972717", 9781861972712)]
    [TestCase("9020254677", 9789020254679)]
    [TestCase("059600351X", 9780596003517)]
    [TestCase("059600351A", 9780596003517)]
    [TestCase("9063251345", 9789063251345)]
    public void FromISBN10StrTest(string isbn10, long expected)
    {
        Ean13.TryConvertFomISBN10(isbn10, out var ean13).Should().Be(Ean13.ResultCode.Success);
        ean13.Ean13Code.Should().Be(expected);
    }

    [TestCase("1861972717", 9781861972712)]
    [TestCase("9020254677", 9789020254679)]
    [TestCase("059600351X", 9780596003517)]
    [TestCase("0306406152", 9780306406157L)]
    [TestCase("9020254677", 9789020254679L)]
    [TestCase("9063251343", 9789063251345L)]
    [TestCase("9063252706", 9789063252700L)]
    public void FromISBN10Test(string isbn10Str, long expected)
    {
        ISBN10.TryParse(isbn10Str, out var isbn10);
        Ean13.TryConvertFomISBN10(isbn10, out var ean13).Should().Be(Ean13.ResultCode.Success);
        ean13.Ean13Code.Should().Be(expected);
    }

    [TestCase(000000012345,   Ean13.ResultCode.Success,                000000012345_7)]
    [TestCase(020067170423,   Ean13.ResultCode.Success,                020067170423_2)]
    [TestCase(200671704232,   Ean13.ResultCode.Success,                200671704232_4)]
    [TestCase(1,              Ean13.ResultCode.Success,                000000000001_7)]
    [TestCase(-1,             Ean13.ResultCode.InvalidNotAValidNumber, 000000000000_0)]
    [TestCase(100000012345_6, Ean13.ResultCode.InvalidNumberOfDigits,  000000000000_0)]
    [TestCase(11111111111111, Ean13.ResultCode.InvalidNumberOfDigits,  000000000000_0)]
    public void CreateTest(long digits, Ean13.ResultCode expectedResultCode, long expected)
    {
        var resultCode = Ean13.Create(digits, out var ean13);
        resultCode.Should().Be(expectedResultCode);
        ean13.Ean13Code.Should().Be(expected);
    }

    [TestCase(000000012345, "0000000123457")]
    [TestCase(020067170423, "0200671704232")]
    [TestCase(200671704232, "2006717042324")]
    public void AsReadOnlyCharSpanTests(long digitsWithoutChecksum, string expectedStr)
    {
        //Arrange
        Ean13.Create(digitsWithoutChecksum, out var ean13).Should().Be(Ean13.ResultCode.Success);

        //Act
        var asSpan = ean13.AsReadOnlyCharSpan();

        //Assert
        new string(asSpan).Should().Be(expectedStr);
    }

    [FsCheck.NUnit.Property]
    public void AsReadOnlyCharSpan_NoExceptions_Test(long digits)
    {
        //Arrange
        Ean13.Create(digits, out var ean13);
        //Act & Assert
        new string(ean13.AsReadOnlyCharSpan()).Should().Be(ean13.ToString());
    }

    [FsCheck.NUnit.Property]
    public void CreateProperty_NoExceptions_Test(long digits)
    {
        Ean13.Create(digits, out _);
    }

    [Test]
    public void Nothing()
    {
        for (int i = 1; i < 11; i++)
        {
            Ean13.Create(200101000000          + i, out var value);
            Console.WriteLine(value.ToString() + "-" + "1");
            Console.WriteLine(value.ToString() + "-" + "1");
        }
    }
}