import { Component,Output,EventEmitter,Input, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../../Shared/token';
import { Router, RouterModule } from '@angular/router';
import { MatsidenavService } from '../../../Shared/matsidenav';
import { NotificationSignalrService } from '../../../Shared/signal-r';
import { NotifcationDto } from '../../../Models/Models';
import { Notification } from '../../../Shared/notification';
import { CartService } from '../../../Shared/cart-service';
import { NotificatoinState } from '../../../Shared/notificatoin-state';
import { SharedThemeService } from '../../../Shared/theme-service';

@Component({
  selector: 'app-header',
 
  imports: [MaterialModule,CommonModule,RouterModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class publicHeader implements OnInit{
  username:string|null=null;
  fullName:string|null=null;
  notification!:NotifcationDto[]
  notificationCount!:number;
  isDropdownOpen = false;
  userId!:string;
  cartItemCount!:number;

   @Output() menuClick = new EventEmitter<void>();

  constructor(private service:TokenService,private router:Router,
  private signalService:NotificationSignalrService,private notificationState:NotificatoinState,
  private cartService:CartService,private notificationService:Notification,private cd:ChangeDetectorRef,
  public themeService:SharedThemeService){
    this.fullName=this.service.GetUsername();
    if(this.fullName){
      this.username=this.getShortName(this.fullName)
    }
    
  }

  ngOnInit(): void {
    
    this.notificationState.notification$.subscribe(data=>{
      this.notification=data;
      console.log(data);
      console.log(this.notification)
      this.cd.detectChanges();

    })

    this.notificationState.unreadnotification$.subscribe(data=>{
      this.notificationCount=data;
      this.cd.detectChanges();
    })

    this.signalService.cartCount$.subscribe(count=>{
      this.cartItemCount=count;
      this.cd.detectChanges();
    })

    const userId=this.service.GetUserId();
    if(userId){
      this.userId=userId;
      this.cartCount(userId)
    }


    
  }

  private getShortName(email:string):string|null{
    if(!email)return null;
     const name=email.split('@')[0];
     return name.length>6?name.substring(0,6)+'..':name
  }

  isLoggedIn():boolean{
    return !!this.service.getToken();
  }

  logout():void{
    this.service.clear();
    this.service.clearRefreshToken();
    this.router.navigate(['/home'])
  }

  goToLogin(){
    this.router.navigate(['/login']);
  }
  goToRegister(){
    this.router.navigate(['/register'])
  }
 
  toggleMenu(){

    this.menuClick.emit();
  }

  
  toggleNotification(event: MouseEvent) {

  this.isDropdownOpen = !this.isDropdownOpen;
  event.stopPropagation();

  if (this.isDropdownOpen) {

    this.notificationService.markAllAsRead(this.userId).subscribe(() => {

      this.notificationService.getAllMessages(this.userId)
      .subscribe(res => {
        if (res.status) {
          this.notificationState.markAllRead();
        }
      });

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
  

  cartCount(userId:string){
    this.cartService.cartCount(userId).subscribe((res)=>{
      if(res.status){
        this.signalService.cartCount$.next(res.data!);
      }
    })
  }

  toggleTheme() {
  const current = this.themeService.isDarkModeValue; 
  const newValue = !current;

  this.themeService.setDarkMode(newValue); 

  //element.classList.toggle(className, condition)
  document.body.classList.toggle('dark-theme', newValue);
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
