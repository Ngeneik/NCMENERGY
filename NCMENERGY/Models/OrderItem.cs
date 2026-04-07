namespace NCMENERGY.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
