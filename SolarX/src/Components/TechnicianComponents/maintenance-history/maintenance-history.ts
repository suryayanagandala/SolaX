import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { MaintenanceReport } from '../maintenance-report/maintenance-report';
import { MaintenanceService } from '../../../Shared/maintenance-service';
import { Router } from '@angular/router';
import { localMessages } from '../../../localMessages';


@Component({
  selector: 'app-maintenance-history',
  imports: [MaterialModule,CommonModule],
  templateUrl: './maintenance-history.html',
  styleUrl: './maintenance-history.scss',
})
export class MaintenanceHistory implements OnInit{
dto!:any[];
userId!:string;
messages=localMessages;
pageSize=5;
pageIndex=0;
constructor(private toastr:ToastrService,private service:MaintenanceService,
  private router:Router
){}
ngOnInit(): void {
  this.getCompletedTasks();
}
getCompletedTasks(){
  this.service.getCompletedMaintenances().subscribe((res)=>{
    if(res.status){
      this.dto=res.data??[];
      console.log('mainatenances-data',this.dto);
      
    }else{
      this.toastr.error(this.messages.error.fail)
    }
  })
}

 get paginatedCompletedMaintenances(){
  const start=this.pageIndex*this.pageSize;
  const end=start+this.pageSize;
  return this.dto.slice(start,end);
 }

 onPageEvent(event:any){
  this.pageSize=event.pageSize;
  this.pageIndex=event.pageIndex;
 }
}
