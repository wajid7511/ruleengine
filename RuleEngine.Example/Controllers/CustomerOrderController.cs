using Microsoft.AspNetCore.Mvc;
using RuleEngine.Abstractions;
using RuleEngine.Dtos;
using RuleEngine.Enums;
using RuleEngine.Example.Dtos;

namespace RuleEngine.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOrderController(IRuleEngineManager ruleEngine) : ControllerBase
    {
        private readonly IRuleEngineManager _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
        [HttpPost("Create")]
        public async ValueTask<bool> PlaceOrder(CustomerOrderPostModel request, CancellationToken cancellationToken = default)
        {
            var response = await _ruleEngine.ExecuteAsync(new RuleEngineRequest()
            {
                Parameters = new Dictionary<string, object>()
                {
                    { "customerName", request.Name },
                    { "customerEmail", request.Email },
                    { "customerPhoneNumber", request.PhoneNumber },
                    { "customerAddress", request.Address },
                    { "customerItems", request.Items },
                }
            }, cancellationToken, RuleType.CreateCustomer, RuleType.CreateCustomerOrder);

            return response.IsSuccess;
        }
    }
}
