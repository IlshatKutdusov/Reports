using System.Threading.Tasks;
using Reports.API.Models;

namespace Reports.API.Services
{
    public interface IFileService
    {
        Task<int> Create(File file);
        Task<File> Get(int fileId);
        Task Update(File file);
        Task Delete(int fileId);
    }
}
