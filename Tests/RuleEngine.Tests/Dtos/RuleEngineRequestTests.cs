using RuleEngine.Dtos;

namespace RuleEngine.Tests.Dtos;

public class RuleEngineRequestTests
{
    [TestMethod]
    public void RuleEngineRequest_No_Parameter_Provided()
    {
        // Act
        var response = new RuleEngineRequest();

        // Assert
        Assert.AreEqual(0, response.Parameters.Count);
    }

    [TestMethod]
    public void RuleEngineRequest_When_Parameter_Provided()
    {
        // Arrange

        Dictionary<string, object> parameters = new()
        {
            {"key", "value"}
        };

        // Act
        var request = new RuleEngineRequest
        {
            Parameters = parameters
        };

        // Assert
        Assert.AreEqual(1, request.Parameters.Count);
        Assert.AreEqual("value", (string)request.Parameters["key"]);
    }
}

