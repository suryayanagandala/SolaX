import { Component, OnInit, ViewChild } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { RouterModule, RouterOutlet } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Auth } from '../../../Shared/auth';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-admin-users',
  standalone:true,
  imports: [MaterialModule,RouterModule,ReactiveFormsModule,CommonModule,MatTableModule],
  templateUrl: './admin-users.html',
  styleUrl: './admin-users.scss',
})
export class AdminUsers implements OnInit{
  
  displayedColumns:string[]=['fullName','emailId','phoneNumber','roleName','actions'];

   searchControl=new FormControl('');
   dataSource=new MatTableDataSource<any>([]);

   @ViewChild(MatPaginator) paginator!:MatPaginator;
   @ViewChild(MatSort) sort!:MatSort;
  constructor(private service:Auth,private toastr:ToastrService){}
  
  ngOnInit(): void {
    this.loadUsers();

    this.searchControl.valueChanges.subscribe(search=>{
      this.ApplyFilter(search||'');
    });
  }

  loadUsers(){
    this.service.GetAllUsers().subscribe({
      next:(res)=>{
        
        if(res.status){
          console.log(res.data);
          this.dataSource.data=res.data;

          //custom filter

          this.dataSource.filterPredicate=(data,filter)=>{
            const search=filter.trim().toLowerCase();

            return (
              data.name?.toLowerCase().includes(search)||
               data.email?.toLowerCase().includes(search) 
              // this.formatDate(data.createdDate).includes(search)
            );
          }

          //sorting for date

          this.dataSource.sortingDataAccessor=(item,property)=>{
            switch(property){
              case 'createdAt':
                return new Date(item.createdDate);

                default:return item[property];
            }
          };
          this.dataSource.paginator=this.paginator;
          this.dataSource.sort=this.sort;
          
        }else{
          this.toastr.error('Failed to load');
          console.log(res.error);
          
        }
        
      },error(err) {
        console.log(err);
        
      },
    })
  }
  

  ApplyFilter(searchText:string){
    this.dataSource.filter=searchText.trim().toLowerCase();
    if(this.dataSource.paginator){
      this.dataSource.paginator.firstPage();
    }
   
  }
  
    formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-IN'); 
  }
}
