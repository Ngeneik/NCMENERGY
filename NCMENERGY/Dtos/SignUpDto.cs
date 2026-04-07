namespace NCMENERGY.Dtos
{
    public class SignUpDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }

    public class Login
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
