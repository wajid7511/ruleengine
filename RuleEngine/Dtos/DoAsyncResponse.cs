using RuleEngine.Enums;

namespace RuleEngine.Dtos;

public class DoAsyncResponse(RuleType? nextExecutableRule = null)
{
    public RuleType? NextExecutableRule { get; private set; } = nextExecutableRule;
}
