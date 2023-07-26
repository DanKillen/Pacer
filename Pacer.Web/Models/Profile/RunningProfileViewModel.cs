public class RunningProfileViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public int WeeklyMileage { get; set; }
    public int FiveKTimeMinutes { get; set; }
    public int FiveKTimeSeconds { get; set; }
    // FiveKTime is read-only and computed from FiveKTimeMinutes and FiveKTimeSeconds
    public TimeSpan FiveKTime => TimeSpan.FromMinutes(FiveKTimeMinutes) + TimeSpan.FromSeconds(FiveKTimeSeconds);

    public string FiveKTimeFormatted => (TimeSpan.FromMinutes(FiveKTimeMinutes) + TimeSpan.FromSeconds(FiveKTimeSeconds)).ToString("m\\:ss");

}
