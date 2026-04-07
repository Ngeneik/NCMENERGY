using NCMENERGY.Dtos;
using NCMENERGY.Response;

namespace NCMENERGY.Services.UserProductService
{
    public interface IUserProductService
    {
        Task<GenericResponse> GetIndexProducts();
        Task<GenericResponse> BuildSystem();
        Task<GenericResponse> GetAllProducts();
        Task<GenericResponse> GetIndexProductById(Guid productId);
        Task<GenericResponse> GetRelatedProductsIndex(string relation);
        Task<GenericResponse> GetRelatedProducts(string relation);
        Task<GenericResponse> SearchQuery(string query);
        Task<GenericResponse> GetCartItems(GetCartDto request);


    }
}
