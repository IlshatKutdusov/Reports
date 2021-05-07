using Reports.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reports.Models
{
    public class UploadFileResponse : DefaultResponse
    {
        public bool Done { get; set; }

        public File? File { get; set; }
    }
}
