using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public interface IUserRepository
    {
        public IQueryable<User> GetAllUsers();
        public Task<User> GetUserBySerialNoAsync(int serialNo);
        public Task<User> GetUserByUserAccountAsync(string account);
        public Task AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
    }
}
