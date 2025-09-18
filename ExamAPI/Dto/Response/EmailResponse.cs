namespace ExamAPI.Dto.Response
{
    public class EmailResponse : BaseResponse
    {
        public string? MessageId { get; set; }
        public DateTime SentTime { get; set; }
    }
}
