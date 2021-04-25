using Microsoft.AspNetCore.Mvc;
using Reports.Models;
using Reports.Services;
using System;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int userId)
        {
            try
            {
                var user = await _userService.Get(userId);

                return Ok(user);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                var userId = await _userService.Create(user);

                return Ok(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(User user)
        {
            try
            {
                await _userService.Update(user);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int userId)
        {
            try
            {
                await _userService.Delete(userId);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
