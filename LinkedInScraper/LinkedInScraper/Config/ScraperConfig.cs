using LinkedInScraper.Enums;
using System.Collections.Generic;

namespace LinkedInScraper.Models
{
    public class ScraperConfig
    {
        public string LinkedInEmail { get; set; }
        public string LinkedInPassword { get; set; }
        public int NumberOfMessages { get; set; }
        public int NumberOfViews { get; set; }
        public bool Headless { get; set; }
        public string ScrapingUrl { get; set; }
        public Dictionary<LinkedInScrapingElements, ScrapingElementDefinition> ScrapingElementsDefinition { get; set; }
    }

    public class ScrapingElementDefinition
    {
        public LinkedInScrapingElements ElementName { get; set; }
        public SelectorType SelectorType { get; set; }
        public string SelectorValue { get; set; }
    }
}
