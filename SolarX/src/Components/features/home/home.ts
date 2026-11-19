import { Component, OnInit } from '@angular/core';
import { Banner } from '../banner/banner';
import { ProductsHome } from '../products/products';
import { Categories } from '../categories/categories';
import { Footer } from '../footer/footer';
import { SolarXBanner } from '../solar-xbanner/solar-xbanner';
import { ServicesBanner } from '../services/services';
import { TokenService } from '../../../Shared/token';
import { Router } from '@angular/router';
import { Role } from '../../../Models/Models';

@Component({
  selector: 'app-home',
  imports: [Banner,Categories,Footer,SolarXBanner,ServicesBanner],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit{
  private compoent!:string;

  constructor(private router:Router,private token:TokenService){}

 ngOnInit(): void {
   const role=this.token.GetRole();
   if(role===Role.User){
    this.router.navigate(['/home'])
   }
   else if(role===Role.Admin){
     this.router.navigate(['/admin'])
   }
   else{
     this.router.navigate(['/panel'])
   }
 }
}
