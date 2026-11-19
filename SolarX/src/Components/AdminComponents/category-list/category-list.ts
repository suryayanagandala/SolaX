import { Component,OnInit } from '@angular/core';
import { CategorieService } from '../../../Shared/categories';
import { CategoryDto } from '../../../Models/Models';
import { MaterialModule } from '../../../MaterialModule';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'
import { MatTooltipModule } from '@angular/material/tooltip';
import { ReactiveFormDalogue } from '../reactive-form-dalogue/reactive-form-dalogue';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-category-list',
  imports: [CommonModule,MaterialModule,MatProgressSpinnerModule,RouterModule,MatTooltipModule],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss',
})
export class CategoryList implements OnInit{

  loading=true;
  categories:CategoryDto[]=[];

  constructor(private service:CategorieService,private toastr:ToastrService,
    private router:Router,private dialog:MatDialog
  ){}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(){
    this.loading = true;
    this.service.getAllCategories().subscribe((res)=>{
      if(res.status){
        this.loading=false;
        this.categories=res.data??[]
        console.log(res.data);
        
      
        // this.toastr.info('Category List')
      }
      else{
        console.log(res.error);
        this.loading=false; 
      }
    })
  }

  deleteCategory(categoryId:number){
   
    const dialogRef=this.dialog.open(ReactiveFormDalogue,{
      width:'400px'
    });

    dialogRef.afterClosed().subscribe(reason=>{
      if(!reason){
        return;
      }
      else{
        const dto={
          categoryId:categoryId,
          reason:reason.reason
        }

        this.service.deleteCategory(dto).subscribe((res)=>{
          if(res.status){
            this.loadCategories();
            this.toastr.success('successfully deactivated');
            this.loadCategories()
          }
          else{
            console.log(res.error);
            this.toastr.error('something went wrong')
          }
        })
      }

    })
  }
  editCategory(categoryId:number){
    this.router.navigate(['/admin/category-form'],{
      queryParams:{id:categoryId}
    }) 
  }

  restoreCategory(categoryId:number){
    this.service.restoreCategory(categoryId).subscribe((res)=>{
      if(res.status){
        this.loadCategories();
        this.toastr.success('activated successfully');
        // this.router.navigate(['/admin/category-list']);
        this.loadCategories();
      }
      else{
        this.toastr.error('something went wrong');
        console.log(res.error);
        
      }
    })
  }

  deleteParentCategory(categoryId:number){
    const dialogRef=this.dialog.open(ReactiveFormDalogue,{
      width:'400px'
    })

    dialogRef.afterClosed().subscribe(reason=>{
      if(!reason){
        return;
      }
      else{
        const dto={
          categoryId:categoryId,
          reason:reason.reason
        }

        this.service.deleteParentCategory(dto).subscribe((res)=>{
          if(res.status){
            console.log(res.data);
            this.toastr.success('successfully de-activated parent category')
            // this.router.navigate(['/admin/category-list']);
            this.loadCategories();
          }
          else{
            console.log(res.error);
            this.toastr.error('Something went wrong!')
          }
        })
      }
    })
  }

  restoreParentCategory(categoryId:number){
    this.service.restoreParentCategory(categoryId).subscribe((res)=>{
      if(res.status){
        console.log(res.data);
        this.toastr.success('successfully activated')
        // this.router.navigate(['/admin/category-list']);
        this.loadCategories();
      }
      else{
        console.log(res.error);
        this.toastr.error('Something went wrong!');
        
      }
    })
  }
}
