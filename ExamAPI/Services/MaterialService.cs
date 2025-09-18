using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Models;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace ExamAPI.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly ExamSQLContext _context;
        private readonly IMaterialRepository _materialRepository;
        private readonly IBomRepository _bomRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MaterialService> _logger;
        public MaterialService(ExamSQLContext context, IMaterialRepository materialRepository, IBomRepository bomRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ILogger<MaterialService> loger)
        {
            _context = context;
            _materialRepository = materialRepository;
            _bomRepository = bomRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _logger = loger;
        }

        public MaterialResponse GetAllMaterials(int page, int pageSize)
        {
            _logger.LogTrace("進入GetAllMaterialsAsync");
            MaterialResponse response = new MaterialResponse();
            var materials = _materialRepository.GetAllMaterials();
            var m = materials.Select(x => new MaterialDto
            {
                MaterialNo = x.MaterialNo,
                MaterialName = x.MaterialName,
                MaterialCost = x.MaterialCost,
                CurrentStock = x.CurrentStock
            });
            _logger.LogDebug("取得物料數量：{m.Count()}", m.Count());
            var pagedList = m.ToPagedList(page, pageSize);
            response.Materials = pagedList.ToList();
            response.PageCount = pagedList.PageCount;
            response.TotalCount = pagedList.TotalItemCount;
            response.Success = true;
            response.Message = $"取得第{page}頁，{pageSize}筆資料";
            _logger.LogTrace("離開GetAllMaterialsAsync");
            return response;
        }
        public async Task<MaterialResponse> AddMaterialAsync(MaterialRequest materialRequest)
        {
            _logger.LogTrace("進入AddMaterialAsync");
            MaterialResponse response = new MaterialResponse();

            try
            {
                if(materialRequest == null)
                {
                    _logger.LogWarning("新增物料資料為空");
                    response.Success = false;
                    response.Message = "新增物料資料為空";
                    _logger.LogTrace("離開AddMaterialAsync");
                    return response;
                }
                if(string.IsNullOrEmpty(materialRequest.MaterialName))
                {
                    _logger.LogWarning("必填欄位不能為空");
                    response.Success = false;
                    response.Message = "必填欄位不能為空";
                    _logger.LogTrace("離開AddMaterialAsync");
                    return response;
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                var lastMaterial = await _context.Materials.OrderByDescending(p => p.MaterialNo).FirstOrDefaultAsync();
                int number = 1;
                if (lastMaterial != null)
                {
                    // 取出流水號部分
                    string lastMaterialNo = lastMaterial.MaterialNo.Substring(2);
                    number = int.Parse(lastMaterialNo) + 1;
                }
                var material = new Material
                {
                    MaterialNo = $"ML{number:D8}",
                    MaterialName = materialRequest.MaterialName,
                    MaterialCost = materialRequest.MaterialCost,
                    CurrentStock = materialRequest.CurrentStock,
                    CreateDate = DateTime.Now,
                    Creator = user.UserName,
                    ModifyDate = DateTime.Now,
                    Modifier = "System"
                };
                await _materialRepository.AddMaterialAsync(material);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var m = new MaterialDto
                    {
                        MaterialNo = material.MaterialNo,
                        MaterialName = material.MaterialName,
                        MaterialCost = material.MaterialCost,
                        CurrentStock = material.CurrentStock
                    };
                    _logger.LogInformation("成功新增物料{MaterialNo}", material.MaterialNo);
                    response.Materials = new List<MaterialDto> { m };
                    response.Success = true;
                    response.Message = "新增物料成功";
                }
                else
                {
                    _logger.LogWarning("新增物料失敗");
                    response.Success = false;
                    response.Message = "新增物料失敗";
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "新增發生錯誤");
                response.Success = false;
                response.Message = "新增發生錯誤";
            }
            _logger.LogTrace("離開AddMaterialAsync");
            return response;
        }
        public async Task<MaterialResponse> UpdateMaterialAsync(string materialNo, MaterialRequest materialRequest)
        {
            _logger.LogTrace("進入UpdateMaterialAsync");
            MaterialResponse response = new MaterialResponse();

            try
            {
                if (string.IsNullOrEmpty(materialNo) || materialRequest == null)
                {
                    _logger.LogWarning("MaterialNo或更新資料為空");
                    response.Success = false;
                    response.Message = "MaterialNo或更新資料為空";
                    _logger.LogTrace("離開UpdateMaterialAsync");
                    return response;
                }
                var existMaterial = await _materialRepository.GetMaterialByMaterialNoAsync(materialNo);
                if (existMaterial == null)
                {
                    _logger.LogWarning("物料{MaterialNo}不存在", materialNo);
                    response.Success = false;
                    response.Message = "物料不存在";
                    _logger.LogTrace("離開UpdateMaterialAsync");
                    return response;
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                existMaterial.MaterialName = string.IsNullOrEmpty(materialRequest.MaterialName)
                    ? existMaterial.MaterialName : materialRequest.MaterialName;
                existMaterial.MaterialCost = materialRequest.MaterialCost == 0
                    ? existMaterial.MaterialCost : materialRequest.MaterialCost;
                existMaterial.CurrentStock = materialRequest.CurrentStock == null
                    ? existMaterial.CurrentStock : materialRequest.CurrentStock;
                existMaterial.ModifyDate = DateTime.Now;
                existMaterial.Modifier = user.UserName;
                await _materialRepository.UpdateMaterialAsync(existMaterial);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var m = new MaterialDto
                    {
                        MaterialNo = materialNo,
                        MaterialName = materialRequest.MaterialName,
                        MaterialCost = materialRequest.MaterialCost,
                        CurrentStock = materialRequest.CurrentStock
                    };
                    _logger.LogInformation("成功修改物料{MaterialNo}", existMaterial.MaterialNo);
                    response.Materials = new List<MaterialDto> { m };
                    response.Success = true;
                    response.Message = "修改物料成功";
                }
                else
                {
                    _logger.LogWarning("修改物料失敗");
                    response.Success = false;
                    response.Message = "修改物料失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "修改發生錯誤");
                response.Success = false;
                response.Message = "修改發生錯誤";
            }
            _logger.LogTrace("離開UpdateMaterialAsync");
            return response;
        }
        // 若有物料曾被Bom關連到產品則不可刪除
        public async Task<MaterialResponse> DeleteMaterialAsync(string materialNo)
        {
            _logger.LogTrace("進入DeleteMaterialAsync");
            MaterialResponse response = new MaterialResponse();

            try
            {
                if (string.IsNullOrEmpty(materialNo))
                {
                    _logger.LogWarning("物料編號為空");
                    response.Success = false;
                    response.Message = "物料編號為空";
                    _logger.LogTrace("離開DeleteMaterialAsync");
                    return response;
                }
                var material = await _materialRepository.GetMaterialByMaterialNoAsync(materialNo);
                if (material == null)
                {
                    _logger.LogWarning("物料{MaterialNo}不存在", materialNo);
                    response.Success = false;
                    response.Message = "物料不存在";
                    _logger.LogTrace("離開DeleteMaterialAsync");
                    return response;
                }
                var boms = _bomRepository.GetAllBoms();
                var cheskBom = boms.Any(b => b.MaterialNo == materialNo);                
                if (cheskBom)
                {
                    _logger.LogWarning("物料編號{MaterialNo}已經被使用於Bom，不能刪除", materialNo);
                    response.Success = false;
                    response.Message = "物料已經被使用於Bom，不能刪除";
                    _logger.LogTrace("離開DeleteMaterialAsync");
                    return response;
                }
                await _materialRepository.DeleteMaterialAsync(material);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    _logger.LogInformation("成功刪除物料{MaterialNo}", materialNo);
                    response.Success = true;
                    response.Message = "刪除成功";
                }
                else
                {
                    _logger.LogWarning("刪除物料失敗{MaterialNo}", materialNo);
                    response.Success = false;
                    response.Message = "刪除失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除發生錯誤");
                response.Success = false;
                response.Message = "刪除發生錯誤";
            }
            _logger.LogTrace("離開DeleteMaterialAsync");
            return response;
        }
    }
}
