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
    [Authorize(Roles = "admin, staff")]
    public class BomController : ControllerBase
    {
        private readonly IBomService _bomService;
        public BomController(IBomService bomService)
        {
            _bomService = bomService;
        }

        /// <summary>
        /// 查詢Bom
        /// </summary>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">一頁幾筆</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<BomResponse> GetAllBomsAsync(int page = 1, int pageSize = 10)
        {
            return await _bomService.GetAllBomsAsync(page, pageSize);
        }

        /// <summary>
        /// 新增Bom
        /// </summary>
        /// <param name="bomRequest">Bom資料</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BomResponse> AddBomAsync([FromBody] BomRequest bomRequest)
        {
            return await _bomService.AddBomAsync(bomRequest);
        }

        /// <summary>
        /// 修改Bom
        /// </summary>
        /// <param name="serialNo">Bom編號</param>
        /// <param name="bomRequest">物料資料</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<BomResponse> UpdateBomAsync([FromBody] BomRequest bomRequest, int serialNo = 0)
        {
            return await _bomService.UpdateBomAsync(serialNo, bomRequest);
        }

        /// <summary>
        /// 刪除Bom
        /// </summary>
        /// <param name="serialNo">Bom編號</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<BomResponse> DeleteBomAsync(int serialNo = 0)
        {
            return await _bomService.DeleteBomAsync(serialNo);
        }
    }
}
