using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="loginRequest">帳號、密碼</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            return new JsonResult(await _userService.LoginAsync(loginRequest));
        }

        /// <summary>
        /// 新增 User 資料
        /// </summary>
        /// <param name="userRequest">User 資料（Role => admin/customer/staff）</param>
        /// <param>Role => admin/customer/staff</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<UserResponse> AddUserAsync([FromBody] UserRequest userRequest)
        {
            return await _userService.AddUserAsync(userRequest);
        }

        /// <summary>
        /// 修改 User 資料
        /// </summary>
        /// <param name="userRequest">User 資料（Role => admin/customer/staff）</param>
        /// <param name="serial">流水號</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "admin, customer")]
        public async Task<UserResponse> UpdateUserAsync([FromBody] UserRequest userRequest, int serial = 0)
        {
            return await _userService.UpdateUserAsync(serial, userRequest);
        }
    }
}
