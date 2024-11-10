using AcmeCorpBusiness.Domain.Orders;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpApi.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerOrdersController : ControllerBase
    {
        [HttpGet("/{customerId}/orders")]
        public async Task<IEnumerable<OrderFacade>> GetOrdersAsync([FromServices] OrderReadHandler handler, int customerId)
        {
            return await handler.QueryByCustomerIdAsync(customerId);
        }
    }
}
