using LinkedInScraper.Models;
using System.Collections.Generic;

namespace LinkedInScraper.Interfaces
{
    public interface ISeleniumScrapingService
    {
        (bool isSuccess, List<ProfileInfo> profiles, string errorMessage) GetProfiles();
        (bool isSuccess, List<ProfileInfo> visitedProfiles, string errorMessage) ViewProfiles(List<ProfileInfo> profiles);
        (bool isSuccess, List<ProfileInfo> messageSentProfiles, string errorMessage) SendMessageToProfiles(List<ProfileInfo> profiles, string message);

    }
}
