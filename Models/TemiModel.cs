using Newtonsoft.Json;

namespace Eagles_Portal.Models
{
    public class TemiModel
    {
        public class TermiiSmsResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            [JsonProperty("message_id")]
            public string MessageId { get; set; }
            public string To { get; set; }
            public string From { get; set; }
            public string Sms { get; set; }
            public string Status { get; set; }
            public decimal Balance { get; set; }
        }

        public class TermiiBulkSmsResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            [JsonProperty("message_id")]
            public string MessageId { get; set; }
            public string From { get; set; }
            public string Sms { get; set; }
            public string Status { get; set; }
            public decimal Balance { get; set; }
            public List<string> To { get; set; }
        }

        public class TermiiBalanceResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string User { get; set; }
            public decimal Balance { get; set; }
            public string Currency { get; set; }
        }
    }
}
