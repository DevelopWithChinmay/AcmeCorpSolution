using AcmeCorpBusiness.Domain.Orders;
using AcmeCorpBusiness.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(OrderWriteHandler orderWriteHandler) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderFacade>> GetOrder([FromServices] OrderReadHandler handler, int id)
        {
            try
            {
                return (await handler.GetByIdAsync(id))!;
            }
            catch (ItemNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        [HttpPost]
        public Task<ActionResult<OrderFacade>> AddOrder(OrderFacade order) => WriteOrder(order);

        [HttpPut("id")]
        public Task<ActionResult<OrderFacade>> UpdateOrder(OrderFacade order) => WriteOrder(order);

        private async Task<ActionResult<OrderFacade>> WriteOrder(OrderFacade order)
        {
            try
            {
                return await orderWriteHandler.WriteAsync(order);
            }
            catch (ItemNotFoundException)
            {
                return new NotFoundResult();
            }
        }
    }
}
