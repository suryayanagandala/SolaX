import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Cart } from './cart';
import { RouterTestingModule } from '@angular/router/testing';
import { CartService } from '../../../Shared/cart-service';
import { TokenService } from '../../../Shared/token';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('Cart', () => {

  let component: Cart;
  let fixture: ComponentFixture<Cart>;

  const cartServiceMock = {
    getUserCart: jasmine.createSpy().and.returnValue(of({
      status: true,
      data: {
        items: [
          { cartItemId: 1, quantity: 2, price: 100 }
        ]
      }
    })),
    updateCart: jasmine.createSpy().and.returnValue(of({ status: true })),
    removeCart: jasmine.createSpy().and.returnValue(of({ status: true }))
  };

  const tokenServiceMock = {
    GetUserId: jasmine.createSpy().and.returnValue('1')
  };

  const toastrMock = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy(),
    warning: jasmine.createSpy()
  };

  beforeEach(async () => {

    await TestBed.configureTestingModule({
      imports: [
        Cart,
        RouterTestingModule
      ],
      providers: [
        { provide: CartService, useValue: cartServiceMock },
        { provide: TokenService, useValue: tokenServiceMock },
        { provide: ToastrService, useValue: toastrMock }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(Cart);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load cart on init', () => {
    component.ngOnInit();
    expect(cartServiceMock.getUserCart).toHaveBeenCalled();
  });

  it('should calculate total amount', () => {
    component.cart = {
      items: [
        { cartItemId: 1, quantity: 2, price: 100 }
      ]
    } as any;

    component.calculateTotalAmount();

    expect(component.subTotal).toBe(200);
    expect(component.total).toBe(300);
  });

  it('should increase quantity', () => {

    component.cart = {
      items: [
        { cartItemId: 1, quantity: 1, price: 100 }
      ]
    } as any;

    component.increaseQty(1);

    expect(cartServiceMock.updateCart).toHaveBeenCalled();
  });

  it('should remove item from cart', () => {

    component.cart = {
      items: [
        { cartItemId: 1, quantity: 1, price: 100 }
      ]
    } as any;

    component.removeCart(1);

    expect(cartServiceMock.removeCart).toHaveBeenCalled();
  });

});