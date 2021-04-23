namespace Reports.Models
{
    public class UserEntity : BaseEntity
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string HashedPassword { get; set; }
    }
}
