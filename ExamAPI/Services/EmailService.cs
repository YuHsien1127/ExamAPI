using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace ExamAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<EmailResponse> SendEmailAsync(EmailRequest request)
        {
            _logger.LogTrace("進入 SendEmailAsync，收件人：{ToEmail}", request.ToEmail);
            EmailResponse response = new EmailResponse();

            try
            {
                // 從設定檔讀取 Email 設定
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort");
                var enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl");
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                // 建立 SMTP 客戶端
                using var smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new NetworkCredential(username, password);

                // 建立郵件訊息
                using var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail, fromName);
                mailMessage.To.Add(new MailAddress(request.ToEmail, request.ToName));
                mailMessage.Subject = request.Subject;
                mailMessage.Body = request.Body;
                mailMessage.IsBodyHtml = request.IsHtml;

                // 寄送郵件
                await smtpClient.SendMailAsync(mailMessage);

                response.Success = true;
                response.Message = "郵件寄送成功";
                response.SentTime = DateTime.Now;

                _logger.LogInformation("郵件寄送成功至 {ToEmail}", request.ToEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "郵件寄送失敗");
                response.Success = false;
                response.Message = "郵件寄送失敗";
            }

            _logger.LogTrace("離開 SendEmailAsync");
            return response;
        }
    }
}
