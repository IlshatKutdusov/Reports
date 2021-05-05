using Microsoft.AspNetCore.Http;
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
    public class FileController : ApiControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;

        public FileController(IFileService fileService, IUserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int fileId)
        {
            try
            {
                var file = await _fileService.GetById(fileId);

                if (file != null)
                {
                    var user = await _userService.GetById(file.UserId);

                    if (GetCurrentUserName() == user.Login)
                    {
                        return Ok(file);
                    }

                    return BadRequest("Access is denied!");
                }

                return BadRequest("The file not found!");
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
        public async Task<IActionResult> Create(File file)
        {
            try
            {
                var user = await _userService.GetById(file.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    var response = await _fileService.Create(file);

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
        [Route("Upload")]
        public async Task<IActionResult> Upload(string userLogin, IFormFile upload)
        {
            try
            {
                if (GetCurrentUserName() == userLogin && upload != null)
                {
                    var response = await _fileService.UploadFile(userLogin, upload);

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
        public async Task<IActionResult> Update(File file)
        {
            try
            {
                var user = await _userService.GetById(file.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    var response = await _fileService.Update(file);

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
        public async Task<IActionResult> Delete(int fileId)
        {
            try
            {
                var file = await _fileService.GetById(fileId);
                var user = await _userService.GetById(file.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    var response = await _fileService.Remove(fileId);

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
