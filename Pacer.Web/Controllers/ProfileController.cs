using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pacer.Data.Services;
using Pacer.Data.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Pacer.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IRunningProfileService _runningProfileService;


        public ProfileController(IRunningProfileService runningProfileService, ILogger<ProfileController> logger) : base(logger)
        {
            _runningProfileService = runningProfileService;
        }

        [HttpGet]
        public IActionResult CreateProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.Sid);
            if (userIdClaim == null)
            {
                // Handle the case where the ID claim is missing
                return RedirectToAction("Login", "User");
            }

            var userId = int.Parse(userIdClaim);

            var existingProfile = _runningProfileService.GetProfileByUserId(userId);
            if (existingProfile != null)
            {
                // If the user already has a profile, redirect them to that profile
                return RedirectToAction("ViewProfile", new { userId = existingProfile.UserId });
            }

            // Otherwise, proceed with profile creation
            return View();
        }

        [HttpPost]
        public IActionResult CreateProfile(RunningProfileViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.Sid));
            var fiveKTime = TimeSpan.FromMinutes(model.FiveKTimeMinutes) + TimeSpan.FromSeconds(model.FiveKTimeSeconds);

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    if (ModelState[key].Errors.Any())
                    {
                        var errorMessages = ModelState[key].Errors.Select(error => error.ErrorMessage);
                        Console.WriteLine($"Key: {key}, Errors: {string.Join(",", errorMessages)}");
                    }
                }
                Alert("Error creating profile", AlertType.danger);
                return View(model);
            }

            var profile = _runningProfileService.CreateProfile(userId, model.DateOfBirth, model.Gender, model.WeeklyMileage, fiveKTime);

            if (profile == null)
            {
                Alert("Error creating profile", AlertType.danger);
                return View();
            }
            else
            {
                Alert("Profile created successfully", AlertType.success);
                return RedirectToAction("ViewProfile", new { userId = profile.UserId });
            }
        }

        [HttpGet]
        public IActionResult ViewProfile()
        {

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out var userId))
            {
                // Log this occurrence or handle it in a way that's appropriate for your application.
                Alert("Issue with User ID. Please log out then try again", AlertType.danger);
            }

            var profile = _runningProfileService.GetProfileByUserId(userId);
            if (profile == null)
            {
                Alert("Please create a running profile.", AlertType.info);
                return RedirectToAction("CreateProfile", "Profile");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.Sid);

            if (currentUserId != userId.ToString())
            {
                // Check if the current user is an admin
                if (!User.IsInRole("Admin"))
                {
                    Alert("You do not have permission to view this profile", AlertType.danger);
                    return RedirectToAction("Index", "Home");
                }
            }

            var model = new RunningProfileViewModel
            {
                UserId = profile.UserId,
                UserName = profile.User.Name,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                WeeklyMileage = profile.WeeklyMileage,
                FiveKTimeMinutes = profile.FiveKTime.Minutes,
                FiveKTimeSeconds = profile.FiveKTime.Seconds,
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.Sid));
            var profile = _runningProfileService.GetProfileByUserId(userId);

            if (profile == null)
            {
                return NotFound();
            }
            if (userId != profile.UserId)
            {
                Alert("You do not have permission to edit this profile", AlertType.danger);
                return RedirectToAction("Index", "Home");
            }

            var model = new RunningProfileViewModel
            {
                UserId = profile.UserId,
                UserName = profile.User.Name,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                WeeklyMileage = profile.WeeklyMileage,
                FiveKTimeMinutes = profile.FiveKTime.Minutes,
                FiveKTimeSeconds = profile.FiveKTime.Seconds,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditProfile(int userId, RunningProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var currentUserId = User.FindFirstValue(ClaimTypes.Sid);

            if (currentUserId != userId.ToString())
            {
                // Check if the current user is an admin
                if (!User.IsInRole("Admin"))
                {
                    Alert("You do not have permission to edit this profile", AlertType.danger);
                    return RedirectToAction("Index", "Home");
                }
            }

            var fiveKTime = TimeSpan.FromMinutes(model.FiveKTimeMinutes) + TimeSpan.FromSeconds(model.FiveKTimeSeconds);
            var updatedProfile = _runningProfileService.UpdateProfile(userId, model.DateOfBirth, model.Gender, model.WeeklyMileage, fiveKTime);

            if (updatedProfile == null)
            {
                Alert("Error updating profile", AlertType.danger);
                return View();
            }
            else
            {
                Alert("Profile updated successfully", AlertType.success);
                return RedirectToAction("ViewProfile", new { userId = updatedProfile.UserId });
            }
        }



    }
}
