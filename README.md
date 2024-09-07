# RuleEngine

The RuleEngine is a flexible and modular system designed to execute business rules in a structured and efficient manner. It supports the dynamic execution of rules based on input parameters, with built-in error handling, logging, and rule chaining capabilities. The engine allows developers to define and manage rules independently, enabling easy updates and maintenance. Ideal for scenarios requiring conditional logic, validation, or complex decision-making processes.

## Features

- **Dynamic Rule Execution**: Execute rules based on provided parameters and conditions.
- **Error Handling**: Robust error handling with custom exceptions and logging.
- **Rule Chaining**: Seamlessly chain rules together, allowing for complex logic flows.
- **Easy Integration**: Simple to integrate and extend with custom rules.
- **Reversion Support**: Revert actions when a rule fails, ensuring system stability.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

Clone the repository:

```bash
git clone https://github.com/wajid7511/ruleengine.git
cd ruleengine
```

Build the solution:

```bash
dotnet build
```

### Usage

Here's a basic example of how to use the RuleEngine:

```csharp
using RuleEngine.Abstractions;
using RuleEngine.Core;
using RuleEngine.Dto;
using RuleEngine.Enums;

// Define your rule
public class ExampleRule : IBasicRule
{
    public RuleType RuleType => RuleType.ExampleRule;

    public Task InitAsync(RuleRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        // Initialization logic
        return Task.CompletedTask;
    }

    public Task<RuleResponse> ExecuteAsync(RuleRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        // Execution logic
        return Task.FromResult(new RuleResponse());
    }
}
// Define your rule1
public class ExampleRule1 : IBasicRule, IRevertRule
{
    public RuleType RuleType => RuleType.ExampleRule1;

    public Task InitAsync(RuleRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        // Initialization logic
        return Task.CompletedTask;
    }

    public Task<RuleResponse> ExecuteAsync(RuleRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
        // Execution logic
        return Task.FromResult(new RuleResponse());
    }
    public Task RevertAsync(RuleEngineRequest request, List<KeyValuePair<RuleType, IBasicRule>> history, CancellationToken cancellationToken = default)
    {
    
        // Initialization revert logic
        return Task.CompletedTask;
    }
}

// Initialize and execute the RuleEngine
var rules = new List<IBasicRule> { new ExampleRule(), new ExampleRule1() };
var ruleEngineManager = new DefaultRuleEngineManager(rules);
var request = new RuleEngineRequest { Parameters = new Dictionary<string, object>() };

var response = await ruleEngineManager.ExecuteAsync(request, default, RuleType.ExampleRule, RuleType.ExampleRule1);
```

### Running Tests

To run the unit tests, use the following command:

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please fork this repository, create a new branch, and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any inquiries or issues, please open an issue on GitHub or contact me at [your email address].

---

**Note**: This is a work-in-progress project, and improvements are ongoing. Feel free to contribute!
