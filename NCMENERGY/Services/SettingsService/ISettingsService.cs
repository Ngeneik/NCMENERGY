using NCMENERGY.Response;

namespace NCMENERGY.Services.SettingsService
{
    public interface ISettingsService
    {
        Task<GenericResponse> ChangeStatus();
        Task<GenericResponse> GetStatus();
        Task<GenericResponse> AddSettings();
    }
}
