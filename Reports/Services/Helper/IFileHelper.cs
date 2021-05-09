using Reports.Entities;
using Reports.Models;
using System.Threading.Tasks;

namespace Reports.Services.Helper
{
    public interface IFileHelper
    {
        Task<ProvidersResponse> GetProviders(File file);

        Task<DefaultResponse> DefaultSaveAsExcel(User user, File file, Report report);

        Task<DefaultResponse> DefaultSaveAsPdf(User user, File file, Report report);

        Task<DefaultResponse> ProviderSaveAsExcel(string provider, User user, File file, Report report);

        Task<DefaultResponse> ProviderSaveAsPdf(string provider, User user, File file, Report report);
    }
}
