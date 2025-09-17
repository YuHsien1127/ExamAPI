using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;

namespace ExamAPI.Services
{
    public interface IBomService
    {
        public BomResponse GetAllBoms(int page, int pageSize);
        public Task<BomResponse> AddBomAsync(BomRequest bomRequest);
        public Task<BomResponse> UpdateBomAsync(int serialNo, BomRequest bomRequest);
        public Task<BomResponse> DeleteBomAsync(int serialNo);
    }
}
