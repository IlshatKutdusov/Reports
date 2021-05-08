using Reports.Entities;

namespace Reports.Models
{
    public class ReportResponse : DefaultResponse
    {
        public Report? Report { get; set; }
    }
}
