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

            if (file != null)
            {
                var reports = _repos.Get<Report>().Where(e => e.FileId == file.Id).ToList();

                var entity = _mapper.Map<File>(file);

                await _repos.SaveChangesAsync();

                return entity; 
            }

            return null;
        }

        public async Task<DefaultResponse> Create(File file)
        {
            var entity = _mapper.Map<File>(file);

            var task = _repos.Add(entity);
            task.Wait();

            if (task.IsCompletedSuccessfully)
            {
                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The file created successfully!"
                }; 
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The file not created!"
            };
        }

        public async Task<DefaultResponse> UploadFile(string userLogin, IFormFile uploadedFile)
        {
            var user = await _userService.GetByLogin(userLogin);

            if (user == null)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "User not founded!"
                };

            if (!System.IO.Directory.Exists(ApplicationPath))
            {
                System.IO.Directory.CreateDirectory(ApplicationPath);
            }

            string path = ApplicationPath + uploadedFile.FileName;

            File file = new File { UserId = user.Id, Name = uploadedFile.FileName, Path = ApplicationPath };

            var fileEntity = await Create(file);

            if (fileEntity.Status == "Success")
            {
                using (var fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The file uploaded successfully!"
                };
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The file already exists!"
            };
        }

        public async Task<DefaultResponse> Update(File file)
        {
            var entity = _mapper.Map<File>(file);

            var task = _repos.Update(entity);
            task.Wait();

            if (task.IsCompletedSuccessfully)
            {
                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The file updated successfully!"
                };
            }

            return new DatabaseResponse()
            {
                Status = "Error",
                Message = "The file not updated!"
            };
        }

        public async Task<DefaultResponse> Remove(int fileId)
        {
            var file = await GetById(fileId);

            if (file == null)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The file not found!"
                };

            if (file.Reports != null)
                foreach (var report in file.Reports)
                {
                    if (System.IO.File.Exists(report.Path + report.Name))
                        System.IO.File.Delete(report.Path + report.Name);
                }

            var task = _repos.Remove<File>(file);
            task.Wait();

            if (task.IsCompletedSuccessfully)
            {
                if (System.IO.File.Exists(file.Path + file.Name))
                    System.IO.File.Delete(file.Path + file.Name);

                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The file removed successfully!"
                };
            }

            return new DatabaseResponse()
            {
                Status = "Error",
                Message = "The file not removed!"
            };
        }
    }
}
