﻿using Ruzzie.Common.Types;

 namespace Ruzzie.Ean
{
    public interface IISBNParser
    {
        Result<Err<ISBNParseErrorKind>, ISBN13.Metadata> Parse(long ean);
        Result<Err<ISBNParseErrorKind>, ISBN13.Metadata> Parse(string ean);
    }

    public enum ISBNParseErrorKind
    {
        InvalidEan,
        NoMetadataFound,
        InvalidISBN13
    }
}