using Reports.Entities;
using Reports.Models;
using System.Threading.Tasks;

namespace Reports.Services
{
    public interface IReportService
    {
        Task<Report> GetById(int reportId);

        Task<CreationResponse> Create(Report report);
        Task<CreationResponse> CreateReportFromFile(File file, string format);

        Task Update(Report report);
        Task Delete(int reportId);
    }
}
