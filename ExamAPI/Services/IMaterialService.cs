using ExamAPI.Dto.Response;
using ExamAPI.Dto.Request;

namespace ExamAPI.Services
{
    public interface IMaterialService
    {
        public MaterialResponse GetAllMaterials(int page, int pageSize);
        public Task<MaterialResponse> AddMaterialAsync(MaterialRequest materialRequest);
        public Task<MaterialResponse> UpdateMaterialAsync(string materialNo, MaterialRequest materialRequest);
        public Task<MaterialResponse> DeleteMaterialAsync(string materialNo);
    }
}
