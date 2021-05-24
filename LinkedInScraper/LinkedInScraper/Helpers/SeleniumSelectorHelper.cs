using LinkedInScraper.Enums;
using LinkedInScraper.Models;
using OpenQA.Selenium;
using System;

namespace LinkedInScraper.Helpers
{
    public static class SeleniumSelectorHelper
    {
        public static By GetSelector(this ScrapingElementDefinition scrapingElementDefinition)
        {
            switch (scrapingElementDefinition.SelectorType)
            {
                case SelectorType.Id:
                    return By.Id(scrapingElementDefinition.SelectorValue);
                case SelectorType.CssSelector:
                    return By.CssSelector(scrapingElementDefinition.SelectorValue);
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(scrapingElementDefinition.SelectorType)} {scrapingElementDefinition.SelectorType} not supported!");
            }
        }
    }
}
