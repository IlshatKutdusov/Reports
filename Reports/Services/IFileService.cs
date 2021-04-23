using System.Threading.Tasks;
using Reports.Models;

namespace Reports.Services
{
    public interface IFileService
    {
        Task<int> Create(File file);
        Task<File> Get(int fileId);
        Task Update(File file);
        Task Delete(int fileId);
    }
}
