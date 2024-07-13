using FluentAssertions;
using NUnit.Framework;
using Ruzzie.Common.Types.Diagnostics;
using Ruzzie.Ean.ISBN;

namespace Ruzzie.Ean.UnitTests;

[TestFixture]
public class ISBNRangeMessageLoadMessageTests
{
    [Test]
    public void LoadFromEmbeddedResourceOk()
    {
        //Act
        var loadResult = ISBNRangeMessage.LoadFromEmbeddedResource();

        //Assert
        loadResult.Unwrap();
    }

    [Test]
    public void LoadXmlInTreeSmokeTest()
    {
        var isbnRangeMessage =
            ISBNRangeMessage.LoadFromFile(TestContext.CurrentContext.TestDirectory + "\\ISBNRangeMessage.xml");

        var ruleTrees = PrefixTreeBuilder.BuildFromISBNRangeMessage(isbnRangeMessage);

        ruleTrees.Count.Should().Be(2);
    }
}