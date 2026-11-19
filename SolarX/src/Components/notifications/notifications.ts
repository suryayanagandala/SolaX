import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { NotificatoinState } from '../../Shared/notificatoin-state';
import { NotifcationDto } from '../../Models/Models';
import { Notification } from '../../Shared/notification';
import { TokenService } from '../../Shared/token';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../MaterialModule';
import { PageEvent } from '@angular/material/paginator';
import { FormsModule } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { ViewChild } from '@angular/core';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-notifications',
 
  imports: [CommonModule, MaterialModule, FormsModule],
  templateUrl: './notifications.html',
  styleUrls: ['./notifications.scss']
})
export class Notifications implements OnInit {

  notifications: NotifcationDto[] = [];
  filter = 'all';
  userId!: string;

  pageIndex=0;
  pageSize=5;
  today: Date = new Date();
  searchText: string = '';
  selectedDate: Date | null = null;
  
  @ViewChild(MatPaginator) paginator!:MatPaginator;
  constructor(
    private notificationState: NotificatoinState,
    private notificationService: Notification,
    private token: TokenService,private cd:ChangeDetectorRef
  ) {}

  ngOnInit(): void {

    const userId=  this.token.GetUserId();
    if(userId){
      this.userId=userId;
    }
    this.notificationState.notification$
      .subscribe(data => {
        this.notifications = data;
      });

      console.log('data',this.notifications);
      
  }

  formatDate(date: any): string {
  const d = new Date(date);

  const year = d.getFullYear();
  const month = (d.getMonth() + 1).toString().padStart(2, '0');
  const day = d.getDate().toString().padStart(2, '0');

  return `${year}-${month}-${day}`; 
}

  get filteredNotifications() {
    let data=this.notifications;


    if (this.filter === 'unread') {
      data=data.filter(x=>!x.isRead)
      
    }

    if(this.searchText){
      const search=this.searchText.toLowerCase();
      data=data.filter(x=>x.message.toLowerCase().includes(search)||x.notificationType.toLowerCase().includes(search));

    }

     if (this.selectedDate) {

    const selected = this.formatDate(this.selectedDate);

    data = data.filter(x =>
      this.formatDate(x.createdAt) === selected
    );
  }

    return data;
  }

      onSearchChange() {
       if (this.paginator) {
      this.paginator.firstPage();
    }
    }

    onDateChange() {
       if (this.paginator) {
      this.paginator.firstPage();
    }
    }


  get paginationNotifications(){
    const start=this.pageIndex*this.pageSize;
    const end=start+this.pageSize;

    return this.filteredNotifications.slice(start,end);
  }

   onPageChange(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
  }



  markAllRead() {

    this.notificationService.markAllAsRead(this.userId)
      .subscribe(() => {
        this.notificationState.markAllRead();
      });

  }
 setFilter(value: string) {
  this.filter = value;
  this.pageIndex = 0;

  
  if (value === 'all') {
    this.searchText = '';
    this.selectedDate = null;
  }

  if (this.paginator) {
    this.paginator.firstPage();
  }
}

goBack(){
  window.history.back();
}
}