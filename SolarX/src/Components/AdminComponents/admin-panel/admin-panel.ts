import { Component, HostListener, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TokenService } from '../../../Shared/token';
import { Notification } from '../../../Shared/notification';
import { NotificatoinState } from '../../../Shared/notificatoin-state';
import {  NotifcationDto } from '../../../Models/Models';
import { CommonModule, DatePipe } from '@angular/common';
import { NotificationSignalrService } from '../../../Shared/signal-r';


@Component({
  selector: 'app-admin-panel',
  imports: [MaterialModule,RouterOutlet,RouterModule,DatePipe,CommonModule],
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.scss',
})
export class AdminPanel implements OnInit{
  notification!:NotifcationDto[]
  notificationCount!:number;
  isDropdownOpen = false;
  userId!:string;
  aiAlerts!:any[];
  constructor(private router:Router,private toastr:ToastrService,private service:TokenService,
    private notificationService:Notification,private state:NotificatoinState,private signalService:NotificationSignalrService
  ){}
  ngOnInit(): void {
    const userId=this.service.GetUserId();
    if(userId){
      this.userId=userId
    }
     this.state.notification$.subscribe(data=>{
    this.notification=[...data];
  })
     this.state.unreadnotification$.subscribe(data=>{
      this.notificationCount=data;
     })

     this.alertMessages();  
  }

  
  isDark = false;

  toggleTheme(){
    this.isDark = !this.isDark;

    //classList internally works as add and remove classes if condition is true dark-theme is works
    document.body.classList.toggle('dark-theme', this.isDark);
  }
  logout(){
    this.service.clear();
    this.service.clearRefreshToken();
    this.router.navigate(['/login']);
    this.toastr.success('logout successfully!');
  }

   toggleNotification(event:MouseEvent){
    this.isDropdownOpen=!this.isDropdownOpen;
    event.stopPropagation();
       if (this.isDropdownOpen) {
      this.notificationService.markAllAsRead(this.userId).subscribe(() => {
        this.state.markAllRead();
      });
    }
  }

  @HostListener('document:click')
  closeDropdown() {
    this.isDropdownOpen = false;
  }
    get recentNotifications() {
  return this.notification.slice(0, 3);
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

  alertMessages(){
   this.signalService.onAIAlert(data=>{
    this.aiAlerts=data;
   })
  }

  
}
