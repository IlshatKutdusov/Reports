using Reports.Entities;

namespace Reports.Models
{
    public class UserResponse : DefaultResponse
    {
        public User? User { get; set; }
    }
}
