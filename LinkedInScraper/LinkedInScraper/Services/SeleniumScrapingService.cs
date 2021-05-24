using LinkedInScraper.Enums;
using LinkedInScraper.Helpers;
using LinkedInScraper.Interfaces;
using LinkedInScraper.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkedInScraper.Services
{
    public class SeleniumScrapingService : ISeleniumScrapingService
    {
        private readonly ScraperConfig _scraperConfig;
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private const int SecondsToWait = 5;
        private const string HrefAttribute = "href";

        public SeleniumScrapingService(ScraperConfig scraperSettings)
        {
            _scraperConfig = scraperSettings;
            ChromeOptions options = new ChromeOptions();
            if (_scraperConfig.Headless)
            {
                options.AddArguments("headless");
            }

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(SecondsToWait));
        }

        public (bool isSuccess, List<ProfileInfo> profiles, string errorMessage) GetProfiles()
        {
            try
            {
                var loginInfo = Login();
                if (!loginInfo.isSuccess) return new(false, null, loginInfo.errorMessage);

                var contactsSelector = _scraperConfig.ScrapingElementsDefinition[LinkedInScrapingElements.ContactsList].GetSelector();
                _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(contactsSelector));
                var contacts = _driver.FindElements(contactsSelector);

                var profileInfos = new List<ProfileInfo>();
                foreach (var contact in contacts)
                {
                    var link = contact.FindElement(_scraperConfig.ScrapingElementsDefinition[LinkedInScrapingElements.ContactLink].GetSelector());
                    var name = contact.FindElement(_scraperConfig.ScrapingElementsDefinition[LinkedInScrapingElements.ContactName].GetSelector()).Text;
                    var url = link.GetAttribute(HrefAttribute);
                    var occupation = contact.FindElement(_scraperConfig.ScrapingElementsDefinition[LinkedInScrapingElements.ContactOccupation].GetSelector()).Text;
                    var image = contact.FindElement(_scraperConfig.ScrapingElementsDefinition[LinkedInScrapingElements.ContactImage].GetSelector());

                    profileInfos.Add(new ProfileInfo
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Occupation = occupation,
                        Url = url,
                        ImageUrl = image.GetAttribute("src")
                    });
                }

                return new(true, profileInfos, null);
            }
            catch (Exception ex)
            {
                return new(false, null, ex.Message);
            }
        }

        public (bool isSuccess, List<ProfileInfo> visitedProfiles, string errorMessage) ViewProfiles(List<ProfileInfo> profiles)
        {
            StringBuilder errorStringBuilder = new();
            var visitedProfiles = new List<ProfileInfo>();
            foreach (var profile in profiles)
            {
                try
                {
                    ViewProfile(profile);
                    visitedProfiles.Add(profile);
                }
                catch (Exception ex)
                {
                    errorStringBuilder.Append($"Error occured for profileId {profile.Id}: {ex.Message}");
                }
            }

            return errorStringBuilder.Length == 0 ? (true, visitedProfiles, null) : (false, visitedProfiles, errorStringBuilder.ToString());
        }

        public (bool isSuccess, List<ProfileInfo> messageSentProfiles, string errorMessage) SendMessageToProfiles(List<ProfileInfo> profiles, string message)
        {
            StringBuilder errorStringBuilder = new();
            var messageSentProfiles = new List<ProfileInfo>();

            //var loginInfo = Login();
            //if (!loginInfo.isSuccess) return new(false, null, loginInfo.errorMessage);

            foreach (var profile in profiles)
            {
                try
                {
                    SendMessage(profile, message);
                    messageSentProfiles.Add(profile);
                }
                catch (Exception ex)
                {
                    errorStringBuilder.Append($"Error occured for profileId {profile.Id}: {ex.Message}");
                }
            }          

            return errorStringBuilder.Length == 0 ? (true, messageSentProfiles, null) : (false, messageSentProfiles, errorStringBuilder.ToString());
        }

        private (bool isSuccess, string errorMessage) Login()
        {
            try
            {
                var scrapingElements = _scraperConfig.ScrapingElementsDefinition;
                _driver.Navigate().GoToUrl(_scraperConfig.ScrapingUrl);
                var username = _driver.FindElement(scrapingElements[LinkedInScrapingElements.Username].GetSelector());
                username.SendKeys(_scraperConfig.LinkedInEmail);

                var password = _driver.FindElement(scrapingElements[LinkedInScrapingElements.Password].GetSelector());
                password.SendKeys(_scraperConfig.LinkedInPassword);

                var loginbutton = _driver.FindElement(scrapingElements[LinkedInScrapingElements.LoginButton].GetSelector());
                if (loginbutton == null)
                {
                    return (false, "LoginButton not found");
                }
                loginbutton.Click();

                return (true, null);
            }
            catch (Exception ex)
            {
                return new(false, $"An error occured on login: {ex.Message}");
            }
        }

        private void ViewProfile(ProfileInfo profileInfo)
        {
            _driver.Navigate().GoToUrl(profileInfo.Url);
        }

        private (bool isSuccess, string errorMessage) SendMessage(ProfileInfo profileInfo, string message)
        {
            ViewProfile(profileInfo);

            var scrapingElements = _scraperConfig.ScrapingElementsDefinition;
            _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(scrapingElements[LinkedInScrapingElements.OpenMessageDialogButton].GetSelector()));
            var openMessageDialogButton = _driver.FindElement(scrapingElements[LinkedInScrapingElements.OpenMessageDialogButton].GetSelector());
            if (openMessageDialogButton == null)
            {
                return new(false, $"{nameof(openMessageDialogButton)} not found!");
            }
            openMessageDialogButton.Click();

            var messageContentSelector = scrapingElements[LinkedInScrapingElements.MessageContent].GetSelector();
            _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(messageContentSelector));
            var messageContent = _driver.FindElement(messageContentSelector);
            messageContent.SendKeys(message);

            var sendMessageButtonSelector = scrapingElements[LinkedInScrapingElements.SendMessageButton].GetSelector();
            _wait.Until(ExpectedConditions.ElementToBeClickable(sendMessageButtonSelector));
            var sendMessageButton = _driver.FindElement(sendMessageButtonSelector);
            if (sendMessageButton == null)
            {
                return new(false, $"{nameof(openMessageDialogButton)} not found!");
            }
            sendMessageButton.Click();

            return (true, null);
        }
    }
}
