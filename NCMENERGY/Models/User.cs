using Microsoft.AspNetCore.Identity;

namespace NCMENERGY.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
