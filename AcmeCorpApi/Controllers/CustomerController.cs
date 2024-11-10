using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerFacade>>> GetCustomers([FromServices] CustomerReadHandler handler)
        {
            return Ok(await handler.QueryAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerFacade>> GetCustomer([FromServices] CustomerReadHandler handler, int id)
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
        public async Task<ActionResult<CustomerFacade>> AddCustomer([FromServices] CustomerWriteHandler handler, CustomerFacade customer)
        {
            return await handler.WriteAsync(customer);
        }

        [HttpPut("id")]
        public async Task<ActionResult<CustomerFacade>> UpdateCustomer([FromServices] CustomerWriteHandler handler, CustomerFacade customer)
        {
            try
            {
                return await handler.WriteAsync(customer);
            }
            catch (ItemNotFoundException)
            {
                return new NotFoundResult();
            }
        }
    }
}
