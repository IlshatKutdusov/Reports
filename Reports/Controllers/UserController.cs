using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Reports.Authentication;
using Reports.Entities;
using Reports.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    [Authorize]
    public class UserController : ApiControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public UserController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IUserService userService)
        {
            this.userManager = userManager;
            _configuration = configuration;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var response = await _userService.Login(loginModel);

                return Ok(response);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                var response = await _userService.Register(registerModel);

                return Ok(response);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByLogin(string userLogin)
        {
            try
            {
                var user = await _userService.GetByLogin(userLogin);

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
        public async Task<IActionResult> Delete(User user)
        {
            try
            {
                await _userService.Delete(user);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
