import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { ApiResponse, ProductImage } from '../Models/Models';
import { Product } from '../Models/Models';


@Injectable({
  providedIn: 'root',
})
export class ProductService {
  
  private baseUrl=`${environment.apiUrl}`;

  constructor(private http:HttpClient){}

 getAllProducts():Observable<ApiResponse<Product[]>>{
  return this.http.get<ApiResponse<Product[]>>(`${this.baseUrl}/Auth/GetAllProducts`)
 }

  createProduct(product:Product):Observable<ApiResponse<Product>>{
    return this.http.post<ApiResponse<Product>>(`${this.baseUrl}/Auth/AddProducts`,product);
  }

  updateProduct(productId:number,product:Product):Observable<ApiResponse<string>>{
    return this.http.put<ApiResponse<string>>(`${this.baseUrl}/Auth/updateProducts/${productId}`,product);
  }

  deleteProductById(productId:number):Observable<ApiResponse<Product>>{
    return this.http.delete<ApiResponse<Product>>(`${this.baseUrl}/Auth/DeleteProduct/${productId}`)
  }

  softDelete(dto:{productId:number,reason:string}):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Auth/soft-delete`,dto)
  }

  restoreProduct(productId:number):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Auth/restore/${productId}`,{})
  }

  deleteProduct(imageId:number):Observable<ApiResponse<string>>{
    return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/Auth/DeleteImage/${imageId}`)
  }

  getProductDetalById(productId:number):Observable<ApiResponse<Product>>{
    return this.http.get<ApiResponse<Product>>(`${this.baseUrl}/Auth/GetProductDetails/${productId}`)
  }

  getProductImagesById(productId:number):Observable<ApiResponse<ProductImage[]>>{
    return this.http.get<ApiResponse<ProductImage[]>>(`${this.baseUrl}/Auth/GetImagesByProductId/${productId}`)
  }

  deleteImage(imageId:number):Observable<ApiResponse<string>>{
    return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/Auth/DeleteImage/${imageId}`)
  }

  addProductImages(productId:number,formData:FormData):Observable<ApiResponse<string>>{
  return this.http.post<ApiResponse<string>>(`${this.baseUrl}/Auth/${productId}/images`,formData)
  }

  updateProductImage(imageId:number,formData:FormData):Observable<ApiResponse<string>>{
    return this.http.put<ApiResponse<string>>(`${this.baseUrl}/Auth/UpdateImages/${imageId}`,formData)
  }
}
