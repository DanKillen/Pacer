using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pacer.Web.Models;
using Pacer.Data.Services;

namespace Pacer.Web.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(ILogger<HomeController> logger) : base(logger)
        {

        }
        // This is the default route for the application.
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string name = User.Identity.Name;
                return View("Index", name);
            }
            else
            {
                return View();
            }

        }
        // This is the route for the User Guide page.
        public IActionResult UserGuide()
        {
            return View();
        }
        // This will move the user to the register page if they are not authenticated.
        public IActionResult GetStarted()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "TrainingPlan");
            }
            else
            {
                return RedirectToAction("Register", "User");
            }
        }
        // This is the route for the Privacy page.
        public IActionResult Privacy()
        {
            return View();
        }
        // This is the route for the Error page.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var RequestedId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError($"An error has occurred for request id: {RequestedId}");
            return View(new ErrorViewModel { RequestId = RequestedId });
        }
    }
}
