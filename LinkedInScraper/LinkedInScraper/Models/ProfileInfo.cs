using System;

namespace LinkedInScraper.Models
{
    public class ProfileInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Occupation { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }

    }
}
