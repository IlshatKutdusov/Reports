using Microsoft.AspNetCore.Mvc;
using Reports.Models;
using Reports.Services;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int reportId)
        {
            var report = await _reportService.Get(reportId);

            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Report report)
        {
            var reportId = await _reportService.Create(report);

            return Ok(reportId);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Report report)
        {
            await _reportService.Update(report);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int reportId)
        {
            await _reportService.Delete(reportId);

            return Ok();
        }
    }
}
