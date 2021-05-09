using Reports.Entities;
using Reports.Models;
using System.Threading.Tasks;

namespace Reports.Services
{
    public interface IReportService
    {
        Task<ReportResponse> GetById(string requestUserLogin, int reportId);

        Task<FileStreamResponse> GetFile(string requestUserLogin, int reportId);

        Task<ReportResponse> Generate(string requestUserLogin, int fileId, string format);

        Task<ReportResponse> Generate(string requestUserLogin, int fileId, string format, string provider);

        Task<DefaultResponse> Update(string requestUserLogin, Report report);

        Task<DefaultResponse> Remove(string requestUserLogin, int reportId);
    }
}
