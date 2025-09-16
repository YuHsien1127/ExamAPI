namespace ExamAPI.Dto.Request
{
    public class UserRequest
    {
        public string UserAccount { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
    }
}
