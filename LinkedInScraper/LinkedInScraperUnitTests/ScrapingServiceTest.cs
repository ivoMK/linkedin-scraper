using LinkedInScraper.Interfaces;
using LinkedInScraper.Models;
using LinkedInScraper.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkedInScraperUnitTests
{
    [TestClass]
    public class ScrapingServiceTest
    {
        private Mock<ISeleniumScrapingService> _seleniumScrapingServiceMock;
        private ScraperConfig _scraperConfig;
        private IScrapingService _scrapingService;

        private List<ProfileInfo> FakeProfiles { get; } 
            = new List<ProfileInfo> { new ProfileInfo { Name = "Profile1", Id = Guid.NewGuid() }, new ProfileInfo { Name = "Profile2", Id = Guid.NewGuid() } };

        public ScrapingServiceTest()
        {
            _seleniumScrapingServiceMock = new Mock<ISeleniumScrapingService>();
            _scraperConfig = new ScraperConfig();

            _scrapingService = new ScrapingService(_seleniumScrapingServiceMock.Object, _scraperConfig);
        }

        [TestMethod]
        public void GetProfileShouldGetProfilesFromLinkedin()
        {
            SetupSeleniumScrapingServiceGetProfiles();

            var result = _scrapingService.GetProfiles();

            Assert.IsTrue(result.IsSucessful);
            Assert.AreEqual(2, result.ProfileInfos.Count);
            Assert.IsNull(result.ErrorMessage);
            VerifySeleniumScrapingServiceGetProfilesWasCalledOnce();
        }

        [TestMethod]
        public void GetProfileShouldReturnCachedResultIfProfilesAreAlreadyLoaded()
        {
            SetupSeleniumScrapingServiceGetProfiles();

            var result = _scrapingService.GetProfiles();
            result = _scrapingService.GetProfiles();

            Assert.IsTrue(result.IsSucessful);
            Assert.AreEqual(2, result.ProfileInfos.Count);
            Assert.IsNull(result.ErrorMessage);
            VerifySeleniumScrapingServiceGetProfilesWasCalledOnce();
        }

        [TestMethod]
        public void ViewProfilesShouldNotAllowViewProfilesInLinkedinWhenMoreViewsThanAllowedInConfig()
        {
            SetupSeleniumScrapingServiceViewProfiles();

            var result = _scrapingService.ViewProfiles(FakeProfiles.Select(x => x.Id).ToList());

            Assert.IsFalse(result.isSuccess);
            Assert.AreEqual("No more views allowed", result.errorMessage);
            VerifySeleniumScrapingServiceViewProfilesWasNotCalled();
        }

        [TestMethod]
        public void ViewProfilesShouldViewProfilesInLinkedin()
        {
            _scraperConfig = new ScraperConfig { NumberOfViews = 50 };
            _scrapingService = new ScrapingService(_seleniumScrapingServiceMock.Object, _scraperConfig);
            SetupSeleniumScrapingServiceViewProfiles();
            SetupSeleniumScrapingServiceGetProfiles();
            _scrapingService.GetProfiles();

            var result = _scrapingService.ViewProfiles(FakeProfiles.Select(x=>x.Id).ToList());

            Assert.IsTrue(result.isSuccess);
            Assert.IsNull(result.errorMessage);
            VerifySeleniumScrapingServiceViewProfilesWasCalledOnce();
        }

        private void SetupSeleniumScrapingServiceGetProfiles()
        {
            _seleniumScrapingServiceMock.Setup(x => x.GetProfiles()).Returns((true, FakeProfiles, null));
        }

        private void VerifySeleniumScrapingServiceGetProfilesWasCalledOnce()
        {
            _seleniumScrapingServiceMock.Verify(x => x.GetProfiles(), Times.Once);
        }

        private void SetupSeleniumScrapingServiceViewProfiles()
        {
            _seleniumScrapingServiceMock.Setup(x => x.ViewProfiles(FakeProfiles)).Returns((true, FakeProfiles, null));
        }

        private void VerifySeleniumScrapingServiceViewProfilesWasCalledOnce()
        {
            _seleniumScrapingServiceMock.Verify(x => x.ViewProfiles(FakeProfiles), Times.Once);
        }

        private void VerifySeleniumScrapingServiceViewProfilesWasNotCalled()
        {
            _seleniumScrapingServiceMock.Verify(x => x.ViewProfiles(FakeProfiles), Times.Never);
        }
    }
}
