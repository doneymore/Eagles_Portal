using Eagles_Portal.contracts.Interface;
using Eagles_Portal.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;


namespace Eagles_Portal.contracts.services
{
    public class NgBulkService : INgBulk
    {
        private readonly HttpClient _httpClient;
        private readonly SmsConfiguration _config;
        private readonly ILogger<NgBulkService> _logger;

        public NgBulkService(
            HttpClient httpClient,
            IOptions<SmsConfiguration> config,
            ILogger<NgBulkService> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;

            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);
        }

        public async Task<SmsResponse> SendSingleSmsAsync(SmsRequest request)
        {
            try
            {
                var phoneNumber = FormatPhoneNumber(request.PhoneNumber);

                if (!await ValidatePhoneNumberAsync(phoneNumber))
                {
                    return new SmsResponse
                    {
                        IsSuccess = false,
                        Message = "Invalid phone number format"
                    };
                }

                var url = $"{_config.ApiUrl}/api/v2/sms";
                var payload = new
                {
                    username = _config.Username,
                    password = _config.Password,
                    sender = request.SenderName ?? _config.DefaultSender,
                    message = request.Message,
                    mobiles = phoneNumber
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending single SMS to {PhoneNumber}", phoneNumber);

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return ParseSmsResponse(responseContent, new[] { phoneNumber });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending single SMS");
                return new SmsResponse
                {
                    IsSuccess = false,
                    Message = $"Failed to send SMS: {ex.Message}"
                };
            }
        }


        public async Task<SmsResponse> SendBulkSmsAsync(BulkSmsRequest request)
        {
            try
            {
                var validNumbers = new List<string>();

                foreach (var number in request.PhoneNumbers)
                {
                    var formattedNumber = FormatPhoneNumber(number);
                    if (await ValidatePhoneNumberAsync(formattedNumber))
                    {
                        validNumbers.Add(formattedNumber);
                    }
                }

                if (!validNumbers.Any())
                {
                    return new SmsResponse
                    {
                        IsSuccess = false,
                        Message = "No valid phone numbers found"
                    };
                }

                var url = $"{_config.ApiUrl}/api/v1/sms/bulk";
                var payload = new
                {
                    username = _config.Username,
                    password = _config.Password,
                    sender = request.SenderName ?? _config.DefaultSender,
                    message = request.Message,
                    mobiles = string.Join(",", validNumbers)
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending bulk SMS to {Count} numbers", validNumbers.Count);

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return ParseSmsResponse(responseContent, validNumbers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk SMS");
                return new SmsResponse
                {
                    IsSuccess = false,
                    Message = $"Failed to send bulk SMS: {ex.Message}"
                };
            }
        }

        public async Task<BalanceResponse> CheckBalanceAsync()
        {
            try
            {
                var url = $"{_config.ApiUrl}/api/v1/balance?username={_config.Username}&password={_config.Password}";

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var balanceData = JsonSerializer.Deserialize<dynamic>(responseContent);
                    return new BalanceResponse
                    {
                        IsSuccess = true,
                        Balance = Convert.ToDecimal(balanceData.GetProperty("balance").GetString()),
                        Currency = "NGN",
                        Message = "Balance retrieved successfully"
                    };
                }

                return new BalanceResponse
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve balance"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking balance");
                return new BalanceResponse
                {
                    IsSuccess = false,
                    Message = $"Failed to check balance: {ex.Message}"
                };
            }
        }


        public async Task<bool> ValidatePhoneNumberAsync(string phoneNumber)
        {
            await Task.CompletedTask; // Simulate async operation

            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Nigerian phone number validation
            var cleanNumber = phoneNumber.Replace("+", "").Replace("-", "").Replace(" ", "");

            // Check for Nigerian formats: 234XXXXXXXXX, 0XXXXXXXXXX, XXXXXXXXXX
            return cleanNumber.Length >= 10 &&
                   (cleanNumber.StartsWith("234") || cleanNumber.StartsWith("0") || cleanNumber.Length == 10);
        }

        public async Task<SmsResponse> SendSmsWithTemplateAsync(string phoneNumber, string templateName, Dictionary<string, string> parameters)
        {
            // Simple template implementation
            var templates = new Dictionary<string, string>
            {
                ["welcome"] = "Welcome {name}! Your account has been created successfully.",
                ["otp"] = "Your OTP is {code}. Valid for 5 minutes.",
                ["reminder"] = "Hi {name}, this is a reminder about your {event} on {date}."
            };

            if (!templates.ContainsKey(templateName))
            {
                return new SmsResponse
                {
                    IsSuccess = false,
                    Message = "Template not found"
                };
            }

            var message = templates[templateName];
            foreach (var param in parameters)
            {
                message = message.Replace($"{{{param.Key}}}", param.Value);
            }

            var request = new SmsRequest
            {
                PhoneNumber = phoneNumber,
                Message = message
            };

            return await SendSingleSmsAsync(request);
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            var cleanNumber = phoneNumber.Replace("+", "").Replace("-", "").Replace(" ", "");

            // Convert to Nigerian format
            if (cleanNumber.StartsWith("0"))
            {
                cleanNumber = "234" + cleanNumber.Substring(1);
            }
            else if (!cleanNumber.StartsWith("234"))
            {
                cleanNumber = "234" + cleanNumber;
            }

            return cleanNumber;
        }

        private SmsResponse ParseSmsResponse(string responseContent, IEnumerable<string> phoneNumbers)
        {
            try
            {
                var response = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var isSuccess = response.GetProperty("status").GetString()?.ToLower() == "success";
                var message = response.GetProperty("message").GetString();
                var responseCode = response.TryGetProperty("code", out var code) ? code.GetString() : "";

                var deliveryStatus = phoneNumbers.Select(number => new SmsDeliveryStatus
                {
                    PhoneNumber = number,
                    Status = isSuccess ? "Sent" : "Failed",
                    MessageId = response.TryGetProperty("message_id", out var msgId) ? msgId.GetString() : ""
                }).ToList();

                return new SmsResponse
                {
                    IsSuccess = isSuccess,
                    Message = message,
                    ResponseCode = responseCode,
                    DeliveryStatus = deliveryStatus,
                    CreditUsed = phoneNumbers.Count() // Assuming 1 credit per SMS
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing SMS response");
                return new SmsResponse
                {
                    IsSuccess = false,
                    Message = "Failed to parse response"
                };
            }
        }
    }
}
