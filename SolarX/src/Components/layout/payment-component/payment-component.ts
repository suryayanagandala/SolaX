import { Component, OnDestroy, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TokenService } from '../../../Shared/token';
import { PaymentService } from '../../../Shared/payment';
import { localMessages } from '../../../localMessages';
import { FormBuilder,FormGroup,ReactiveFormsModule,Validators } from '@angular/forms';
import { ReactiveFormDalogue } from '../../AdminComponents/reactive-form-dalogue/reactive-form-dalogue';
import { CommonModule, ɵnormalizeQueryParams } from '@angular/common';

@Component({
  selector: 'app-payment-component',
  standalone:true,
  imports: [MaterialModule,ReactiveFormsModule,CommonModule],
  templateUrl: './payment-component.html',
  styleUrl: './payment-component.scss',
})
export class PaymentComponent implements OnInit,OnDestroy{

  userId:string|null=null;
  totalAmount:number|null=null;
  orderId:number|null=null;
  qrCodeIamge='';
  expiredTime=0;
  timerRef: any;
  creditCard!:FormGroup;
  showImage=false;

  private message=localMessages;
  paymentSelectedMethod:'CARD'|'UPI'|'CASH'|''='';
  constructor(private router:Router,private toastr:ToastrService,
    private route:ActivatedRoute,private tokenSerive:TokenService,private paymentService:PaymentService,
    private builder:FormBuilder
  ){

  }

  ngOnInit(): void {
    this.userId=this.tokenSerive.GetUserId();
    if(!this.userId){
      this.router.navigate(['/login'],{
        queryParams:{
          returnUrl:this.router.url
        }
      })
    }

    const navigation=this.router.getCurrentNavigation();
    const state=navigation?.extras.state|| history.state;

    if(state){
       this.orderId=state.orderId;
       this.totalAmount=state.amount
    }
    else{
      this.toastr.error('No Order found')
      this.router.navigate(['/order']);
    }

   this.creditCard = this.builder.group({
  cardNumber: ['', [
    Validators.required,
    Validators.pattern(/^\d{16}$/)
  ]],
  name: ['', [
    Validators.required,
    Validators.pattern(/^[A-Za-z]+( [A-Za-z]+)*$/)
  ]],
  expiry: ['', [
    Validators.required,
    Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/),
    this.expiryValidator()
  ]],
  cvv: ['', [
    Validators.required,
    Validators.pattern(/^\d{3,4}$/)
  ]]
});

  }

  private cleanCardForm() {
  Object.keys(this.creditCard.controls).forEach(key => {
    const control = this.creditCard.get(key);

    if (control && typeof control.value === 'string') {
      control.setValue(control.value.trim());
    }
  });
}

  onTabChange(index:number):void{
    switch(index){
      case 0:
        this.paymentSelectedMethod="CARD";
        break;
      case 1:
        this.paymentSelectedMethod="UPI"
        this.generateQr();
        break;
      case 2:
        this.paymentSelectedMethod="CASH";
        break;
    }



  }


  generateQr(){
    if(this.orderId){
      this.paymentService.genearteQrCode(this.orderId).subscribe((res)=>{
        if(res.status){
         console.log(res.data);
         this.qrCodeIamge=res.data.qrCodeImage;
         this.expiredTime=res.data.expiresIn;
         this.startTimer();
         

        }
        else{
          console.log(res.error);
          
        }
        
      })
      
    }
    
  }

  startTimer(){
   if(this.timerRef){
    clearInterval(this.timerRef)
   }

    this.timerRef=setInterval(()=>{
      this.expiredTime--;

      if(this.expiredTime<=0){
        this.clearQr();
      }

    },1000)
  }

  clearQr(){
  clearInterval(this.timerRef);
  this.qrCodeIamge='';
  }

   ngOnDestroy(): void {
    clearInterval(this.timerRef)
  }

  confirmPayment(){
    const dto={
      orderId:this.orderId,
      paymentMethod:this.paymentSelectedMethod
    }


    this.paymentService.ProcessPayment(dto).subscribe((res)=>{
      this.cleanCardForm();
      if(res.status){
        this.showImage=true;
        console.log(res.data);
        this.clearQr();
        this.toastr.success(this.message.payment);
        this.router.navigate(['/payment-success'],{
          queryParams:{
            orderId:this.orderId
          }
        })
      }
      else{
        console.log(res.error);
        this.toastr.error(this.message.error.fail)
      }
    })
  }

  expiryValidator() {
  return (control: any) => {
    if (!control.value) return null;

    const [month, year] = control.value.split('/');

    if (!month || !year) return null;

    const expiryDate = new Date(
      2000 + parseInt(year),
      parseInt(month),
      0
    );

    const today = new Date();

    if (expiryDate < today) {
      return { expired: true };
    }

    return null;
  };
}

 
}
