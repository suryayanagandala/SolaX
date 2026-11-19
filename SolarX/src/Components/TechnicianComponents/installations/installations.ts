import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { InstallationService } from '../../../Shared/installation-service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { Signal } from '@angular/core';
import { NotificationSignalrService } from '../../../Shared/signal-r';

@Component({
  selector: 'app-installations',
  standalone:true,
  imports: [MaterialModule,CommonModule],
  templateUrl: './installations.html',
  styleUrl: './installations.scss',
})
export class Installations implements OnInit {

  installationDto: any[] = [];
  pageSize=5;
  pageIndex=0;

  constructor(private service: InstallationService, private toastr: ToastrService,
    private router: Router,private signalService:NotificationSignalrService, private cd:ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.getInstallations();
  }

  getInstallations(){
    this.service.getInstallationStatus().subscribe((res)=>{
      if(res.status){
        this.installationDto=res.data??[];
        console.log('installations',this.installationDto);
        
      }
    })
  }

  get paginatedInstallations(){
    const start=this.pageIndex*this.pageSize;
    const end=start+this.pageSize;
    return this.installationDto.slice(start,end);
  }

  UpdatedInstallationStatus(report:any){
   if(report.actions!='Mark as Completed'){
    this.service.updateStatus(report.installationId).subscribe((res)=>{
      if(res.status){
        console.log(res.data);
        // this.installationDto.filter(x=>x.installationId!=report.installationId);
        this.getInstallations();
        this.cd.detectChanges();
      }

    })
   }
   else{
    this.router.navigate(['panel/report-form'],{
      queryParams:{
        installationId:report.installationId
      }
    })
   }
  }

  onPageEvent(event:any){
    this.pageIndex=event.pageIndex;
    this.pageSize=event.pageSize;
  }

}
