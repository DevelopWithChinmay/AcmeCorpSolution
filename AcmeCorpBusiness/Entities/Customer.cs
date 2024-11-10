namespace AcmeCorpBusiness.Entities
{
    public class Customer
    {
        public int Id { get; set; } // Primary Key
        public string? Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MailingAddress { get; set; }
        public virtual List<Order>? Orders { get; set; }
    }
}
