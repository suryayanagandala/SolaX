import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule, DatePipe } from '@angular/common';
import { ProductService } from '../../../Shared/product';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-view-images',
  imports: [MaterialModule,CommonModule,RouterModule],
  templateUrl: './view-images.html',
  styleUrl: './view-images.scss',
})
export class ViewImages implements OnInit{
  
  productId!:number;
  images:any[]=[];
  loading=false;

  constructor(private service:ProductService,private toastr:ToastrService,
    private route:ActivatedRoute,private router:Router
  ){}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params=>{
      if(params['productId']){
        this.productId=Number(params['productId'])
        this.loadImages()
      }
    })

  }

  loadImages(){
  this.loading = true;

  this.service.getProductImagesById(this.productId).subscribe((res)=>{
    if(res.status){
      this.images = res.data ?? [];
    } else {
      this.images = [];
    }

    this.loading = false;
  })
}

 

getImageUrl(url: string): string {
  if (url.startsWith('http')) {
    return url;
  }
  return environment.imageUrl + url;
}

deleteProduct(id:number){
  if (!confirm('Are you sure you want to delete this image?')) return;
  this.service.deleteImage(id).subscribe((res)=>{
    if(res.status){
     this.images=this.images.filter(x=>x.imageId!==id);
     this.toastr.success(`image with ${id} id sucessfully removed`);
    }
    else{
      console.log(res.error);
      this.toastr.error(res.error)
      
    }
  })
}
 gotoProducts(){
  this.router.navigateByUrl('/admin/product-details')
 }
 


}
