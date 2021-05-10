using Reports.Entities;
using Reports.Models.Responses;
using System.Threading.Tasks;

namespace Reports.Services.Helper
{
    public interface IFileHelper
    {
        Task<DefaultResponse> SourceFileDataCheck(File file);

        Task<ProvidersResponse> GetProviders(File file);
    }
}
