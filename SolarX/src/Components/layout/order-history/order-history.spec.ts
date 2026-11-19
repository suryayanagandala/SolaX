import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { OrderHistory } from './order-history';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { OrderService } from '../../../Shared/order-service';
import { TokenService } from '../../../Shared/token';
import { MatDialog } from '@angular/material/dialog';

describe('OrderHistory', () => {
  let component: OrderHistory;
  let fixture: ComponentFixture<OrderHistory>;

  
  const mockRouter = { navigate: jasmine.createSpy('navigate') };
  const mockToastr = { success: jasmine.createSpy('success'), warning: jasmine.createSpy('warning'), error: jasmine.createSpy('error') };
  const mockOrderService = {
    orderHistory: jasmine.createSpy('orderHistory').and.returnValue(of({
      status: true,
      data: [
        { orderId: 1, status: 'CONFIRMED', items: [{ name: 'Panel', quantity: 1 }] },
        { orderId: 2, status: 'SHIPPED', items: [{ name: 'Panel', quantity: 2 }] }
      ]
    })),
    cancelOrder: jasmine.createSpy('cancelOrder').and.returnValue(of({ status: true }))
  };
  const mockTokenService = { GetUserId: jasmine.createSpy('GetUserId').and.returnValue('user123') };
  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(true)
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderHistory],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: ToastrService, useValue: mockToastr },
        { provide: OrderService, useValue: mockOrderService },
        { provide: TokenService, useValue: mockTokenService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(OrderHistory);
    component = fixture.componentInstance;

   
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load order history on init', () => {
    expect(component.ordersData.length).toBe(2);
    expect(component.paginatedOrders.length).toBe(2);
    expect(component.paginatedOrders[0].orderId).toBe(1);
  });

  it('should navigate to track-order for SHIPPED orders', () => {
    component.methodCalling('SHIPPED', 2);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/track-order'], { queryParams: { orderId: 2 } });
  });

  it('should call cancel order via dialog confirmation', fakeAsync(() => {
    component.methodCalling('CONFIRMED', 1);
    tick(); 
    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockOrderService.cancelOrder).toHaveBeenCalledWith(1);
    expect(mockToastr.success).toHaveBeenCalled();
  }));

  it('should toggle expanded order', () => {
    expect(component.expandedOrderId).toBeNull();
    component.toggleExpand(1);
    expect(component.expandedOrderId).toBe(1);
    component.toggleExpand(1);
    expect(component.expandedOrderId).toBeNull();
  });

  it('should update paginated data on page change', () => {
    component.pageSize = 1;
    component.pageIndex = 1;
    component.updatePaginatedData();
    expect(component.paginatedOrders.length).toBe(1);
    expect(component.paginatedOrders[0].orderId).toBe(2);
  });
});