namespace AcmeCorpBusiness.Entities
{
    public class Order
    {
        public int Id { get; set; } // Primary Key
        public string? ProductName { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; } // Many to one
    }
}
