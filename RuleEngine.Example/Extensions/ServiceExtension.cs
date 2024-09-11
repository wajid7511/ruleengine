using RuleEngine.Abstractions;
using RuleEngine.Example.Rules;
using RuleEngine.Extensions;

namespace RuleEngine.Example;

public static class ServiceExtensions
{
    public static IServiceCollection AddRuleEngineServices(this IServiceCollection services)
    {
        services.AddRuleEngine();
        services.AddScoped<IBasicRule, CreateCustomerRule>();
        services.AddScoped<IBasicRule, CreateCustomerOrderRule>();
        return services;
    }
}
