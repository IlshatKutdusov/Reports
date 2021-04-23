using System.Threading.Tasks;
using Reports.Models;

namespace Reports.Services
{
    public interface IUserService
    {
        Task<int> Create(User user);
        Task<User> Get(int userId);
        Task Update(User user);
        Task Delete(int userId);
    }
}
