using Microsoft.AspNetCore.Mvc;
using Reports.Models;
using Reports.Services;
using System;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    public class FileController : BaseController
    {
        private readonly IFileService _userService;

        public FileController(IFileService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int fileId)
        {
            try
            {
                var file = await _userService.Get(fileId);

                return Ok(file);
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
                var fileId = await _userService.Create(file);

                return Ok(fileId);
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
                await _userService.Update(file);

                return Ok();
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
                await _userService.Delete(fileId);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
