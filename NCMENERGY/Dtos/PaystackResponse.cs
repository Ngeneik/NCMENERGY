using System.Text.Json.Serialization;

namespace NCMENERGY.Dtos
{
    public class PaystackResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public PaystackData Data { get; set; }
    }

    public class PaystackData
    {
        [JsonPropertyName("authorization_url")]
        public string AuthorizationUrl { get; set; }

        [JsonPropertyName("access_code")]
        public string AccessCode { get; set; }

        public string Reference { get; set; }
    }

    public class PaystackVerifyResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public PaystackVerifyData Data { get; set; }
    }

    public class PaystackVerifyData
    {
        public string Status { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string GatewayResponse { get; set; }
    }
}


