import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../Models/Models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TechnicianService {
  private baserUrl=environment.apiUrl;
  constructor(private http:HttpClient){}

  TechinicianDashbaord():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Technician/Dashboard`)
  }

  getTechnicianData(userId:string):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Technician/GetTechnician/${userId}`)
  }

  GetDeliveries():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Delivery/GetOutOfStock`)
  }

  markAsDelivered(deliveryId:number):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baserUrl}/Delivery/MarkAsCompletedDelivery/${deliveryId}`,null)
  }

  markAsDeliveryStataus(deliveryId:number):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baserUrl}/Delivery/MarkAsOutOfDelivery/${deliveryId}`,null)
  }

  getDeliveredOrders():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Delivery/DeliveredOrders`)
  }

  getWorkStats():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Technician/GetTechnicianWorkStats`)
  }

  getMonthlyUpdates():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Technician/GetMonthlyUpdates`)
  }

  getEmployeeRevenue():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Technician/GetRevenue`)
  }

  getEmployeeEfficinecy():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baserUrl}/Technician/GetEfficinecy`)
  }
}
