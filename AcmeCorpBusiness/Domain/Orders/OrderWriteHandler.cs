using AcmeCorpBusiness.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusiness.Domain.Orders
{
    public class OrderWriteHandler(AcmeCorpDbContext context)
    {
        public async Task<OrderFacade> WriteAsync(OrderFacade orderFacade)
        {
            if (orderFacade.Id <= 0)
            {
                return await Add(orderFacade);
            }
            else
            {
                return await Update(orderFacade);
            }
        }

        private async Task<OrderFacade> Add(OrderFacade orderFacade)
        {
            var order = orderFacade.Adapt<Order>();
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            return order.Adapt<OrderFacade>();
        }
        
        private async Task<OrderFacade> Update(OrderFacade orderFacade)
        {
            if (await context.Orders.FirstOrDefaultAsync(o => o.Id == orderFacade.Id) is { } order)
            {
                // Specify only fields, which can be updated.
                // CustomerId and OrderDate for example should not be allowed to be updated, for an order
                order.TotalAmount = orderFacade.TotalAmount;
                order.ProductName = orderFacade.ProductName;

                context.Orders.Update(order);
                await context.SaveChangesAsync();
                return order.Adapt<OrderFacade>();
            };

            return orderFacade;
        }
    }
}
