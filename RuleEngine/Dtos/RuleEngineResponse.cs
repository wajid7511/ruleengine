using RuleEngine.Abstractions;
using RuleEngine.Enums;

namespace RuleEngine.Dtos;

public class RuleEngineResponse(bool isSuccess, List<KeyValuePair<RuleType, IBasicRule>> history)
{
    public bool IsSuccess { get; private set; } = isSuccess;
    public List<KeyValuePair<RuleType, IBasicRule>> History { get; private set; } = history;
}
