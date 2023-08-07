using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class TooltipViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(string key)
    {
        var tooltips = new Dictionary<string, string>
        {
            { "RecoveryRun", "Very easy runs designed to allow your muscles to recover while still getting in some mileage." },
            { "EasyRun", "These are typically longer runs done at a relaxed pace." },
            { "LongRun", "This is usually the longest run of the week, designed to build up your endurance." },
            { "MarathonPace", "These runs are done at the pace you hope to maintain in your target race." },
            { "IntervalTraining", "These workouts involve periods of high-intensity running interspersed with periods of low-intensity recovery." },
            { "TempoRun", "These runs are done at a 'comfortably hard' pace to help increase your anaerobic threshold." },
            { "VO2Max", "These workouts are as fast as you can go, designed to improve your maximum oxygen uptake, an important measure of aerobic fitness." }
        };

        var tooltipText = tooltips[key];
        return Task.FromResult((IViewComponentResult)View("Default", tooltipText));

    }
}