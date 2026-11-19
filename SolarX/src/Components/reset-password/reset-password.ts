import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../MaterialModule';
import { ReactiveFormsModule } from '@angular/forms';
import { FormGroup,FormBuilder,Validators } from '@angular/forms';
import { Auth } from '../../Shared/auth';
import { ResetPasswordCreateDto } from '../../Models/Models';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { localMessages } from '../../localMessages';

@Component({
  selector: 'app-reset-password',
  standalone:true,
  imports: [MaterialModule,ReactiveFormsModule,CommonModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss',
})
export class ResetPassword implements OnInit{
  resetForm!:FormGroup;
  response:any;
  resetTokenResponse!:string;
  hidePassword:boolean=true;
  private messages=localMessages

  constructor(private toastr:ToastrService,private router:Router,
    private service:Auth,private builder:FormBuilder){ }

    ngOnInit(): void {
      
      const token=sessionStorage.getItem('tokenResponse')

      if(!token){
        this.toastr.error(this.messages.resetPassword.expired)
        this.router.navigate(['/login']);
        return;
      }

      this.resetTokenResponse=token;

      this.resetForm=this.builder.group({
        newPassword:this.builder.control('',Validators.compose([Validators.required,
          Validators.minLength(8),Validators.maxLength(12),Validators.pattern(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&]).{8,12}$/)
        ])),
        confirmPassword:this.builder.control('',Validators.required)
      })
    }
    
    resetPassword(){
      if(this.resetForm.invalid){
        console.log(this.resetForm.value);
        
        return;
      }
      

      const dto:ResetPasswordCreateDto={
      resetToken:this.resetTokenResponse,
      newPassword:this.resetForm.value.newPassword!,
      confirmPassword:this.resetForm.value.confirmPassword
    };

    if(dto.newPassword!==dto.confirmPassword){
      this.toastr.error(this.messages.resetPassword.required);
      return;
    }
    this.service.resetPassword(dto).subscribe({
      next:(res)=>{
        this.response=res;
        if(res.status===true){
         this.toastr.success(this.messages.resetPassword.success);
         sessionStorage.removeItem('tokenResponse');
         this.router.navigate(['/login'])
         console.log(res.data);
         
        }
        else{
          this.toastr.error(this.messages.resetPassword.fail);
          console.log(res.error);
          
        }
      },
      error:(err)=>{
        console.log(err);
        
      }
    })
    } 

    togglePassword(){
  this.hidePassword = !this.hidePassword;
}
}
