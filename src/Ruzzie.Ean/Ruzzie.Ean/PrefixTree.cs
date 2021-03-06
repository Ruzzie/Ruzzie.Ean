using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ruzzie.Ean
{

    public class PrefixTree
    {
        private readonly int _prefixNumberOfDigits;
        private readonly string _agency;
        private readonly IReadOnlyList<RangeRule> _rulesSortedByNumberOfDigits;
        private readonly SortedSet<PrefixTree> _children;
        private readonly string _prefixStr;
        private readonly int _rulesCount;

        public PrefixTree(string prefix, int prefixNumberOfDigits, string agency, IEnumerable<RangeRule> rules)
        {
            _prefixNumberOfDigits = prefixNumberOfDigits;
            _agency = agency;

            var rulesList = new List<RangeRule>(rules);
            rulesList.Sort(
                Comparer<RangeRule>.Create((x, y) =>
                    y.NumberOfDigits.CompareTo(x.NumberOfDigits))); //Rules with most digits first; 'greedy'

            _rulesSortedByNumberOfDigits = rulesList;
            _rulesCount = _rulesSortedByNumberOfDigits.Count;

            _children = new SortedSet<PrefixTree>(
                Comparer<PrefixTree>.Create((x, y) =>
                    {
                        if (ReferenceEquals(x, y))
                        {
                            return 0;
                        }

                        if (x._prefixNumberOfDigits == y._prefixNumberOfDigits)
                        {
                            return string.Compare(y._prefixStr, x._prefixStr, StringComparison.Ordinal);
                        }

                        return y._prefixNumberOfDigits.CompareTo(x._prefixNumberOfDigits);
                    }
                ));

            _prefixStr = prefix;
        }

        public bool AddChild(string groupPrefix, int groupPrefixNumberOfDigits, string agencyName, IEnumerable<RangeRule> groupRules)
        {
            for (var i = 0; i < _rulesCount; i++)
            {
                var mainRule = _rulesSortedByNumberOfDigits[i];

                if (mainRule.IsMatch(groupPrefix))
                {
                    var group = new PrefixTree(groupPrefix, groupPrefixNumberOfDigits, agencyName, groupRules);
                    return _children.Add(@group);
                }
            }

            return false;
        }

        public MatchResult Match(ReadOnlySpan<char> number)
        {
            if (!MatchesPrefix(number, _prefixStr, _prefixNumberOfDigits))
            {
                return new MatchResult(MatchResultType.NoMatchPrefixesDoNotMatch, new List<MatchValue>(0));
            }

            var matches = new List<MatchValue>();
            matches.Add(new MatchValue(_prefixStr, _agency));
            var numberWithoutPrefix = number.Slice(_prefixNumberOfDigits);

            AddMatchRulesAndChildren(numberWithoutPrefix, matches);

            return new MatchResult(MatchResultType.Success, matches);
        }

        private void AddMatchRulesAndChildren(ReadOnlySpan<char> number, List<MatchValue> matches)
        {
            for (var i = 0; i < _rulesCount; i++)
            {
                var mainRule = _rulesSortedByNumberOfDigits[i];
                if (mainRule.IsMatch(number))
                {
                    var prefix = number.Slice(0, mainRule.NumberOfDigits);
                    var numberWithoutPrefix = number.Slice(mainRule.NumberOfDigits);

                    var prefixStr = new string(prefix);

                    matches.Add(new MatchValue(prefixStr, _agency));

                    MatchChildren(matches, prefixStr, numberWithoutPrefix);

                    return;
                }
            }
        }

        private void MatchChildren(List<MatchValue> matches, string prefixStr,  ReadOnlySpan<char> numberWithoutPrefix)
        {
            foreach (var child in _children)
            {
                if (string.Equals(child._prefixStr, prefixStr, StringComparison.Ordinal))
                {
                    child.AddMatchRulesAndChildren(numberWithoutPrefix, matches);
                    return;
                }
            }
        }

        private static bool MatchesPrefix(ReadOnlySpan<char> number, ReadOnlySpan<char> prefix, int prefixNumberOfDigits)
        {
            var numberPrefix = number.Slice(0, prefixNumberOfDigits);

            return prefix.Equals(numberPrefix, StringComparison.Ordinal);
        }
    }

    [DebuggerDisplay("Prefix = {" + nameof(Prefix) + "}, Agency = {" + nameof(Agency) + "} ")]
    public readonly struct MatchValue
    {
        public MatchValue(string prefix, string agency)
        {
            Prefix = prefix;
            Agency = agency;
        }

        public string Prefix { get;  }
        public string Agency { get;  }
    }

    public enum MatchResultType
    {
        NoMatchPrefixesDoNotMatch,
        NoMatchesFound = 0,
        Success = 1,
    }

    public readonly struct MatchResult
    {
        public MatchResultType MatchResultType { get; }
        public IReadOnlyList<MatchValue> Matches { get; }

        public MatchResult(MatchResultType matchResultType, List<MatchValue> matches)
        {
            MatchResultType = matchResultType;
            Matches = matches;
        }
    }
}