import { CanActivateFn,Router } from "@angular/router";
import { inject } from "@angular/core";
import { TokenService } from "../../Shared/token";
import { ToastrService } from "ngx-toastr";

export const authGuard:CanActivateFn=()=>{
    const token=inject(TokenService);
    const router=inject(Router);
    const toastr=inject(ToastrService)

    if(!token.isLoggedIn()){
        toastr.warning('please login first');
        router.navigate(['/login']);
        return false;
    }

    const role=token.GetRole();
    if(role!=='Admin'){
      router.navigate(['/home']);
      return false;
    }
    return true;
}