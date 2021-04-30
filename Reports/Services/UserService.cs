using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Models;
using AutoMapper;
using System.Linq;
using Reports.Entities;
using Microsoft.Extensions.Configuration;
using Reports.Services.Helpers;

namespace Reports.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly IConfiguration _configuration;

        public UserService(IMapper mapper, IRepos repos, User userEntity, IConfiguration configuration)
        {
            _mapper = mapper;
            _repos = repos;
            _configuration = configuration;
        }

        public async Task<User> GetById(int userId)
        {
            var user = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Id == userId);

            var files = _repos.Get<File>().Where(e => e.UserId == userId).ToList();

            var reports = _repos.Get<Report>().Where(e => e.UserId == userId).ToList();

            var entity = _mapper.Map<User>(user);
            
            await _repos.SaveChangesAsync();

            return entity;
        }

        public async Task<int> Create(User user)
        {
            var entity = _mapper.Map<User>(user);

            var result = await _repos.Add(entity);

            await _repos.SaveChangesAsync();

            await Register(user);

            return result;
        }

        public async Task Update(User user)
        {
            var entity = _mapper.Map<User>(user);

            await _repos.Update(entity);
            await _repos.SaveChangesAsync();
        }

        public async Task Delete(User userId)
        {
            await _repos.Remove<User>(userId);
            await _repos.SaveChangesAsync();
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest authenticateRequest)
        {
            var user = _repos.GetAll<User>().FirstOrDefault(e => e.Login == authenticateRequest.Login && e.HashedPassword == authenticateRequest.Password);

            if (user == null)
                return null;

            var token = _configuration.GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public async Task<AuthenticateResponse> Register(User user)
        {
            var response = Authenticate(new AuthenticateRequest
            { 
                Login = user.Login,
                Password = user.HashedPassword
            });

            return response;
        }
    }
}
