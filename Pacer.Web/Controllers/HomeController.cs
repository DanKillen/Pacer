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
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUserService userService, ILogger<HomeController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

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

        public IActionResult About()
        {
            return View();
        }

        public IActionResult GetStarted()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "TrainingPlan");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [Authorize]
        public IActionResult Secure()
        {
            return View();
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
