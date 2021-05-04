using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Reports.Entities;
using Reports.Models;

namespace Reports.Services
{
    public interface IFileService
    {
        Task<File> GetById(int fileId);

        Task<CreationResponse> Create(File file);
        Task<Response> UploadFile(string userLogin, IFormFile upload);

        Task Update(File file);
        Task Delete(int fileId);
    }
}
