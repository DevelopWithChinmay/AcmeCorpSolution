using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusiness.Domain.Orders
{
    public class OrderWriteHandler(AcmeCorpDbContext context)
    {
        public async Task<OrderFacade> WriteAsync(OrderFacade orderFacade) =>
            await (orderFacade.Id is null || orderFacade.Id <= 0 ? Add(orderFacade) : Update(orderFacade));

        private async Task<OrderFacade> Add(OrderFacade orderFacade)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(x => x.Id == orderFacade.CustomerId)
                ?? throw new ItemNotFoundException(nameof(Customer), orderFacade.CustomerId);

            var order = orderFacade.Adapt<Order>();
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            return order.Adapt<OrderFacade>();
        }

        private async Task<OrderFacade> Update(OrderFacade orderFacade)
        {
            var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == orderFacade.Id)
                ?? throw new ItemNotFoundException(nameof(Order), orderFacade.Id);

            // Specify only fields, which can be updated.
            // CustomerId and OrderDate for example should not be allowed to be updated, for an order
            order.TotalAmount = orderFacade.TotalAmount;
            order.ProductName = orderFacade.ProductName;

            context.Orders.Update(order);
            await context.SaveChangesAsync();
            return order.Adapt<OrderFacade>();
        }
    }
}
