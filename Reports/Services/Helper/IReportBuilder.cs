using Reports.Entities;
using Reports.Models.Responses;
using System.Threading.Tasks;

namespace Reports.Services.Helper
{
    public interface IReportBuilder
    {
        Task<DefaultResponse> DefaultSaveAsExcel(User user, File file, Report report);

        Task<DefaultResponse> DefaultSaveAsPdf(User user, File file, Report report);

        Task<DefaultResponse> ProviderSaveAsExcel(string provider, User user, File file, Report report);

        Task<DefaultResponse> ProviderSaveAsPdf(string provider, User user, File file, Report report);
    }
}
