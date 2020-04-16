﻿using System;

namespace Ruzzie.Ean
{
    public class RangeRule
    {
        private readonly int[] _digitsFrom;
        private readonly int[] _digitsTo;
        public int NumberOfDigits { get; }

        public RangeRule(int from, int to, int numberOfDigits)
        {
            if (numberOfDigits <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfDigits));

            if (to < from)
                throw new ArgumentOutOfRangeException(nameof(to),"to must be > than from");

            if (to > Math.Pow(to, numberOfDigits))
                throw new ArgumentOutOfRangeException(nameof(to));

            if (from > Math.Pow(from, numberOfDigits))
                throw new ArgumentOutOfRangeException(nameof(from));

            NumberOfDigits = numberOfDigits;
            _digitsFrom = new int[numberOfDigits];
            _digitsTo = new int[numberOfDigits];

            for (int i = 1; i <= numberOfDigits; i++)
            {
                _digitsFrom[i - 1] = Ean13.NthDigitOf(from, i);
                _digitsTo[i - 1] = Ean13.NthDigitOf(to, i);
            }
        }

        private bool IsMatch(int number)
        {
            //Start with largest power
            for (var i = NumberOfDigits; i >= 1; i--)
            {
                var nthDigitOfNumber = Ean13.NthDigitOf(number, i);

                if (nthDigitOfNumber < _digitsFrom[i - 1])
                    return false;
                if (nthDigitOfNumber > _digitsTo[i - 1])
                    return false;
                if (nthDigitOfNumber < _digitsTo[i - 1])
                    return true;
            }

            return true;
        }

        public bool IsMatch(string number)
        {
            if (string.IsNullOrWhiteSpace(number) || number.Length < NumberOfDigits)
            {
                return false;
            }

            string numberPrefix;
            if (number.StartsWith('-') || number.StartsWith('+'))
            {
                numberPrefix = number.Substring(0, NumberOfDigits + 1);
            }
            else
            {
                numberPrefix = number.Substring(0, NumberOfDigits);
            }

            int numberDigits = int.Parse(numberPrefix);
            return IsMatch(numberDigits);
        }
    }
}