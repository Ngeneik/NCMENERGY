namespace NCMENERGY.Dtos
{
    public class CreateProductDto
    {
        public string? Category { get; set; }
        public string? BrandName { get; set; }
        public string? ProductName { get; set; }
        public decimal ReviewPoint { get; set; }
        public int ReviewCount { get; set; }
        public decimal Price { get; set; }
        public int? PercentOff { get; set; }
        public int? Stock { get; set; }
        public bool InStock { get; set; }
        public string? Warranty { get; set; }
        public string? Description { get; set; }
        public string? InstallationNote { get; set; }
        public string? Related { get; set; }
        public IFormFile? ThumbNail { get; set; }

        public List<IFormFile> Images { get; set; } = new();

        // Change here
        public List<string>? Specifications { get; set; }
    }

    public class SpecificationDto
    {
        public string? metrics { get; set; }
        public string? value { get; set; }
    }
}
