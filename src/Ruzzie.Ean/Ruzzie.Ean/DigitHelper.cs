using System.Runtime.CompilerServices;

namespace Ruzzie.Ean;

internal static class DigitHelper
{
    /// Get the value of the nth position digit, for an integer
    ///   returns -10 when digitPosition is out of bounds in debug mode
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NthDigit(this long number, int digitPosition)
    {
        return (int)(number / GetPowerForNthDigit(digitPosition) % 10);
    }

    /// Returns the power of the nth digit in an integer for example:
    /// 1 returns 1
    /// 2 returns 10
    /// 3 returns 100 etc.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long GetPowerForNthDigit(int position)
    {
        switch (position)
        {
            case 1:  return 1;
            case 2:  return 10;
            case 3:  return 100;
            case 4:  return 1000;
            case 5:  return 10_000;
            case 6:  return 100_000;
            case 7:  return 1000_000;
            case 8:  return 10_000_000;
            case 9:  return 100_000_000;
            case 10: return 1000_000_000;
            case 11: return 10_000_000_000;
            case 12: return 100_000_000_000;
            case 13: return 1000_000_000_000;
            default:
                return -10;
        }
    }
}