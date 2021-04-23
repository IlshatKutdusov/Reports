using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.API.Database;
using Reports.API.Models;
using AutoMapper;

namespace Reports.API.Services
{
    public class FileService : IFileService
    {
        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly UserEntity _userEntity;

        public FileService(IMapper mapper, IRepos repos, UserEntity userEntity)
        {
            _mapper = mapper;
            _repos = repos;
            _userEntity = userEntity;
        }
        public async Task<File> Get(int fileId)
        {
            var file = await _repos.Get<File>().FirstOrDefaultAsync(e => e.Id == fileId);

            var entity = _mapper.Map<File>(file);

            await _repos.SaveChangesAsync();

            return entity;
        }

        public async Task<int> Create(File file)
        {
            var entity = _mapper.Map<File>(file);

            var result = await _repos.Add(entity);

            await _repos.SaveChangesAsync();

            return result;
        }

        public async Task Update(File file)
        {
            var entity = _mapper.Map<File>(file);

            await _repos.Update(entity);
            await _repos.SaveChangesAsync();
        }

        public async Task Delete(int fileId)
        {
            await _repos.Delete<File>(fileId);
            await _repos.SaveChangesAsync();
        }
    }
}
