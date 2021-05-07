using Reports.Entities;
using Reports.Models;
using System.Threading.Tasks;

namespace Reports.Services.Helper
{
    public interface IReportBuilder
    {
        Task<DefaultResponse> DefaultSaveAsExcel(User user, File file, Report report);

        Task<DefaultResponse> DefaultSaveAsPdf(User user, File file, Report report);
    }
}
