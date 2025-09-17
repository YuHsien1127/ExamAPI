using ExamAPI.Dto.Response;
using ExamAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamAPI.Repositories
{
    public class BomRepository : IBomRepository
    {
        private readonly ExamSQLContext _context;
        public BomRepository(ExamSQLContext context)
        {
            _context = context;
        }

        public IQueryable<Bom> GetAllBoms()
        {
            return _context.Boms;
        }

        public async Task<Bom> GetBomBySerialNoAsync(int serialNo)
        {
            return await _context.Boms.FindAsync(serialNo);
        }

        public async Task AddBomAsync(Bom bom)
        {
            await _context.Boms.AddAsync(bom);
        }

        public async Task CancelBomAsync(Bom bom)
        {
            _context.Boms.Remove(bom);
            await Task.CompletedTask;
        }

        public async Task UpdateBomAsync(Bom bom)
        {
            _context.Boms.Update(bom);
            await Task.CompletedTask;
        }
    }
}
