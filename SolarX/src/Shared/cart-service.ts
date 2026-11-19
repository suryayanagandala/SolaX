import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ApiResponse } from '../Models/Models';


@Injectable({
  providedIn: 'root',
})
export class CartService {
  
  private baseUrl=environment.apiUrl;

  constructor(private http:HttpClient){}

  getUserCart(userId:string):Observable<ApiResponse<any>>{
    const params=new HttpParams().set('userId',userId);
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/Cart/UserCart`,{params})
  }

  updateCart(payload:any):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Cart/UpdateCart`,payload);
  }

  removeCart(cartItemId:number):Observable<ApiResponse<string>>{
    return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/Cart/RemoveCart/${cartItemId}`)
  }

  cartCount(userId:string):Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(`${this.baseUrl}/Cart/CountOfCartItems/${userId}`)
  }
}
