import { Component, OnInit } from '@angular/core';
import { HomeService } from '../../../Shared/home-category';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterModule } from '@angular/router';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { CategoryDto } from '../../../Models/Models';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-categories',
  
  imports: [MaterialModule,CommonModule,RouterModule],
  templateUrl: './categories.html',
  styleUrl: './categories.scss',
})
export class Categories implements OnInit{
  loading=true;

  categoryImages:Record<string,string>={
    'commercial':'Assets/Themes/Commercial-Image.jpg',
    'residential-solar-panels':'Assets/Themes/Residential-Image.jpg',
    'industrial-solar-panels':'Assets/Themes/Industrail-Image.jpg',
    'portable-solar-panels':'Assets/Themes/Potable-Image.jpg',
    'batteries':'Assets/Themes/Solar-Batteries.jpg',
    'inverters':'Assets/Themes/Solar-Inverters.jpg',
    'accessories':'Assets/Themes/Solar-Accessories.jpg'
  }

  categories:CategoryDto[]=[];
  
   constructor(private toastr:ToastrService,private service:HomeService,
    private router:Router
   ){}

   ngOnInit(): void {
     this.loadHomeCategories();
   }

   loadHomeCategories(){
    this.service.getHomeCategories().subscribe((res)=>{
      if(res.status){
        this.categories=res.data??[];
        
        console.log('Parent Categories',res.data);
        this.loading=false;
      }
      else{
        console.log(res.error);
        this.loading=true;
        this.toastr.error('Something went wrong for getting Categories')
      }
    })
   }

   

   getCategoryImage(slug: string) {
  return this.categoryImages[slug] ?? 'Assets/Themes/Banner Image.png';
}
   getImageUrl(url?: string): string {
         if (!url) {
           return 'Assets/Themes/panelimage1.png';
         }
     
         if (url.startsWith('http')) {
           return url;
         }
     
         return environment.imageUrl + url;
       }
}
