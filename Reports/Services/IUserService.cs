using System.Threading.Tasks;
using Reports.Models;
using Reports.Entities;

namespace Reports.Services
{
    public interface IUserService
    {
        Task<DefaultResponse> Login(LoginModel loginModel);
        Task<DefaultResponse> Register(RegisterModel registerModel);

        Task<User> GetById(int userId);
        Task<User> GetByLogin(string userLogin);
        Task<DefaultResponse> Create(User user);
        Task<DefaultResponse> Update(User user);
    }
}
