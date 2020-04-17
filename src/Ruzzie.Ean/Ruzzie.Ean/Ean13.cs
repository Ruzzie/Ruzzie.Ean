namespace Ruzzie.Ean
{
    public struct Ean13
    {
        public long Ean13Code { get; }
        public int Checksum { get; }

        private Ean13(long ean13Code, int checksum)
        {
            Ean13Code = ean13Code;
            Checksum = checksum;
        }

        public static ResultCode TryConvertFomISBN10(in ISBN10 isbn10, out Ean13 value)
        {
            return ConvertISBN10(isbn10.ISBN10Code, out value);
        }

        public static ResultCode TryConvertFomISBN10(string isbn10, out Ean13 value)
        {
            if (string.IsNullOrWhiteSpace(isbn10) )
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
            var isbn10WithoutChecksum = isbn10.Substring(0, 9);
            var ean13WitDummyChecksum = $"978{isbn10WithoutChecksum}0";

            if (!long.TryParse(ean13WitDummyChecksum, out var ean13L))
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
            if (string.IsNullOrWhiteSpace(codeToValidate) )
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
            var sum = 0;

            // for (var i = 1; i <= 12; i++)
            // {
            //     var digit = NthDigitOf(ean13L, i);
            //     sum += (i % 2 != 0 ? digit : digit * 3);
            // }

            sum += ean13L.NthDigit(13);
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

            var check = 10 - (sum % 10);//if sum == 0 then check = 0

            if (check == 10)
            {
                return 0;
            }
            return check;
        }

        public enum ResultCode
        {
            InvalidNumberOfDigits = -3,
            InvalidChecksum = -2,
            InvalidNotAValidNumber = -1,
            Invalid = 0,
            Success = 1,
        }
    }
}