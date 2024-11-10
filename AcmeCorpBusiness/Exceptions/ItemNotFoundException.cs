namespace AcmeCorpBusiness.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public string? ItemName { get; }
        public int? ItemId { get; }

        public ItemNotFoundException()
            : base("The specified item was not found.")
        {
        }

        public ItemNotFoundException(string message)
            : base(message)
        {
        }

        public ItemNotFoundException(string itemName, int? itemId = null)
            : base($"Item '{itemName}' {(itemId.HasValue ? $"with ID {itemId}" : string.Empty)} was not found.")
        {
            ItemName = itemName;
            ItemId = itemId;
        }

        public ItemNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
