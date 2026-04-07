using NCMENERGY.Response;

namespace NCMENERGY.Services.DashboardService
{
    public interface IDashBoardService
    {
        Task<GenericResponse> GetCards();
        Task<GenericResponse> GetResentOrders();
    }
}
