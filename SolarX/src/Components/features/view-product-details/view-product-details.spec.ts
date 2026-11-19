import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViewProductDetails } from './view-product-details';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

import { ToastrService } from 'ngx-toastr';
import { HomeService } from '../../../Shared/home-category';
import { TokenService } from '../../../Shared/token';
import { CartService } from '../../../Shared/cart-service';

describe('ViewProductDetails', () => {
  let component: ViewProductDetails;
  let fixture: ComponentFixture<ViewProductDetails>;
  let router: Router;

  // Mock Product
  const mockProduct = {
    productId: 1,
    productName: 'Solar Panel',
    price: 5000,
    stock: 10,
    productImages: [
      { imageUrl: 'panel1.jpg' },
      { imageUrl: 'panel2.jpg' }
    ]
  };

  // Mock Services
  const homeServiceMock = {
    viewProductDetails: jasmine.createSpy().and.returnValue(
      of({ status: true, data: mockProduct })
    ),
    addToCart: jasmine.createSpy().and.returnValue(
      of({ status: true })
    ),
    productAnalytics: jasmine.createSpy().and.returnValue(
      of({ status: true, data: {} })
    )
  };

  const toastrMock = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy()
  };

  const tokenServiceMock = {
    isLoggedIn: jasmine.createSpy().and.returnValue(true),
    GetUserId: jasmine.createSpy().and.returnValue('123')
  };

  const cartServiceMock = {
    updateCart: jasmine.createSpy().and.returnValue(
      of({ status: true })
    )
  };

  const activatedRouteMock = {
    queryParams: of({ productId: 1 })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ViewProductDetails,
        RouterTestingModule
      ],
      providers: [
        { provide: HomeService, useValue: homeServiceMock },
        { provide: ToastrService, useValue: toastrMock },
        { provide: TokenService, useValue: tokenServiceMock },
        { provide: CartService, useValue: cartServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ViewProductDetails);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);

    spyOn(router, 'navigate');

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load product details on init', () => {
    expect(homeServiceMock.viewProductDetails).toHaveBeenCalledWith(1);
    expect(component.productDetails.productName).toBe('Solar Panel');
  });

  it('should increase quantity if less than stock', () => {
    component.stock = 5;
    component.quantity = 1;

    component.increaseQty();

    expect(component.quantity).toBe(2);
  });

  it('should not increase quantity beyond stock', () => {
    component.stock = 2;
    component.quantity = 2;

    component.increaseQty();

    expect(component.quantity).toBe(2);
  });

  it('should decrease quantity', () => {
    component.quantity = 2;

    component.decreaseQty();

    expect(component.quantity).toBe(1);
  });

  it('should not decrease quantity below 1', () => {
    component.quantity = 1;

    component.decreaseQty();

    expect(component.quantity).toBe(1);
  });

  it('should select image', () => {
    component.selectImage('panel2.jpg');

    expect(component.selectedImage).toBe('panel2.jpg');
  });

  it('should return default image if url empty', () => {
    const result = component.getImageUrl();

    expect(result).toContain('panelimage1.png');
  });

  it('should navigate to login if not logged in', () => {
    tokenServiceMock.isLoggedIn.and.returnValue(false);

    component.addToCart();

    expect(router.navigate).toHaveBeenCalledWith(
      ['/login'],
      { queryParams: { returnUrl: router.url } }
    );
  });

  it('should add product to cart', () => {
    component.productDetails = mockProduct as any;
    component.quantity = 1;

    component.addToCart();

    expect(homeServiceMock.addToCart).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalled();
  });

  it('should navigate to order on buyNow', () => {
    component.productDetails = mockProduct as any;

    component.buyNow();

    expect(router.navigate).toHaveBeenCalledWith(
      ['/order'],
      jasmine.any(Object)
    );
  });

  it('should set tab index', () => {
    component.onTabIndex(2);

    expect(component.selectedTabIndex).toBe(2);
  });

  it('should load analytics', () => {
    component.getProductAnalytics(1);

    expect(homeServiceMock.productAnalytics).toHaveBeenCalledWith(1);
  });

});