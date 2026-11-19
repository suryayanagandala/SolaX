import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { PaymentService } from '../../../Shared/payment';
@Component({
  selector: 'app-payment-sucess',
  imports: [MaterialModule],
  templateUrl: './payment-sucess.html',
  styleUrl: './payment-sucess.scss',
})
export class PaymentSucess implements OnInit{

  orderId:number|null=null;
  amount=0;
  paymentMethod:string|null=null;
  constructor(private toastr:ToastrService,private route:ActivatedRoute,
    private router:Router,private service:PaymentService,private cdr:ChangeDetectorRef
  ){

  }

  ngOnInit(): void {
    
    this.route.queryParams.subscribe(params=>{
      const id=params['orderId']
      
      if(id){
        this.orderId=Number(id);
        this.getInvoice(this.orderId)
      }
      else{
        this.router.navigate(['/order'])
      }
    })
  }

  getInvoice(orderId:number){
  
    this.service.getInvoice(orderId).subscribe((res)=>{
      if(res.status){
        console.log(res.data);
        
       const amount=res.data.payment.amount;
       const payment=res.data.payment.paymentMethod;
       this.amount=amount;
       this.paymentMethod=payment
      }
    })
  }

  openInvoice(isDownlaod:boolean):void{

    if(!this.orderId){
      return;
    }
      this.service.invoicePdf(this.orderId).subscribe((res)=>{
        if(res.status){

            const base64 = res.data;

            //convert base64 to binary string

            const binary=atob(base64!);

            const length=binary.length;
            const bytes=new Uint8Array(length);
            for(let i=0;i<length;i++){
              bytes[i]=binary.charCodeAt(i)
            }

            //create blob
            const blob=new Blob([bytes],{type:'application/pdf'})

            const url=URL.createObjectURL(blob);

            if(isDownlaod){
              const anchor=document.createElement('a');
              anchor.href=url;
              anchor.download = `Invoice_${this.orderId}.pdf`;
              anchor.click();
            }
            else{
              window.open(url,'_blank')
            }
        }
      })
  }
}
