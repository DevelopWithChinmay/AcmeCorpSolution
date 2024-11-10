using AcmeCorpBusiness.Entities;
using AcmeCorpBusiness.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpBusiness.Domain.Customers
{
    public class CustomerWriteHandler(AcmeCorpDbContext context)
    {
        public async Task<CustomerFacade> WriteAsync(CustomerFacade customerFacade) =>
            await (customerFacade.Id <= 0 ? Add(customerFacade) : Update(customerFacade));

        private async Task<CustomerFacade> Add(CustomerFacade customerFacade)
        {
            var customer = customerFacade.Adapt<Customer>();
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            return customer.Adapt<CustomerFacade>();
        }
        private async Task<CustomerFacade> Update(CustomerFacade customerFacade)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(x => x.Id == customerFacade.Id)
                ?? throw new ItemNotFoundException(nameof(Customer), customerFacade.Id);

            // Specify only fields, which can be updated.
            // DoB for example should not be allowed to be updated for a Customer
            customer.Name = customerFacade.Name;
            customer.PhoneNumber = customerFacade.ContactDetail.PhoneNumber;
            customer.Email = customerFacade.ContactDetail.Email;
            customer.MailingAddress = customerFacade.ContactDetail.MailingAddress;

            context.Customers.Update(customer);
            await context.SaveChangesAsync();
            var updatedCusomter = customer.Adapt<CustomerFacade>();
            return updatedCusomter;
        } 
    }
}
