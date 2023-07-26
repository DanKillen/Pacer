using Microsoft.AspNetCore.Mvc;
using Pacer.Data.Entities;
using Pacer.Data.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Pacer.Web.Controllers
{
    public class TrainingPlanController : BaseController
    {
        private readonly ITrainingPlanService _trainingPlanService;
        private readonly IRunningProfileService _runningProfileService;

        public TrainingPlanController(ITrainingPlanService trainingPlanService, IRunningProfileService runningProfileService)
        {
            _trainingPlanService = trainingPlanService;
            _runningProfileService = runningProfileService;
        }

        private int GetUserId()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                // Log this occurrence or handle it in a way that's appropriate for your application.
                Alert("Issue with User ID. Please log out then try again", AlertType.danger);
            }

            return userId;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var userId = GetUserId();
            var trainingPlan = _trainingPlanService.GetPlanByUserId(userId);

            if (trainingPlan != null)
            {
                return RedirectToAction("ViewTrainingPlan", new { id = trainingPlan.Id });
            }

            return RedirectToAction("CreateTrainingPlan");
        }

        [HttpGet]
        public IActionResult CreateTrainingPlan()
        {
            var userId = GetUserId();
            var runningProfile = _runningProfileService.GetProfileByUserId(userId);

            var model = new TrainingPlanCreateModel
            {
                RunningProfileId = runningProfile.Id
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

            return (View(model));
        }

        [HttpGet]
        public IActionResult ViewTrainingPlan(int id)
        {
            var trainingPlan = _trainingPlanService.GetPlanById(id);

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

            var viewModel = new TrainingPlanViewModel
            {
                Id = trainingPlan.Id,
                PlanName = trainingPlan.TargetRace.ToString() + " " + trainingPlan.TargetTime.ToString(@"h\:mm\:ss"),
                Weeks = trainingPlan.Workouts.GroupBy(w => w.Date.GetIso8601WeekOfYear())
                             .Select(g => new WeekViewModel
                             {
                                 WeekNumber = g.Key,
                                 Workouts = g.Select(w => new WorkoutViewModel
                                 {
                                     Type = w.Type,
                                     Date = w.Date,
                                     TargetDistance = w.TargetDistance,
                                     TargetPaceMinMinutes = w.TargetPaceMinMinutes,
                                     TargetPaceMinSeconds = w.TargetPaceMinSeconds,
                                     TargetPaceMaxMinutes = w.TargetPaceMaxMinutes,
                                     TargetPaceMaxSeconds = w.TargetPaceMaxSeconds,
                                     WorkoutDescription = w.WorkoutDescription
                                 }).ToList()
                             }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ViewTrainingPlanCalendar()
        {
            var userId = GetUserId();
            var trainingPlan = _trainingPlanService.GetPlanByUserId(userId);

            if (trainingPlan == null)
            {
                return Json(new { message = "Error: Training plan not found", type = "danger" });
            }

            if (trainingPlan.Workouts == null)
            {
                return Json(new { message = "Error: Training plan has null workouts", type = "danger" });
            }

            if (!trainingPlan.Workouts.Any())
            {
                return Json(new { message = "Error: Training plan has no workouts", type = "danger" });
            }

            var firstWorkoutDate = trainingPlan.Workouts.OrderBy(w => w.Date).First().Date;


            var viewModel = new TrainingPlanCalendarViewModel
            {
                Id = trainingPlan.Id,
                PlanName = trainingPlan.TargetTime.ToString(@"h\:mm\:ss") + " " + trainingPlan.TargetRace.ToString(),
                Month = firstWorkoutDate.Month,
                Year = firstWorkoutDate.Year,
                TargetPace = trainingPlan.TargetPace,
                RaceDate = trainingPlan.RaceDate,

                Workouts = trainingPlan.Workouts.Select(w => new WorkoutCalendarViewModel
                {
                    Type = w.Type,
                    Date = w.Date,
                    TargetDistance = w.TargetDistance,
                    TargetPaceMinMinutes = w.TargetPaceMinMinutes,
                    TargetPaceMinSeconds = w.TargetPaceMinSeconds,
                    TargetPaceMaxMinutes = w.TargetPaceMaxMinutes,
                    TargetPaceMaxSeconds = w.TargetPaceMaxSeconds,
                    WorkoutDescription = w.WorkoutDescription
                }).ToList()
            };
            //remove after testing
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(viewModel));



            return Json(viewModel);
        }

        [HttpGet]
        public IActionResult Calendar()
        {
            var viewModel = GetTrainingPlanCalendarViewModel();
            if (viewModel == null)
            {
                // Handle error. This might redirect to an error page, for example.
                return RedirectToAction("Error", "Home");
            }

            return View("Calendar", viewModel);
        }

        private TrainingPlanCalendarViewModel GetTrainingPlanCalendarViewModel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainingPlan = _trainingPlanService.GetPlanByUserId(Int32.Parse(userId));

            if (trainingPlan == null || trainingPlan.Workouts == null || !trainingPlan.Workouts.Any())
            {
                return null;
            }

            var firstWorkoutDate = trainingPlan.Workouts.OrderBy(w => w.Date).First().Date;

            return new TrainingPlanCalendarViewModel
            {
                Id = trainingPlan.Id,
                PlanName = trainingPlan.TargetTime.ToString(@"h\:mm\:ss") + " " + trainingPlan.TargetRace.ToString(),
                Month = firstWorkoutDate.Month,
                Year = firstWorkoutDate.Year,
                RaceDate = trainingPlan.RaceDate,
                TargetRace = trainingPlan.TargetRace,
                TargetPace = trainingPlan.TargetPace,
                

                Workouts = trainingPlan.Workouts.Select(w => new WorkoutCalendarViewModel
                {
                    Id = w.Id,
                    Type = w.Type,
                    Date = w.Date,
                    TargetDistance = w.TargetDistance,
                    TargetPaceMinMinutes = w.TargetPaceMinMinutes,
                    TargetPaceMinSeconds = w.TargetPaceMinSeconds,
                    TargetPaceMaxMinutes = w.TargetPaceMaxMinutes,
                    TargetPaceMaxSeconds = w.TargetPaceMaxSeconds,
                    WorkoutDescription = w.WorkoutDescription,
                    ActualDistance = w.ActualDistance,
                    ActualTime = w.ActualTime
                }).ToList()
            };
        }

        [HttpPost("SaveWorkoutActuals")]
        public IActionResult SaveWorkoutActuals(int WorkoutId, double ActualDistance, int ActualHours, int ActualMinutes, int ActualSeconds)
        {
            var actualTime = new TimeSpan();
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
                _trainingPlanService.SaveWorkoutActuals(WorkoutId, userId, ActualDistance, actualTime);

                // Redirecting to the Calendar action in the TrainingPlan controller
                return RedirectToAction("Calendar", "TrainingPlan");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error saving actuals: " + ex.Message });
            }
        }

    }
}
