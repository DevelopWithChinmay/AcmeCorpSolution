using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusiness.Domain.Customers
{
    public class CustomerReadHandler(AcmeCorpDbContext context)
    {
        public async Task<IEnumerable<CustomerFacade>> QueryAsync() =>
            (await Query().ToListAsync())
            .Adapt<IEnumerable<CustomerFacade>>();
        
        public async Task<CustomerFacade> GetByIdAsync(int id) =>
            (await Query().FirstOrDefaultAsync(c => c.Id == id) ?? throw new ItemNotFoundException(nameof(Customer), id))
            .Adapt<CustomerFacade>();

        private IQueryable<Customer> Query() => context.Customers.Include(c => c.Orders);
    }
}
