using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusiness.Domain.Orders
{
    public class OrderReadHandler(AcmeCorpDbContext context)
    {
        public async Task<IEnumerable<OrderFacade>> QueryByCustomerIdAsync(int customerId)
        {
            await ValidateCustomerExists(customerId);
            return (await Query().Where(c => c.CustomerId == customerId).ToListAsync())
                .Adapt<IEnumerable<OrderFacade>>();
        }
            

        public async Task<OrderFacade> GetByIdAsync(int id) =>
            (await Query().FirstOrDefaultAsync(c => c.Id == id) ?? throw new ItemNotFoundException(nameof(Order), id))
            .Adapt<OrderFacade>();

        private async Task ValidateCustomerExists(int customerId)
        {
            if (await context.Customers.FirstOrDefaultAsync(c => c.Id == customerId) is null)
            {
                throw new ItemNotFoundException(nameof(Customer), customerId);
            }
        }
            
        private DbSet<Order> Query()
        {
            return context.Orders;
        }
    }
}
