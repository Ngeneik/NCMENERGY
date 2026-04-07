using NCMENERGY.Response;

namespace NCMENERGY.Services.OrderService
{
    public interface IOrderService
    {
        Task<GenericResponse> GetAllOrders();
        Task<GenericResponse> GetOrderById(Guid OrderId);
    }
}
