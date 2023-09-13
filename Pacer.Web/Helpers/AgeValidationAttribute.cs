using System.ComponentModel.DataAnnotations;



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
        if (value is not DateTime birthDate)
        {
            return new ValidationResult("Invalid date format.");
        }

        int age = DateTime.UtcNow.Year - birthDate.Year;
        if (birthDate > DateTime.UtcNow.AddYears(-age)) age--;

        if (age < MinAge)
        {
            return new ValidationResult($"You must be at least {MinAge} years old.");
        }

        if (age > MaxAge)
        {
            return new ValidationResult($"You must be less than {MaxAge} years old.");
        }

        return ValidationResult.Success;
    }
}
