using System;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Attributes
{
    public class ReleaseYearRangeAttribute : ValidationAttribute
    {
        private readonly int _minYear;

        public ReleaseYearRangeAttribute(int minYear)
        {
            _minYear = minYear;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dt)
            {
                if (dt.Year < _minYear || dt.Year > DateTime.Now.Year)
                {
                    return new ValidationResult($"Year must be between {_minYear} and {DateTime.Now.Year}");
                }
            }

            return ValidationResult.Success;
        }
    }
}