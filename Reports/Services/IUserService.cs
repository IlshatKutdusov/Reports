using System.Threading.Tasks;
using Reports.Models;
using Reports.Entities;

namespace Reports.Services
{
    public interface IUserService
    {
        Task<UserResponse> GetById(string requestUserLogin, int userId);
        Task<UserResponse> GetByLogin(string requestUserLogin, string userLogin);

        Task<DefaultResponse> Update(string requestUserLogin, User user);

        Task<DefaultResponse> Login(LoginModel loginModel);
        Task<DefaultResponse> Register(RegisterModel registerModel);
    }
}
