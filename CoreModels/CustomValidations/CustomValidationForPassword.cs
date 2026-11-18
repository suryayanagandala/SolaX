using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.CustomValidations
{
    public static class CustomValidationForPassword
    {
        public static ValidationResult ValidatePassword(string value)
        {

            string specialChars = "@#$%^&*";
            if (string.IsNullOrEmpty(value))
            {
                return new ValidationResult("Password Must not be empty");
            }
            value = value.Trim();


            if (!value.Any(char.IsLower))
            {
                return new ValidationResult("pasword must contain alteast lower case  letter");

            }
            else if (!value.Any(char.IsNumber))
            {
                return new ValidationResult(" Atleast contains one number");
            }
            else if (!value.Any(char.IsUpper))
            {
                return new ValidationResult("pasword must contain alteast One Upper case  letter");
            }

            else if (!char.IsUpper(value[0]))
            {
                return new ValidationResult("password first letter must be upper case only");
            }
            else if (value.Length < 6)
            {
                return new ValidationResult("Atleast passowrd length must be greater than 6 characters");
            }
            else if (value.Length > 12)
            {
                return new ValidationResult("Password must be less than 12 characters");
            }
            else if (!specialChars.Any(value.Contains))
            {
                return new ValidationResult("password contains atleast one special character");
            }

            return ValidationResult.Success!;
        }
    }
}
