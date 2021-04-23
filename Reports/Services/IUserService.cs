using Reports.API.Models;
using System;
using System.Threading.Tasks;

namespace ReportsService.Database.Services
{
    public interface IUserService
    {
        Task<int> Create(User user);
        Task<User> Get(int id);
        Task Update(User user);
        Task Delete(int userId);
    }
}
