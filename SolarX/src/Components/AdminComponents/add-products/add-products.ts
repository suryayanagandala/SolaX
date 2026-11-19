import { ChangeDetectorRef, Component,OnInit,ViewChild } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { FormBuilder,FormControlName,FormGroup,ReactiveFormsModule,FormControl,Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonModule, DatePipe } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { Auth } from '../../../Shared/auth';
import { ProductService } from '../../../Shared/product';
import { Product } from '../../../Models/Models';
import { MatDialog } from '@angular/material/dialog';
import { DeleteReasonDialogComponent } from '../delete-reason-dialog-component/delete-reason-dialog-component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { localMessages } from '../../../localMessages';

@Component({
  selector: 'app-add-products',
  standalone:true,
  imports: [MaterialModule,RouterModule,ReactiveFormsModule,CommonModule,MatTableModule,DatePipe,
    MatTooltipModule
  ],
  templateUrl: './add-products.html',
  styleUrl: './add-products.scss',
})
export class AddProducts implements OnInit{
  
  displayedColumns:string[]=['productName','categoryId','brand','type','price','stock','description','capacity','createdAt',
    'status','actions']

  searchControl=new FormControl('');

  dataSource=new MatTableDataSource<Product>([]);
  private mesages=localMessages;
 

  @ViewChild(MatPaginator) paginator!:MatPaginator;
  @ViewChild(MatSort) sort!:MatSort; 

  constructor(private toastr:ToastrService,private service:ProductService,
    private router:Router,private cd:ChangeDetectorRef,private dialog: MatDialog,
  )
{}
  ngOnInit(): void {
    this.loadProducts();
    this.cd.detectChanges();
    this.searchControl.valueChanges.subscribe(search=>{
      this.applyFilter(search||'');
    })
  }

  loadProducts(){
    this.service.getAllProducts().subscribe((res)=>{
      if(res.status){
        console.log(res.data);
        this.dataSource.data=res.data??[]   
        this.cd.detectChanges();
        //custom filter
        this.dataSource.filterPredicate=(data,filter)=>{
          const search=filter.trim().toLowerCase();
          return(
             data.productName?.toLowerCase().includes(search)||data.capacity?.toLowerCase().includes(search)||
             data.type?.toLowerCase().includes(search)||data.createdAt?.toLowerCase().includes(search)
          )
        }

        //sorting data
        this.dataSource.sortingDataAccessor=(item,property)=>{
          switch(property)
           {
             case 'createdAt':
              return item.createdAt? new Date(item.createdAt):0;
              default:return (item as any)[property]
           }
        };
        this.dataSource.paginator=this.paginator;
        this.dataSource.sort=this.sort;
      }
      else{
        console.log(res.error);
        this.toastr.error(res.error)
      }
    })
  }

  applyFilter(serachText:string){
    this.dataSource.filter=serachText.trim().toLowerCase();
    if(this.dataSource.paginator){
      this.dataSource.paginator.firstPage();
    }
  }

   deleteProduct(productId:number){
       const dialogRef=this.dialog.open(DeleteReasonDialogComponent,{
        width:'400px',
        data: { reason: '' }
       });

       dialogRef.afterClosed().subscribe(reason=>{
        if(!reason)return;

        const dto={
         productId: productId,
         reason: reason
        }

        this.service.softDelete(dto).subscribe((res)=>{
          if(res.status){
            this.loadProducts();
            this.toastr.success();
          }
          else{
            console.log(res.error);
            this.toastr.error(this.mesages.error.fail)
          }
        })
       })
   }

   restoreProduct(productId:number){
    this.service.restoreProduct(productId).subscribe((res)=>{
      if(res.status&&res.data){
        this.loadProducts();
        this.toastr.success('sucessfully activated1');
      }
      else{
        this.toastr.error(this.mesages.error.fail);
        console.log(res.error);
        
      }
    })
   }

   showReason(reason?: string) {
  this.dialog.open(DeleteReasonDialogComponent, {
    data: { reason }
  });
}
}
