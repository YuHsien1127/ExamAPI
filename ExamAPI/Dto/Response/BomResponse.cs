namespace ExamAPI.Dto.Response
{
    public class BomResponse : BaseResponse
    {
        public List<BomDto> Boms { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
    }
    public class BomDto
    {
        public int SerialNo { get; set; }
        public string ProductNo { get; set; } = null!;
        public ProductDto? Product { get; set; }
        public string MaterialNo { get; set; } = null!;
        public MaterialDto? Material { get; set; }
        public int MaterialUseQuantity { get; set; }
    }
}
