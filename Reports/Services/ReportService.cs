using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Entities;
using AutoMapper;
using Reports.Models;
using Reports.Services.Helper;

namespace Reports.Services
{
    public class ReportService : IReportService
    {
        private readonly string ApplicationPath = System.IO.Directory.GetCurrentDirectory() + "\\SourceData\\Reports\\";

        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IFileHelper _reportBuilder;

        public ReportService(IMapper mapper, IRepos repos, IFileService fileService, IUserService userService, IFileHelper reportBuilder)
        {
            _mapper = mapper;
            _repos = repos;
            _fileService = fileService;
            _userService = userService;
            _reportBuilder = reportBuilder;
        }

        public async Task<ReportResponse> GetById(string requestUserLogin, int reportId)
        {
            var report = await _repos.Get<Report>().FirstOrDefaultAsync(e => e.Id == reportId);

            if (report == null)
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = "The report not found!",
                    Done = false
                };

            var userResponse = await _userService.GetById(requestUserLogin, report.UserId);

            if (!userResponse.Done)
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = userResponse.Message,
                    Done = false
                };

            var fileResponse = await _fileService.GetById(requestUserLogin, report.FileId);

            if (!fileResponse.Done)
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = userResponse.Message,
                    Done = false
                };

            var entity = _mapper.Map<Report>(report);

            return new ReportResponse()
            {
                Status = "Success",
                Message = "The report found successfully!",
                Done = true,
                Report = entity
            };
        }

        public async Task<FileStreamResponse> GetFile(string requestUserLogin, int reportId)
        {
            var report = await _repos.Get<Report>().FirstOrDefaultAsync(e => e.Id == reportId);

            if (report == null)
                return new FileStreamResponse()
                {
                    Status = "Error",
                    Message = "The report not found!",
                    Done = false
                };

            var userResponse = await _userService.GetById(requestUserLogin, report.UserId);

            if (!userResponse.Done)
                return new FileStreamResponse()
                {
                    Status = "Error",
                    Message = userResponse.Message,
                    Done = false
                };

            var fileResponse = await _fileService.GetById(requestUserLogin, report.FileId);

            if (!fileResponse.Done)
                return new FileStreamResponse()
                {
                    Status = "Error",
                    Message = userResponse.Message,
                    Done = false
                };

            if (!System.IO.File.Exists(report.Path + report.Name))
                return new FileStreamResponse()
                {
                    Status = "Error",
                    Message = "The report not generated!",
                    Done = false
                };

            var fs = new System.IO.FileStream(report.Path + report.Name, System.IO.FileMode.Open);

            return new FileStreamResponse()
            {
                Status = "Success",
                Message = "The report found successfully!",
                Done = true,
                FileStream = fs,
                FileFormat = "application/" + report.Format.Substring(1),
                FileName = report.Name
            };
        }

        public async Task<ReportResponse> Generate(string requestUserLogin, int fileId, string format)
        {
            var fileResponse = await _fileService.GetById(requestUserLogin, fileId);

            if (!fileResponse.Done)
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = fileResponse.Message,
                    Done = false
                };

            var userResponse = await _userService.GetById(requestUserLogin, fileResponse.File.UserId);

            if (!(format == "xlsx" || format == "pdf"))
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = "This format is not supported! (only .xlsx or .pdf)",
                    Done = false
                };

            string _format = ".xlsx";

            if (format == "pdf")
                _format = ".pdf";

            var newReport = new Report()
            {
                UserId = fileResponse.File.UserId,
                FileId = fileResponse.File.Id,
                Name = "Report_" + fileResponse.File.Name.Remove(fileResponse.File.Name.Length - 4, 4) + _format,
                Path = ApplicationPath,
                Format = _format
            };

            if (fileResponse.File.Reports != null)
                foreach (var report in fileResponse.File.Reports)
                    if (report.Name == newReport.Name)
                        return new ReportResponse()
                        {
                            Status = "Error",
                            Message = "A report of this type has already been created!",
                            Done = false
                        };

            var creationTask = await Create(userResponse.User, fileResponse.File, newReport);

            if (creationTask.Done)
                return new ReportResponse()
                {
                    Status = "Success",
                    Message = creationTask.Message,
                    Done = true,
                    Report = newReport
                };

            return new ReportResponse()
            {
                Status = "Error",
                Message = "The report not created! " + creationTask.Message,
                Done = false
            };
        }

        public async Task<ReportResponse> Generate(string requestUserLogin, int fileId, string format, string provider)
        {
            if (provider == null)
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = "The provider cannot be NULL!",
                    Done = false
                };

            var fileResponse = await _fileService.GetById(requestUserLogin, fileId);

            if (!fileResponse.Done)
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = fileResponse.Message,
                    Done = false
                };

            var userResponse = await _userService.GetById(requestUserLogin, fileResponse.File.UserId);

            if (!(format == "xlsx" || format == "pdf"))
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = "This format is not supported! (only .xlsx or .pdf)",
                    Done = false
                };

            string _format = ".xlsx";

            if (format == "pdf")
                _format = ".pdf";

            var newReport = new Report()
            {
                UserId = fileResponse.File.UserId,
                FileId = fileResponse.File.Id,
                Name = "Report_" + provider + "_" + fileResponse.File.Name.Remove(fileResponse.File.Name.Length - 4, 4) + _format,
                Path = ApplicationPath,
                Format = _format
            };

            if (fileResponse.File.Reports != null)
                foreach (var report in fileResponse.File.Reports)
                    if (report.Name == newReport.Name)
                        return new ReportResponse()
                        {
                            Status = "Error",
                            Message = "A report of this type has already been created!",
                            Done = false
                        };

            var creationTask = await Create(userResponse.User, fileResponse.File, newReport, provider);

            if (creationTask.Done)
                return new ReportResponse()
                {
                    Status = "Success",
                    Message = creationTask.Message,
                    Done = true,
                    Report = newReport
                };

            return new ReportResponse()
            {
                Status = "Error",
                Message = "The report not created! " + creationTask.Message,
                Done = false
            };
        }

        public async Task<DefaultResponse> Update(string requestUserLogin, Report report)
        {
            var reportResponse = await GetById(requestUserLogin, report.Id);

            if (!reportResponse.Done)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = reportResponse.Message,
                    Done = false
                };

            var entity = _mapper.Map<Report>(report);

            var updatingTask = _repos.Update(entity);
            updatingTask.Wait();

            await _repos.SaveChangesAsync();

            if (updatingTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report updated successfully!",
                    Done = true
                };

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not updated!",
                Done = false
            };
        }

        public async Task<DefaultResponse> Remove(string requestUserLogin, int reportId)
        {
            var reportResponse = await GetById(requestUserLogin, reportId);

            if (!reportResponse.Done)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = reportResponse.Message,
                    Done = false
                };

            var removingTask = _repos.Remove<Report>(reportResponse.Report);
            removingTask.Wait();

            await _repos.SaveChangesAsync();

            if (!removingTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The report not removed!",
                    Done = false
                };

            if (System.IO.File.Exists(reportResponse.Report.Path + reportResponse.Report.Name))
                System.IO.File.Delete(reportResponse.Report.Path + reportResponse.Report.Name);

            return new DefaultResponse()
            {
                Status = "Success",
                Message = "The report removed successfully!",
                Done = true
            };
        }

        private async Task<DefaultResponse> Create(User user, File file, Report report)
        {
            var entity = _mapper.Map<Report>(report);

            var addingTask = _repos.Add(entity);
            addingTask.Wait();

            if (!addingTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The report not added!",
                    Done = false
                };

            var generateTask = await Save(user, file, entity);

            if (generateTask.Done)
            {
                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report created successfully!",
                    Done = true
                };
            }

            await _repos.Remove(entity);
            await _repos.SaveChangesAsync();

            return new DefaultResponse()
            {
                Status = "Error",
                Message = generateTask.Message,
                Done = false
            };
        }

        private async Task<DefaultResponse> Create(User user, File file, Report report, string provider)
        {
            if (provider == null)
                return new ReportResponse()
                {
                    Status = "Error",
                    Message = "The provider cannot be NULL!",
                    Done = false
                };

            var entity = _mapper.Map<Report>(report);

            var addingTask = _repos.Add(entity);
            addingTask.Wait();

            if (!addingTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The report not added!",
                    Done = false
                };

            var generateTask = await Save(user, file, entity, provider);

            if (generateTask.Done)
            {
                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report created successfully!",
                    Done = true
                };
            }

            await _repos.Remove(entity);
            await _repos.SaveChangesAsync();

            return new DefaultResponse()
            {
                Status = "Error",
                Message = generateTask.Message,
                Done = false
            };
        }

        private async Task<DefaultResponse> Save(User user, File file, Report report)
        {
            if (!System.IO.Directory.Exists(ApplicationPath))
                System.IO.Directory.CreateDirectory(ApplicationPath);

            string fromFile = file.Path + file.Name;

            if (!System.IO.File.Exists(fromFile))
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The file not uploaded!",
                    Done = false
                };

            var savingTask = new DefaultResponse();

            if (report.Format == ".xlsx")
                savingTask = await _reportBuilder.DefaultSaveAsExcel(user, file, report);

            if (report.Format == ".pdf")
                savingTask = await _reportBuilder.DefaultSaveAsPdf(user, file, report);

            if (savingTask.Done)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report generated successfully! (" + report.Format + ")",
                    Done = true
                };

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not generated! " + savingTask.Message,
                Done = false
            };
        }

        private async Task<DefaultResponse> Save(User user, File file, Report report, string provider)
        {
            if (!System.IO.Directory.Exists(ApplicationPath))
                System.IO.Directory.CreateDirectory(ApplicationPath);

            string fromFile = file.Path + file.Name;

            if (!System.IO.File.Exists(fromFile))
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The file not uploaded!",
                    Done = false
                };

            var savingTask = new DefaultResponse();

            if (report.Format == ".xlsx")
                savingTask = await _reportBuilder.ProviderSaveAsExcel(provider, user, file, report);

            if (report.Format == ".pdf")
                savingTask = await _reportBuilder.ProviderSaveAsPdf(provider, user, file, report);

            if (savingTask.Done)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report generated successfully! (" + report.Format + ")",
                    Done = true
                };

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not generated! " + savingTask.Message,
                Done = false
            };
        }
    }
}
