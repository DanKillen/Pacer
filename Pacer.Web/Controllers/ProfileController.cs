using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pacer.Data.Services;
using Pacer.Data.Entities;
using System.Security.Claims;

namespace Pacer.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IRunningProfileService _runningProfileService;
        private readonly IUserService _userService;

        public ProfileController(IRunningProfileService runningProfileService, IUserService userService)
        {
            _userService = userService;
            _runningProfileService = runningProfileService;
        }

        [HttpGet]
        public IActionResult CreateProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Handle the case where the NameIdentifier claim is missing
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
        public IActionResult CreateProfile(DateTime dateOfBirth, string gender, int weeklyMileage, int fiveKTimeMinutes, int fiveKTimeSeconds)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var fiveKTime = TimeSpan.FromMinutes(fiveKTimeMinutes) + TimeSpan.FromSeconds(fiveKTimeSeconds);

            var profile = _runningProfileService.CreateProfile(userId, dateOfBirth, gender, weeklyMileage, fiveKTime);

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
        public IActionResult ViewProfile(int userId)
        {
            var profile = _runningProfileService.GetProfileByUserId(userId);
            if (profile == null)
            {
                return NotFound();
            }
            var model = new RunningProfileViewModel
            {
                UserId = profile.UserId,
                UserName = _userService.GetUser(profile.UserId).Name,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                WeeklyMileage = profile.WeeklyMileage,
                FiveKTimeMinutes = profile.FiveKTime.Minutes,
                FiveKTimeSeconds = profile.FiveKTime.Seconds,
                // Add the rest of your properties here...
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = _runningProfileService.GetProfileByUserId(userId);

            if (profile == null)
            {
                return NotFound();
            }

            var model = new RunningProfileViewModel
            {
                UserId = profile.UserId,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                WeeklyMileage = profile.WeeklyMileage,
                FiveKTimeMinutes = profile.FiveKTime.Minutes,
                FiveKTimeSeconds = profile.FiveKTime.Seconds,
                // Add the rest of your properties here...
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditProfile(RunningProfileViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != model.UserId)
            {
                return Unauthorized();
            }

            var fiveKTime = TimeSpan.FromMinutes(model.FiveKTimeMinutes) + TimeSpan.FromSeconds(model.FiveKTimeSeconds);
            var updatedProfile = _runningProfileService.UpdateProfile(model.UserId, model.DateOfBirth, model.Gender, model.WeeklyMileage, model.FiveKTime);

            if (updatedProfile == null)
            {
                Alert("Error updating profile", AlertType.danger);
                return View(model);
            }
            else
            {
                Alert("Profile updated successfully", AlertType.success);
                return RedirectToAction("ViewProfile", new { userId = updatedProfile.UserId });
            }
        }


        
    }
}
