using System.Collections.Generic;

namespace LinkedInScraper.Models
{
    public class GetProfilesResponse
    {
        public bool IsSucessful { get; set; }
        public string ErrorMessage { get; set; }
        public List<ProfileInfo> ProfileInfos { get; set; }
    }
}
