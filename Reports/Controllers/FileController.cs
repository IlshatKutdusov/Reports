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
                var user = await _userService.GetById(file.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    return Ok(file); 
                }

                return BadRequest();
            }
            catch (Exception)
            {

                throw;
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
                    var fileId = await _fileService.Create(file);

                    return Ok(fileId); 
                }

                return BadRequest();
            }
            catch (Exception)
            {

                throw;
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
                    await _fileService.Update(file);

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
        public async Task<IActionResult> Delete(int fileId)
        {
            try
            {
                var file = await _fileService.GetById(fileId);
                var user = await _userService.GetById(file.UserId);

                if (GetCurrentUserName() == user.Login)
                {
                    await _fileService.Delete(fileId);

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
