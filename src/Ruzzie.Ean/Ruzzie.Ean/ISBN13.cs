using System.Collections.Generic;
using Ruzzie.Common.Types;

namespace Ruzzie.Ean
{
    public static class ISBN13
    {
        public static bool TryParse(IReadOnlyDictionary<string, PrefixTree> ruleTrees, Ean13 ean, out Option<Metadata> metadata)
        {
            var eanStr    = ean.AsReadOnlyCharSpan();
            var gs1Prefix = new string(eanStr.Slice(0, 3));

            if (!ruleTrees.TryGetValue(gs1Prefix, out var infoTree))
            {
                metadata = Option<Metadata>.None;
                return false;
            }

            var matchResult = infoTree.Match(eanStr);
            var matchList = matchResult.Matches;
            var matchesCount = matchList.Count;

            if (matchResult.MatchResultType != MatchResultType.Success || matchesCount == 0)
            {
                metadata = Option<Metadata>.None;
                return false;
            }

            int numberOfMatches = matchesCount;

            switch (numberOfMatches)
            {
                case 1:
                    metadata = new Metadata(ean, matchList[0].Prefix);
                    return true;
                case 2:
                    metadata = new Metadata(ean, matchList[0].Prefix, matchList[1].Prefix, matchList[1].Agency);
                    return true;
                case 3:
                    var numberLengthUntilPublisherCode =
                        matchList[0].Prefix.Length + matchList[1].Prefix.Length + matchList[2].Prefix.Length;

                    var titleCode = eanStr.Slice(numberLengthUntilPublisherCode, eanStr.Length - numberLengthUntilPublisherCode -1);
                    metadata = new Metadata(ean, matchList[0].Prefix, matchList[1].Prefix, matchList[2].Agency,
                        matchList[2].Prefix, new string(titleCode));
                    return true;
                default:
                    metadata = Option<Metadata>.None;
                    return false;
            }
        }

        public readonly struct Metadata
        {
            private readonly string _formattedStr;

            public Ean13 Ean { get; }
            public string MainPrefix { get; }
            public string CountryAgency { get; }
            public string CountryGroupCode { get; }
            public string PublisherCode { get; }
            public string TitleCode { get; }

            /// <summary>
            /// The Code delimited by - without the TitleCode and Checksum
            /// </summary>
            public string ISBNPrefix { get; }
            public Metadata(
                in Ean13 ean,
                string   mainPrefix,
                string   countryGroupCode = "",
                string   countryAgency    = "",
                string   publisherCode    = "",
                string   titleCode        = "")
            {
                Ean              = ean;
                MainPrefix       = mainPrefix;
                CountryGroupCode = countryGroupCode;
                CountryAgency    = countryAgency;
                PublisherCode    = publisherCode;
                TitleCode        = titleCode;

                _formattedStr =
                    $"{mainPrefix}-{FormatOptionalInfo(countryGroupCode, publisherCode, titleCode, ean)}-{ean.Checksum}";
                ISBNPrefix =
                    $"{mainPrefix}-{FormatOptionalInfo(countryGroupCode, publisherCode, "", ean)}";
            }

            private static string FormatOptionalInfo(string countryGroupCode, string publisherCode, string titleCode, in Ean13 ean)
            {
                if (string.IsNullOrWhiteSpace(countryGroupCode))
                {
                    return ean.Ean13Code.ToString().Substring(3, 9);
                }

                var countryGroupCodeLength = countryGroupCode.Length;

                if (string.IsNullOrWhiteSpace(publisherCode))
                {
                    return
                        $"{countryGroupCode}-{ean.Ean13Code.ToString().Substring(3 + countryGroupCodeLength, 9 - countryGroupCodeLength)}";
                }

                if (string.IsNullOrWhiteSpace(titleCode))
                {
                    return $"{countryGroupCode}-{publisherCode}";
                }

                return $"{countryGroupCode}-{publisherCode}-{titleCode}";
            }

            public override string ToString()
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                // WE DO want this check, if the struct is defined as default then we want an empty string and not null
                return _formattedStr ?? "";
            }
        }
    }
}