using ExamAPI.Dto.Response;

namespace ExamAPI.Services
{
    public interface IOrderExportService
    {
        public Task<BaseResponse> Production_ExportOrdersAsync();
        public Task<BaseResponse> ExportProductAsync();
        public Task<BaseResponse> ExportOrdersAsync();
    }
}
