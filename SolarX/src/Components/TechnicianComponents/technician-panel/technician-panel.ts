import { Component, HostListener, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { TechnicianService } from '../../../Shared/technician-service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router, RouterModule, RouterOutlet } from '@angular/router';
import { routes } from '../../../app/app.routes';
import { MenuItem, NotifcationDto, TECHNICIAN_MENU, TechnicianRole } from '../../../Models/Models';
import { TokenService } from '../../../Shared/token';
import { NotificatoinState } from '../../../Shared/notificatoin-state';
import { Notification } from '../../../Shared/notification';
import { Subject, takeUntil } from 'rxjs';
import { NotificationSignalrService } from '../../../Shared/signal-r';

@Component({
  selector: 'app-technician-panel',
  
  imports: [MaterialModule,CommonModule,RouterModule,RouterOutlet],
  templateUrl: './technician-panel.html',
  styleUrl: './technician-panel.scss',
})
export class TechnicianPanel implements OnInit{
  userId:string|null=null;
  menuItems:MenuItem[]=[]
  specilization:TechnicianRole|null=null;
  notification!:NotifcationDto[]
  notificationCount!:number;
  isDropdownOpen=false;
  
constructor(private service:TechnicianService,private toastr:ToastrService,
  private router:Router,private route:ActivatedRoute,private token:TokenService,private notificationState:NotificatoinState,
  private notificationService:Notification,private signalService:NotificationSignalrService
){}
 
ngOnInit(): void {
   const user=this.token.GetUser();
    if (!user || !user.userId) {
    this.router.navigate(['/login']);
    return;
  }

  this.notificationState.notification$.subscribe(data=>{
    this.notification=data;
  })

  this.notificationState.unreadnotification$.subscribe(data=>{
    this.notificationCount=data;
  })

  this.signalService.listen();


  this.userId = user.userId;
  this.specilization = user.specifications;
   this.GetDashboard();
   this.getTechnicinaData(this.userId!);
}

getTechnicinaData(userId:string){
  this.service.getTechnicianData(userId).subscribe((res)=>{
    if(res.status){
      this.specilization=res.data?.specifications as TechnicianRole
      this.menuItems=TECHNICIAN_MENU[this.specilization] ??[]     
      
    }
  })

  

}
isDark = false;

  toggleTheme() {
    this.isDark = !this.isDark;
    document.body.classList.toggle('dark-theme', this.isDark);
  }
GetDashboard(){
  this.service.TechinicianDashbaord().subscribe((res)=>{
    if(res.status){
      console.log('Dashboard-data',res.data);
      
    }
  })
}
 logout(){
  // sessionStorage.clear();
   localStorage.clear();
   this.token.clearRefreshToken();
   this.router.navigate(['/login'])
   
 }

  markAllread(){
    if(!this.userId){
      return;
    }
    this.notificationService.markAllAsRead(this.userId).subscribe((res)=>{
      if(res.status){
        this.notificationState.markAllRead();
      }
    })
  }
  @HostListener('document:click')
  closeDropdown() {
    this.isDropdownOpen = false;
  }

  get recentNotifications() {
  return this.notification.slice(0, 3);
}

  toggleNotification(event:MouseEvent){
    this.isDropdownOpen=!this.isDropdownOpen;
    event.stopPropagation();
    if(!this.userId)return;
       if (this.isDropdownOpen) {
      this.notificationService.markAllAsRead(this.userId).subscribe(() => {
        this.notificationState.markAllRead();
      });
    }
  }

   goToNotifications(){
  this.isDropdownOpen = false;
  this.router.navigate(['/notification']);
}

 openNotification(item: NotifcationDto) {

    if (!item.isRead) {
      this.notificationService.markAsRead(item.notificationId).subscribe();
    }

    
  }
}


