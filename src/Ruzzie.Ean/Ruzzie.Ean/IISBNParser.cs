﻿using Ruzzie.Common.Types;

 namespace Ruzzie.Ean
{
    public interface IISBNParser
    {
        Result<Err<ISBNParseErrorKind>, ISBN13.Metadata> Parse(long input);
        Result<Err<ISBNParseErrorKind>, ISBN13.Metadata> Parse(string input);
    }

    public enum ISBNParseErrorKind
    {
        InvalidEan,
        NoMetadataFound,
        InvalidISBN13,
        InvalidISBN10
    }
}