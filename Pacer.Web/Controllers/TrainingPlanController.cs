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

    public TrainingPlanController(ITrainingPlanService trainingPlanService, IRunningProfileService runningProfileService, ILogger<UserController> logger) : base(logger)
    {
        _trainingPlanService = trainingPlanService;
        _runningProfileService = runningProfileService;
    }

    private int? GetUserId()
    {
        if (!User.Identity.IsAuthenticated)
        {
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

        return RedirectToAction("CreateTrainingPlan");
    }

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
            Recommendation = _trainingPlanService.GetRecommendation(runningProfile.EstimatedMarathonTime, runningProfile.EstimatedHalfMarathonTime, runningProfile.WeeklyMileage, runningProfile.DateOfBirth),
            RaceDate = suggestedDate,
            EstimatedMarathonTime = runningProfile.EstimatedMarathonTime,
            EstimatedHalfMarathonTime = runningProfile.EstimatedHalfMarathonTime
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult CreateTrainingPlan(TrainingPlanCreateModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var plan = _trainingPlanService.CreatePlan(model.RunningProfileId, model.TargetRace, model.RaceDate, model.TargetTime);
                Alert("Training Plan created successfully", AlertType.success);
                return RedirectToAction("ViewTrainingPlan", new { id = plan.Id });
            }
            catch (Exception ex)
            {
                Alert("Error creating Training Plan: " + ex.Message, AlertType.danger);
            }
        }
        else
        {
            Alert("Error creating Training Plan", AlertType.danger);
        }

        return RedirectToAction("ViewTrainingPlan");

    }

    [HttpGet]
    public IActionResult ViewTrainingPlan()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "User");
        }
        var trainingPlan = _trainingPlanService.GetPlanByUserId(userId.Value);

        if (trainingPlan == null)
        {
            TempData["Alert.Message"] = "Error: Training plan not found";
            TempData["Alert.Type"] = "danger";
            return RedirectToAction("Error", "Home");
        }

        if (trainingPlan.Workouts == null)
        {
            TempData["Alert.Message"] = "Error: Training plan has null workouts";
            TempData["Alert.Type"] = "danger";
            return RedirectToAction("Error", "Home");
        }

        if (!trainingPlan.Workouts.Any())
        {
            TempData["Alert.Message"] = "Error: Training plan has no workouts";
            TempData["Alert.Type"] = "danger";
            return RedirectToAction("Error", "Home");
        }
        string FormattedTargetTime;
        if (trainingPlan.TargetTime < TimeSpan.FromHours(1))
        {
            FormattedTargetTime = trainingPlan.TargetTime.ToString(@"mm\:ss");
        }
        else
        {
            FormattedTargetTime = trainingPlan.TargetTime.ToString(@"h\:mm");
        }
        int currentWeek = 1;
        var viewModel = new TrainingPlanViewModel
        {
            Id = trainingPlan.Id,
            TargetRace = trainingPlan.TargetRace,
            TargetTime = FormattedTargetTime,
            TargetPace = trainingPlan.TargetPace,
            RaceDate = trainingPlan.RaceDate,
            Weeks = trainingPlan.Workouts.GroupBy(w => w.Date.GetWeekStartingSunday())
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
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Calendar()
    {
        var viewModel = GetTrainingPlanCalendarViewModel();

        if (viewModel == null)
        {
            // Redirecting to TrainingPlanIndex. This action will further check if the user has a training plan, is logged in, etc.
            Alert("Error: Training plan not found", AlertType.danger);
            return RedirectToAction("Index", "TrainingPlan");
        }

        var grouped = viewModel.Workouts
            .GroupBy(x => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(x.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday))
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

        ViewBag.WeekDistances = grouped;
        ViewBag.WeekNumbers = JsonSerializer.Serialize(grouped.Select(x => x.Week).ToList());
        ViewBag.TargetDistances = JsonSerializer.Serialize(grouped.Select(x => x.TargetDistance).ToList());
        ViewBag.ActualDistances = JsonSerializer.Serialize(grouped.Select(x => x.ActualDistance).ToList());


        return View("Calendar", viewModel);
    }

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
        var trainingPlan = _trainingPlanService.GetPlanByUserId(Int32.Parse(userId));

        if (trainingPlan == null || trainingPlan.Workouts == null || !trainingPlan.Workouts.Any())
        {
            return null;
        }

        var firstWorkoutDate = trainingPlan.Workouts.OrderBy(w => w.Date).First().Date;
        string FormattedTargetTime;
        if (trainingPlan.TargetTime < TimeSpan.FromHours(1))
        {
            FormattedTargetTime = trainingPlan.TargetTime.ToString(@"mm\:ss");
        }
        else
        {
            FormattedTargetTime = trainingPlan.TargetTime.ToString(@"h\:mm");
        }

        return new TrainingPlanCalendarViewModel
        {
            Id = trainingPlan.Id,
            TargetTime = FormattedTargetTime,
            Month = firstWorkoutDate.Month,
            Year = firstWorkoutDate.Year,
            RaceDate = trainingPlan.RaceDate,
            TargetRace = trainingPlan.TargetRace,
            TargetPace = trainingPlan.TargetPace,


            Workouts = trainingPlan.Workouts.Select(w => new WorkoutViewModel
            {
                Id = w.Id,
                Type = w.Type,
                Date = w.Date,
                TargetDistance = w.TargetDistance,
                ActualDistance = w.ActualDistance,
                TargetPace = LookupTargetPaceForWorkout(w.Type, trainingPlan.Paces),
                ActualTime = w.ActualTime,
                ActualPace = w.ActualDistance > 0 ? TimeSpan.FromMinutes(w.ActualTime.TotalMinutes / w.ActualDistance).ToString(@"mm\:ss") : null

            }).ToList()
        };
    }
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

    [HttpPost("SaveWorkoutActuals")]
    public IActionResult SaveWorkoutActuals(int WorkoutId, double ActualDistance, int ActualHours, int ActualMinutes, int ActualSeconds, string returnUrl)
    {
        TimeSpan actualTime;
        try
        {
            actualTime = new TimeSpan(ActualHours, ActualMinutes, ActualSeconds);
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Error: Invalid time format" });
        }

        try
        {
            var userId = GetUserId();
            _trainingPlanService.SaveWorkoutActuals(WorkoutId, userId.Value, ActualDistance, actualTime);


            Alert("Workout added successfully. Great job!", AlertType.success);
            return RedirectToAction(returnUrl);

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error saving data: " + ex.Message });
        }
    }

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
            Alert("Workout not cleared. Please try again", AlertType.danger);
            return RedirectToAction(returnUrl);
        }
    }

    [HttpGet("EditTargetTime/{id}")]
    public ActionResult EditTargetTime(int id)
    {
        // Fetch the existing training plan from your data store. 
        var trainingPlan = _trainingPlanService.GetPlanById(id);
        if (trainingPlan == null)
        {
            // No training plan with the provided ID exists.
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

    [HttpPost("EditTargetTime")]
    public IActionResult EditTargetTime(EditTargetTimeViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = _trainingPlanService.EditTargetTime(model.TrainingPlanId, model.TargetRace, model.TargetTime);

        if (!result)
        {
            Alert("New Target Time did not save", AlertType.danger);
            return Index();

        }
        Alert("New Target Time saved successfully!", AlertType.success);
        return RedirectToAction("ViewTrainingPlan", new { id = model.TrainingPlanId });
    }
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
    [HttpPost("DeleteTrainingPlan")]
    public IActionResult DeleteTrainingPlan(TrainingPlanDeleteModel model)
    {
        var trainingPlan = _trainingPlanService.GetPlanById(model.Id);

        if (trainingPlan == null)
        {
            TempData["Alert.Message"] = "Error: Training plan not found";
            TempData["Alert.Type"] = "danger";
            return RedirectToAction("Error", "Home");
        }

        var result = _trainingPlanService.DeletePlan(trainingPlan);

        if (!result)
        {
            Alert("Training Plan not deleted. Please try again", AlertType.danger);
            return RedirectToAction("ViewTrainingPlan");
        }
        Alert("Training Plan deleted successfully.", AlertType.info);
        return RedirectToAction("Index", "Home");
    }
}
