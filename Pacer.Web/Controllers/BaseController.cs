using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Pacer.Web.Controllers
{
    public enum AlertType { success, danger, warning, info }

    // Implements General functionality which is then accessible to any 
    // Controller inheriting from BaseController
    public class BaseController : Controller
    {
        protected readonly ILogger _logger;
        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
        // set alert message
        public void Alert(string message, AlertType type = AlertType.info)
        {
            TempData["Alert.Message"] = message;
            TempData["Alert.Type"] = type.ToString();
        }

    }

}