using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reports.Authentication;
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
        private readonly IUserService _userService;

        public ReportController(IReportService reportService, IUserService userService)
        {
            _reportService = reportService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int reportId)
        {
            try
            {
                var report = await _reportService.GetById(reportId);
                var user = await _userService.GetById(report.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    return Ok(report); 
                }

                return BadRequest();
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
                var user = await _userService.GetById(report.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    var reportId = await _reportService.Create(report);

                    return Ok(reportId); 
                }

                return BadRequest();
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
                var user = await _userService.GetById(report.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    await _reportService.Update(report);

                    return Ok(); 
                }

                return BadRequest();
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
                var report = await _reportService.GetById(reportId);
                var user = await _userService.GetById(report.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    await _reportService.Delete(reportId);

                    return Ok(); 
                }

                return BadRequest();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetCurrentUserName() => User.FindFirstValue(ClaimTypes.Name);
    }
}
