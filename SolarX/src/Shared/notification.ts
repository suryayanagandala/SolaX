import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../Models/Models';
import { NotifcationDto } from '../Models/Models';
@Injectable({
  providedIn: 'root',
})
export class Notification {
  private baseUrl=environment.apiUrl+'/Notification/';
  constructor(private http:HttpClient){}
  
  getAllMessages(userId:string):Observable<ApiResponse<NotifcationDto[]>>{
    return this.http.get<ApiResponse<NotifcationDto[]>>(`${this.baseUrl}GetNotifications/${userId}`)
  }
  
  countOfUnreadNotifications(userId:string):Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(`${this.baseUrl}CountOfUnreadNotifications/${userId}`)
  }

  markAsRead(notificationId:number):Observable<ApiResponse<number>>{
    return this.http.patch<ApiResponse<number>>(`${this.baseUrl}MarkAsRead/${notificationId}`,null)
  }

  markAllAsRead(userId:string):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}MarkAllRead/${userId}`,null)
  }
}
