namespace NCMENERGY.Response
{
    public class GenericResponse
    {
        public bool Success { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public object? Meta { get; set; }
    }
}
