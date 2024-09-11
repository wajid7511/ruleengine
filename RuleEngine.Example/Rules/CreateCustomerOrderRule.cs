using System;
using System.Data;
using RuleEngine.Abstractions;
using RuleEngine.Dtos;
using RuleEngine.Enums;

namespace RuleEngine.Example.Rules;

public class CreateCustomerOrderRule : IBasicRule, IRevertRule
{
    public RuleType RuleType => RuleType.CreateCustomerOrder;

    public ValueTask<DoAsyncResponse> DoAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Customer Order Created");
        return ValueTask.FromResult(new DoAsyncResponse(nextExecutableRule: null));
    }

    public ValueTask InitAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Customer Order Init");
        return ValueTask.CompletedTask;
    }

    public ValueTask RevertAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("I will revert Customer if required");
        return ValueTask.CompletedTask;
    }
}
