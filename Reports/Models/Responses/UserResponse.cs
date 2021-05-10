using Reports.Entities;

namespace Reports.Models.Responses
{
    public class UserResponse : DefaultResponse
    {
        public User? User { get; set; }
    }
}
