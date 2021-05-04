using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using AutoMapper;
using System.Linq;
using Reports.Entities;
using Microsoft.Extensions.Configuration;
using Reports.Authentication;
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

            var files = _repos.Get<File>().Where(e => e.UserId == user.Id).ToList();

            var reports = _repos.Get<Report>().Where(e => e.UserId == user.Id).ToList();

            var entity = _mapper.Map<User>(user);

            await _repos.SaveChangesAsync();

            return entity;
        }

        public async Task<User> GetByLogin(string userLogin)
        {
            var user = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Login == userLogin);

            var files = _repos.Get<File>().Where(e => e.UserId == user.Id).ToList();

            var reports = _repos.Get<Report>().Where(e => e.UserId == user.Id).ToList();

            var entity = _mapper.Map<User>(user);
            
            await _repos.SaveChangesAsync();

            return entity;
        }

        public async Task<int> Create(User user)
        {
            var entity = _mapper.Map<User>(user);

            var result = await _repos.Add(entity);

            await _repos.SaveChangesAsync();

            return result;
        }

        public async Task Update(User user)
        {
            var entity = _mapper.Map<User>(user);

            await _repos.Update(entity);
            await _repos.SaveChangesAsync();
        }

        public async Task Delete(User user)
        {
            await _repos.Remove<User>(user);
            await _repos.SaveChangesAsync();
        }

        public async Task<Response> Login(LoginModel loginModel)
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

                return new Response
                {
                    Status = "Success",
                    Message = "Token: " + new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
            return new Response
            {
                Status = "Error",
                Message = "User not founded!"
            };
        }

        public async Task<Response> Register(RegisterModel registerModel)
        {
            var userExists = await _userManager.FindByNameAsync(registerModel.Login);
            if (userExists != null)
                return new Response { Status = "Error", Message = "User already exists!" };

            ApplicationUser user = new ApplicationUser()
            {
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.Login
            };

            await Create(new User()
            {
                Surname = registerModel.Surname,
                Name = registerModel.Name,
                Login = registerModel.Login
            });

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
                return new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." };

            return new Response { Status = "Success", Message = "User created successfully!" };
        }
    }
}
