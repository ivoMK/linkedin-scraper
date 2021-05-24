using LinkedInScraper.Interfaces;
using LinkedInScraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkedInScraper.Services
{
    public class ScrapingService : IScrapingService
    {
        private readonly ISeleniumScrapingService _seleniumScrapingService;
        private readonly ScraperConfig _scraperConfig;

        private List<ProfileInfo> _profileInfosAll;
        private List<ProfileInfo> _proileInfosViewed;
        private List<ProfileInfo> _proileInfosMessageSent;

        public ScrapingService(ISeleniumScrapingService seleniumScrapingService, ScraperConfig scraperConfig)
        {
            _seleniumScrapingService = seleniumScrapingService;
            _scraperConfig = scraperConfig;
            _profileInfosAll = new List<ProfileInfo>();
            _proileInfosViewed = new List<ProfileInfo>();
            _proileInfosMessageSent = new List<ProfileInfo>();
        }

        public GetProfilesResponse GetProfiles()
        {
            if (_profileInfosAll != null && _profileInfosAll.Any())
            {
                return new GetProfilesResponse { IsSucessful = true, ProfileInfos = _profileInfosAll, ErrorMessage = null };
            }

            var profilesResponse = _seleniumScrapingService.GetProfiles();
            _profileInfosAll = profilesResponse.profiles;
            return new GetProfilesResponse { IsSucessful = profilesResponse.isSuccess, ProfileInfos = profilesResponse.profiles, ErrorMessage = profilesResponse.errorMessage };
        }

        public (bool isSuccess, string errorMessage) ViewProfiles(List<Guid> profileIds)
        {
            if (_proileInfosViewed.Count + profileIds.Count >= _scraperConfig.NumberOfViews)
            {
                return new(false, "No more views allowed");
            }

            var profilesWithMessageSent = _proileInfosMessageSent.Where(x => profileIds.Contains(x.Id));
            var profilesAlreadyViewed = _proileInfosMessageSent.Where(x => profileIds.Contains(x.Id));
            var profilesNotAllowedForView = profilesWithMessageSent.Concat(profilesAlreadyViewed);

            if (profilesNotAllowedForView.Any())
            {
                return new(false, $"The list contains profiles not allowed for view {string.Join(";", profilesNotAllowedForView.Select(x => $"{x.Id}, {x.Name}"))}");
            }

            var viewProfilesResponse = _seleniumScrapingService.ViewProfiles(_profileInfosAll.Where(x => profileIds.Contains(x.Id)).ToList());

            _proileInfosViewed.AddRange(viewProfilesResponse.visitedProfiles);
            return (viewProfilesResponse.isSuccess, viewProfilesResponse.errorMessage);
        }

        public (bool isSuccess, string errorMessage) SendMessageToProfiles(List<Guid> profileIds, string message)
        {
            if (_proileInfosMessageSent.Count + profileIds.Count >= _scraperConfig.NumberOfMessages)
            {
                return new(false, "No more messages allowed");
            }

            var profilesWithMessageSent = _proileInfosMessageSent.Where(x => profileIds.Contains(x.Id));
            var profilesAlreadyViewed = _proileInfosMessageSent.Where(x => profileIds.Contains(x.Id));
            var profilesNotAllowedForView = profilesWithMessageSent.Concat(profilesAlreadyViewed);

            if (profilesNotAllowedForView.Any())
            {
                return new(false, $"The list contains profiles not allowed to receive message {string.Join(";", profilesNotAllowedForView.Select(x => $"{x.Id}, {x.Name}"))}");
            }

            var sentMessagesResponse = _seleniumScrapingService.SendMessageToProfiles(_profileInfosAll.Where(x => profileIds.Contains(x.Id)).ToList(), message);

            _proileInfosMessageSent.AddRange(sentMessagesResponse.messageSentProfiles);
            return (sentMessagesResponse.isSuccess, sentMessagesResponse.errorMessage);
        }
    }
}
