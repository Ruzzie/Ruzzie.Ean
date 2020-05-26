namespace Ruzzie.Ean
{
    public readonly struct ISBN10
    {
        public static ISBN10 Empty = new ISBN10(string.Empty, -1);
        public string ISBN10Code { get; }
        public int Checksum { get; }

        private ISBN10(string isbn10Code, int checksum)
        {
            ISBN10Code = isbn10Code;
            Checksum = checksum;
        }

        public static ResultCode TryParse(string isbn, out ISBN10 value)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                value = Empty;
                return ResultCode.Invalid;
            }

            isbn = isbn.Trim();
            if (isbn.Length != 10)
            {
                value = Empty;
                return ResultCode.InvalidNumberOfDigits;
            }

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                if (!int.TryParse(isbn[i].ToString(), out var digit))
                {
                    value = Empty;
                    return ResultCode.InvalidNotAValidNumber;
                }
                sum += (i + 1) * digit;
            }

            int remainder = sum % 11;
            if (remainder == 10)
            {
                if (isbn[9] == 'X')
                {
                    value = new ISBN10(isbn, 10);
                    return ResultCode.Success;
                }
            }

            var validChecksum = isbn[9] == (char) ('0' + remainder);
            if (!validChecksum)
            {
                value = Empty;
                return ResultCode.InvalidChecksum;
            }

            value = new ISBN10(isbn, remainder);
            return ResultCode.Success;
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