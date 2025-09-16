namespace ExamAPI.Dto.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public string UserName { get; set; } = "";
        public string UserRole { get; set; } = "";
        public DateTime? LoginTime { get; set; }
        public string Token { get; set; } = "";
    }
}
