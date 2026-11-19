import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../MaterialModule';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Auth } from '../../Shared/auth';
import { Router } from '@angular/router';


@Component({
  selector: 'app-forget-password',
  imports: [ReactiveFormsModule, MaterialModule],
  templateUrl: './forget-password.html',
  styleUrl: './forget-password.scss',
})
export class ForgetPassword {
  form!: FormGroup;
  response: any;

  constructor(private service: Auth, private router: Router, private toastr: ToastrService, private builder: FormBuilder) {
    this.form = this.builder.group({
      email: this.builder.control('', [Validators.email, Validators.required])

    })
  }

  sendOtp() {
    if(this.form.invalid){
      this.toastr.error('invalid email Id');
      return;
    }
    const email = this.form.value.email;
    this.service.sendOtp(email).subscribe({
      next: (res) => {
        if(res.status===true)
        {
        sessionStorage.setItem('emailId',this.form.value.email!);
        this.toastr.success(`${res.data} please don't share `);
        console.log(res);
        this.router.navigate(['/verify-otp'])
        }else{
          this.toastr.error('Invalid emailId');
          return;
        }
      },
     
      error: (err) => console.error(err)
    });
  }

}
