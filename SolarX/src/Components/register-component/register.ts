import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../MaterialModule';
import { Auth } from '../../Shared/auth';
import { RegisterationCreateDto } from '../../Models/Models';
import { PasswordValidators } from './passwordValidator';
import { localMessages } from '../../localMessages';
import { UserService } from '../../Shared/user-service';

@Component({
  selector: 'app-register',
  standalone:true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent implements OnInit {

  registerForm!: FormGroup;
  isEditMode = false;
  currentUserId!: string;
   hidePassword: boolean = true;
  private messages = localMessages;

  constructor(
    private builder: FormBuilder,
    private service: Auth,
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
    private userService:UserService
  ) {
this.registerForm = this.builder.group({
  fullName: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9]+( [A-Za-z0-9]+)*$/)]],
  emailId: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required, PasswordValidators()]],
  phoneNumber: ['', [Validators.required, Validators.pattern(/^[6-9]\d{9}$/)]],
  address: ['', [Validators.required, Validators.pattern(/\S+/)]]
});
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'];
      if (userId) {
        console.log(userId);
        
        this.isEditMode = true;
        this.currentUserId = userId;
        this.loadUser(userId);
      }
    });
  }


  // Load user for edit mode
 
  loadUser(userId: string) {
    this.userService.getUserDetails(userId).subscribe({
      next: (res) => {
        if (res.status) {
          this.registerForm.patchValue({
            fullName: res.data.fullName,
            emailId: res.data.emailId,
            phoneNumber: res.data.phoneNumber,
            address: res.data.address
          });
          console.log('user-details',res.data);
          
          // Disable email and password in edit mode
          // this.registerForm.get('emailId')?.disable();
          this.registerForm.get('password')?.disable();
          this.registerForm.get('password')?.clearValidators();
          this.registerForm.get('password')?.updateValueAndValidity();
        }
      }
    });
  }

  private cleanFormValues() {
  Object.keys(this.registerForm.controls).forEach(key => {
    const control = this.registerForm.get(key);

    if (control && typeof control.value === 'string') {
      let cleaned = control.value
        .trim()                
        .replace(/\s+/g, ' ');  

      control.setValue(cleaned);
    }
  });
}
 
  // Submit
 
  submit() {
    this.cleanFormValues();
    if (this.registerForm.invalid) return;

    if (this.isEditMode) {
      this.updateProfile();
    } else {
      this.createUser();
    }
  }

 
  // Create
 
  createUser() {
    const dto: RegisterationCreateDto = this.registerForm.getRawValue();

    this.service.regeister(dto).subscribe({
  next: (res) => {
    if (res.status) {
      this.toastr.success(this.messages.Register.registered);
      this.router.navigate(['/login']);
    }
  },
  error: (err) => {
    if (err.error?.message) {
      this.toastr.error(err.error.message);
    } else {
      this.toastr.error('Something went wrong');
    }
  }
});
  }

  
  // Update
 
  updateProfile() {

    const dto = {
      fullName: this.registerForm.getRawValue().fullName,
      phoneNumber: this.registerForm.getRawValue().phoneNumber,
      address: this.registerForm.getRawValue().address,
      email:this.registerForm.getRawValue().emailId
    };

    this.userService.updateProfile(this.currentUserId, dto).subscribe({
      next: (res) => {
        if (res.status) {
          this.toastr.success('Profile Updated Successfully');
          this.router.navigate(['/login']);
        } else {
          this.toastr.error(res.error);
        }
      }
    });
  }


  get password() {
    return this.registerForm.get('password');
  }

  hasCapital() {
    return /^[A-Z]/.test(this.password?.value || '');
  }

  hasDigit() {
    return /\d/.test(this.password?.value || '');
  }

  hasSpecialChar() {
    return /[@$!%*?&]/.test(this.password?.value || '');
  }

  minLength() {
    return (this.password?.value?.length || 0) >= 8;
  }

  maxLength() {
    return (this.password?.value?.length || 0) <= 12;
  }

    togglePassword(){
  this.hidePassword = !this.hidePassword;
}
}