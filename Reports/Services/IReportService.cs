using Reports.Entities;
using Reports.Models;
using System.Threading.Tasks;

namespace Reports.Services
{
    public interface IReportService
    {
        Task<Report> GetById(int reportId);

        Task<DefaultResponse> Create(Report report);
        Task<DefaultResponse> CreateReportFromFile(File file, string format);

        Task<DefaultResponse> Update(Report report);
        Task<DefaultResponse> Remove(int reportId);
    }
}
