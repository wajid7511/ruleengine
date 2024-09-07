using System.Data;
using Microsoft.Extensions.Logging;
using Moq;
using RuleEngine.Abstractions;
using RuleEngine.Core;
using RuleEngine.Dtos;
using RuleEngine.Enums;
using RuleEngine.Exceptions;

namespace RuleEngine.Tests
{
    [TestClass]
    public class DefaultRuleEngineManagerTests
    {
        private readonly Mock<ILogger<DefaultRuleEngineManager>> _mockLogger = new();

        private DefaultRuleEngineManager _ruleEngineManager = null!;

        [TestInitialize]
        public void Setup()
        {
            // Initialize Rule Engine Manager with mock rules and logger
            _ruleEngineManager = new DefaultRuleEngineManager([], _mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act
            _ = await _ruleEngineManager.ExecuteAsync(null, CancellationToken.None, RuleType.CreateCustomerOrder);
        }

        [TestMethod]
        public void ExecuteAsync_ShouldThrowRuleException_WhenNoRulesProvided()
        {
            // Arrange
            var request = new RuleEngineRequest { Parameters = [] };

            // Act

            //Assert
            var exception = Assert.ThrowsException<RuleException>(() => _ruleEngineManager.ExecuteAsync(request, CancellationToken.None).GetAwaiter().GetResult());
            Assert.AreEqual("Rule should not be empty", exception.Message);
        }

        [TestMethod]
        public void ExecuteAsync_ShouldThrowRuleException_WhenRuleNotFound()
        {
            // Arrange
            var request = new RuleEngineRequest { Parameters = [] };

            // Act

            var exception = Assert.ThrowsException<RuleException>(() => _ruleEngineManager.ExecuteAsync(request, CancellationToken.None, RuleType.GetCustomerById).GetAwaiter().GetResult());
            Assert.AreEqual("No rule found with name GetCustomerById", exception.Message);
        }

        [TestMethod]
        public async Task ExecuteAsync_ShouldLogError_WhenRuleExceptionOccurs()
        {
            // Arrange
            var request = new RuleEngineRequest { Parameters = [] };

            // Setup mock rule
            List<IBasicRule> _mockRules = [];
            var mockRule = new Mock<IBasicRule>();
            mockRule.Setup(r => r.RuleType).Returns(RuleType.CreateCustomerOrder);
            _mockRules.Add(mockRule.Object);

            // Act & Assert
            try
            {
                await _ruleEngineManager.ExecuteAsync(request, CancellationToken.None, RuleType.CreateCustomerOrder);
            }
            catch (Exception)
            {
                // Verify that logger was called with error
                _mockLogger.Verify(
                    l => l.Log(
                        LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => true),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                    Times.Once);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync_ShouldReturnSuccessfulResponse_WhenAllRulesExecutedSuccessfully()
        {
            // Arrange
            var request = new RuleEngineRequest
            {
                Parameters = new Dictionary<string, object>(){
                { "key", "value" }
            }
            };
            var history = new List<KeyValuePair<RuleType, IBasicRule>>();

            // Setup mock rule 
            List<IBasicRule> _mockRules = [];
            var mockRule = new Mock<IBasicRule>();
            mockRule.Setup(r => r.RuleType).Returns(RuleType.CreateCustomerOrder);
            mockRule.Setup(r => r.InitAsync(It.Is<RuleEngineRequest>(r => r.Parameters.Count == 1 && (string)r.Parameters["key"] == "value"),
             It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);
            mockRule.Setup(r => r.DoAsync(It.Is<RuleEngineRequest>(r => r.Parameters.Count == 1 && (string)r.Parameters["key"] == "value"),
              It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new DoAsyncResponse());
            _mockRules.Add(mockRule.Object);

            var mockRule1 = new Mock<IBasicRule>();
            mockRule1.Setup(r => r.RuleType).Returns(RuleType.GetCustomerById);
            mockRule1.Setup(r => r.InitAsync(It.Is<RuleEngineRequest>(r => r.Parameters.Count == 1 && (string)r.Parameters["key"] == "value"),
              It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 1 && h.FirstOrDefault().Key == RuleType.CreateCustomerOrder), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);
            mockRule1.Setup(r => r.DoAsync(It.Is<RuleEngineRequest>(r => r.Parameters.Count == 1 && (string)r.Parameters["key"] == "value"),
              It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 1 && h.FirstOrDefault().Key == RuleType.CreateCustomerOrder), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new DoAsyncResponse());
            _mockRules.Add(mockRule1.Object);

            var ruleEngineManager = new DefaultRuleEngineManager(_mockRules, _mockLogger.Object);
            // Act
            var response = await ruleEngineManager.ExecuteAsync(request, CancellationToken.None, RuleType.CreateCustomerOrder, RuleType.GetCustomerById);

            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(2, response.History.Count);
            Assert.AreEqual(RuleType.CreateCustomerOrder, response.History.First().Key);
            Assert.AreEqual(RuleType.GetCustomerById, response.History.ElementAt(1).Key);
        }
        [TestMethod]
        public async Task ExecuteAsync_JumpTo_NextExecutableRule_Success()
        {
            // Arrange
            var request = new RuleEngineRequest { Parameters = [] };
            var history = new List<KeyValuePair<RuleType, IBasicRule>>();

            // Setup mock rule
            List<IBasicRule> _mockRules = [];
            var mockRule = new Mock<IBasicRule>();
            mockRule.Setup(r => r.RuleType).Returns(RuleType.CreateCustomerOrder);
            mockRule.Setup(r => r.InitAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);
            mockRule.Setup(r => r.DoAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new DoAsyncResponse(RuleType.MotifyMessageBroker));
            _mockRules.Add(mockRule.Object);

            var mockRule1 = new Mock<IBasicRule>();
            mockRule1.Setup(r => r.RuleType).Returns(RuleType.MotifyMessageBroker);
            mockRule1.Setup(r => r.InitAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 1 && h.FirstOrDefault().Key == RuleType.CreateCustomerOrder), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);
            mockRule1.Setup(r => r.DoAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 1 && h.FirstOrDefault().Key == RuleType.CreateCustomerOrder), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new DoAsyncResponse());
            _mockRules.Add(mockRule1.Object);

            var ruleEngineManager = new DefaultRuleEngineManager(_mockRules, _mockLogger.Object);
            // Act
            var response = await ruleEngineManager.ExecuteAsync(request, CancellationToken.None, RuleType.CreateCustomerOrder, RuleType.GetCustomerById, RuleType.MotifyMessageBroker);

            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(2, response.History.Count);
            Assert.AreEqual(RuleType.CreateCustomerOrder, response.History.First().Key);
            Assert.AreEqual(RuleType.MotifyMessageBroker, response.History.ElementAt(1).Key);
        }
        [TestMethod]
        public void ExecuteAsync_ShouldThrowRuleException_WhenNextExecutableRuleNotFound_Log()
        {

            // Arrange
            var request = new RuleEngineRequest { Parameters = [] };
            var history = new List<KeyValuePair<RuleType, IBasicRule>>();

            // Setup mock rule
            List<IBasicRule> _mockRules = [];
            var mockRule = new Mock<IBasicRule>();
            mockRule.Setup(r => r.RuleType).Returns(RuleType.CreateCustomerOrder);
            mockRule.Setup(r => r.InitAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);
            mockRule.Setup(r => r.DoAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new DoAsyncResponse(RuleType.GetProductsById));
            _mockRules.Add(mockRule.Object);


            var ruleEngineManager = new DefaultRuleEngineManager(_mockRules, _mockLogger.Object);
            // Act
            var exeception = Assert.ThrowsException<RuleException>(() => ruleEngineManager.ExecuteAsync(request, CancellationToken.None, RuleType.CreateCustomerOrder, RuleType.GetProductsById).GetAwaiter().GetResult());

            // Assert
            Assert.IsNotNull(exeception);
            Assert.AreEqual("No rule found with name GetProductsById", exeception.Message);
            _mockLogger.Verify(
                  l => l.Log(
                      LogLevel.Error,
                      It.IsAny<EventId>(),
                      It.Is<It.IsAnyType>((v, t) => true),
                      It.IsAny<Exception>(),
                      It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                  Times.Once);
        }

        [TestMethod]
        public void ExecuteAsync_Call_RevertAsync_When_Error_Occured_AndRevertAsync_Rule_Is_IRevertRule()
        {

            // Arrange
            var request = new RuleEngineRequest { Parameters = [] };
            var history = new List<KeyValuePair<RuleType, IBasicRule>>();

            // Setup mock rule
            List<IBasicRule> _mockRules = [];
            var mockRule = new Mock<ITestRule>();
            mockRule.Setup(r => r.RuleType).Returns(RuleType.CreateCustomerOrder);
            mockRule.Setup(r => r.InitAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);
            mockRule.Setup(r => r.DoAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new DoAsyncResponse());
            mockRule.Setup(s => s.RevertAsync(It.IsAny<RuleEngineRequest>(), It.Is<List<KeyValuePair<RuleType, IBasicRule>>>(h => h.Count == 0), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);
            _mockRules.Add(mockRule.Object);


            var ruleEngineManager = new DefaultRuleEngineManager(_mockRules, _mockLogger.Object);
            // Act
            var exeception = Assert.ThrowsException<RuleException>(() => ruleEngineManager.ExecuteAsync(request, CancellationToken.None, RuleType.CreateCustomerOrder, RuleType.GetProductsById).GetAwaiter().GetResult());

            // Assert
            Assert.IsNotNull(exeception);
            Assert.AreEqual("No rule found with name GetProductsById", exeception.Message);
            _mockLogger.Verify(
                  l => l.Log(
                      LogLevel.Error,
                      It.IsAny<EventId>(),
                      It.Is<It.IsAnyType>((v, t) => true),
                      It.IsAny<Exception>(),
                      It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                  Times.Once);
        }

    }
    public class ITestRule : IBasicRule, IRevertRule
    {
        public virtual RuleType RuleType => throw new NotImplementedException();

        public virtual ValueTask<DoAsyncResponse> DoAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask InitAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask RevertAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
