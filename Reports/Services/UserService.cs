using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using AutoMapper;
using System.Linq;
using Reports.Entities;
using Microsoft.Extensions.Configuration;
using Reports.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Reports.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IMapper mapper, IRepos repos, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _repos = repos;
            _configuration = configuration;
            this._userManager = userManager;
        }

        public async Task<UserResponse> GetById(string requestUserLogin, int userId)
        {
            var user = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Login == requestUserLogin);

            if (user == null)
                return new UserResponse()
                {
                    Status = "Error",
                    Message = "User not found!",
                    Done = false
                };

            if (user.Id != userId)
                return new UserResponse()
                {
                    Status = "Error",
                    Message = "Access is denied!",
                    Done = false
                };

            var files = _repos.Get<File>().Where(e => e.UserId == user.Id).ToList();

            var reports = _repos.Get<Report>().Where(e => e.UserId == user.Id).ToList();

            var entity = _mapper.Map<User>(user);

            return new UserResponse()
            {
                Status = "Success",
                Message = "The file found successfully!",
                Done = true,
                User = entity
            };
        }

        public async Task<UserResponse> GetByLogin(string requestUserLogin, string userLogin)
        {
            var user = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Login == requestUserLogin);

            if (user == null)
                return new UserResponse()
                {
                    Status = "Error",
                    Message = "User not found!",
                    Done = false
                };

            if (user.Login != userLogin)
                return new UserResponse()
                {
                    Status = "Error",
                    Message = "Access is denied!",
                    Done = false
                };

            var files = _repos.Get<File>().Where(e => e.UserId == user.Id).ToList();

            var reports = _repos.Get<Report>().Where(e => e.UserId == user.Id).ToList();

            var entity = _mapper.Map<User>(user);

            return new UserResponse()
            {
                Status = "Success",
                Message = "User found successfully!",
                Done = true,
                User = entity
            };
        }

        private async Task<DefaultResponse> Create(User user)
        {
            var entity = _mapper.Map<User>(user);

            var creatingTask = _repos.Add(entity);
            creatingTask.Wait();

            await _repos.SaveChangesAsync();

            if (creatingTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "User created successfully!",
                    Done = true
                }; 

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "User not created!",
                Done = false
            };
        }

        public async Task<DefaultResponse> Update(string requestUserLogin, User user)
        {
            var requestUser = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Login == requestUserLogin);

            if (requestUser == null)
                return new UserResponse()
                {
                    Status = "Error",
                    Message = "User not found!",
                    Done = false
                };

            if (requestUser.Login != user.Login)
                return new UserResponse()
                {
                    Status = "Error",
                    Message = "Access is denied!",
                    Done = false
                };

            var entity = _mapper.Map<User>(user);

            var updatingTask = _repos.Update(entity);
            updatingTask.Wait();

            await _repos.SaveChangesAsync();

            if (updatingTask.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "User updated successfully!",
                    Done = true
                };

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "User not updated!",
                Done = false
            };
        }

        public async Task<DefaultResponse> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Login);

            if (!(user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password)))
                return new DefaultResponse
                {
                    Status = "Error",
                    Message = "User not found!"
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            //await _repos.SaveChangesAsync();

            return new DefaultResponse
            {
                Status = "Success",
                Message = "Token: " + new JwtSecurityTokenHandler().WriteToken(token),
                Done = true
            };
        }

        public async Task<DefaultResponse> Register(RegisterModel registerModel)
        {
            var userLoginExists = await _userManager.FindByNameAsync(registerModel.Login);

            if (userLoginExists != null)
                return new DefaultResponse { 
                    Status = "Error", 
                    Message = "User with this login already exists!",
                    Done = false
                };

            var user = new ApplicationUser()
            {
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.Login,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
                return new DefaultResponse { 
                    Status = "Error", 
                    Message = "User creation failed! Errors: " + result.Errors,
                    Done = false
                };

            var creationTask = await Create(new User()
            {
                Surname = registerModel.Surname,
                Name = registerModel.Name,
                Login = registerModel.Login
            });

            if (creationTask.Done)
                return new DefaultResponse
                {
                    Status = "Success",
                    Message = "User created successfully!",
                    Done = true
                };

            return new DefaultResponse
            {
                Status = "Error",
                Message = "User not created!",
                Done = false
            };
        }
    }
}
