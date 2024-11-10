using AcmeCorpBusiness.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusiness.Domain.Orders
{
    public class OrderReadHandler(AcmeCorpDbContext context)
    {
        public async Task<IEnumerable<OrderFacade>> QueryAsync() =>
            await Query().ProjectToType<OrderFacade>().ToListAsync();
        public async Task<OrderFacade?> GetByIdAsync(int id) =>
            (await Query().ProjectToType<OrderFacade>().FirstOrDefaultAsync(c => c.Id == id));
        public async Task<IEnumerable<OrderFacade>> QueryByCustomerIdAsync(int customerId) =>
            (await Query().Where(c => c.CustomerId == customerId).ProjectToType<OrderFacade>().ToListAsync());
        public async Task<OrderFacade?> QueryByCustomerAndOrderIdAsync(int customerId, int orderId) =>
            (await Query().Where(c => c.CustomerId == customerId && c.Id == orderId)
            .ProjectToType<OrderFacade>().FirstOrDefaultAsync());

        private DbSet<Order> Query()
        {
            return context.Orders;
        }
    }
}
