namespace Reports.Models
{
    public class Report : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public string Size { get; set; }
    }
}
