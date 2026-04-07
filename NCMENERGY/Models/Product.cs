namespace NCMENERGY.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Category { get; set; }
        public string? BrandName { get; set; }
        public string? ProductName { get; set; }
        public decimal ReviewPoint { get; set; }
        public int ReviewCount { get; set; }
        public decimal Price { get; set; }
        public decimal SlashedPrice { get; set; }
        public int? PercentOff { get; set; }
        public int? Stock { get; set; }
        public decimal? Yousave { get; set; }
        public bool InStock { get; set; }
        public string? Warranty { get; set; }
        public string? Description { get; set; }
        public string? InstallationNote { get; set; }
        public string? Related { get; set; }
        public string? ThumbNail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public List<ProductImage> Images { get; set; } = new();
        public List<Specification> Specifications { get; set; } = new();
        public List<OrderItem> OrderItems { get; set; } = new();


    }
}
