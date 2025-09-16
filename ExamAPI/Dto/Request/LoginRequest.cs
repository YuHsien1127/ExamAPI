namespace ExamAPI.Dto.Request
{
    public class LoginRequest
    {
        public string UserAccount { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
