using AcmeCorpBusiness.Domain.Orders;
using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpApi.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerOrdersController : ControllerBase
    {
        [HttpGet("/{customerId}/orders")]
        public async Task<ActionResult<IEnumerable<OrderFacade>>> GetOrdersAsync([FromServices] OrderReadHandler handler, int customerId)
        {
            try
            {
                return Ok(await handler.QueryByCustomerIdAsync(customerId));
            }
            catch (ItemNotFoundException)
            {
                return new NotFoundResult();
            }
        }
    }
}
