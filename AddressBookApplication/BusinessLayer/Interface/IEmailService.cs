namespace BusinessLayer.Interface
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string subject, string token);
    }
}
