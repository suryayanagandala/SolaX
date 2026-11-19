import { ValidatorFn,ValidationErrors,AbstractControl } from "@angular/forms";

export function PasswordValidators():ValidatorFn{
    return (control:AbstractControl):ValidationErrors|null=>{
        const value = control.value || '';
        const hasUpper = /[A-Z]/.test(value);
        const hasDigit = /\d/.test(value);
        const hasSpecial = /[@$!%*?&]/.test(value);
        const minLength = value.length >= 8;
        const maxLength = value.length <= 12;

         const valid = hasUpper && hasDigit && hasSpecial && minLength && maxLength;

        return valid ? null : { passwordInvalid: true };//named key in the errors object
    }
}