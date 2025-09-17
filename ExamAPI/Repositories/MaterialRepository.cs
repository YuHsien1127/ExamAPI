using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly ExamSQLContext _context;
        public MaterialRepository(ExamSQLContext context)
        {
            _context = context;
        }

        public IQueryable<Material> GetAllMaterials()
        {
            return _context.Materials;
        }

        public async Task<Material> GetMaterialByMaterialNoAsync(string materialNo)
        {
            return await _context.Materials.FindAsync(materialNo);
        }

        public async Task AddMaterialAsync(Material material)
        {
            await _context.Materials.AddAsync(material);
        }

        public async Task DeleteMaterialAsync(Material material)
        {
            _context.Materials.Remove(material);
            await Task.CompletedTask;
        }

        public async Task UpdateMaterialAsync(Material material)
        {
            _context.Materials.Update(material);
            await Task.CompletedTask;
        }
    }
}
