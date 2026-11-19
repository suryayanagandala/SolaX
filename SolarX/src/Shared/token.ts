import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private readonly token_key='auth';
  private readonly refresh_token='refreshToken'
  SaveToken(token:string){
    localStorage.setItem(this.token_key,token);
  }
  getToken():string | null{
    return localStorage.getItem(this.token_key);
  }

  SaveRefreshToken(refreshToken:string){
    localStorage.setItem(this.refresh_token,refreshToken)
  }

  GetRefreshToken():string|null{
   return localStorage.getItem(this.refresh_token)
  }

  clearRefreshToken(){
    localStorage.removeItem(this.refresh_token);
  }

  clear():void{
    localStorage.removeItem(this.token_key);
  }
  
  isLoggedIn(): boolean {
    return !!this.getToken();  //strict boolean 
  }
  GetDecodedToken(): any {
    const token = this.getToken();
    if (!token) return null;
    return JSON.parse(atob(token.split('.')[1]));
  }

  GetUserId(): string | null {
    return this.GetDecodedToken()?.UserId ?? null;
  }

  GetRole(): string | null {
    return this.GetDecodedToken()?.[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
    ] ?? null;
  }

   GetUsername(): string | null {
    return this.GetDecodedToken()?.sub ?? null;  // 'sub' usually contains email/username
  }

  SaveUser(user: any) {
  localStorage.setItem('user', JSON.stringify(user));
}

GetUser() {
  return JSON.parse(localStorage.getItem('user') || '{}');
}
}
