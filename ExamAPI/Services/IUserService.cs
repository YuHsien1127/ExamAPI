using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;

namespace ExamAPI.Services
{
    public interface IUserService
    {
        public Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        public Task<UserResponse> AddUserAsync(UserRequest userRequest);
        public Task<UserResponse> UpdateUserAsync(int serialNo,UserRequest userRequest);
    }
}
