namespace ExamAPI.Dto.Request
{
    public class EmailRequest
    {        
        public string ToEmail { get; set; } = null!; // 收件人電子郵件（必填）
        public string ToName { get; set; } = ""; // 收件人姓名
        public string Subject { get; set; } = null!; // 郵件主旨（必填）
        public string Body { get; set; } = null!; // 郵件內容（必填）
        public bool IsHtml { get; set; } = true; // 是否為 HTML 格式郵件
    }
}
