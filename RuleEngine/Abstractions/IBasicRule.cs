using RuleEngine.Dtos;
using RuleEngine.Enums;

namespace RuleEngine.Abstractions;

public interface IBasicRule
{
    public abstract RuleType RuleType { get; }
    public ValueTask InitAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default);
    public ValueTask<DoAsyncResponse> DoAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default);

}