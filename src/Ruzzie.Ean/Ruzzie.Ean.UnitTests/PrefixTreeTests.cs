using FluentAssertions;
using NUnit.Framework;

namespace Ruzzie.Ean.UnitTests
{
    [TestFixture]
    public class PrefixTreeTests
    {
        private readonly PrefixTree _treeUnderTest;

        public PrefixTreeTests()
        {
            _treeUnderTest = new PrefixTree(
                "978", 3, "International ISBN Agency",
                new[]
                {
                    new RangeRule(0, 4, 1),
                    new RangeRule(11, 22, 2)
                });

            _treeUnderTest.AddChild("0", 1, "English language",
                new[]
                {
                    new RangeRule(00, 12, 2),
                    new RangeRule(229, 368, 3)
                });
        }

        [Test]
        public void MatchSmokeTest()
        {
            var matchResult = _treeUnderTest.Match(9780345548566.ToString());

            matchResult.MatchResultType.Should().Be(MatchResultType.Success);
            matchResult.Matches.Count.Should().Be(3);
        }

        [TestCase("9789988776655", 1)]
        [TestCase("9780345548566", 3)]
        [TestCase("9781210548566", 2)]
        public void MatchCountTest(string ean, int expectedMatchCount)
        {
            var matchResult = _treeUnderTest.Match(ean);

            matchResult.Matches.Count.Should().Be(expectedMatchCount);
            matchResult.MatchResultType.Should().Be(MatchResultType.Success);
        }

        [TestCase("9780345548566", "978", "0", "345")]
        [TestCase("9781210548566", "978", "12")]
        public void CorrectMatchPrefixValues(string ean, params string[] matches)
        {
            var matchResult = _treeUnderTest.Match(ean);
            for (var i = 0; i < matchResult.Matches.Count; i++)
            {
                matchResult.Matches[i].Prefix.Should().Be(matches[i]);
            }
        }

        [TestCase("9781210548566", "International ISBN Agency", "International ISBN Agency")]
        [TestCase("9780345548566", "International ISBN Agency","International ISBN Agency", "English language")]
        public void CorrectMatchPrefixAgency(string ean, params string[] agencyNames)
        {
            var matchResult = _treeUnderTest.Match(ean);
            for (var i = 0; i < matchResult.Matches.Count; i++)
            {
                matchResult.Matches[i].Agency.Should().Be(agencyNames[i]);
            }
        }

        [Test]
        public void NoMatches()
        {
            var matchResult = _treeUnderTest.Match("1230345548566");

            matchResult.MatchResultType.Should().Be(MatchResultType.NoMatchesFound);
        }

        [Test]
        public void AddChildTest()
        {
            var tree = new PrefixTree("978", 3, "International ISBN Agency",
                new[]
                {
                    new RangeRule(0, 4, 1),
                    new RangeRule(11, 22, 2)
                });

            bool addResult = tree.AddChild("0", 1, "English language",
                new[]
                {
                    new RangeRule(00, 12, 2),
                    new RangeRule(229, 368, 3)
                });

            addResult.Should().BeTrue();
        }

        [Test]
        public void AddTwoChildrenTest()
        {
            var tree = new PrefixTree("978", 3, "International ISBN Agency",
                new[]
                {
                    new RangeRule(0, 4, 1),
                    new RangeRule(11, 22, 2)
                });

            //First child
            bool addResult = tree.AddChild("0", 1, "English language",
                new[]
                {
                    new RangeRule(00, 12, 2),
                    new RangeRule(229, 368, 3)
                });
            addResult.Should().BeTrue();

            //Second child
            addResult = tree.AddChild("1", 1, "English language",
                new[]
                {
                    new RangeRule(000, 009, 3),//TODO: Check this range
                    new RangeRule(01, 06, 2)
                });

            addResult.Should().BeTrue();
        }
    }
}