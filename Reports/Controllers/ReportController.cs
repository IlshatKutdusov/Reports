using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reports.Models;
using Reports.Entities;
using Reports.Services;
using System;
using System.Security.Claims;
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
        public async Task<IActionResult> GetById(int reportId)
        {
            try
            {
                var report = await _reportService.GetById(GetCurrentUserName(), reportId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message,
                    Done = false
                });
            }
        }


        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> Generate(int fileId, string format)
        {
            try
            {
                var response = await _reportService.GenerateFromFile(GetCurrentUserName(), fileId, format);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message,
                    Done = false
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Report report)
        {
            try
            {
                var response = await _reportService.Update(GetCurrentUserName(), report);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message,
                    Done = false
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int reportId)
        {
            try
            {
                var response = await _reportService.Remove(GetCurrentUserName(), reportId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message,
                    Done = false
                });
            }
        }

        private string GetCurrentUserName() => User.FindFirstValue(ClaimTypes.Name);
    }
}
