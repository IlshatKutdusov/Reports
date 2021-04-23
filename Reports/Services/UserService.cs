using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Models;
using AutoMapper;

namespace Reports.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly UserEntity _userEntity;

        public UserService(IMapper mapper, IRepos repos, UserEntity userEntity)
        {
            _mapper = mapper;
            _repos = repos;
            _userEntity = userEntity;
        }
        public async Task<User> Get(int userId)
        {
            var user = await _repos.Get<User>().FirstOrDefaultAsync(e => e.Id == userId);
            
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

        public async Task Delete(int userId)
        {
            await _repos.Delete<User>(userId);
            await _repos.SaveChangesAsync();
        }
    }
}
