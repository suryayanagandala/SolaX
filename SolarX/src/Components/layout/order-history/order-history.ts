import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { OrderService } from '../../../Shared/order-service';
import { TokenService } from '../../../Shared/token';
import { Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { localMessages } from '../../../localMessages';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { trigger, state, style, transition, animate } from '@angular/animations';

import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../dialogue-component/dialogue-component';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [MaterialModule, CommonModule, RouterModule],
  templateUrl: './order-history.html',
  styleUrl: './order-history.scss',
  animations: [
    trigger('expandCollapse', [
      state('collapsed', style({ height: '0px', opacity: 0, overflow: 'hidden' })),
      state('expanded', style({ height: '*', opacity: 1 })),
      transition('expanded <=> collapsed', animate('300ms ease-in-out'))
    ])
  ]
})
export class OrderHistory implements OnInit {

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  userId: string | null = null;
  messages = localMessages;
  ordersData: any[] = [];

  paginatedOrders: any[] = [];
  pageSize = 5;
  pageIndex = 0;

  expandedOrderId: number | null = null;

  constructor(
    private router: Router,
    private toastr: ToastrService,
    private orderService: OrderService,
    private tokenService: TokenService,
    private cd: ChangeDetectorRef,private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.userId = this.tokenService.GetUserId();

    if (!this.userId) {
      this.toastr.warning(this.messages.warnings.login);
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
    } else {
      this.orderHistory();
    }
  }

  orderHistory() {
    this.orderService.orderHistory().subscribe((res) => {
      if (res.status) {
        this.ordersData = res.data;
        this.updatePaginatedData();
      }
    });
  }

  updatePaginatedData() {
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedOrders = this.ordersData.slice(startIndex, endIndex);
  }

  onPageChange(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
    this.updatePaginatedData();
  }

  toggleExpand(orderId: number) {
    this.expandedOrderId =
      this.expandedOrderId === orderId ? null : orderId;
  }

  getImageUrl(url?: string): string {
    if (!url) return 'Assets/Themes/panelimage1.png';
    if (url.startsWith('http')) return url;
    return environment.imageUrl + url;
  }

  methodCalling(status: string, orderId: number) {
    switch (status) {
      case 'CONFIRMED':
        this.cancelOrder(orderId);
        break;
      case 'SHIPPED':
        this.trackOrder(orderId);
        break;
    }
  }

  cancelOrder(orderId: number) {

  const dialogRef = this.dialog.open(ConfirmDialogComponent, {
    width: '350px'
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {

      this.orderService.cancelOrder(orderId).subscribe((res) => {
        if (res.status) {
          this.orderHistory();
          this.toastr.success(this.messages.order.cancelOrder);
        } else {
          this.toastr.error(this.messages.order.error);
        }
      });

    }
  });
}

  trackOrder(orderId: number) {
    this.router.navigate(['/track-order'], {
      queryParams: { orderId: orderId }
    });
  }
}