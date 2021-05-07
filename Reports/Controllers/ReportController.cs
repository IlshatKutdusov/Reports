﻿using Microsoft.AspNetCore.Identity;
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
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public ReportController(IReportService reportService, IUserService userService, IFileService fileService)
        {
            _reportService = reportService;
            _userService = userService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int reportId)
        {
            try
            {
                var report = await _reportService.GetById(reportId);

                if (report != null)
                {
                    var user = await _userService.GetById(report.UserId);

                    if (GetCurrentUserName() == user.Login)
                    {
                        return Ok(report);
                    }

                    return BadRequest("Access is denied!");
                }

                return BadRequest("The report not found!");
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Report report)
        {
            try
            {
                var file = await _fileService.GetById(report.FileId);

                if (file == null)
                    return BadRequest(new DefaultResponse()
                    {
                        Status = "Error",
                        Message = "The file not found!"
                    });

                var user = await _userService.GetById(report.UserId);

                if (GetCurrentUserName() == user?.Login && user != null)
                {
                    var response = await _reportService.Create(user, file, report);

                    return Ok(response);
                }

                return BadRequest("Access is denied!");
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message
                });
            }
        }

        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> Generate(int fileId, string format)
        {
            try
            {
                var file = await _fileService.GetById(fileId);

                if (file == null)
                    return BadRequest(new DefaultResponse()
                    {
                        Status = "Error",
                        Message = "The file not found!"
                    });

                var user = await _userService.GetById(file.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    var response = await _reportService.Generate(fileId, format);

                    return Ok(response);
                }

                return BadRequest("Access is denied!");
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message
                });
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
                    var response = await _reportService.Update(report);

                    return Ok(response); 
                }

                return BadRequest("Access is denied!");
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int reportId)
        {
            try
            {
                var report = await _reportService.GetById(reportId);

                if (report == null)
                    return BadRequest(new DefaultResponse()
                    {
                        Status = "Error",
                        Message = "The report not found!"
                    });

                var user = await _userService.GetById(report.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    var response = await _reportService.Remove(reportId);

                    return Ok(response); 
                }

                return BadRequest("Access is denied!");
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse()
                {
                    Status = "Error",
                    Message = "Message:  " + ex.Message
                });
            }
        }

        private string GetCurrentUserName() => User.FindFirstValue(ClaimTypes.Name);
    }
}
