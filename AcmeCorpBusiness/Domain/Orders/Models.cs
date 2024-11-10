namespace AcmeCorpBusiness.Domain.Orders
{
    public class OrderFacade
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int CustomerId { get; set; }
    }
}
