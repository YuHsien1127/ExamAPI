namespace ExamAPI.Dto.Request
{
    public class OrderRequest
    {
        public string OrderSubject { get; set; } = null!;
        public List<OrderDetailRequest> Details { get; set; } = new List<OrderDetailRequest>();
    }
    public class OrderDetailRequest
    {
        public string ProductNo { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
