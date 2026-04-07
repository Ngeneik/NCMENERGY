using Microsoft.EntityFrameworkCore;
using NCMENERGY.Data;
using NCMENERGY.Models;
using NCMENERGY.Response;

namespace NCMENERGY.Services.SettingsService
{
    public class SettingsService : ISettingsService
    {
        private readonly ApplicationDbContext _context;

        public SettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse> AddSettings()
        {
            var newSettings = new Settings
            {
                AllowRegistrations = false
            };

            _context.Settings.Add(newSettings);
            await _context.SaveChangesAsync();

            return new GenericResponse
            {
                Success = true,
                Data = newSettings,
                Message = "Settings added successfully"
            };
        }

        public async Task<GenericResponse> ChangeStatus()
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();

            if (settings == null)
            {
                // create default settings if none exist
                settings = new Settings
                {
                    AllowRegistrations = false
                };
                _context.Settings.Add(settings);
            }
            else
            {
                // toggle the current value
                settings.AllowRegistrations = !settings.AllowRegistrations;
            }

            await _context.SaveChangesAsync();

            return new GenericResponse
            {
                Success = true,
                Data = new
                {
                    settings.AllowRegistrations
                },
                Message = "Settings updated successfully"
            };
        }

        public async Task<GenericResponse> GetStatus()
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            return new GenericResponse
            {
                Success = true,
                Data = settings
            };
        }
    }
}
