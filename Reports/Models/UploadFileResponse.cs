using Reports.Entities;

namespace Reports.Models
{
    public class UploadFileResponse : DefaultResponse
    {
        public bool Done { get; set; }

        public File? File { get; set; }
    }
}
