using Reports.Entities;
using Reports.Models;
using System.Threading.Tasks;

namespace Reports.Services
{
    public interface IReportService
    {
        Task<ReportResponse> GetById(string requestUserLogin, int reportId);

        Task<ReportResponse> GenerateFromFile(string requestUserLogin, int fileId, string format);

        Task<DefaultResponse> Update(string requestUserLogin, Report report);

        Task<DefaultResponse> Remove(string requestUserLogin, int reportId);
    }
}
