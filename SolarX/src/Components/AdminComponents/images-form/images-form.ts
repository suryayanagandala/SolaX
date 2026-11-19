import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../../Shared/product';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-images-form',
  imports: [MaterialModule,CommonModule,RouterModule],
  templateUrl: './images-form.html',
  styleUrl: './images-form.scss',
})
export class ImagesForm implements OnInit{
  selectedFiles:File[]=[];
  productId!: number;         
  imageId!: number;            
  isEditMode = false;
 
  constructor(private service:ProductService,private toastr:ToastrService,
    private route:ActivatedRoute,private router:Router
  ){}


  ngOnInit(): void {
     this.route.queryParams.subscribe(params=>{
      if(params['productId']){
        this.productId=Number(params['productId']);
       
      }

      if(params['imageId']){
        this.imageId=Number(params['imageId']);
        this.isEditMode=true;
      }
     })
  }

  onFileSelected(event:any){
     const files: FileList = event.target.files;
  this.selectedFiles = [];
  for (let i = 0; i < files.length; i++) {
    this.selectedFiles.push(files[i]);
  }
  console.log(this.selectedFiles);
  }

  submit(){
    if(!this.selectedFiles.length){
      this.toastr.warning('Please select image');
      return;
    }
    if(this.isEditMode){
       const formData=new FormData();
       formData.append('file',this.selectedFiles[0])

       this.service.updateProductImage(this.imageId,formData).subscribe((res)=>{
        if(res.status){
          console.log(res.data);
          this.toastr.success('successfully update image');
          setTimeout(() => this.gotoBack(this.productId), 800);
        }
        else{
          console.log(res.error);
          this.toastr.error('failed to update image')
        }
       })
    }

    else{
      const formData=new FormData();
      this.selectedFiles.forEach(file=>formData.append('images',file));

      this.service.addProductImages(this.productId,formData).subscribe((res)=>{
        if(res.status){
          console.log(res.data);
          this.toastr.success('successfully add images')
          setTimeout(() => this.gotoBack(this.productId), 800);
        } 
        else{
          console.log(res.error);
          this.toastr.error('failed to add images');
        }
      })
    }
  }

gotoBack(productId: number) {
  console.log('Navigating to view-images with productId:', productId);
  this.router.navigate(['/admin/viewImages'], { queryParams: { productId } })
    .then(success => console.log('Navigation success:', success))
    .catch(err => console.error('Navigation error:', err));
}



}
