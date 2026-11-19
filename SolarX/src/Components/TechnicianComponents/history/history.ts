import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { TechnicianService } from '../../../Shared/technician-service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { localMessages } from '../../../localMessages';

@Component({
  selector: 'app-history',
  imports: [MaterialModule,CommonModule],
  templateUrl: './history.html',
  styleUrl: './history.scss',
})
export class History implements OnInit{
deliveredOrdersDto:any[]=[];
private messages=localMessages;
pageSize=5;
pageIndex=0;

constructor(private toastr:ToastrService,private router:Router,private service:TechnicianService)
{}
ngOnInit(): void {
  this.deliveredOrders();
}

deliveredOrders(){
  this.service.getDeliveredOrders().subscribe((res)=>{
    if(res.status){
      this.deliveredOrdersDto=res.data;
    }
    else{
      this.toastr.error(this.messages.error.fail)
    }
  })
}
 get paginatedMessages(){
  const start=this.pageIndex*this.pageSize;
  const end=start+this.pageSize;
  return this.deliveredOrdersDto.slice(start,end)
 }

 onPageEvent(event:any){
  this.pageIndex=event.pageIndex;
  this.pageSize=event.pageSize;
 }

}
