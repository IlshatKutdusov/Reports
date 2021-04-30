using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Entities;
using Reports.Models;
using Reports.Services;
using System;
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

        [HttpPut("Autherticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest authenticateRequest)
        {
            try
            {
                var user = _userService.Authenticate(authenticateRequest);

                if (user == null)
                    return BadRequest(new { message = "Неправильно введен логин или пароль!" });

                return Ok(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("Register")]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                var response = await _userService.Create(user);

                if (user == null)
                    return BadRequest(new { message = "Регистрация пользователя завершилась с ошибкой!" });

                return Ok(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get(int userId)
        {
            try
            {
                var user = await _userService.GetById(userId);

                return Ok(user);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(User userId)
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
