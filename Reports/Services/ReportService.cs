using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Entities;
using AutoMapper;
using OfficeOpenXml;
using System.IO;
using Reports.Models;
using Aspose.Cells;

namespace Reports.Services
{
    public class ReportService : IReportService
    {
        private readonly string ApplicationPath = System.IO.Directory.GetCurrentDirectory() + "\\SourceData\\Reports\\";

        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly IFileService _fileService;

        public ReportService(IMapper mapper, IRepos repos, IFileService fileService)
        {
            _mapper = mapper;
            _repos = repos;
            _fileService = fileService;
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

        public async Task<DefaultResponse> Create(Report report)
        {
            var entity = _mapper.Map<Report>(report);

            await _repos.Add(entity);

            var generateTask = await Generate(entity);

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
                Message = "The report not generated!"
            };
        }

        public async Task<DefaultResponse> CreateReportFromFile(Reports.Entities.File file, string format)
        {
            string _format = ".xlsx";

            if(format == "pdf")
                _format = ".pdf";

            var newReport = new Report()
            {
                UserId = file.UserId,
                FileId = file.Id,
                Name = "Report_" + file.Name.Remove(file.Name.Length - 4, 4) + _format,
                Path = ApplicationPath,
                Format = format
            };

            var task = await Create(newReport);

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
            var task = _repos.Remove<Report>(reportId);

            if (task.IsCompletedSuccessfully)
            {
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

        private async Task<DefaultResponse> Generate(Report report)
        {
            if (!System.IO.Directory.Exists(ApplicationPath))
            {
                System.IO.Directory.CreateDirectory(ApplicationPath);
            }

            var file = await _fileService.GetById(report.FileId);

            if (file == null)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The file not created!"
                };

            string fromFile = file.Path + file.Name;

            if (!System.IO.File.Exists(fromFile))
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The file not uploaded!"
                };


            if (report.Format == "excel")
            {
                var format = new ExcelTextFormat();
                format.Delimiter = ';';
                format.EOL = "\r";

                using (ExcelPackage package = new ExcelPackage(new FileInfo(report.Path + report.Name)))
                {
                    ExcelWorksheet excelWorksheet = package.Workbook.Worksheets.Add("Report №" + report.Id);
                    excelWorksheet.Cells.LoadFromText(new FileInfo(fromFile), format, OfficeOpenXml.Table.TableStyles.Medium1, true);
                    package.Save();
                }

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report (.xlsx) generated successfully!"
                };
            }

            if (report.Format == "pdf")
            {
                using (Workbook workbook = new Workbook(file.Path + file.Name))
                {
                    workbook.Save(report.Path + report.Name, SaveFormat.Pdf);
                }

                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "The report (.pdf) generated successfully!"
                };
            }

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "The report not generated!"
            };
        }
    }
}
