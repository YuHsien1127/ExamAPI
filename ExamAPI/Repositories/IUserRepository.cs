using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public interface IUserRepository
    {
        public Task<IQueryable<User>> GetAllUsersAsync();
        public Task<User> GetUserBySerialNoAsync(int serialNo);
        public Task<User> GetUserByUserAccountAsync(string account);
        public Task AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
    }
}
