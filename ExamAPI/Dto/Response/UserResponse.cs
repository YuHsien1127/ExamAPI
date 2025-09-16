namespace ExamAPI.Dto.Response
{
    public class UserResponse : BaseResponse
    {
        public List<UserDto> Users { get; set; }
    }
    public class UserDto
    {
        public int SerialNo { get; set; }
        public string UserAccount { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
    }
}
