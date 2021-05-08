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

        public async Task<FileResponse> GetById(string requestUserLogin, int fileId)
        {
            var file = await _repos.Get<File>().FirstOrDefaultAsync(e => e.Id == fileId);

            if (file == null)
                return new FileResponse()
                {
                    Status = "Error",
                    Message = "The file not found!",
                    Done = false
                };

            var userResponse = await _userService.GetById(requestUserLogin, file.UserId);

            if (!userResponse.Done)
                return new FileResponse()
                {
                    Status = "Error",
                    Message = userResponse.Message,
                    Done = false
                };

            var reports = _repos.Get<Report>().Where(e => e.FileId == file.Id).ToList();

            var entity = _mapper.Map<File>(file);

            return new FileResponse()
            {
                Status = "Success",
                Message = "The file found successfully!",
                Done = true,
                File = entity
            };
        }

        private async Task<DefaultResponse> Create(string requestUserLogin, File file)
        {
            var userResponse = await _userService.GetById(requestUserLogin, file.UserId);

            if (!userResponse.Done)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = userResponse.Message,
                    Done = false
                };

            var entity = _mapper.Map<File>(file);

            var addTask = _repos.Add(entity);
            addTask.Wait();

            await _repos.SaveChangesAsync();

            if (addTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The file created successfully!",
                    Done = true
                }; 

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The file not created!",
                Done = false
            };
        }

        public async Task<FileResponse> UploadFile(string requestUserLogin, string userLogin, IFormFile uploadedFile)
        {
            string uploadFileName = uploadedFile.FileName.Substring(uploadedFile.FileName.Length - 4);

            if (uploadFileName != ".csv")
                return new FileResponse()
                {
                    Status = "Error",
                    Message = "This format is not supported! (only .csv)",
                    Done = false
                };

            var userResponse = await _userService.GetByLogin(requestUserLogin, userLogin);

            if (!userResponse.Done)
                return new FileResponse()
                {
                    Status = "Error",
                    Message = userResponse.Message,
                    Done = false
                };

            if (!System.IO.Directory.Exists(ApplicationPath))
                System.IO.Directory.CreateDirectory(ApplicationPath);

            string path = ApplicationPath + userLogin + "_" + uploadedFile.FileName;

            if (System.IO.File.Exists(path))
                return new FileResponse()
                {
                    Status = "Error",
                    Message = "The file already exists!",
                    Done = false
                };

            var file = new File 
            { 
                UserId = userResponse.User.Id, 
                Name = requestUserLogin + "_" + uploadedFile.FileName, 
                Path = ApplicationPath 
            };

            var creationTask = await Create(requestUserLogin, file);

            if (creationTask.Done)
            {
                using (var fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                return new FileResponse()
                {
                    Status = "Success",
                    Message = "The file uploaded successfully!",
                    Done = true,
                    File = file
                };
            }

            return new FileResponse()
            {
                Status = "Error",
                Message = "The file already exists! " + creationTask.Message,
                Done = false
            };
        }

        public async Task<DefaultResponse> Update(string requestUserLogin, File file)
        {
            var fileResponse = await GetById(requestUserLogin, file.UserId);

            if (!fileResponse.Done)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = fileResponse.Message,
                    Done = false
                };


            var entity = _mapper.Map<File>(file);

            var updatingTask = _repos.Update(entity);
            updatingTask.Wait();

            await _repos.SaveChangesAsync();

            if (updatingTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The file updated successfully!",
                    Done = true
                };

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The file not updated!",
                Done = false
            };
        }

        public async Task<DefaultResponse> Remove(string requestUserLogin, int fileId)
        {
            var fileResponse = await GetById(requestUserLogin, fileId);

            if (!fileResponse.Done)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = fileResponse.Message,
                    Done = false
                };

            if (fileResponse.File.Reports != null)
                foreach (var report in fileResponse.File.Reports)
                {
                    if (System.IO.File.Exists(report.Path + report.Name))
                        System.IO.File.Delete(report.Path + report.Name);
                }

            var removingTask = _repos.Remove<File>(fileResponse.File);
            removingTask.Wait();

            await _repos.SaveChangesAsync();

            if (removingTask.IsCompletedSuccessfully)
            {
                if (System.IO.File.Exists(fileResponse.File.Path + fileResponse.File.Name))
                    System.IO.File.Delete(fileResponse.File.Path + fileResponse.File.Name);

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The file removed successfully!",
                    Done = true
                };
            }

            return new CreationResponse()
            {
                Status = "Error",
                Message = "The file not removed!",
                Done = false
            };
        }
    }
}
