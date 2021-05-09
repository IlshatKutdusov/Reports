using System.Collections.Generic;

namespace Reports.Models
{
    public class ProvidersResponse : DefaultResponse
    {
        public IList<string>? Providers { get; set; }
    }
}
