using RuleEngine.Dtos;
using RuleEngine.Enums;

namespace RuleEngine.Abstractions;

public interface IRuleEngineManager
{
    public ValueTask<RuleEngineResponse> ExecuteAsync(RuleEngineRequest request, CancellationToken cancellationToken = default, params RuleType[] rules);
}
