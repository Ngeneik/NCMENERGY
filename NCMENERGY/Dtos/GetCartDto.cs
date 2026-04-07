namespace NCMENERGY.Dtos
{
    public class GetCartDto
    {
        public List<CartId>? CartIds { get; set; }
    }

    public class CartId
    {
        public Guid Id { get; set; }
    }
}
