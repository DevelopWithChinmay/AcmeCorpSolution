namespace AcmeCorpBusiness.Entities
{
    public class ContactInfo(string email, string phone)
    {
        public string Email { get; private set; } = email;
        public string Phone { get; private set; } = phone;
    }
}
