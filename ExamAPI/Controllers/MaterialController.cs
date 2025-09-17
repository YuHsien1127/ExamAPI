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
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        /// <summary>
        /// 查詢物料
        /// </summary>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">一頁幾筆</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MaterialResponse> GetAllMaterialsAsync(int page = 1, int pageSize = 10)
        {
            return await _materialService.GetAllMaterialsAsync(page, pageSize);
        }

        /// <summary>
        /// 新增物料
        /// </summary>
        /// <param name="materialRequest">物料資料</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MaterialResponse> AddMaterialAsync([FromBody] MaterialRequest materialRequest)
        {
            return await _materialService.AddMaterialAsync(materialRequest);
        }

        /// <summary>
        /// 修改物料
        /// </summary>
        /// <param name="materialNo">物料編號</param>
        /// <param name="materialRequest">物料資料</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<MaterialResponse> UpdateMaterialAsync(string materialNo, [FromBody] MaterialRequest materialRequest)
        {
            return await _materialService.UpdateMaterialAsync(materialNo, materialRequest);
        }

        /// <summary>
        /// 刪除物料
        /// </summary>
        /// <param name="materialNo">物料編號</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MaterialResponse> DeleteMaterialAsync(string materialNo)
        {
            return await _materialService.DeleteMaterialAsync(materialNo);
        }
    }
}
