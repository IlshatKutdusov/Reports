using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using AutoMapper;
using System.Linq;
using Reports.Entities;
using Microsoft.AspNetCore.Http;
using Reports.Models.Responses;
using Reports.Services.Helper;

namespace Reports.Services
{
    public class FileService : IFileService
    {
        private readonly string ApplicationPath = System.IO.Directory.GetCurrentDirectory() + "\\SourceData\\Files\\";

        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly IUserService _userService;
        private readonly IFileHelper _fileHelper;

        public FileService(IMapper mapper, IRepos repos, IUserService userService, IFileHelper fileHelper)
        {
            _mapper = mapper;
            _repos = repos;
            _userService = userService;
            _fileHelper = fileHelper;
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
                return new FileResponse(userResponse);

            if (!System.IO.File.Exists(file.Path + file.Name))
            {
                await _repos.Remove<File>(file);
                await _repos.SaveChangesAsync();

                return new FileResponse()
                {
                    Status = "Error",
                    Message = "The source file not found!",
                    Done = false
                };
            }

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

        public async Task<FileStreamResponse> GetFile(string requestUserLogin, int fileId)
        {
            var fileResponse = await GetById(requestUserLogin, fileId);

            if (!fileResponse.Done)
                return new FileStreamResponse(fileResponse);

            var fs = new System.IO.FileStream(fileResponse.File.Path + fileResponse.File.Name, System.IO.FileMode.Open);

            return new FileStreamResponse()
            {
                Status = "Success",
                Message = "The file found successfully!",
                Done = true,
                FileStream = fs,
                FileFormat = "application/csv",
                FileName = fileResponse.File.Name
            };
        }

        public async Task<ProvidersResponse> GetProviders(string requestUserLogin, int fileId)
        {
            var file = await _repos.Get<File>().FirstOrDefaultAsync(e => e.Id == fileId);

            if (file == null)
                return new ProvidersResponse()
                {
                    Status = "Error",
                    Message = "The file not found!",
                    Done = false
                };

            var userResponse = await _userService.GetById(requestUserLogin, file.UserId);

            if (!userResponse.Done)
                return new ProvidersResponse(userResponse);

            var entity = _mapper.Map<File>(file);

            var providersResponse = await _fileHelper.GetProviders(entity);

            if (providersResponse.Done)
                return new ProvidersResponse()
                {
                    Status = "Success",
                    Message = "The file found successfully!",
                    Done = true,
                    Providers = providersResponse.Providers
                };

            return new ProvidersResponse(providersResponse);
        }

        public async Task<FileResponse> UploadFile(string requestUserLogin, string userLogin, IFormFile uploadedFile)
        {
            if (uploadedFile == null)
                return new FileResponse() 
                { 
                    Status = "Error",
                    Message = "The file to be uploaded must not be NULL!",
                    Done = false
                };

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
                return new FileResponse(userResponse);

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

                var dataCheckedResponse = await _fileHelper.SourceFileDataCheck(file);

                if (dataCheckedResponse.Done)
                    return new FileResponse()
                    {
                        Status = "Success",
                        Message = "The file uploaded successfully!",
                        Done = true,
                        File = file
                    };

                var fileEntity = await _repos.Get<File>().FirstOrDefaultAsync(e => e.Name == file.Name);
                await _repos.Remove<File>(fileEntity);
                await _repos.SaveChangesAsync();

                System.IO.File.Delete(fileEntity.Path + fileEntity.Name);

                return new FileResponse(dataCheckedResponse);
            }

            return new FileResponse(creationTask);
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

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The file not removed!",
                Done = false
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

            var addingTask = _repos.Add(entity);
            addingTask.Wait();

            await _repos.SaveChangesAsync();

            if (addingTask.IsCompletedSuccessfully)
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
    }
}
