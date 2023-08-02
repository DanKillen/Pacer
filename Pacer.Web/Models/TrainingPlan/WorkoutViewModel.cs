using Pacer.Data.Entities;

public class WorkoutViewModel
{
    public int Id { get; set; }
    public WorkoutType Type { get; set; }
    public DateTime Date { get; set; }
    public string DateString => Date.ToString("yyyy-MM-ddTHH:mm:ssZ");

    public string FormattedDate => Date.ToString("yyyy-MM-dd");
    public double TargetDistance { get; set; }
    public int TargetPaceMinMinutes { get; set; }
    public int TargetPaceMinSeconds { get; set; }
    public int TargetPaceMaxMinutes { get; set; }
    public int TargetPaceMaxSeconds { get; set; }
    public string WorkoutDescription { get; set; }

    public double ActualDistance { get; set; }
    public TimeSpan ActualTime { get; set; }
    public string ActualPace { get; set; }
}
