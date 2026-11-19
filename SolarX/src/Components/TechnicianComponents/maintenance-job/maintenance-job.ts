import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { MaintenanceService } from '../../../Shared/maintenance-service';
import { TokenService } from '../../../Shared/token';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-maintenance-job',
  
  imports: [MaterialModule,CommonModule],
  templateUrl: './maintenance-job.html',
  styleUrl: './maintenance-job.scss',
})
export class MaintenanceJob implements OnInit{
userId!:string;
dto!:any[];
pageSize=5;
pageIndex=0;
constructor(private service:MaintenanceService,private token:TokenService,private toastr:ToastrService,
  private router:Router,private cd:ChangeDetectorRef
){}
  ngOnInit(): void {
    const userId=this.token.GetUserId();
    if(userId){
      this.userId=userId;
      this.getAssignedJobs(this.userId);
    }
  }

  getAssignedJobs(userId:string){
    this.service.technicianMaintenanceJobs(userId).subscribe((res)=>{
      if(res.status){
        this.dto=res.data??[];
        console.log('jobs',this.dto);
        this.cd.detectChanges();
      }
    })
  }

  get paginatedMaintenances(){
    const start=this.pageIndex*this.pageSize;
    const end=start+this.pageSize;
    return this.dto.slice(start,end);
  }

  reportForm(requestId:string){
    this.router.navigate(['/panel/maintenance-form'],{
      queryParams:{
        requestId:requestId
      }
    })
  }

  onPageEvent(event:any){
    this.pageIndex-event.pageIndex;
    this.pageSize=event.pageSize;
  }
}
