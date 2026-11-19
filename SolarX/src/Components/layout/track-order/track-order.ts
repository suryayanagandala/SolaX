import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { ToastrService } from 'ngx-toastr';
import { TrackDelivery } from '../../../Models/Models';
import { OrderService } from '../../../Shared/order-service';
import { ActivatedRoute,Router, RouterModule } from '@angular/router';
import { localMessages } from '../../../localMessages';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-track-order',
  imports: [MaterialModule,CommonModule,RouterModule],
  templateUrl: './track-order.html',
  styleUrl: './track-order.scss',
})
export class TrackOrder implements OnInit{
 trackDto!:TrackDelivery[];
 orderId!:number;
 messages=localMessages
 constructor(private service:OrderService,private toastr:ToastrService,private router:Router,
  private route:ActivatedRoute
 ){}
 ngOnInit(): void {
   this.route.queryParams.subscribe(params=>{
    const id=params['orderId'];
    if(id){
      this.orderId=id;
      this.trackOrder(this.orderId);
      
    }
    else{
      this.router.navigate(['/order-history'])
    }
   })
 }

 trackOrder(orderId:number){
  this.service.trackOrder(orderId).subscribe((res)=>{
    if(res.status){
      this.trackDto=res.data??[];
      console.log('track',this.trackDto);
      
    }
    else{
      this.toastr.error(this.messages.order.error);
    }
  })
 }

 changeStep(index:number):string{
  var currentIndex=-1;
  this.trackDto.forEach((item,i)=>{
    if(item.status==='Active'){
      currentIndex=i;
    }
  })
  if(index<currentIndex) return 'completed'
  if(index===currentIndex) return 'active';
  return 'pending';
 }


}
