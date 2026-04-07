using NCMENERGY.Dtos;
using NCMENERGY.Response;

namespace NCMENERGY.Services.ProductService
{
    public interface IProductService
    {
        Task<GenericResponse> ViewAllProducts();
        Task<GenericResponse> ViewProductById(Guid ProductId);
        Task<GenericResponse> CreateProduct(CreateProductDto request);
        Task<GenericResponse> EditProduct(Guid ProductId, CreateProductDto request);
        Task<GenericResponse> DeleteProduct(Guid ProductId);


    }
}
