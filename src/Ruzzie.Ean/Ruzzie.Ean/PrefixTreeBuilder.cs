using System;
using System.Collections.Generic;
using System.IO;
using Ruzzie.Ean.ISBN;

namespace Ruzzie.Ean
{
    public static class PrefixTreeBuilder
    {
        public static IReadOnlyDictionary<string, PrefixTree> BuildFromISBNRangeMessage(ISBNRangeMessage isbnRangeMessage)
        {
            if (isbnRangeMessage == null)
            {
                throw new ArgumentNullException(nameof(isbnRangeMessage));
            }

            if (isbnRangeMessage.EAN_UCCPrefixes?.EAN_UCC == null)
            {
                throw new ArgumentException("EAN_UCC is null in message.", nameof(isbnRangeMessage));
            }

            var rootPrefixTrees = new Dictionary<string, PrefixTree>(2, StringComparer.Ordinal);

            foreach (var eanUcc in isbnRangeMessage.EAN_UCCPrefixes.EAN_UCC)
            {
                if (eanUcc.Rules?.Rule == null)
                {
                    continue;
                }

                var mainRangeRules = new List<RangeRule>(eanUcc.Rules.Rule.Count);

                foreach (var rule in eanUcc.Rules.Rule)
                {
                    var numberOfDigits = rule.Length;
                    if (numberOfDigits > 0)
                    {
                        var ranges = rule.Range.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        var from = int.Parse(ranges[0].Substring(0, numberOfDigits));
                        var to = int.Parse(ranges[1].Substring(0, numberOfDigits));

                        mainRangeRules.Add(new RangeRule(@from, to, numberOfDigits));
                    }
                }

                var mainPrefix = eanUcc.Prefix;
                var prefixTree = new PrefixTree(
                    mainPrefix,
                    eanUcc.Prefix.Length,
                    eanUcc.Agency,
                    mainRangeRules);

                rootPrefixTrees.Add(mainPrefix, prefixTree);
            }

            if (rootPrefixTrees.Count == 0)
            {
                throw new ArgumentException("Invalid data, the message does not contain any UCCPrefixes.",
                    nameof(isbnRangeMessage));
            }

            var allGroups = isbnRangeMessage.RegistrationGroups?.Group;
            if (allGroups == null)
            {
                return rootPrefixTrees;
            }

            for (var i = 0; i < allGroups.Count; i++)
            {
                var currentGroup = allGroups[i];
                var groupPrefixes = currentGroup.Prefix.Split('-', StringSplitOptions.RemoveEmptyEntries);
                var parentPrefixStr = groupPrefixes[0];
                var groupPrefixStr = groupPrefixes[1];

                if (currentGroup.Rules?.Rule == null)
                {
                    continue;
                }

                var groupRules = new List<RangeRule>(currentGroup.Rules.Rule.Count);

                foreach (var rule in currentGroup.Rules.Rule)
                {
                    var numberOfDigits = rule.Length;
                    if (numberOfDigits > 0)
                    {
                        var ranges = rule.Range.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        var from = int.Parse(ranges[0].Substring(0, numberOfDigits));
                        var to = int.Parse(ranges[1].Substring(0, numberOfDigits));
                        var rangeRule = new RangeRule(@from, to, numberOfDigits);
                        groupRules.Add(rangeRule);
                    }
                }

                if (groupRules.Count > 0)
                {
                    if (!rootPrefixTrees[parentPrefixStr].AddChild(groupPrefixStr, groupPrefixStr.Length,
                        currentGroup.Agency, groupRules))
                    {
                        throw new InvalidDataException(
                            $"Could not add child {groupPrefixStr} to root {parentPrefixStr}.");
                    }
                }
            }

            return rootPrefixTrees;
        }
    }
}