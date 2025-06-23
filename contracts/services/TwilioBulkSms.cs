using Eagles_Portal.contracts.Interface;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Eagles_Portal.contracts.services
{
    public class TwilioBulkSms: ITwilioBulkSms
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public TwilioBulkSms(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _fromPhoneNumber = configuration["Twilio:FromPhoneNumber"];

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                return messageResource.Status != MessageResource.StatusEnum.Failed;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }

        public async Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message)
        {
            var tasks = phoneNumbers.Select(phoneNumber => SendSmsAsync(phoneNumber, message));
            var results = await Task.WhenAll(tasks);

            return results.All(result => result);
        }
    }
}
