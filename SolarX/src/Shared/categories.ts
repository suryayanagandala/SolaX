import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, CategoryDto } from '../Models/Models';
import { environment } from '../environments/environment';
import { CategoryForm } from '../Models/Models';

@Injectable({
  providedIn: 'root',
})
export class CategorieService {
  private baseUrl=environment.apiUrl;
  constructor(private http:HttpClient){}
  
  getAllCategories():Observable<ApiResponse<CategoryDto[]>>{
    return this.http.get<ApiResponse<CategoryDto[]>>(`${this.baseUrl}/Category/GetAllCategories`)
  }


 createCategory(data: CategoryForm): Observable<ApiResponse<CategoryForm>> {
  const formData = new FormData();

  formData.append('name', data.name);
  formData.append('description', data.description || '');
  formData.append('parentId', data.parentId ? data.parentId.toString() : '');

  if (data.image) {
    formData.append('image', data.image); 
  }

  return this.http.post<ApiResponse<CategoryForm>>(
    `${this.baseUrl}/Category/UploadCategory`,
    formData
  );
}

  getCategoryById(id:number):Observable<ApiResponse<CategoryDto>>{
     return this.http.get<ApiResponse<CategoryDto>>(`${this.baseUrl}/Category/GetCategoryListById/${id}`)
  }
  updateCategory(id:number,data:CategoryForm):Observable<ApiResponse<CategoryForm>>{
    return this.http.patch<ApiResponse<CategoryForm>>(`${this.baseUrl}/Category/UpdateCategory/${id}`,data)
  }

  deleteCategory(dto:{categoryId:number;reason:string}):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Category/DeleteCategory`,dto)
  }

  restoreCategory(categoryId:number):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Category/RestoreCategory/${categoryId}`,{})
  }

  updateParentCategory(categoryId: number, data: CategoryForm): Observable<ApiResponse<string>> {
  const formData = new FormData();

  formData.append('name', data.name);
  formData.append('description', data.description || '');
  formData.append('parentId', data.parentId ? data.parentId.toString() : '');

  if (data.image) {
    formData.append('image', data.image);
  }

  return this.http.patch<ApiResponse<string>>(
  `${this.baseUrl}/Category/ParentCategory/${categoryId}`, 
  formData
);
}

  deleteParentCategory(payload:{categoryId:number,reason:string}):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Category/DeleteParentCategory`,payload)
  }

  restoreParentCategory(categoryId:number):Observable<ApiResponse<string>>{
    return this.http.patch<ApiResponse<string>>(`${this.baseUrl}/Category/RestoreParentCategory/${categoryId}`,{})
  }
  

}
