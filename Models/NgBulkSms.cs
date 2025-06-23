using System.ComponentModel.DataAnnotations;

namespace Eagles_Portal.Models
{
   
 
        public class BulkSmsRequest
        {
            [Required]
            public List<string> PhoneNumbers { get; set; } = new();

            [Required]
            [StringLength(160, ErrorMessage = "Message cannot exceed 160 characters")]
            public string Message { get; set; }

            public string SenderName { get; set; } = "YourApp";
        }

        public class SmsResponse
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public string ResponseCode { get; set; }
            public string TransactionId { get; set; }
            public int CreditUsed { get; set; }
            public List<SmsDeliveryStatus> DeliveryStatus { get; set; } = new();
        }

        public class SmsDeliveryStatus
        {
            public string PhoneNumber { get; set; }
            public string Status { get; set; }
            public string MessageId { get; set; }
        }

        public class SmsConfiguration
        {
            public string ApiUrl { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string ApiKey { get; set; }
            public string DefaultSender { get; set; }
            public int TimeoutSeconds { get; set; } = 30;
        }

        public class BalanceResponse
        {
            public bool IsSuccess { get; set; }
            public decimal Balance { get; set; }
            public string Currency { get; set; }
            public string Message { get; set; }
        }
    
}
