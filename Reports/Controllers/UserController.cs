﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reports.API.Models;
using ReportsService.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reports.API.Controllers
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
            var user = await _userService.Get(userId);

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            var userId = await _userService.Create(user);

            return Ok(userId);
        }

        [HttpPut]
        public async Task<IActionResult> Update(User user)
        {
            await _userService.Update(user);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int userId)
        {
            await _userService.Delete(userId);

            return Ok();
        }
    }
}