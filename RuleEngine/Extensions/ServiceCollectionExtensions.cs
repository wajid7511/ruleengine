using Microsoft.Extensions.DependencyInjection;
using RuleEngine.Abstractions;
using RuleEngine.Core;

namespace RuleEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRuleEngine(this IServiceCollection services)
    {
        services.AddScoped<IRuleEngineManager, DefaultRuleEngineManager>();
        return services;
    }
}
