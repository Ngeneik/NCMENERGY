namespace NCMENERGY.Models
{
    public class Specification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }   // navigation
        public List<ProductSpec> ProductSpecs { get; set; } = new();
    }

    public class ProductSpec
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SpecificationId { get; set; }
        public Specification? Specification { get; set; }

        public string? Metrics { get; set; }
        public string? Value { get; set; }
    }
}
