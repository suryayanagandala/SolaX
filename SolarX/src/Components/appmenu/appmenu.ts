import { Component,inject, OnInit } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { MaterialModule } from '../../MaterialModule';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../Shared/token';
import { Router } from '@angular/router';
import { publicHeader } from '../features/public-header/header';
import { Footer } from '../features/footer/footer';
import { HomeService } from '../../Shared/home-category';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-appmenu',
 
  imports: [RouterOutlet,publicHeader,MaterialModule,RouterModule,CommonModule],
  templateUrl: './appmenu.html',
  styleUrl: './appmenu.scss',
})
export class Appmenu implements OnInit{

 private service=inject(TokenService);
 private router=inject(Router);
 private homeService=inject(HomeService);
 private toastr=inject(ToastrService)
  fullName:string|null=null;
  username:string|null=null;
  userId!:string;
 sideNavOpen = false;
 permission!:any[];
 logout(){
  this.service.clear();
  this.router.navigate(['/home'])
 }

 ngOnInit(): void {
   const name=this.service.GetUsername();
   if(name){
    this.fullName=this.getShortName(name);
    this.username=this.fullName;
   }
   const userId=this.service.GetUserId();
   if(userId){
    this.userId=userId
    this.getCustomerPermission(this.userId)
   }
 }

   private getShortName(email:string):string|null{
    if(!email)return null;
     const name=email.split('@')[0];
     return name.length>6?name.substring(0,6)+'..':name
  }
  

 toggleSideNav(){
  this.sideNavOpen=!this.sideNavOpen
 }

 getCustomerPermission(userId:string){
   this.homeService.accessPermissions(userId).subscribe((res)=>{
    if(res.status){
      this.permission=res.data

      const customerCheck=this.permission[0].roleId;
      if(customerCheck!=3){
        this.toastr.error('something went wrong')
      }
    }
   })
 }

}
