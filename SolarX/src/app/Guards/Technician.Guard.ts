import { TokenService } from "../../Shared/token";
import { CanActivateFn,Router } from "@angular/router";
import { inject } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { Role } from "../../Models/Models";
import { localMessages } from "../../localMessages";

export const technicianGuard:CanActivateFn=()=>{
  const toastr=inject(ToastrService);
  const token=inject(TokenService);
  const router=inject(Router);
  const messages=localMessages;

  if(!token.isLoggedIn()){
    // toastr.warning(messages.warnings.login);
    return false;
  }
  const userRole=token.GetRole()?.trim();

  if(userRole!==Role.Technician){
    router.navigate(['/home']);
    return false;
  }
  return true;
  
}