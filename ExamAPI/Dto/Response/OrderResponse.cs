namespace ExamAPI.Dto.Response
{
    public class OrderResponse : BaseResponse
    {
        public List<OrderDto> Orders { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
    }
    public class OrderDto
    {
        public string OrderNo { get; set; } = null!;
        public string OrderSubject { get; set; } = null!;
        public string OrderApplicant { get; set; } = null!;
        public string Status { get; set; } = null!;
        public List<OrderDetailDTO> orderDetails { get; set; } = new List<OrderDetailDTO>();
    }
    public class OrderDetailDTO
    {
        public int SerialNo { get; set; }
        public string OrderNo { get; set; } = null!;
        public string ProductNo { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
