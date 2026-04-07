using NCMENERGY.Services.AuthService;
using NCMENERGY.Services.DashboardService;
using NCMENERGY.Services.FileUploadService;
using NCMENERGY.Services.MailService;
using NCMENERGY.Services.OrderService;
using NCMENERGY.Services.Payment;
using NCMENERGY.Services.ProductService;
using NCMENERGY.Services.SettingsService;
using NCMENERGY.Services.UserProductService;

namespace NCMENERGY.Injections
{
    public static class ServiceInjections
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<IFileUploadService, FileUploadService>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISettingsService, SettingsService>();


            services.AddScoped<IUserProductService, UserProductService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUserProductService, UserProductService>();
            services.AddScoped<IMailService, MailService>();

        }
    }

}
