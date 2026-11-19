import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { CartDto, OrderCreateDto, TrackDelivery } from '../Models/Models';
import { ApiResponse } from '../Models/Models';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
 private baseUrl=environment.apiUrl;
 
 constructor(private http:HttpClient){}

 placeOrder(payload:{userId:string,deliveryAddress:string}):Observable<ApiResponse<string>>{
  return this.http.post<ApiResponse<string>>(`${this.baseUrl}/Order/PlaceOrder`,payload)
 }

 getUserDetails(userId:string):Observable<ApiResponse<any>>{
  return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Order/UserDetails/${userId}`);
 }


 ceratePendingOrder(payload:OrderCreateDto):Observable<ApiResponse<any>>{
  return this.http.post<ApiResponse<any>>(`${this.baseUrl}/Order/PlaceOrder`,payload)
 }

 orderHistory():Observable<ApiResponse<any>>{
  return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Order/OrderHistory`)
 }

 cancelOrder(orderId:number):Observable<ApiResponse<string>>{
   return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Order/CancelOrder/${orderId}`,null)
 }

 trackOrder(orderId:number):Observable<ApiResponse<TrackDelivery[]>>{
  return this.http.get<ApiResponse<TrackDelivery[]>>(`${this.baseUrl}/Order/TrackDelivery/${orderId}`)
 }
}
