using System.Globalization;

public class MileageGraphViewModel
{
    public List<WorkoutCalendarViewModel> Workouts { get; set; }

    public Dictionary<int, List<WorkoutCalendarViewModel>> WorkoutsByWeek
    {
        get
        {
            return GetWorkoutsByWeek();
        }
    }

    private Dictionary<int, List<WorkoutCalendarViewModel>> GetWorkoutsByWeek()
    {
        var groupedWorkouts = new Dictionary<int, List<WorkoutCalendarViewModel>>();

        foreach (var workout in Workouts)
        {
            int weekNum = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                workout.Date,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Sunday);

            if (!groupedWorkouts.ContainsKey(weekNum))
            {
                groupedWorkouts[weekNum] = new List<WorkoutCalendarViewModel>();
            }

            groupedWorkouts[weekNum].Add(workout);
        }

        return groupedWorkouts;
    }
}


