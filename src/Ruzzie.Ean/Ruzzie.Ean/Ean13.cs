﻿using System.Runtime.CompilerServices;

 namespace Ruzzie.Ean
{
    public struct Ean13
    {
        private static readonly long[] DigitPowers = {1, 10, 100, 1000, 10000, 1000_00, 1000_000, 1000_000_0, 1000_000_00, 1000_000_000, 1000_000_000_0, 1000_000_000_00, 1000_000_000_000};
        private static readonly int DigitPowersLength = DigitPowers.Length;

        public long Ean13Code { get; }
        public int Checksum { get; }

        private Ean13(long ean13Code, int checksum)
        {
            Ean13Code = ean13Code;
            Checksum = checksum;
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

            var checksumDigitFromInput = NthDigitOf(ean13L, 1);

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

            sum += NthDigitOf(ean13L, 13);
            sum += NthDigitOf(ean13L, 12) * 3;
            sum += NthDigitOf(ean13L, 11);
            sum += NthDigitOf(ean13L, 10) * 3;
            sum += NthDigitOf(ean13L, 9);
            sum += NthDigitOf(ean13L, 8) * 3;
            sum += NthDigitOf(ean13L, 7);
            sum += NthDigitOf(ean13L, 6) * 3;
            sum += NthDigitOf(ean13L, 5);
            sum += NthDigitOf(ean13L, 4) * 3;
            sum += NthDigitOf(ean13L, 3);
            sum += NthDigitOf(ean13L, 2) * 3;

            var check = 10 - (sum % 10);
            return check;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NthDigitOf(long number, int digitPosition)
        {
#if DEBUG
            if (digitPosition > DigitPowersLength)
            {
                return -10;
            }
#endif
            return (int) (number / DigitPowers[digitPosition - 1] % 10);
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