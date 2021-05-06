using Reports.Entities;
using Reports.Models;
using System.Threading.Tasks;

namespace Reports.Services.Helper
{
    public interface IReportBuilder
    {
        Task<DefaultResponse> SaveAsExcel(User user, File file, Report report);

        Task<DefaultResponse> SaveAsPdf(User user, File file, Report report);
    }
}
