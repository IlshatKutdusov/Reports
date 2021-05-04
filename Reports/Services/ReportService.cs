using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Entities;
using AutoMapper;
using OfficeOpenXml;
using System.IO;
using Reports.Models;

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

            var entity = _mapper.Map<Report>(report);

            await _repos.SaveChangesAsync();

            return entity;
        }

        public async Task<CreationResponse> Create(Report report)
        {
            var entity = _mapper.Map<Report>(report);

            var result = await _repos.Add(entity);

            await Generate(entity);

            await _repos.SaveChangesAsync();

            return new CreationResponse() { IsCreated = true, Result = result};
        }

        public async Task<CreationResponse> CreateReportFromFile(Reports.Entities.File file, string format)
        {
            var newReport = new Report()
            {
                UserId = file.UserId,
                FileId = file.Id,
                Name = "Report_" + file.Name.Remove(file.Name.Length - 4, 4) + ".xlsx",
                Path = ApplicationPath,
                Format = format
            };

            var report = await Create(newReport);

            if (report.IsCreated)
            {
                Generate(newReport);

                return new CreationResponse() { IsCreated = true, Result = report.Result};
            }
            else
            {
                return new CreationResponse() { IsCreated = false};
            }
        }

        public async Task Update(Report report)
        {
            var entity = _mapper.Map<Report>(report);

            await _repos.Update(entity);
            await _repos.SaveChangesAsync();
        }

        public async Task Delete(int reportId)
        {
            await _repos.Delete<Report>(reportId);
            await _repos.SaveChangesAsync();
        }

        public async Task Generate(Report report)
        {
            if (!System.IO.Directory.Exists(ApplicationPath))
            {
                System.IO.Directory.CreateDirectory(ApplicationPath);
            }

            if (report.Format == "excel")
            {
                var file = await _fileService.GetById(report.FileId);
                string fromFile = file.Path + file.Name;

                var format = new ExcelTextFormat();
                format.Delimiter = ';';
                format.EOL = "\r";

                using (ExcelPackage package = new ExcelPackage(new FileInfo(report.Path + report.Name)))
                {
                    ExcelWorksheet excelWorksheet = package.Workbook.Worksheets.Add("Report №" + report.Id);
                    excelWorksheet.Cells.LoadFromText(new FileInfo(fromFile), format, OfficeOpenXml.Table.TableStyles.Medium1, true);
                    package.Save();
                }
            }

            if (report.Format == "pdf")
            {

            }
        }
    }
}
