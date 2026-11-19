import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../MaterialModule';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder,FormGroup,Validators } from '@angular/forms';
import { Auth } from '../../Shared/auth';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { localMessages } from '../../localMessages';

@Component({
  selector: 'app-verify-otp',
  standalone:true,
  imports: [MaterialModule,ReactiveFormsModule,CommonModule],
  templateUrl: './verify-otp.html',
  styleUrl: './verify-otp.scss',
})
export class VerifyOtp implements OnInit{
  otpForm!:FormGroup;
  response:any;
  private messages=localMessages
  constructor(private toastr:ToastrService,private service:Auth,private builder:FormBuilder,
    private router:Router
  ){
    this.otpForm=this.builder.group({
      otpText:this.builder.control('',[Validators.required]),
    })
  }
      timer = 30; 
      expired = false;
      intervalId: any;

  ngOnInit(): void {
   this.startTimer() 
  }
  startTimer(){
    this.intervalId=setInterval(()=>{
      this.timer--;

      if (this.timer <= 0) 
        {
          this.timer = 0;
          this.expired = true;
          clearInterval(this.intervalId);
        }},1000)
  }

  get timerDisplay(){
    const min=Math.floor(this.timer/60);
    const sec=this.timer%60;
    return `${min}:${sec <10 ?'0'+sec:sec}`
  }

  onOtpChange(){
    const otp=this.otpForm.value.otpText;
    if (otp?.length === 6 && !this.expired){
      this.verifyOtp();
    }
  }

  verifyOtp(){
    const emailId=sessionStorage.getItem('emailId');
    const otp=this.otpForm.value.otpText;
     if (!emailId) {
    this.toastr.error(this.messages.otp.required);
    this.router.navigate(['/login'])
    
  }

  if (!otp || this.expired) {
    this.toastr.error(this.messages.otp.invalid);
    return;
  }
  this.service.verifyOtp({email:emailId!,otpText:otp}).subscribe({
    next:(res)=>{
      this.response = res;
      if(res.status===true){
        this.toastr.success(this.messages.otp.sucsess);
        this.router.navigate(['/resetpassword'],{ replaceUrl: true });
        console.log(res.data);
        sessionStorage.setItem('tokenResponse',res.data)
        
      }
      else{
        this.toastr.error(this.messages.otp.invalid);
        console.log(res.error);
        
      }
    },
    error:(err)=>{
      console.log(err);
      
    }
  })
  }

  resendOtp(){
     const email = sessionStorage.getItem('emailId');
     this.service.sendOtp(email!).subscribe((res)=>{
      if(res.status){
       
       this.toastr.success(res.data);
        this.timer=30;
      this.expired=false;
      this.startTimer();
      }
      
     })
  }
}
