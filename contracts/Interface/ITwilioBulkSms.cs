namespace Eagles_Portal.contracts.Interface
{
    public interface ITwilioBulkSms
    {
       
            Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message);
            Task<bool> SendSmsAsync(string phoneNumber, string message);
        

    }
}
