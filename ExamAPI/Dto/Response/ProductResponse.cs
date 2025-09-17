namespace ExamAPI.Dto.Response
{
    public class ProductResponse : BaseResponse
    {
        public List<ProductDto> Products { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
    }
    public class ProductDto
    {
        public string ProductNo { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
    }
}
