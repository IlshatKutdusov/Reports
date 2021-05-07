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

        public async Task<User> GetById(int userId)
        {
            var user = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Id == userId);

            if (user != null)
            {
                var files = _repos.Get<File>().Where(e => e.UserId == user.Id).ToList();

                var reports = _repos.Get<Report>().Where(e => e.UserId == user.Id).ToList();

                var entity = _mapper.Map<User>(user);

                await _repos.SaveChangesAsync();

                return entity; 
            }

            return null;
        }

        public async Task<User> GetByLogin(string userLogin)
        {
            var user = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Login == userLogin);

            if (user != null)
            {
                var files = _repos.Get<File>().Where(e => e.UserId == user.Id).ToList();

                var reports = _repos.Get<Report>().Where(e => e.UserId == user.Id).ToList();

                var entity = _mapper.Map<User>(user);

                await _repos.SaveChangesAsync();

                return entity; 
            }

            return null;
        }

        public async Task<DefaultResponse> Create(User user)
        {
            var entity = _mapper.Map<User>(user);

            var task = _repos.Add(entity);
            task.Wait();

            await _repos.SaveChangesAsync();

            if (task.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "User created successfully!"
                }; 

            return new DefaultResponse()
            {
                Status = "Error",
                Message = "User not created!"
            };
        }

        public async Task<DefaultResponse> Update(User user)
        {
            var entity = _mapper.Map<User>(user);

            var task = _repos.Update(entity);
            task.Wait();

            await _repos.SaveChangesAsync();

            if (task.IsCompletedSuccessfully)
                return new DefaultResponse()
                {
                    Status = "Success",
                    Message = "User updated successfully!"
                };

            return new CreationResponse()
            {
                Status = "Error",
                Message = "User not updated!"
            };
        }

        public async Task<DefaultResponse> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Login);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
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

                await _repos.SaveChangesAsync();

                return new DefaultResponse
                {
                    Status = "Success",
                    Message = "Token: " + new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
            return new DefaultResponse
            {
                Status = "Error",
                Message = "User not found!"
            };
        }

        public async Task<DefaultResponse> Register(RegisterModel registerModel)
        {
            var userExists = await _userManager.FindByNameAsync(registerModel.Login);

            if (userExists != null)
                return new DefaultResponse { 
                    Status = "Error", 
                    Message = "User already exists!" 
                };

            var user = new ApplicationUser()
            {
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.Login
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
                return new DefaultResponse { 
                    Status = "Error", 
                    Message = "User creation failed! Please check user details and try again." 
                };

            var task = await Create(new User()
            {
                Surname = registerModel.Surname,
                Name = registerModel.Name,
                Login = registerModel.Login
            });

            if (task.Status == "Success")
                return new DefaultResponse
                {
                    Status = "Success",
                    Message = "User created successfully!"
                };

            return new DefaultResponse
            {
                Status = "Error",
                Message = "User not created!"
            };
        }
    }
}
