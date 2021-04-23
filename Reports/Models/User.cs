using System.Collections.Generic;

namespace Reports.API.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool isAdmin { get; set; }
        public List<File> Files { get; set; }
    }
}
