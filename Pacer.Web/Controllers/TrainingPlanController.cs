using Microsoft.AspNetCore.Mvc;
using Pacer.Data.Entities;
using Pacer.Data.Services;
using Pacer.Web.Models.TrainingPlan;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace Pacer.Web.Controllers;

public class TrainingPlanController : BaseController
{
    private readonly ITrainingPlanService _trainingPlanService;
    private readonly IRunningProfileService _runningProfileService;

    public TrainingPlanController(ITrainingPlanService trainingPlanService, IRunningProfileService runningProfileService, ILogger<TrainingPlanController> logger) : base(logger)
    {
        _trainingPlanService = trainingPlanService;
        _runningProfileService = runningProfileService;
    }
    // Private Training Plan Methods
    // This method is used to get the user ID from the claims.
    private int? GetUserId()
    {
        if (!User.Identity.IsAuthenticated)
        {
            _logger.LogWarning("User is not authenticated.");
            Alert("You need to be logged in to access this page.", AlertType.danger);
            return null;
        }
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out var userId))
        {
            _logger.LogError($"Unable to parse user id from claim. Claim: {JsonSerializer.Serialize(User.Claims)}");
            Alert("Issue with User ID. Please log out then try again", AlertType.danger);
            return null;
        }

        return userId;
    }
    // This method is used to get the Training Plan View Model for the Week List.
    private TrainingPlanViewModel ConstructTrainingPlanViewModel(TrainingPlan trainingPlan, string formattedTargetTime)
    {
        int currentWeek = 1;
        var viewModel = new TrainingPlanViewModel
        {
            Id = trainingPlan.Id,
            TargetRace = trainingPlan.TargetRace,
            TargetTime = formattedTargetTime,
            TargetPace = trainingPlan.TargetPace,
            RaceDate = trainingPlan.RaceDate,
            Weeks = trainingPlan.Workouts.GroupBy(w => w.Date.GetWeekStartingMonday())
                             .Select(g => new WeekViewModel
                             {
                                 WeekNumber = currentWeek++,
                                 Workouts = g.Select(w => new WorkoutViewModel
                                 {
                                     Id = w.Id,
                                     Type = w.Type,
                                     Date = w.Date,
                                     TargetDistance = w.TargetDistance,
                                     TargetPace = LookupTargetPaceForWorkout(w.Type, trainingPlan.Paces),
                                     ActualDistance = w.ActualDistance,
                                     ActualTime = w.ActualTime,
                                     ActualPace = w.ActualDistance > 0 ? TimeSpan.FromMinutes(w.ActualTime.TotalMinutes / w.ActualDistance).ToString(@"mm\:ss") : null,
                                     WorkoutDescription = w.WorkoutDescription
                                 }).ToList()
                             }).ToList()
        };
        return viewModel;
    }
    // This method is used to get the Training Plan Calendar View Model.
    private TrainingPlanCalendarViewModel GetTrainingPlanCalendarViewModel()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID (Sid claim) is not set.");
            return null;
        }

        if (!int.TryParse(userId, out int parsedUserId))
        {
            _logger.LogError($"Failed to parse User ID: {userId}");
            return null;
        }

        var trainingPlan = _trainingPlanService.GetPlanByUserId(parsedUserId);
        if (!ValidateTrainingPlan(trainingPlan))
        {
            return null;
        }

        var firstWorkoutDate = GetFirstWorkoutDate(trainingPlan);
        var formattedTargetTime = FormatTargetTime(trainingPlan.TargetTime);

        var workouts = CreateWorkoutViewModels(trainingPlan);

        return new TrainingPlanCalendarViewModel
        {
            Id = trainingPlan.Id,
            TargetTime = formattedTargetTime,
            Month = firstWorkoutDate.Month,
            Year = firstWorkoutDate.Year,
            RaceDate = trainingPlan.RaceDate,
            TargetRace = trainingPlan.TargetRace,
            TargetPace = trainingPlan.TargetPace,
            Workouts = workouts
        };
    }
    // This method is used to get the first workout date.
    private DateTime GetFirstWorkoutDate(TrainingPlan trainingPlan)
    {
        return trainingPlan.Workouts.OrderBy(w => w.Date).First().Date;
    }
    // This method is used to format the target time.
    private string FormatTargetTime(TimeSpan targetTime)
    {
        return targetTime.ToString(@"h\:mm");
    }
    // This method is used to create the workout view models.
    private List<WorkoutViewModel> CreateWorkoutViewModels(TrainingPlan trainingPlan)
    {
        return trainingPlan.Workouts.Select(w => new WorkoutViewModel
        {
            Id = w.Id,
            Type = w.Type,
            Date = w.Date,
            TargetDistance = w.TargetDistance,
            ActualDistance = w.ActualDistance,
            TargetPace = LookupTargetPaceForWorkout(w.Type, trainingPlan.Paces),
            ActualTime = w.ActualTime,
            ActualPace = w.ActualDistance > 0 ? TimeSpan.FromMinutes(w.ActualTime.TotalMinutes / w.ActualDistance).ToString(@"mm\:ss") : null
        }).ToList();
    }
    // This method is used to lookup the target pace for a workout.
    private string LookupTargetPaceForWorkout(WorkoutType type, ICollection<TrainingPlanPace> paces)
    {
        var minPace = paces.FirstOrDefault(p => p.WorkoutType == type && p.PaceType == PaceType.Min)?.PaceString;
        var maxPace = paces.FirstOrDefault(p => p.WorkoutType == type && p.PaceType == PaceType.Max)?.PaceString;

        // If both paces are available, return a range.
        if (minPace != null && maxPace != null)
        {
            return $"{minPace} - {maxPace}";
        }
        // Otherwise, return whichever is available, or null if neither are.
        return minPace ?? maxPace;
    }
    // This method is used to validate the training plan.
    private bool ValidateTrainingPlan(TrainingPlan trainingPlan)
    {
        if (trainingPlan == null)
        {
            _logger.LogError("Training plan not found.");
            Alert("Error: Training plan not found", AlertType.danger);
            return false;
        }
        if (trainingPlan.Workouts == null)
        {
            _logger.LogError("Training plan has null workouts.");
            Alert("Error: Training plan has null workouts", AlertType.danger);
            return false;
        }
        if (!trainingPlan.Workouts.Any())
        {
            _logger.LogError("Training plan has no workouts.");
            Alert("Error: Training plan has no workouts", AlertType.danger);
            return false;
        }
        return true;
    }

    // Controller Endpoints
    // This is the GET route for the Training Plan Index page.
    [HttpGet]
    public IActionResult Index()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "User");
        }
        var runningProfile = _runningProfileService.GetProfileByUserId(userId.Value);
        if (runningProfile == null)
        {
            TempData["Alert.Message"] = "Please Create a running profile";
            TempData["Alert.Type"] = "info";
            return RedirectToAction("CreateProfile", "Profile");
        }
        var trainingPlan = _trainingPlanService.GetPlanByUserId(userId.Value);

        if (trainingPlan != null)
        {
            return RedirectToAction("ViewTrainingPlan");
        }

        return RedirectToAction("Disclaimer");
    }
    // This is the GET route for the Disclaimer page.
    [HttpGet]
    public IActionResult Disclaimer()
    {
        return View();
    }
    // This is the GET for the Create Training Plan page.
    [HttpGet]
    public IActionResult CreateTrainingPlan()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "User");
        }
        var runningProfile = _runningProfileService.GetProfileByUserId(userId.Value);
        if (runningProfile == null)
        {
            TempData["Alert.Message"] = "Error: Running Profile not found";
            TempData["Alert.Type"] = "danger";
            return RedirectToAction("Error", "Home");
        }
        var trainingPlan = _trainingPlanService.GetPlanByUserId(userId.Value);
        if (trainingPlan != null)
        {
            TempData["Alert.Message"] = "Error: Training Plan already exists";
            TempData["Alert.Type"] = "danger";
            return RedirectToAction("Index", "TrainingPlan");
        }
        int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)DateTime.Now.DayOfWeek + 7) % 7;
        DateTime suggestedDate = DateTime.Now.Date.AddDays(daysUntilSunday).AddDays(12 * 7); // 12 weeks later

        var model = new TrainingPlanCreateModel
        {
            RunningProfileId = runningProfile.Id,
            Recommendation = _trainingPlanService.GetRecommendation(runningProfile.EstimatedMarathonTime, runningProfile.EstimatedHalfMarathonTime, runningProfile.WeeklyMileage, runningProfile.DateOfBirth, runningProfile.FiveKTime),
            RaceDate = suggestedDate,
            EstimatedMarathonTime = runningProfile.EstimatedMarathonTime,
            EstimatedHalfMarathonTime = runningProfile.EstimatedHalfMarathonTime
        };

        return View(model);
    }
    // This is the POST route for the Create Training Plan page.
    [HttpPost]
    public IActionResult CreateTrainingPlan(TrainingPlanCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = GetUserId();
            var planCheck = _trainingPlanService.GetPlanByUserId(userId.Value);
            if (planCheck != null)
            {
                Alert("Error: Training Plan already exists", AlertType.danger);
                return RedirectToAction("Index", "TrainingPlan");
            }
            try
            {
                var plan = _trainingPlanService.CreatePlan(model.RunningProfileId, model.TargetRace, model.RaceDate, model.TargetTime);
                Alert("Training Plan created successfully", AlertType.success);
                return RedirectToAction("ViewTrainingPlan", new { id = plan.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating Training Plan: {ex}");
                Alert("Error creating Training Plan: " + ex.Message, AlertType.danger);
            }
        }
        else
        {
            _logger.LogWarning("Model state is invalid.");
            Alert("Error creating Training Plan", AlertType.danger);
        }

        return RedirectToAction("ViewTrainingPlan");

    }
    // This is the GET route for the View Training Plan page.
    [HttpGet]
    public IActionResult ViewTrainingPlan()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "User");
        }
        var trainingPlan = _trainingPlanService.GetPlanByUserId(userId.Value);

        if (!ValidateTrainingPlan(trainingPlan))
        {
            return RedirectToAction("Error", "Home");
        }
        string formattedTargetTime = FormatTargetTime(trainingPlan.TargetTime);
        var viewModel = ConstructTrainingPlanViewModel(trainingPlan, formattedTargetTime);
        return View(viewModel);
    }
    // This is the GET route for the Training Plan Calendar page.
    [HttpGet]
    public IActionResult Calendar()
    {
        var viewModel = GetTrainingPlanCalendarViewModel();

        if (viewModel == null)
        {
            // Redirecting to TrainingPlanIndex. This action will further check if the user has a training plan, is logged in, etc.
            return RedirectToAction("Index", "TrainingPlan");
        }

        var grouped = viewModel.Workouts
            .GroupBy(x => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(x.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
            .Select(g => new WeekDistance
            {
                Week = 0, // placeholder value
                TargetDistance = g.Sum(x => x.TargetDistance),
                ActualDistance = g.Sum(x => x.ActualDistance)
            })
            .OrderBy(x => x.Week)
            .ToList();

        for (int i = 0; i < grouped.Count; i++)
        {
            grouped[i].Week = i + 1; // assign week numbers starting from 1
        }

        viewModel.WeekDistances = grouped;

        return View("Calendar", viewModel);
    }
    // This is the GET route for the Training Plan Tutorial page.
    [HttpGet]
    public IActionResult Tutorial(int id)
    {
        var trainingPlan = _trainingPlanService.GetPlanById(id);
        if (trainingPlan == null)
        {
            Alert("Error: Training plan not found", AlertType.danger);
            return RedirectToAction("Index", "TrainingPlan");
        }
        var tutorial = new TutorialViewModel
        {
            Id = trainingPlan.Id,
            TargetRace = trainingPlan.TargetRace,
        };
        return View(tutorial);
    }
    // This is the POST route to save completed workout data.
    [HttpPost("SaveWorkoutActuals")]
    public IActionResult SaveWorkoutActuals(int WorkoutId, double ActualDistance, int ActualHours, int ActualMinutes, int ActualSeconds, string returnUrl)
    {
        TimeSpan actualTime;
        try
        {
            actualTime = new TimeSpan(ActualHours, ActualMinutes, ActualSeconds);
        }
        catch (FormatException ex)
        {
            _logger.LogError($"Error parsing time: {ex.Message}");
            return BadRequest(new { message = "Error: Invalid time format" });
        }

        try
        {
            var userId = GetUserId();
            _trainingPlanService.SaveWorkoutActuals(WorkoutId, userId.Value, ActualDistance, actualTime);
            _logger.LogInformation("Workout actuals saved successfully.");
            Alert("Workout added successfully. Great job!", AlertType.success);
            return RedirectToAction(returnUrl);

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving workout actuals: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error saving data: " + ex.Message });
        }
    }
    // This is the POST route to clear completed workout data.
    [HttpPost("ClearWorkoutActuals")]
    public IActionResult ClearWorkoutActuals(int WorkoutId, string returnUrl)
    {

        var userId = GetUserId();
        var result = _trainingPlanService.ClearWorkoutActuals(WorkoutId, userId.Value);
        if (result)
        {
            Alert("Workout cleared successfully.", AlertType.info);
            return RedirectToAction(returnUrl);
        }
        else
        {
            _logger.LogWarning("Workout not cleared. Potential issue.");
            Alert("Workout not cleared. Please try again", AlertType.danger);
            return RedirectToAction(returnUrl);
        }
    }
    // This is the GET route for the Edit Target Time page.
    [HttpGet("EditTargetTime/{id}")]
    public ActionResult EditTargetTime(int id)
    {
        // Fetch the existing training plan from your data store. 
        var trainingPlan = _trainingPlanService.GetPlanById(id);
        if (trainingPlan == null)
        {
            // No training plan with the provided ID exists.
            _logger.LogWarning("No Training Plan found for ID.");
            return NotFound();
        }

        // Create a new view model with the current values of the training plan.
        var viewModel = new EditTargetTimeViewModel
        {
            TrainingPlanId = trainingPlan.Id,
            TargetRace = trainingPlan.TargetRace,
            TargetTime = trainingPlan.TargetTime,
        };

        // Pass the view model to the view.
        return View(viewModel);
    }
    // This is the POST route for the Edit Target Time page.
    [HttpPost("EditTargetTime")]
    public IActionResult EditTargetTime(EditTargetTimeViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = _trainingPlanService.EditTargetTime(model.TrainingPlanId, model.TargetTime);

        if (!result)
        {
            Alert("New Target Time did not save", AlertType.danger);
            _logger.LogWarning("Edited Target Time did not save.");
            return Index();

        }
        Alert("New Target Time saved successfully!", AlertType.success);
        return RedirectToAction("ViewTrainingPlan", new { id = model.TrainingPlanId });
    }
    // This is the GET route for Deleting a Training Plan.
    [HttpGet("DeleteTrainingPlan")]
    public IActionResult DeleteTrainingPlan()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "User");
        }
        var trainingPlan = _trainingPlanService.GetPlanByUserId(userId.Value);
        var model = new TrainingPlanDeleteModel
        {
            Id = trainingPlan.Id,
            TargetRace = trainingPlan.TargetRace,
            RaceDate = trainingPlan.RaceDate,
            TargetTime = trainingPlan.TargetTime
        };
        return View(model);
    }
    // This is the POST route for Deleting a Training Plan.
    [HttpPost("DeleteTrainingPlan")]
    public IActionResult DeleteTrainingPlan(TrainingPlanDeleteModel model)
    {
        var trainingPlan = _trainingPlanService.GetPlanById(model.Id);

        if (trainingPlan == null)
        {
            _logger.LogError("Training plan not found.");
            Alert("Error: Training plan not found", AlertType.danger);
            return RedirectToAction("Error", "Home");
        }

        var result = _trainingPlanService.DeletePlan(trainingPlan);

        if (!result)
        {
            _logger.LogWarning("Training Plan not deleted. Potential issue.");
            Alert("Training Plan not deleted. Please try again", AlertType.danger);
            return RedirectToAction("ViewTrainingPlan");
        }
        Alert("Training Plan deleted successfully.", AlertType.info);
        return RedirectToAction("Index", "Home");
    }
    // This is the GET route to return the available dates for a workout date change.
    [HttpGet]
    public IActionResult GetAvailableDates(int workoutId)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User is not authenticated.");
            }

            var availableDates = _trainingPlanService.GetAvailableDates(workoutId, userId.Value);
            if (availableDates == null || !availableDates.Any())
            {
                return NotFound("No available dates found.");
            }
            return Ok(availableDates.Select(date => date.ToString("yyyy-MM-dd")).ToList());
        }
        catch (Exception e)
        {
            _logger.LogError($"Error getting available dates: {e.Message}");
            return BadRequest("Error getting available dates.");
        }
    }
    // This is the POST route to update a workout date.
    [HttpPost]
    public IActionResult UpdateWorkoutDate(int workoutId, DateTime newDate)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User is not authenticated.");
            }

            var success = _trainingPlanService.UpdateWorkoutDate(workoutId, userId.Value, newDate);
            if (success)
            {
                Alert("Workout date updated successfully.", AlertType.success);
                return Ok(new { success = true });
            }
            Alert("Failed to update workout date.", AlertType.danger);
            return BadRequest(new { success = false, message = "Failed to update workout date." });
        }
        catch (Exception e)
        {
            _logger.LogError($"Error updating workout date: {e.Message}");
            return BadRequest(new { success = false, message = "Error updating workout date." });
        }
    }
}

