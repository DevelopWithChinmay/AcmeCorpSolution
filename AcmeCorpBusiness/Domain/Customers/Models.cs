using AcmeCorpBusiness.Domain.Contacts;

namespace AcmeCorpBusiness.Domain.Customers
{
    public record CustomerFacade
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ContactDetailFacade ContactDetail { get; set; } = new ContactDetailFacade();
    }
}
