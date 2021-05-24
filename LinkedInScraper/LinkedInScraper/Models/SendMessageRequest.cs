using System;
using System.Collections.Generic;

namespace LinkedInScraper.Models
{
    public class SendMessageRequest
    {
        public List<Guid> ProfileIds { get; set; }
        public string Message { get; set; }
    }
}
