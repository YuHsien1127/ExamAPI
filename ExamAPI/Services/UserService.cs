using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Models;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.RegularExpressions;

namespace ExamAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ExamSQLContext _context;
        private readonly IUserRepository _userRepository;
        public readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;
        public UserService(ExamSQLContext context, IUserRepository userRepository, ITokenService tokenService, ILogger<UserService> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            _logger.LogTrace("進入LoginAsync");
            LoginResponse response = new LoginResponse();

            try
            {
                if(loginRequest == null)
                {
                    _logger.LogWarning("登入資料為空");
                    response.Success = false;
                    response.Message = "登入資料為空";
                    _logger.LogTrace("離開LoginAsync");
                    return response;
                }
                if(string.IsNullOrEmpty(loginRequest.UserAccount) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    _logger.LogWarning("使用者帳號/使用者密碼為空");
                    response.Success = false;
                    response.Message = "使用者帳號/使用者密碼為空";
                    _logger.LogTrace("離開LoginAsync");
                    return response;
                }
                var user = await _userRepository.GetUserByUserAccountAsync(loginRequest.UserAccount);
                if(user == null || loginRequest.Password != user.UserPassword)
                {
                    _logger.LogWarning("帳號或密碼錯誤：{UserAccount}", loginRequest.UserAccount);
                    response.Success = false;
                    response.Message = "帳號或密碼錯誤";
                    _logger.LogTrace("離開LoginAsync");
                    return response;
                }
                response.Success = true;
                response.Message = "登入成功";
                response.UserName = user.UserName;
                response.UserRole = user.UserRole;
                response.LoginTime = DateTime.Now;
                response.Token = $"Bearer {_tokenService.GenerateJwtToken(user.UserAccount, user.UserRole)}";
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "登入發生錯誤"); //log 嚴重錯誤
                response.Success = false;
                response.Message = "登入發生錯誤";
            }
            _logger.LogTrace("離開LoginAsync");
            return response;
        }

        // userAccout帳號不可重複
        public async Task<UserResponse> AddUserAsync(UserRequest userRequest)
        {
            _logger.LogTrace("進入AddUserAsync");
            UserResponse response = new UserResponse();

            try
            {
                if(userRequest == null)
                {
                    _logger.LogWarning("新增使用者資料為空");
                    response.Success = false;
                    response.Message = "新增使用者資料為空";
                    _logger.LogTrace("離開AddUserAsync");
                    return response;
                }
                if(string.IsNullOrEmpty(userRequest.UserAccount) || string.IsNullOrEmpty(userRequest.UserPassword) ||
                    string.IsNullOrEmpty(userRequest.UserName) || string.IsNullOrEmpty(userRequest.UserEmail) || string.IsNullOrEmpty(userRequest.UserRole))
                {
                    _logger.LogWarning("必填欄位不能為空");
                    response.Success = false;
                    response.Message = "必填欄位不能為空";
                    _logger.LogTrace("離開AddUserAsync");
                    return response;
                }
                // UserAccount是否存在
                var existUser = await _userRepository.GetUserByUserAccountAsync(userRequest.UserAccount);
                if(existUser != null)
                {
                    _logger.LogWarning("UserAccount（{userRequest.UserAccount}）已存在", userRequest.UserAccount);
                    response.Success = false;
                    response.Message = "UserAccount";
                    _logger.LogTrace("離開AddStudent");
                    return response;
                }
                // UserEmail格式檢查
                if (!Regex.IsMatch(userRequest.UserEmail, @"^[^@\s]+@([^@\s]+\.)+[^@\s]+$"))
                {
                    _logger.LogWarning("UserEmail（{userRequest.UserEmail}）格式不正確", userRequest.UserEmail);
                    response.Success = false;
                    response.Message = "UserEmail格式不正確";
                    _logger.LogTrace("離開AddStudent");
                    return response;
                }                
                var user = new User
                {
                    UserAccount = userRequest.UserAccount,
                    UserPassword = userRequest.UserPassword,
                    UserName = userRequest.UserName,
                    UserEmail = userRequest.UserEmail,
                    UserRole = userRequest.UserRole,
                    CreateDate = DateTime.Now,
                    Creator = userRequest.UserName,
                    Modifier = "System"
                };
                await _userRepository.AddUserAsync(user);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var u = new UserDto
                    {
                        SerialNo = user.SerialNo,
                        UserAccount = user.UserAccount,
                        UserPassword = user.UserPassword,
                        UserName = user.UserName,
                        UserEmail= user.UserEmail
                    };
                    _logger.LogInformation("新增成功（Id：{student.Id}）", user.SerialNo); // log
                    response.Users = new List<UserDto> { u };
                    response.Success = true;
                    response.Message = "新增成功";
                }
                else
                {
                    _logger.LogWarning("新增失敗");
                    response.Success = false;
                    response.Message = "新增失敗";
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"新增發生錯誤");
                response.Success = false;
                response.Message = "新增發生錯誤";
            }
            _logger.LogTrace("離開AddUserAsync");
            return response;
        }
        
        public async Task<UserResponse> UpdateUserAsync(int serialNo, UserRequest userRequest)
        {
            _logger.LogTrace("進入UpdateUserAsync");
            UserResponse response = new UserResponse();

            try
            {
                if(serialNo == 0 || userRequest == null)
                {
                    _logger.LogWarning("serialNo或修改項目為空");
                    response.Success = false;
                    response.Message = "serialNo或修改項目為空";
                    _logger.LogTrace("離開UpdateUserAsync");
                    return response;
                }
                var existUser = await _userRepository.GetUserBySerialNoAsync(serialNo);
                if(existUser == null)
                {
                    _logger.LogWarning("此SerialNo（{serialNo}）的User資料為空", serialNo); //log
                    response.Success = false;
                    response.Message = "此SerialNo的User資料為空";
                    _logger.LogTrace("離開UpdateUserAsync");
                    return response;
                }
                existUser.UserAccount = string.IsNullOrEmpty(userRequest.UserAccount) 
                    ? existUser.UserAccount : userRequest.UserAccount;
                existUser.UserPassword = string.IsNullOrEmpty(userRequest.UserPassword)
                    ? existUser.UserPassword : userRequest.UserPassword;
                existUser.UserName = string.IsNullOrEmpty(userRequest.UserName)
                    ? existUser.UserName : userRequest.UserName;
                existUser.UserRole = string.IsNullOrEmpty(userRequest.UserRole)
                    ? existUser.UserRole : userRequest.UserRole;
                existUser.UserEmail = !Regex.IsMatch(userRequest.UserEmail, @"^[^@\s]+@([^@\s]+\.)+[^@\s]+$")
                    ? existUser.UserEmail : userRequest.UserEmail;
                existUser.ModifyDate = DateTime.Now;
                existUser.Modifier = string.IsNullOrEmpty(userRequest.UserName)
                    ? existUser.UserName : userRequest.UserName;
                await _userRepository.UpdateUserAsync(existUser);

                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var u = new UserDto
                    {
                        SerialNo = serialNo,
                        UserAccount = existUser.UserAccount,
                        UserPassword = existUser.UserPassword,
                        UserName = existUser.UserName,
                        UserEmail = existUser.UserEmail
                    };
                    _logger.LogInformation("修改成功（SerialNo：{serialNo}）", serialNo); // log
                    response.Users = new List<UserDto> { u };
                    response.Success = true;
                    response.Message = "修改成功";
                }
                else
                {
                    _logger.LogWarning("修改失敗");
                    response.Success = false;
                    response.Message = "修改失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "修改發生錯誤");
                response.Success = false;
                response.Message = "修改發生錯誤";
            }
            _logger.LogTrace("離開UpdateUserAsync");
            return response;
        }
    }
}
