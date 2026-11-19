import { TokenService } from "../../Shared/token";
import { ToastrService } from "ngx-toastr";
import { CanActivateFn, Router } from "@angular/router";
import { inject } from "@angular/core";

export const loginGaurd:CanActivateFn=()=>{
    const token=inject(TokenService);
    const toastr=inject(ToastrService);
    const router=inject(Router);

    if(!token.isLoggedIn()){
        toastr.warning('Please login first');
        router.navigate(['/login']);
        return false;
    }
    return true;
}