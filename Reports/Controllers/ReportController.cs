using Microsoft.AspNetCore.Mvc;
using Reports.Entities;
using Reports.Services;
using System;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    public class ReportController : ApiControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int reportId)
        {
            try
            {
                var report = await _reportService.Get(reportId);

                return Ok(report);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Report report)
        {
            try
            {
                var reportId = await _reportService.Create(report);

                return Ok(reportId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Report report)
        {
            try
            {
                await _reportService.Update(report);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int reportId)
        {
            try
            {
                await _reportService.Delete(reportId);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
