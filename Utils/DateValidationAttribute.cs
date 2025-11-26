using System.ComponentModel.DataAnnotations;
namespace BG_Tec_Assesment_Minimal_Api.Utils
{
    public class DateValidationAttribute : ValidationAttribute
    {
 
        private readonly DateOnly minDateOnly = DateOnly.FromDateTime(DateTime.Today).AddYears(-125);

        public DateValidationAttribute()
        {
        }
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Date is required.");
            }
            if (value is not DateOnly)
            {
                return new ValidationResult("Invalid date format.");
            }

            if ((DateOnly) value > DateOnly.FromDateTime(DateTime.Now.Date))
            {
                return new ValidationResult("Dob cannot be in the future");
            }

            if ((DateOnly) value < minDateOnly)
            {
                return new ValidationResult("Dob cannot be more than 125 years ago");
            }

            return  ValidationResult.Success;
        }
    }
}
