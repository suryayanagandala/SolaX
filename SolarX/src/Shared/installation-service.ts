import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../Models/Models';

@Injectable({
  providedIn: 'root',
})
export class InstallationService {
  private baseUrl=environment.apiUrl;
  constructor(private http:HttpClient){}

  getInstallationStatus():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Installation/GetInstallationStatus`)
  }

  updateStatus(installationId:string):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Installation/InstallationStatus/${installationId}`,null)
  }

  uploadReport(payload:FormData):Observable<ApiResponse<string>>{
    return this.http.post<ApiResponse<string>>(`${this.baseUrl}/Installation/InstallationReport`,payload)
  }
}
