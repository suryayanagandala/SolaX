import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Orders } from './orders';
import { of } from 'rxjs';
import { FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { OrderService } from '../../../Shared/order-service';
import { CartService } from '../../../Shared/cart-service';
import { TokenService } from '../../../Shared/token';
import { environment } from '../../../environments/environment';
describe('Orders', () => {
  let component: Orders;
  let fixture: ComponentFixture<Orders>;

  const mockRouter = {
    navigate: jasmine.createSpy('navigate'),
    getCurrentNavigation: jasmine.createSpy('getCurrentNavigation').and.returnValue({ extras: { state: null } }),
    url: '/orders'
  };

  const mockActivatedRoute = { snapshot: {}, params: of({}) };

  const mockToastr = { error: jasmine.createSpy('error') };

  const mockOrderService = {
    getUserDetails: jasmine.createSpy('getUserDetails').and.returnValue(of({ data: { fullName: 'John Doe', phoneNumber: '9876543210' } })),
    ceratePendingOrder: jasmine.createSpy('ceratePendingOrder').and.returnValue(of({ status: true, data: { orderId: 'ORD123' } }))
  };

  const mockCartService = {
    getUserCart: jasmine.createSpy('getUserCart').and.returnValue(of({ status: true, data: { items: [{ quantity: 2, price: 500 }] } }))
  };

  const mockTokenService = { GetUserId: jasmine.createSpy('GetUserId').and.returnValue('user123') };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Orders],
      providers: [
        FormBuilder,
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: ToastrService, useValue: mockToastr },
        { provide: OrderService, useValue: mockOrderService },
        { provide: CartService, useValue: mockCartService },
        { provide: TokenService, useValue: mockTokenService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Orders);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.userId).toBe('user123');
  });

  it('should calculate total amount correctly', () => {
    component.cart = { items: [{ quantity: 2, price: 500 }] } as any;
    component.calculateTotalAmount();
    expect(component.totalAmount).toBe(2 * 500 + component.shipping - component.discount);
  });

  it('should return default image if url is missing', () => {
    expect(component.getImageUrl()).toBe('Assets/Themes/panelimage1.png');
  });

  it('should return full url if image path is relative', () => {
    expect(component.getImageUrl('panel.jpg')).toBe(environment.imageUrl + 'panel.jpg');
  });

  it('should return same url if it starts with http', () => {
    expect(component.getImageUrl('http://example.com/panel.jpg')).toBe('http://example.com/panel.jpg');
  });

  it('should call placeOrder and navigate', fakeAsync(() => {
    component.addressForm.patchValue({
      deliveryName: 'John Doe',
      deliveryNumber: '9876543210',
      fullAddress: 'Some Address',
      lat: 17,
      lng: 78
    });

    component.placeOrder();
    tick();
    expect(mockOrderService.ceratePendingOrder).toHaveBeenCalled();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/payment'], jasmine.any(Object));
  }));
});