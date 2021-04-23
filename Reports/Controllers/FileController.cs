using Microsoft.AspNetCore.Mvc;
using Reports.API.Models;
using Reports.API.Services;
using System.Threading.Tasks;

namespace Reports.API.Controllers
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
            var file = await _userService.Get(fileId);

            return Ok(file);
        }

        [HttpPost]
        public async Task<IActionResult> Create(File file)
        {
            var fileId = await _userService.Create(file);

            return Ok(fileId);
        }

        [HttpPut]
        public async Task<IActionResult> Update(File file)
        {
            await _userService.Update(file);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int fileId)
        {
            await _userService.Delete(fileId);

            return Ok();
        }
    }
}
