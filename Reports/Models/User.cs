using System.Collections.Generic;

namespace Reports.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string HashedPassword { get; set; }

        public List<File> Files { get; set; }
        public List<Report> Reports { get; set; }
    }
}
