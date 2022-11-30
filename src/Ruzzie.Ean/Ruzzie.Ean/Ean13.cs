using System;

namespace Ruzzie.Ean;

public readonly struct Ean13
{
    public long Ean13Code { get; }
    public int  Checksum  { get; }

    private Ean13(long ean13Code, int checksum)
    {
        Ean13Code = ean13Code;
        Checksum  = checksum;
    }

    public override string ToString()
    {
        return Ean13Code.ToString("D13");
    }

    private static readonly char[] Zero = { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };

    public ReadOnlySpan<char> AsReadOnlyCharSpan()
    {
        if (Ean13Code == default)
        {
            return Zero;
        }

        var numberChars = new char[13];

        numberChars[0]  = (char)(48 + Ean13Code.NthDigit(13));
        numberChars[1]  = (char)(48 + Ean13Code.NthDigit(12));
        numberChars[2]  = (char)(48 + Ean13Code.NthDigit(11));
        numberChars[3]  = (char)(48 + Ean13Code.NthDigit(10));
        numberChars[4]  = (char)(48 + Ean13Code.NthDigit(9));
        numberChars[5]  = (char)(48 + Ean13Code.NthDigit(8));
        numberChars[6]  = (char)(48 + Ean13Code.NthDigit(7));
        numberChars[7]  = (char)(48 + Ean13Code.NthDigit(6));
        numberChars[8]  = (char)(48 + Ean13Code.NthDigit(5));
        numberChars[9]  = (char)(48 + Ean13Code.NthDigit(4));
        numberChars[10] = (char)(48 + Ean13Code.NthDigit(3));
        numberChars[11] = (char)(48 + Ean13Code.NthDigit(2));
        numberChars[12] = (char)(48 + Ean13Code.NthDigit(1));

        return numberChars;
    }

    public static ResultCode Create(long digitsWithoutChecksum, out Ean13 value)
    {
        if (digitsWithoutChecksum <= 0)
        {
            value = default;
            return ResultCode.InvalidNotAValidNumber;
        }

        //If the number of digits >= 13 then
        //invalid number of digits
        if (digitsWithoutChecksum >= 100_000_000_000_0)
        {
            value = default;
            return ResultCode.InvalidNumberOfDigits;
        }

        var ean13LWithDummyChecksum = digitsWithoutChecksum * 10;
        var checksum                = CalculateChecksum(ean13LWithDummyChecksum);
        value = new Ean13(ean13LWithDummyChecksum + checksum, checksum);
        return ResultCode.Success;
    }

    public static ResultCode TryConvertFomISBN10(in ISBN10 isbn10, out Ean13 value)
    {
        return ConvertISBN10(isbn10.ISBN10Code, out value);
    }

    public static ResultCode TryConvertFomISBN10(string isbn10, out Ean13 value)
    {
        if (string.IsNullOrWhiteSpace(isbn10))
        {
            value = default;
            return ResultCode.Invalid;
        }

        isbn10 = isbn10.Trim();

        if (isbn10.Length != 10)
        {
            value = default;
            return ResultCode.InvalidNumberOfDigits;
        }

        return ConvertISBN10(isbn10, out value);
    }

    private static ResultCode ConvertISBN10(string isbn10, out Ean13 value)
    {
        var isbn10WithoutChecksum  = isbn10.Substring(0, 9);
        var ean13WithDummyChecksum = $"978{isbn10WithoutChecksum}0";

        if (!long.TryParse(ean13WithDummyChecksum, out var ean13L))
        {
            value = default;
            return ResultCode.InvalidNotAValidNumber;
        }

        var checkSumDigit = CalculateChecksum(ean13L);

        value = new Ean13(ean13L + checkSumDigit, checkSumDigit);
        return ResultCode.Success;
    }

    public static ResultCode TryParse(string codeToValidate, out Ean13 value)
    {
        if (string.IsNullOrWhiteSpace(codeToValidate))
        {
            value = default;
            return ResultCode.Invalid;
        }

        codeToValidate = codeToValidate.Trim();

        if (codeToValidate.Length != 13)
        {
            value = default;
            return ResultCode.InvalidNumberOfDigits;
        }

        if (!long.TryParse(codeToValidate, out var ean13L))
        {
            value = default;
            return ResultCode.InvalidNotAValidNumber;
        }

        var checksumDigitFromInput = DigitHelper.NthDigit(ean13L, 1);

        if (checksumDigitFromInput == -10)
        {
            value = default;
            return ResultCode.InvalidNotAValidNumber;
        }

        var checksumDigitFromCalculation = CalculateChecksum(ean13L);

        if (checksumDigitFromInput != checksumDigitFromCalculation)
        {
            value = default;
            return ResultCode.InvalidChecksum;
        }

        value = new Ean13(ean13L, checksumDigitFromCalculation);
        return ResultCode.Success;
    }

    private static int CalculateChecksum(long ean13L)
    {
        //var sum = 0;

        // for (var i = 1; i <= 12; i++)
        // {
        //     var digit = NthDigitOf(ean13L, i);
        //     sum += (i % 2 != 0 ? digit : digit * 3);
        // }

        var sum = ean13L.NthDigit(13);
        sum += ean13L.NthDigit(12) * 3;
        sum += ean13L.NthDigit(11);
        sum += ean13L.NthDigit(10) * 3;
        sum += ean13L.NthDigit(9);
        sum += ean13L.NthDigit(8) * 3;
        sum += ean13L.NthDigit(7);
        sum += ean13L.NthDigit(6) * 3;
        sum += ean13L.NthDigit(5);
        sum += ean13L.NthDigit(4) * 3;
        sum += ean13L.NthDigit(3);
        sum += ean13L.NthDigit(2) * 3;

        var check = 10 - (sum % 10); //if sum == 0 then check = 0

        if (check == 10)
        {
            return 0;
        }

        return check;
    }

    public enum ResultCode
    {
        InvalidNumberOfDigits  = -3
      , InvalidChecksum        = -2
      , InvalidNotAValidNumber = -1
      , Invalid                = 0
      , Success                = 1
    }
}