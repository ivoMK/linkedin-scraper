using LinkedInScraper.Interfaces;
using LinkedInScraper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LinkedInScraper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IScrapingService _scrapingService;
        private readonly ScraperConfig _scraperSettings;

        public HomeController(ILogger<HomeController> logger,
            IScrapingService scrapingService,
            ScraperConfig scraperSettings)
        { 
            _scraperSettings = scraperSettings;        
            _logger = logger;
            _scrapingService = scrapingService;
        }

        public IActionResult Index()
        {           
            return View("Index",_scraperSettings.LinkedInEmail);
        }

        public IActionResult GetProfiles()
        {
            var profiles = _scrapingService.GetProfiles();            

            return PartialView("_Profiles", profiles);
        }

        [HttpPost]
        public string SendMessage(SendMessageRequest request)
        {
            var (isSuccess, errorMessage) = _scrapingService.SendMessageToProfiles(request.ProfileIds,request.Message);

            return JsonConvert.SerializeObject(new BaseResponse { IsSuccess = isSuccess, ErrorMessage = errorMessage });
        }

        [HttpPost]
        public string ViewProfile(List<Guid> userprofileIds)
        {
            var (isSuccess, errorMessage) = _scrapingService.ViewProfiles(userprofileIds);

            return JsonConvert.SerializeObject( new BaseResponse { IsSuccess = isSuccess, ErrorMessage = errorMessage });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
