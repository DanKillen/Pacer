using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class TooltipViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(string key)
    {
        var tooltips = new Dictionary<string, string>
        {
            
            // Running Profile Tooltips
            {"WeeklyMileage", "Please enter how many miles you have ran each week, on average, for the past 4 weeks."},
            {"5KTime", "Please input a recent 5k time. If you don't have one then please time yourself running 5 kilometres, or 3.1 miles."},
            {"CreateProfile","We will analyse this profile to provide you with training plan recommendations. You will be able to edit this information later."},
            {"EditProfile","This updated 5k time will affect the next new training plan you create. If you want to edit your paces for an existing training plan, then please directly edit the target time."},
            
            // Training Plan Tooltips
            {"TrainingPlan", "All paces are given in minutes and seconds per mile. Add workout data to track your training."},
            {"CreateTrainingPlan","The Standard Half Marathon plan will begin at 10 miles a week and the Standard Marathon Plan will begin at 25 miles a week."},
            {"CreateDateofRace","This will update to the length of the selected training plan. Your race should be on this date or later."},
            {"CreateTargetTime","Times are given in hours and minutes. You may edit your target time if you like."},

            // Workout Type Tooltips
            { "RecoveryRun", "Very easy runs designed to allow your muscles to recover while still getting in some mileage." },
            { "EasyRun", "These are typically longer runs done at a relaxed pace." },
            { "LongRun", "This is usually the longest run of the week, designed to build up your endurance." },
            { "MarathonPace", "These runs are done at the pace you hope to maintain in your target race." },
            { "IntervalTraining", "These workouts involve periods of high-intensity running interspersed with periods of low-intensity recovery." },
            { "TempoRun", "These runs are done at a 'comfortably hard' pace to help increase your anaerobic threshold." },
            { "VO2Max", "These workouts are as fast as you can go, designed to improve your maximum oxygen uptake, an important measure of aerobic fitness." },
            
            // Weather Tooltips
            {"Timezone", "All times are in your local timezone."},
            {"Weather", "Weather data is provided by OpenWeatherMap."},
            {"CountryCode", "Country codes are checked against ISO 3166-1 alpha-2. Eg. GB = Great Britain, IE = Ireland"}

        };
        tooltips.TryGetValue(key, out var tooltipText);

        // If the key doesn't exist, set a default tooltip text
        tooltipText ??= "Are you enjoying Pacer? Please email us! We would love to hear from you";
        return Task.FromResult((IViewComponentResult)View("Default", tooltipText));

    }
}