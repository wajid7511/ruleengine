using Microsoft.Extensions.Logging;
using RuleEngine.Abstractions;
using RuleEngine.Dtos;
using RuleEngine.Enums;
using RuleEngine.Exceptions;

namespace RuleEngine.Core;

public class DefaultRuleEngineManager(IEnumerable<IBasicRule> executionRules, ILogger<DefaultRuleEngineManager>? logger = null) : IRuleEngineManager
{
    private readonly IEnumerable<IBasicRule> _executionRules = executionRules ?? throw new ArgumentNullException(nameof(executionRules));
    private readonly ILogger<DefaultRuleEngineManager>? _logger = logger;
    public async ValueTask<RuleEngineResponse> ExecuteAsync(RuleEngineRequest request, CancellationToken cancellationToken = default, params RuleType[] rules)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (rules.Length == 0)
        {
            throw new RuleException("Rule should not be empty");
        }
        var history = new List<KeyValuePair<RuleType, IBasicRule>>();
        var isSuccess = true;
        try
        {
            for (int i = 0; i < rules.Length; i++)
            {
                var rule = _executionRules.FirstOrDefault((r) => r.RuleType == rules[i]) ?? throw new RuleException($"No rule found with name {rules[i]}");

                await rule.InitAsync(request, history, cancellationToken);
                var response = await rule.DoAsync(request, history, cancellationToken);
                var executedRule = new KeyValuePair<RuleType, IBasicRule>(rules[i], rule);
                history.Add(executedRule);
                if (response.NextExecutableRule != null)
                {
                    var index = Array.FindIndex(rules, (r) => r == response.NextExecutableRule);
                    if (index == -1)
                    {
                        throw new RuleException($"Next Rule {response.NextExecutableRule} not found in the parameters");
                    }
                    else
                    {
                        i = index - 1;
                    }
                }
            }
        }
        catch (RuleException ex)
        {
            isSuccess = false;
            _logger?.LogError(ex, "Rule Exception occured");
            throw;
        }
        catch (Exception ex)
        {
            isSuccess = false;
            _logger?.LogError(ex, "Exception occured");
            throw;
        }
        finally
        {
            if (isSuccess == false && history.Count > 0)
            {
                for (int i = history.Count - 1; i > -1; i--)
                {
                    if (history[i].Value is IRevertRule revertRule)
                    {
                        await revertRule.RevertAsync(request, history, cancellationToken);
                    }
                }
            }

        }
        return new RuleEngineResponse(isSuccess, history);
    }
}
