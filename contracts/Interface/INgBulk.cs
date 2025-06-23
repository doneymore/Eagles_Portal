using Eagles_Portal.Models;
namespace Eagles_Portal.contracts.Interface
{
    public interface INgBulk
    {
        
            Task<SmsResponse> SendSingleSmsAsync(SmsRequest request);
            Task<SmsResponse> SendBulkSmsAsync(BulkSmsRequest request);
            Task<BalanceResponse> CheckBalanceAsync();
            Task<bool> ValidatePhoneNumberAsync(string phoneNumber);
            Task<SmsResponse> SendSmsWithTemplateAsync(string phoneNumber, string templateName, Dictionary<string, string> parameters);
        
    }
}
