using System.IO;

namespace Reports.Models
{
    public class FileStreamResponse : DefaultResponse
    {
        public FileStream? FileStream { get; set; }

        public string? FileFormat { get; set; }

        public string? FileName { get; set; }
    }
}
