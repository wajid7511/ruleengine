

using RuleEngine.Dtos;
using RuleEngine.Enums;

namespace RuleEngine.Abstractions;

public interface IRevertRule
{
    public ValueTask RevertAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default);
}
