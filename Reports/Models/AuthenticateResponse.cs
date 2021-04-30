using Reports.Entities;

namespace Reports.Models
{
    public class AuthenticateResponse
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(User user, string token)
        {
            Surname = user.Surname;
            Name = user.Name;
            Login = user.Login;
            Token = token;
        }
    }
}
