using LinkedInScraper.Models;
using System;
using System.Collections.Generic;

namespace LinkedInScraper.Interfaces
{
    public interface IScrapingService
    {
        GetProfilesResponse GetProfiles();
        (bool isSuccess, string errorMessage) ViewProfiles(List<Guid> profileIds);
        (bool isSuccess, string errorMessage) SendMessageToProfiles(List<Guid> profileIds, string message);
    }
}
