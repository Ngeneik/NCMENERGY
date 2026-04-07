namespace NCMENERGY.Models
{
    public class Settings
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        public bool AllowRegistrations { get; set; } = true;
    }
}
