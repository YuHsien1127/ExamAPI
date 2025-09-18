namespace ExamAPI.Dto.Request
{
    public class MaterialRequest
    {
        public string? MaterialName { get; set; }
        public decimal MaterialCost { get; set; }
        public int CurrentStock { get; set; }
    }
}
