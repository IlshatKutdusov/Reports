using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using AutoMapper;
using System.Linq;
using Reports.Entities;
using Microsoft.AspNetCore.Http;
using Reports.Models;

namespace Reports.Services
{
    public class FileService : IFileService
    {
        private readonly string ApplicationPath = System.IO.Directory.GetCurrentDirectory() + "\\SourceData\\Files\\";

        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly IUserService _userService;

        public FileService(IMapper mapper, IRepos repos, IUserService userService)
        {
            _mapper = mapper;
            _repos = repos;
            _userService = userService;
        }
        public async Task<File> GetById(int fileId)
        {
            var file = await _repos.Get<File>().FirstOrDefaultAsync(e => e.Id == fileId);
            var reports = _repos.Get<Report>().Where(e => e.FileId == file.Id).ToList();

            var entity = _mapper.Map<File>(file);

            await _repos.SaveChangesAsync();

            return entity;
        }

        public async Task<CreationResponse> Create(File file)
        {
            var entity = _mapper.Map<File>(file);

            var result = await _repos.Add(entity);

            await _repos.SaveChangesAsync();

            return new CreationResponse() { IsCreated = true, Result = result};
        }

        public async Task<Response> UploadFile(string userLogin, IFormFile uploadedFile)
        {
            var user = await _userService.GetByLogin(userLogin);

            if (!System.IO.Directory.Exists(ApplicationPath))
            {
                System.IO.Directory.CreateDirectory(ApplicationPath);
            }

            string path = ApplicationPath + uploadedFile.FileName;

            File file = new File { UserId = user.Id, Name = uploadedFile.FileName, Path = ApplicationPath};

            var fileId = await Create(file);

            if (fileId.IsCreated)
            {
                using (var fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                return new Response() { Status = "Success" };
            }

            return new Response() { Status = "Error", Message = "The file already exists!" };
        }

        public async Task Update(File file)
        {
            var entity = _mapper.Map<File>(file);

            await _repos.Update(entity);
            await _repos.SaveChangesAsync();
        }

        public async Task Delete(int fileId)
        {
            await _repos.Delete<File>(fileId);
            await _repos.SaveChangesAsync();
        }
    }
}
