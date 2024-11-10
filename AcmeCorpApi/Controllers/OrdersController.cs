using AcmeCorpBusiness.Domain.Orders;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderFacade>> GetOrder([FromServices] OrderReadHandler handler, int id)
        {
            return (await handler.GetByIdAsync(id) is { } result)
                ? result
                : new NotFoundResult();
        }

        [HttpPost]
        public async Task<ActionResult<OrderFacade>> AddOrder([FromServices] OrderWriteHandler handler, OrderFacade Order)
        {
            return await handler.WriteAsync(Order);
        }

        [HttpPut("id")]
        public async Task<ActionResult<OrderFacade>> UpdateOrder([FromServices] OrderWriteHandler handler, OrderFacade Order)
        {
            return await handler.WriteAsync(Order);
        }
    }
}
