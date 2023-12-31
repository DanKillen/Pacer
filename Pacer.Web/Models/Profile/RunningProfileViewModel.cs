using System.ComponentModel.DataAnnotations;

public class RunningProfileViewModel
{
    public int UserId { get; set; }
    
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [AgeValidation(18, 85)]
    public DateTime DateOfBirth { get; set; }
    
    public int Age => (int)((DateTime.Now - DateOfBirth).TotalDays / 365.25);
    
    [Required]
    public string Gender { get; set; }

    [Required]
    [Range(0, 250, ErrorMessage = "Weekly Mileage must be between 0 and 250.")]
    public int WeeklyMileage { get; set; }
    
    [Required]
    [Range(12, 30, ErrorMessage = "Five K Time must be between 12 minutes 30 seconds and 30 minutes.")]
    public int FiveKTimeMinutes { get; set; }
    
    public int FiveKTimeSeconds { get; set; }
    
    public string FiveKTimeFormatted => (TimeSpan.FromMinutes(FiveKTimeMinutes) + TimeSpan.FromSeconds(FiveKTimeSeconds)).ToString("m\\:ss");
}