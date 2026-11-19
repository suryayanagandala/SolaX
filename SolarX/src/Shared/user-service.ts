import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../Models/Models';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private  baseUrl=environment.apiUrl;
  constructor(private http:HttpClient){}
  getUserDetails(userId:string):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/User/GetUserById?id=${userId}`)
  }
  updateProfile(userId: string, dto: any) {
  return this.http.patch<any>(
    `${this.baseUrl}/User/UpdateProfile?userId=${userId}`,
    dto
  );
}
}
