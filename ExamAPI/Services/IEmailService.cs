using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;

namespace ExamAPI.Services
{
    public interface IEmailService
    {
        public Task<EmailResponse> SendEmailAsync(EmailRequest request);
    }
}
