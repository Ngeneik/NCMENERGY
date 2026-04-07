namespace NCMENERGY.Models
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? OrderTag { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
        public Payment Payments { get; set; } = new();


    }
}
