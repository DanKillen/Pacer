using System.ComponentModel.DataAnnotations;

public class RunningProfileViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [AgeValidation(18, 100)]
    public DateTime DateOfBirth { get; set; }
    public int Age => (int)((DateTime.Now - DateOfBirth).TotalDays / 365.25);
    public string Gender { get; set; }
    [Required]
    [Range(0, 120, ErrorMessage = "Weekly Mileage must be between 0 and 120.")]
    public int WeeklyMileage { get; set; }
    public int FiveKTimeMinutes { get; set; }
    public int FiveKTimeSeconds { get; set; }
    // FiveKTime is read-only and computed from FiveKTimeMinutes and FiveKTimeSeconds
    public TimeSpan FiveKTime => TimeSpan.FromMinutes(FiveKTimeMinutes) + TimeSpan.FromSeconds(FiveKTimeSeconds);

    public string FiveKTimeFormatted => (TimeSpan.FromMinutes(FiveKTimeMinutes) + TimeSpan.FromSeconds(FiveKTimeSeconds)).ToString("m\\:ss");

    public TimeSpan EstimatedMarathonTime { get; set; }
    public TimeSpan EstimatedHalfMarathonTime { get; set; }

}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AgeValidationAttribute : ValidationAttribute
{
    public int MinAge { get; }
    public int MaxAge { get; }

    public AgeValidationAttribute(int minAge, int maxAge)
    {
        MinAge = minAge;
        MaxAge = maxAge;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;

            if (age < MinAge)
            {
                return new ValidationResult($"You must be at least {MinAge} years old.");
            }

            if (age > MaxAge)
            {
                return new ValidationResult($"You must be less than {MaxAge} years old.");
            }
        }
        return ValidationResult.Success;
    }
}
