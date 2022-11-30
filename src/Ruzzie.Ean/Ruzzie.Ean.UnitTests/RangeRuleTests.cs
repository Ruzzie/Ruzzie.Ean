using FluentAssertions;
using NUnit.Framework;

namespace Ruzzie.Ean.UnitTests;

[TestFixture]
public class RangeRuleTests
{
    [Test]
    public void IsMatchTrue()
    {
        var rule = new RangeRule(0, 5, 1);

        rule.IsMatch("2").Should().BeTrue();
    }

    [Test]
    public void IsMatchFalse()
    {
        var rule = new RangeRule(0, 5, 1);

        rule.IsMatch("6").Should().BeFalse();
    }

    [Test]
    public void IsMatchWhenFromAndToAreSame()
    {
        var rule = new RangeRule(65, 65, 2);

        rule.IsMatch("65").Should().BeTrue();
    }


    [TestCase("5",   true)]
    [TestCase("55",  true)]
    [TestCase("0",   true)]
    [TestCase("09",  true)]
    [TestCase("090", true)]
    [TestCase("-1",  false)]
    public void IsMatchStrSingleDigitTests(string input, bool expected)
    {
        var rule = new RangeRule(0, 5, 1);

        rule.IsMatch(input).Should().Be(expected);
    }

    [TestCase("80",  true)]
    [TestCase("83",  true)]
    [TestCase("89",  true)]
    [TestCase("94",  true)]
    [TestCase("5",   false)]
    [TestCase("55",  false)]
    [TestCase("0",   false)]
    [TestCase("95",  false)]
    [TestCase("100", false)]
    [TestCase("080", false)]
    public void IsMatchDoubleDigitTests(string input, bool expected)
    {
        var rule = new RangeRule(80, 94, 2);

        rule.IsMatch(input).Should().Be(expected);
    }
}