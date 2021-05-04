using System.Threading.Tasks;
using Reports.Authentication;
using Reports.Entities;

namespace Reports.Services
{
    public interface IUserService
    {
        Task<Response> Login(LoginModel loginModel);
        Task<Response> Register(RegisterModel registerModel);

        Task<User> GetById(int userId);
        Task<User> GetByLogin(string userLogin);
        Task<int> Create(User user);
        Task Update(User user);
        Task Delete(User user);
    }
}
