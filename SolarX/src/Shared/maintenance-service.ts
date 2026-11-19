import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../Models/Models';

@Injectable({
  providedIn: 'root',
})
export class MaintenanceService {
  private baseUrl=environment.apiUrl+'/Maintenance'
  constructor(private http:HttpClient){}
  MaintenanceDataForCustomerPage(userId:string):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/GetMaintenaceView/${userId}`)
  }

  raiseRequest(payload:any):Observable<ApiResponse<any>>{
    return this.http.post<ApiResponse<any>>(`${this.baseUrl}/RaiseRequest`,payload)
  }

  technicianMaintenanceJobs(userId:string):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/GetMaintenances/${userId}`);
  }

  uploadReport(payload:FormData):Observable<ApiResponse<string>>{
    return this.http.post<ApiResponse<string>>(`${this.baseUrl}/MaintenanceReport`,payload)
  }
  
  getCompletedMaintenances():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/GetCompletedMaintenanaces`)
  }

}
