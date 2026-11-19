import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from '@angular/core';
import { TokenService } from "../../Shared/token";
import { catchError, throwError, switchMap, finalize } from "rxjs";
import { Router } from "@angular/router";
import { Auth } from "../../Shared/auth";
import { LoaderService } from "../../Shared/loaderService";

export const authInterceptor: HttpInterceptorFn = (req, next) => {

 const tokenService = inject(TokenService);
 const authService = inject(Auth);
 const router = inject(Router);
 const loader=inject(LoaderService);
 const token = tokenService.getToken();

 
 if (token) {
  req = req.clone({
   setHeaders: { Authorization: `Bearer ${token}` }
  });
 }

 return next(req).pipe(

  catchError(error => {

   // Access token expired
   if (error.status === 401) {

    const refreshToken = tokenService.GetRefreshToken()?.toString();

    if (!refreshToken) {
     tokenService.clear();
     router.navigate(['/login']);
     return throwError(() => error);
    }

    // Call refresh token API
    return authService.verifyRefreshToken( {refreshToken} ).pipe(
   
     //You cannot just return another HTTP call inside another HTTP call.
     switchMap((res: any) => {

      const newAccessToken = res.data.accessToken;
      tokenService.SaveToken(newAccessToken);

     
      const newRequest = req.clone({
       setHeaders: { Authorization: `Bearer ${newAccessToken}` }
      });

      return next(newRequest);

     }),

     catchError(err => {

      
      tokenService.clear();
      router.navigate(['/login'], {
       queryParams: { sessionExpired: true }
      });

      return throwError(() => err);

     })
    );
   }

   return throwError(() => error);
  })
 );
};