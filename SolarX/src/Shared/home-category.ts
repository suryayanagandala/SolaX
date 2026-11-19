import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../environments/environment';
import { ApiResponse, CategoryDto } from '../Models/Models';
import { Observable } from 'rxjs';
import { CartDto } from '../Models/Models';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  private baseurl=environment.apiUrl;
  constructor(private http:HttpClient){}

  getHomeCategories():Observable<ApiResponse<CategoryDto[]>>{
    return this.http.get<ApiResponse<CategoryDto[]>>(`${this.baseurl}/Home/GetHomeCategories`)
  }

  getCategoriesBySlug(slug:string):Observable<ApiResponse<any>>{
     const params=new HttpParams().set('slug',slug);
     return this.http.get<ApiResponse<any>>(`${this.baseurl}/Home/GetProductsBySlug`,{params})
  }

  getHomeContent():Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Home/GetHomePackage`)
  }

  viewProductDetails(proudctId:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Home/GetProductDetails/${proudctId}`)
  }
  
  addToCart(data:CartDto):Observable<ApiResponse<string>>{
    return this.http.post<ApiResponse<string>>(`${this.baseurl}/Cart/AddToCart`,data);
  }

  productAnalytics(productId:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Home/GetProductAnalytics/${productId}`)
  }

  getRecommendationDetails(productId:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Accessory/GetRecommendations?productId=${productId}`)
  }

  getWhyThisExplainations(productId:number,recommendedProductId:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Accessory/Why-this?mainProductId=${productId}&recommendedProductId=${recommendedProductId}`)
  }
  
  compareTwoProducts(productId1:number,productId2:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Accessory/ComapareProducts?product1Id=${productId1}&product2Id=${productId2}`)
  }
  
  comboProducts(productId:number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Accessory/GetBestComboProducts?productId=${productId}`)
  }

  accessPermissions(userId:string):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(`${this.baseurl}/Roles/AccessPermissions/${userId}`)
  }
   
}
