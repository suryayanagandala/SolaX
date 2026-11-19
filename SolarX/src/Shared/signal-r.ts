import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { NotifcationDto } from '../Models/Models';
import { inject } from '@angular/core';
import { NotificatoinState } from './notificatoin-state';
import { ToastrService } from 'ngx-toastr';
import { NgZone } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationSignalrService {
  constructor(private toastr:ToastrService,private zone: NgZone){}

  private hubConnection!:signalR.HubConnection;

   notification$=new BehaviorSubject<NotifcationDto|null>(null);
  
  notificationCount$=new BehaviorSubject<number>(0);

  cartCount$=new BehaviorSubject<number>(0);

  private notificationState=inject(NotificatoinState);

 startConnection(token: string): Promise<void> {

  this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:44362/notifications', {
      accessTokenFactory: () => token
    })
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

  return this.hubConnection.start()
    .then(() => {
      console.log('SignalR connected successfully');

      this.listen();
      this.listenToCartCount();
    })
    .catch(err => {
      console.log('SignalR connection error', err);
      throw err;
    });
}
  listen(){
    this.hubConnection.on('NewNotification', (data) => {

  this.zone.run(() => {

    console.log('NewNotification', data);

    this.notificationState.addNotification(data);

    this.toastr.info(
      data.message,
      data.title || 'New Notification',
      {
        timeOut: 4000,
        positionClass: 'toast-bottom-right',
        closeButton: true,
        progressBar: true
      }
    );

  });

});
  }

  listenToCartCount(){

  this.hubConnection.off('CarItemsCount'); 

  this.hubConnection.on('CarItemsCount',(data)=>{
    console.log('cartdata',data);

    
    this.cartCount$.next(data.cartCount);
  });
}

  onReceivePerformance(callback:(data:any)=>void){
    this.hubConnection.on('SolarPerformance',callback)
    
    
  }

  onAIAlert(callback: (data: any) => void) {
    this.hubConnection.on('ReceiveAIAlert', (data) => {
      callback(data);
    });


    this.toastr.warning(
      'Something needs attention!',
         'Warning',
      {
        timeOut: 4000,
        positionClass: 'toast-bottom-right',
        closeButton: true,
        progressBar: true
      }
    );


  }

  stopConnection(){
    if(this.hubConnection){
      this.hubConnection.stop().then(()=>console.log('signalR is stopped')
      ).catch((err)=>console.log('Signalr is not stopped',err)
      
      )
    }
  }
}
