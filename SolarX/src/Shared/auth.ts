import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { ApiResponse, RefreshTokenRequest, RegisterationCreateDto, ResetPasswordCreateDto, TechnicianCreateDto } from '../Models/Models';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) { }
  login(payload: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/User/LoginRegistration`, payload)
  }

  regeister(data: RegisterationCreateDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/User/RegisterUser`, data);
  }

  sendOtp(email: string):Observable<any> {
    return this.http.post<any>(
      `${this.baseUrl}/User/SentOtpEmail`,
      { email }, 
      { headers: { 'Content-Type': 'application/json' } }
    );
  }

  verifyOtp(payload:{email:string;otpText:string}):Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/User/VerfiyOtp`,payload)
  }

  resetPassword(data:ResetPasswordCreateDto):Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/User/ResetPassword`,data);
  }

  getStats():Observable<any>{
    return this.http.get(`${this.baseUrl}/Auth/GetAllstats`)
  }

  GetAllUsers():Observable<any>{
    return this.http.get<any>(`${this.baseUrl}/Auth/GetAllCustomer`);
  }
  
  AssignedTechnician(data:TechnicianCreateDto):Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/Auth/AssignedTechnican`,data);
  }


   create(data: FormData) {
    return this.http.post(this.baseUrl, data);
  }

  logout(): void {
    localStorage.clear();
  }

  getNotification(userId:string):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Notification/GetNotifications/${userId}`)
  }

  getProductionData():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Auth/GetProductionData`)
  }

  getProductionTrend():Observable<ApiResponse<object>>{
    return this.http.get<ApiResponse<object>>(`${this.baseUrl}/Auth/GetProductionTrend`)
  }

  getProductionUpdate():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Auth/ProductionUpdate`)
  }

  getSoldProductsData():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Auth/SoldProducts`)
  }

  updateTechnicianProfile(userId:string,dto:any):Observable<ApiResponse<any>>{
    return this.http.put<ApiResponse<any>>(`${this.baseUrl}/Auth/UpdateTechnicianProfile/${userId}`,dto)
  }


  getTechnicianData(userId:string):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Auth/TechnicianData/${userId}`)
  }

  verifyRefreshToken(payload:RefreshTokenRequest):Observable<ApiResponse<any>>{
    return this.http.post<ApiResponse<any>>(`${this.baseUrl}/User/VerifyRefreshToken`,payload);
  }

  getProductionUpdateWithAI():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Auth/GetProductionUpdatesWithAi`)
  }

}
