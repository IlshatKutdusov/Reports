using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Reports.Entities;
using Reports.Models;

namespace Reports.Services
{
    public interface IFileService
    {
        Task<File> GetById(int fileId);

        Task<DefaultResponse> Create(File file);
        Task<UploadFileResponse> UploadFile(string userLogin, IFormFile upload);

        Task<DefaultResponse> Update(File file);
        Task<DefaultResponse> Remove(int fileId);
    }
}
