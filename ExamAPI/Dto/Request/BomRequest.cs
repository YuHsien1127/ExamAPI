namespace ExamAPI.Dto.Request
{
    public class BomRequest
    {
        public string ProductNo { get; set; } = null!;
        public string MaterialNo { get; set; } = null!;
        public int MaterialUseQuantity { get; set; }
    }
}
