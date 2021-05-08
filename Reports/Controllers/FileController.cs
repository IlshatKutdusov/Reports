using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reports.Models;
using Reports.Entities;
using Reports.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    public class FileController : ApiControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int fileId)
        {
            try
            {
                var fileResponse = await _fileService.GetById(GetCurrentUserName(), fileId);

                if (fileResponse.Done)
                    return Ok(fileResponse);

                return BadRequest(fileResponse);
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
        [Route("Upload")]
        public async Task<IActionResult> Upload(string userLogin, IFormFile upload)
        {
            try
            {
                var response = await _fileService.UploadFile(GetCurrentUserName(), userLogin, upload);

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
        public async Task<IActionResult> Update(File file)
        {
            try
            {
                var response = await _fileService.Update(GetCurrentUserName(), file);

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
        public async Task<IActionResult> Delete(int fileId)
        {
            try
            {
                var response = await _fileService.Remove(GetCurrentUserName(), fileId);

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
