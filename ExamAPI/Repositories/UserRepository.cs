using ExamAPI.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ExamAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ExamSQLContext _context;
        public UserRepository(ExamSQLContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_context.Users);
        }
        public async Task<User> GetUserBySerialNoAsync(int serialNo)
        {
            return await _context.Users.FindAsync(serialNo);
        }
        public async Task<User> GetUserByUserAccountAsync(string userAccount)
        {
            return await _context.Users.FirstOrDefaultAsync(a => a.UserAccount == userAccount);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }
    }
}
