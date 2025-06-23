using Eagles_Portal.contracts.Interface;
using Eagles_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace Eagles_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NgBulkSmsController : ControllerBase
    {
        private readonly INgBulk _smsService;
        private readonly ILogger<NgBulkSmsController> _logger;

        public NgBulkSmsController(INgBulk smsService, ILogger<NgBulkSmsController> logger)
        {
            _smsService = smsService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<SmsResponse>> SendSingleSms([FromBody] SmsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _smsService.SendSingleSmsAsync(request);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber}", request.PhoneNumber);
                    return Ok(result);
                }

                _logger.LogWarning("Failed to send SMS to {PhoneNumber}: {Message}", request.PhoneNumber, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendSingleSms endpoint");
                return StatusCode(500, new SmsResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPost("bulk")] //test project
        public async Task<ActionResult<SmsResponse>> SendBulkSms([FromBody] BulkSmsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!request.PhoneNumbers.Any())
            {
                return BadRequest(new SmsResponse
                {
                    IsSuccess = false,
                    Message = "At least one phone number is required"
                });
            }

            try
            {
                var result = await _smsService.SendBulkSmsAsync(request);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Bulk SMS sent successfully to {Count} numbers", request.PhoneNumbers.Count);
                    return Ok(result);
                }

                _logger.LogWarning("Failed to send bulk SMS: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendBulkSms endpoint");
                return StatusCode(500, new SmsResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpGet("balance")]
        public async Task<ActionResult<BalanceResponse>> CheckBalance()
        {
            try
            {
                var result = await _smsService.CheckBalanceAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking balance");
                return StatusCode(500, new BalanceResponse
                {
                    IsSuccess = false,
                    Message = "Failed to check balance"
                });
            }
        }

        [HttpPost("template")]
        public async Task<ActionResult<SmsResponse>> SendTemplateMessage([FromBody] TemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _smsService.SendSmsWithTemplateAsync(
                    request.PhoneNumber,
                    request.TemplateName,
                    request.Parameters);

                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending template message");
                return StatusCode(500, new SmsResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPost("validate")]
        public async Task<ActionResult<ValidationResponse>> ValidatePhoneNumber([FromBody] ValidationRequest request)
        {
            try
            {
                var isValid = await _smsService.ValidatePhoneNumberAsync(request.PhoneNumber);

                return Ok(new ValidationResponse
                {
                    IsValid = isValid,
                    PhoneNumber = request.PhoneNumber,
                    Message = isValid ? "Valid phone number" : "Invalid phone number format"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating phone number");
                return StatusCode(500, new ValidationResponse
                {
                    IsValid = false,
                    Message = "Validation failed"
                });
            }
        }


        public class TemplateRequest
        {
            [Required]
            public string PhoneNumber { get; set; }

            [Required]
            public string TemplateName { get; set; }

            public Dictionary<string, string> Parameters { get; set; } = new();
        }

        public class ValidationRequest
        {
            [Required]
            public string PhoneNumber { get; set; }
        }

        public class ValidationResponse
        {
            public bool IsValid { get; set; }
            public string PhoneNumber { get; set; }
            public string Message { get; set; }
        }
    }

}

