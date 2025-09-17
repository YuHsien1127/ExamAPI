namespace ExamAPI.Dto.Response
{
    public class MaterialResponse : BaseResponse
    {
        public List<MaterialDto> Materials { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class MaterialDto
    {
        public string MaterialNo { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public decimal MaterialCost { get; set; }
        public int CurrentStock { get; set; }
    }
}
