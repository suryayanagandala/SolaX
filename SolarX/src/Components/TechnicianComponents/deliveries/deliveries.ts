import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { TechnicianService } from '../../../Shared/technician-service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenService } from '../../../Shared/token';
import { CommonModule } from '@angular/common';
import { localMessages } from '../../../localMessages';
import { TechnicianRole } from '../../../Models/Models';


@Component({
  selector: 'app-deliveries',
  standalone:true,
  imports: [MaterialModule,CommonModule],
  templateUrl: './deliveries.html',
  styleUrl: './deliveries.scss',
})
export class Deliveries implements OnInit{

  deliveryDto: any [] = [];
  private messages=localMessages;
  pageSize=5;
  pageIndex=0;
  technicianRole!:TechnicianRole;
  constructor(private service: TechnicianService, private toastr: ToastrService,
    private router: Router, private route: ActivatedRoute, private token: TokenService,private cd:ChangeDetectorRef
  ) {
     
    }
  ngOnInit(): void {
    const userId=this.token.GetUserId();
    if(!userId){
      this.router.navigate(['/login'],{
        queryParams:{
          returnUrl:this.router.url
        }
      })
    }
    this.GetDeliveries();
    this.cd.detectChanges()
  }

  GetDeliveries(){
    this.service.GetDeliveries().subscribe((res)=>{
      if(res.status){
        this.deliveryDto=res.data??[];
        
      }
    })
  }

  get paginatedDeleveries(){
    const start=this.pageIndex*this.pageSize;
    const end=start+this.pageSize;
    return this.deliveryDto.slice(start,end)
  }

  changeStatus(item:any){
    switch(item.status)
     {
        case 'OutForDelivery':
          this.delivered(item.deliveryId);
          break;

        default:
          this.changeDeliveryStatus(item.deliveryId);
          break;
     }

  }

  delivered(id:number){
    this.service.markAsDelivered(id).subscribe((res)=>{
      if(res.status){
        this.toastr.success(this.messages.delivery.status)
     
      this.GetDeliveries();
      this.cd.detectChanges();
      }
    })
  }

  changeDeliveryStatus(deliveryId:number){
    this.service.markAsDeliveryStataus(deliveryId).subscribe((res)=>{
      if(res.status){
        this.toastr.success(this.messages.delivery.delivered)
        this.GetDeliveries();
        this.cd.detectChanges();
      }
    })
  }

  onPageEvent(event:any){
    this.pageSize=event.pageSize;
    this.pageIndex=event.pageIndex;
  } 
}
