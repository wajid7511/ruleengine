using RuleEngine.Abstractions;
using RuleEngine.Dtos;
using RuleEngine.Enums;

namespace RuleEngine.Example.Rules;

public class CreateCustomerRule : IBasicRule
{
    public RuleType RuleType => RuleType.CreateCustomer;

    public ValueTask<DoAsyncResponse> DoAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Customer Created");
        return ValueTask.FromResult(new DoAsyncResponse(nextExecutableRule: null));
    }

    public ValueTask InitAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Customer Creation Init");
        return ValueTask.CompletedTask;
    }
}
