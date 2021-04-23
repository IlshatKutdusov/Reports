using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reports.Database;
using Reports.Models;
using AutoMapper;

namespace Reports.Services
{
    public class ReportService : IReportService
    {
        private readonly IMapper _mapper;
        private readonly IRepos _repos;
        private readonly User _userEntity;

        public ReportService(IMapper mapper, IRepos repos, User userEntity)
        {
            _mapper = mapper;
            _repos = repos;
            _userEntity = userEntity;
        }
        public async Task<Report> Get(int reportId)
        {
            var report = await _repos.Get<Report>().FirstOrDefaultAsync(e => e.Id == reportId);

            var entity = _mapper.Map<Report>(report);

            await _repos.SaveChangesAsync();

            return entity;
        }

        public async Task<int> Create(Report report)
        {
            var entity = _mapper.Map<Report>(report);

            var result = await _repos.Add(entity);

            await _repos.SaveChangesAsync();

            return result;
        }

        public async Task Update(Report report)
        {
            var entity = _mapper.Map<Report>(report);

            await _repos.Update(entity);
            await _repos.SaveChangesAsync();
        }

        public async Task Delete(int reportId)
        {
            await _repos.Delete<Report>(reportId);
            await _repos.SaveChangesAsync();
        }
    }
}
