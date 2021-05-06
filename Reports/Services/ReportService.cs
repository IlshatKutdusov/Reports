using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Entities;
using AutoMapper;
using OfficeOpenXml;
using Reports.Models;
using Aspose.Cells;
using System.Data;
using System.Text.RegularExpressions;
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
        private readonly IReportBuilder _reportBuilder;

        public ReportService(IMapper mapper, IRepos repos, IFileService fileService, IUserService userService, IReportBuilder reportBuilder)
        {
            _mapper = mapper;
            _repos = repos;
            _fileService = fileService;
            _userService = userService;
            _reportBuilder = reportBuilder;
        }

        public async Task<Report> GetById(int reportId)
        {
            var report = await _repos.Get<Report>().FirstOrDefaultAsync(e => e.Id == reportId);

            if (report != null)
            {
                var entity = _mapper.Map<Report>(report);

                await _repos.SaveChangesAsync();

                return entity; 
            }

            return null;
        }

        public async Task<DefaultResponse> Create(User user, File file, Report report)
        {
            var entity = _mapper.Map<Report>(report);

            await _repos.Add(entity);

            var generateTask = await Generate(user, file, entity);

            if (generateTask.Status == "Success")
            {
                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report created successfully!"
                };
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not generated! " + generateTask.Message
            };
        }

        public async Task<DefaultResponse> Update(Report report)
        {
            var entity = _mapper.Map<Report>(report);

            var task = _repos.Update(entity);

            if (task.IsCompletedSuccessfully)
            {
                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report updated successfully!"
                };
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not updated!"
            };
        }

        public async Task<DefaultResponse> Remove(int reportId)
        {
            var report = await GetById(reportId);

            if (report == null)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The report not found!"
                };

            var task = _repos.Remove<Report>(report);
            task.Wait();

            if (task.IsCompletedSuccessfully)
            {
                if (System.IO.File.Exists(report.Path + report.Name))
                    System.IO.File.Delete(report.Path + report.Name);

                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report removed successfully!"
                };
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not removed!"
            };
        }

        public async Task<DefaultResponse> Generate(int fileId, string format)
        {
            var file = await _fileService.GetById(fileId);

            if (file == null)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The file not found!"
                };

            string _format = ".xlsx";

            if (format == "pdf")
                _format = ".pdf";

            var newReport = new Report()
            {
                UserId = file.UserId,
                FileId = file.Id,
                Name = "Report_" + file.Name.Remove(file.Name.Length - 4, 4) + _format,
                Path = ApplicationPath,
                Format = format
            };

            var user = await _userService.GetById(file.UserId);

            if (user == null)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "User not found!"
                };

            var task = await Create(user, file, newReport);

            if (task.Status == "Success")
            {

                await _repos.SaveChangesAsync();

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report created successfully!"
                };
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not created! " + task.Message
            };
        }

        private async Task<DefaultResponse> Generate(User user, File file, Report report)
        {
            if (!System.IO.Directory.Exists(ApplicationPath))
                System.IO.Directory.CreateDirectory(ApplicationPath);

            string fromFile = file.Path + file.Name;

            if (!System.IO.File.Exists(fromFile))
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The file not uploaded!"
                };


            if (report.Format == "excel")
            {
                /*var format = new ExcelTextFormat();
                format.Delimiter = ';';
                format.EOL = "\r";

                using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(report.Path + report.Name)))
                {
                    ExcelWorksheet excelWorksheet = package.Workbook.Worksheets.Add("Report");
                    excelWorksheet.Cells[3, 3].Value = "Report:";
                    excelWorksheet.Cells[3, 4].Value = report.Name;
                    excelWorksheet.Cells[4, 3].Value = "From file:";
                    excelWorksheet.Cells[4, 4].Value = file.Name;
                    excelWorksheet.Cells[6, 3].Value = "Owner";
                    excelWorksheet.Cells[7, 3].Value = "Surname:";
                    excelWorksheet.Cells[7, 4].Value = user.Surname;
                    excelWorksheet.Cells[8, 3].Value = "Name:";
                    excelWorksheet.Cells[8, 4].Value = user.Name;
                    excelWorksheet.Cells[10, 3].Value = "Source data:";
                    excelWorksheet.Cells[11, 3].LoadFromText(new System.IO.FileInfo(fromFile), format, OfficeOpenXml.Table.TableStyles.Medium1, true);
                    try
                    {
                        package.Save();
                    }
                    catch (System.Exception)
                    {
                        return new DefaultResponse()
                        { 
                            Status = "Error",
                            Message = "The report not saved!"
                        };
                    }
                }*/

                var task = _reportBuilder.SaveAsExcel(user, file, report);
                task.Wait();

                if (task.IsCompletedSuccessfully)
                {
                    return new DefaultResponse()
                    {
                        Status = "Success",
                        Message = "The report (.xlsx) generated successfully!"
                    }; 
                }
            }

            if (report.Format == "pdf")
            {
                /*using (Workbook workbook = new Workbook(file.Path + file.Name))
                {
                    workbook.Save(report.Path + report.Name, SaveFormat.Pdf);
                }*/

                var task = _reportBuilder.SaveAsPdf(user, file, report);
                task.Wait();

                if (task.IsCompletedSuccessfully)
                {
                    return new DefaultResponse()
                    {
                        Status = "Success",
                        Message = "The report (.pdf) generated successfully!"
                    }; 
                }
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not generated!"
            };
        }
    }
}
