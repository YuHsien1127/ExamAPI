using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public interface IMaterialRepository
    {
        public IQueryable<Material> GetAllMaterials();
        public Task<Material> GetMaterialByMaterialNoAsync(string materialNo);
        public Task AddMaterialAsync(Material material);
        public Task UpdateMaterialAsync(Material material);
        public Task DeleteMaterialAsync(Material material);
    }
}
