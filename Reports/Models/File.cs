using System.Collections.Generic;

namespace Reports.Models
{
    public class File : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }

        public List<Report> Reports { get; set; }

    }
}
