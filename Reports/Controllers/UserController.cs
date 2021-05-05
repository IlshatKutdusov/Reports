using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Reports.Models;
using Reports.Entities;
using Reports.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var response = await _userService.Login(loginModel);

                return Ok(response);
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


        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                var response = await _userService.Register(registerModel);

                return Ok(response);
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

        [HttpGet]
        public async Task<IActionResult> GetByLogin(string userLogin)
        {
            try
            {
                if (GetCurrentUserName() == userLogin)
                {
                    var user = await _userService.GetByLogin(userLogin);

                    if (user != null)
                    {
                        return Ok(user); 
                    }

                    return BadRequest("User not found!");
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
        public async Task<IActionResult> Update(User user)
        {
            try
            {
                if (GetCurrentUserName() == user.Login)
                {
                    var response = await _userService.Update(user);

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
