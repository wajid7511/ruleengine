using RuleEngine.Dtos;
using RuleEngine.Enums;

namespace RuleEngine.Tests.Dtos;

[TestClass]
public class DoAsyncResponseTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeWithNull_WhenNoArgumentsProvided()
    {
        // Act
        var response = new DoAsyncResponse();

        // Assert
        Assert.IsNull(response.NextExecutableRule);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeWithProvidedRuleType_WhenArgumentIsProvided()
    {
        // Arrange
        var expectedRuleType = RuleType.CreateCustomerOrder;

        // Act
        var response = new DoAsyncResponse(expectedRuleType);

        // Assert
        Assert.AreEqual(expectedRuleType, response.NextExecutableRule);
    }
}

