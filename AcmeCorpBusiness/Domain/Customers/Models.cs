using AcmeCorpBusiness.Domain.Contacts;
using AcmeCorpBusiness.Domain.Orders;

namespace AcmeCorpBusiness.Domain.Customers
{
    public class CustomerFacade
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ContactDetailFacade ContactDetail { get; set; } = new ContactDetailFacade();
    }
}
