using System.Threading.Tasks;
using Reports.Entities;
using Reports.Models;

namespace Reports.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest authenticateRequest);
        Task<AuthenticateResponse> Register(User user);

        Task<User> GetById(int userId);
        Task<int> Create(RegistrationRequest user);
        Task Update(User user);
        Task Delete(User userId);
    }
}
