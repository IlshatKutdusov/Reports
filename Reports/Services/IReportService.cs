using Reports.Entities;
using System.Threading.Tasks;

namespace Reports.Services
{
    public interface IReportService
    {
        Task<int> Create(Report report);
        Task<Report> GetById(int reportId);
        Task Update(Report report);
        Task Delete(int reportId);
    }
}
