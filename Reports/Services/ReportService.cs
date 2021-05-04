using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Entities;
using AutoMapper;
using OfficeOpenXml;
using System.IO;

namespace Reports.Services
{
    public class ReportService : IReportService
    {
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

        public async Task<int> Create(Report report)
        {
            var entity = _mapper.Map<Report>(report);

            var result = await _repos.Add(entity);

            await Generate(entity);

            await _repos.SaveChangesAsync();

            return result;
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
