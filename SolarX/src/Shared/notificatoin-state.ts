import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { NotifcationDto } from '../Models/Models';
import { ToastrService } from 'ngx-toastr';
import { Notification } from './notification';
@Injectable({
  providedIn: 'root',
})
export class NotificatoinState {
  constructor(private tastr:ToastrService,notificationService:Notification){}
  private notificationSubject=new BehaviorSubject<NotifcationDto[]>([]);
  notification$=this.notificationSubject.asObservable();
  
  private unreadCountSubject=new BehaviorSubject<number>(0);
  unreadnotification$=this.unreadCountSubject.asObservable();

  setNotification(list:NotifcationDto[]){
    this.notificationSubject.next(list);
    this.unreadCountSubject.next(list.filter(x=>!x.isRead).length);
  }
  
  addNotification(dto:NotifcationDto){
    const upadted=[dto,...this.notificationSubject.value];
    this.notificationSubject.next(upadted);
    this.unreadCountSubject.next(this.unreadCountSubject.value+1);
  }

  markAllRead(){
    const updated=this.notificationSubject.value.map(x=>({
      ...x,isRead:true
    }));

    this.notificationSubject.next(updated);
    this.unreadCountSubject.next(0);
  }

  loadNotifications(userId:string,service:any){
  service.getAllMessages(userId).subscribe((res:any)=>{
    if(res.status){
      this.setNotification(res.data);
    }
  });
}
}
