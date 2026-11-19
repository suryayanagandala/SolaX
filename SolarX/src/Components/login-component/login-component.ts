import { Component } from '@angular/core';
import { MaterialModule } from '../../MaterialModule';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder,FormGroup,ReactiveFormsModule,Validators } from '@angular/forms';
import { TokenService } from '../../Shared/token';
import { Auth } from '../../Shared/auth';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PasswordValidators } from '../register-component/passwordValidator';
import { localMessages } from '../../localMessages';
import { Role } from '../../Models/Models';
import { NotificationSignalrService } from '../../Shared/signal-r';

@Component({
  selector: 'app-login-component',
  imports: [MaterialModule,ReactiveFormsModule,RouterModule,CommonModule],
  templateUrl: './login-component.html',
  styleUrl: './login-component.scss',
})
export class LoginComponent {
  _loginForm!:FormGroup;
  _response:any;
   submitted = false;
   private message=localMessages;
   userId:string|null=null;
   hidePassword: boolean = true;
  constructor(
   private builder:FormBuilder,
   private service:Auth,
   private token:TokenService,
   private router:Router,private toastr:ToastrService,
   private route:ActivatedRoute,
   private signalService:NotificationSignalrService
  ){
    this._loginForm=this.builder.group({
      email:this.builder.control('',[Validators.email,Validators.required]),
      password:this.builder.control('',Validators.compose([Validators.required]))
    })
  }

  Login():void{
    if(this._loginForm.invalid) return;


    this.service.login(this._loginForm.value as any).subscribe({
      next:res=>{
        if(res.status===true)
        {
          this.submitted=true;
          this.token.SaveToken(res?.data?.tokenResponse);
          this.token.SaveUser(res.data);
          this.token.SaveRefreshToken(res?.data?.refreshToken);
          const token = this.token.getToken();

          if(token){
            this.signalService.startConnection(token).then(()=>{
              console.log("SignalR started after login");
            });
          }
          this.toastr.success(this.message.login.sucess);

          const returnUrl=this.route.snapshot.queryParams['returnUrl'];
          if(returnUrl){
            this.router.navigateByUrl(returnUrl);
          }

          else if(res.data.roleName===Role.Admin){
            this.router.navigate(['/admin'])
          }
          else if(res.data.roleName===Role.Technician){
            this.userId=this.token.GetUserId();
            this.router.navigate(['/panel'],{
              queryParams:{
                userId:this.userId
              }
            })
          }
          else{
            this.router.navigate(['/home']);
          }
          
      
        }
        else{
          this.toastr.error(res.error??+this.message.error.credentials);
        }
      },error:()=>{
        this.toastr.error(this.message.error.server);
      }
      
    });
   
  }
  goToRegister(){
    this.router.navigate(['/register']);
  }
  gotoForgotPassword(){
    this.router.navigate(['/forgotPassword'])
  }

  togglePassword(){
  this.hidePassword = !this.hidePassword;
}
}
