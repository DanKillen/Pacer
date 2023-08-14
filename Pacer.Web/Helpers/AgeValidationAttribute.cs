using System.ComponentModel.DataAnnotations;

namespace Pacer.Web.AgeValidationAttribute;

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