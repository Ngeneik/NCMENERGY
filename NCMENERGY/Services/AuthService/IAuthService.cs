using NCMENERGY.Dtos;
using NCMENERGY.Response;

namespace NCMENERGY.Services.AuthService
{
    public interface IAuthService
    {
        Task<GenericResponse> SignUp(SignUpDto request);
        Task<GenericResponse> Login(Login request);
    }
}
