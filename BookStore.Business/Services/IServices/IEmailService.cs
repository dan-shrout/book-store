namespace BookStore.Business.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent);
        Task<bool> SendOrderConfirmationEmailAsync(string toEmail, int orderId, decimal orderTotal);
    }
}
