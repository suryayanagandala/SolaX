import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { ApiResponse } from '../Models/Models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private baseUrl=environment.apiUrl;

  constructor(private http:HttpClient){}

  genearteQrCode(orderId:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Payment/GetQrCode/${orderId}`)
  }

  ProcessPayment(data:any):Observable<ApiResponse<string>>{
    return this.http.post<ApiResponse<string>>(`${this.baseUrl}/Payment/PaymentGateway`,data)
  }

  getInvoice(orderId:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<string>>(`${this.baseUrl}/Invoice/GetInvoice/${orderId}`)
  }

  invoicePdf(orderId:number):Observable<ApiResponse<string>>{
    return this.http.get<ApiResponse<string>>(`${this.baseUrl}/Invoice/invoice/${orderId}`)
  }
}
