using RuleEngine.Abstractions;
using RuleEngine.Dtos;
using RuleEngine.Enums;

namespace RuleEngine.Tests.Dtos;

public class RuleEngineResponseTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeWithProvidedRuleType_WhenArgumentIsProvided()
    {
        // Arrange
        var expectedIsSuccess = true;
        var rule = new TestRule();
        List<KeyValuePair<RuleType, IBasicRule>> history = [];
        history.Add(new KeyValuePair<RuleType, IBasicRule>(RuleType.CreateCustomerOrder, rule));
        // Act
        var response = new RuleEngineResponse(expectedIsSuccess, history);

        // Assert
        Assert.AreEqual(expectedIsSuccess, response.IsSuccess);
        Assert.AreEqual(1, response.History.Count);
        Assert.AreEqual(RuleType.CreateCustomerOrder, response.History.First().Key);
        Assert.AreEqual(rule, response.History.First().Value);
    }

}
public class TestRule : IBasicRule
{
    public RuleType RuleType => throw new NotImplementedException();

    public ValueTask<DoAsyncResponse> DoAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask InitAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

