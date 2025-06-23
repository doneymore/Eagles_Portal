using Eagles_Portal.contracts.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eagles_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwiloController : ControllerBase
    {
        private readonly ITwilioBulkSms _twilioSmsService;

        public TwiloController(ITwilioBulkSms twilioSmsService)
        {
            _twilioSmsService = twilioSmsService;
        }

        [HttpPost("send-bulk")]
        public async Task<IActionResult> SendBulkSms([FromBody] BulkSmsRequests request)
        {
            var result = await _twilioSmsService.SendBulkSmsAsync(request.PhoneNumbers, request.Message);

            if (result)
                return Ok(new { success = true, message = "Bulk SMS sent successfully" });

            return BadRequest(new { success = false, message = "Failed to send bulk SMS" });
        }

        public class BulkSmsRequests
        {
            public List<string> PhoneNumbers { get; set; }
            public string Message { get; set; }
        }
    }
}
