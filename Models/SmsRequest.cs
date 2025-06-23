using System.ComponentModel.DataAnnotations;

namespace Eagles_Portal.Models
{
    public class SmsRequest
    {

       
            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(160, ErrorMessage = "Message cannot exceed 160 characters")]
            public string Message { get; set; }

            public string SenderName { get; set; } = "YourApp";
        

    }
}
