namespace NCMENERGY.Dtos
{
    public class MailDto
    {
        public required string RecipientEmail { get; set; }
        public required string Subject { get; set; }
        public required string HtmlBody { get; set; }
    }
}
