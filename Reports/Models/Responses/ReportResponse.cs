﻿using Reports.Entities;

namespace Reports.Models.Responses
{
    public class ReportResponse : DefaultResponse
    {
        public Report? Report { get; set; }

        public ReportResponse() { }

        public ReportResponse(DefaultResponse defaultResponse)
        {
            this.Status = defaultResponse.Status;
            this.Message = defaultResponse.Message;
            this.Done = defaultResponse.Done;
        }
    }
}
