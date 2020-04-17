using System.Runtime.CompilerServices;

namespace Ruzzie.Ean
{
    internal static class DigitHelper
    {
        private static readonly long[] DigitPowers = {1, 10, 100, 1000, 10000, 1000_00, 1000_000, 1000_000_0, 1000_000_00, 1000_000_000, 1000_000_000_0, 1000_000_000_00, 1000_000_000_000};
        private static readonly int DigitPowersLength = DigitPowers.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NthDigit(this long number, int digitPosition)
        {
#if DEBUG
            if (digitPosition > DigitPowersLength)
            {
                return -10;
            }
#endif
            return (int) (number / DigitPowers[digitPosition - 1] % 10);
        }
    }
}