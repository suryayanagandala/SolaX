import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse } from '../Models/Models';
import type { SolarSizing } from '../Models/Models';
import type { SolarSizingResponse } from '../Models/Models';
@Injectable({
  providedIn: 'root',
})
export class SolarPerformanceService {
  private baseUrl=environment.apiUrl+'/SolarPerfomance';
  constructor(private http:HttpClient){}

  getPerformance(panelId?:number):Observable<ApiResponse<any>>{
    let params:any={};
    if(panelId){
      params.panelId=panelId
    }
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/GetPerformance`,{params})

  }

  getMonthlyUpdates(panelId?:number):Observable<ApiResponse<any>>{
    let params:any={};
    if(panelId){
      params.panelId=panelId
    }
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/GetMonthlyUpdates`,{params})
  }

  solarSummaryDashboard(panelId?:number):Observable<ApiResponse<any>>{
    let params:any={};
    if(panelId){
      params.panelId=panelId
    }
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/GetDashboardSummary`,{params})
  }

  getActivatedSolarPanles():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/GetActivatedSolarPanles`)
  }

  getPanelDetails(panelId:number):Observable<ApiResponse<object>>{
    return this.http.get<ApiResponse<object>>(`${this.baseUrl}/GetPanelDetails/${panelId}`)
  }

  getWeatherForecast():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`https://localhost:44362/api/Accessory/PredictionRequests`)
  }
  
  getDailyWeatherInput():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`https://localhost:44362/api/Accessory/GetDailyPredictions`)
  }

  calculateSolarSizing(payload:SolarSizing):Observable<ApiResponse<SolarSizingResponse>>{
    return this.http.post<ApiResponse<SolarSizingResponse>>(`https://localhost:44362/api/Accessory/CalculateSizing`,payload)
  }
  
  

}
