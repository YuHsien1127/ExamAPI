using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public interface IBomRepository
    {
        public IQueryable<Bom> GetAllBoms();
        public Task<Bom> GetBomBySerialNoAsync(int serialNo);
        public Task AddBomAsync(Bom bom);
        public Task UpdateBomAsync(Bom bom);
        public Task CancelBomAsync(Bom bom);
    }
}
