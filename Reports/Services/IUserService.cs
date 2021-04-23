using System.Threading.Tasks;
using Reports.API.Models;

namespace Reports.API.Services
{
    public interface IUserService
    {
        Task<int> Create(User user);
        Task<User> Get(int userId);
        Task Update(User user);
        Task Delete(int userId);
    }
}
