import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentComponent } from './payment-component';
import { MaterialModule } from '../../../MaterialModule';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PaymentService } from '../../../Shared/payment';
import { TokenService } from '../../../Shared/token';
import { of } from 'rxjs';

describe('PaymentComponent', () => {
  let component: PaymentComponent;
  let fixture: ComponentFixture<PaymentComponent>;

  let mockRouter: any;
  let mockToastr: any;
  let mockPaymentService: any;
  let mockTokenService: any;

  beforeEach(async () => {

    mockRouter = {
      navigate: jasmine.createSpy('navigate'),
      getCurrentNavigation: () => ({ extras: { state: { orderId: 1, amount: 500 } } })
    };

    mockToastr = {
      success: jasmine.createSpy('success'),
      error: jasmine.createSpy('error')
    };

    mockPaymentService = {
      ProcessPayment: jasmine.createSpy('ProcessPayment').and.returnValue(of({ status: true, data: {} })),
      genearteQrCode: jasmine.createSpy('genearteQrCode').and.returnValue(of({ status: true, data: { qrCodeImage: 'qr', expiresIn: 60 } }))
    };

    mockTokenService = {
      GetUserId: jasmine.createSpy('GetUserId').and.returnValue('USR1')
    };

    await TestBed.configureTestingModule({
      imports: [PaymentComponent, MaterialModule, ReactiveFormsModule],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: ToastrService, useValue: mockToastr },
        { provide: PaymentService, useValue: mockPaymentService },
        { provide: TokenService, useValue: mockTokenService },
        { provide: ActivatedRoute, useValue: { queryParams: of({}) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize credit card form', () => {
    expect(component.creditCard).toBeDefined();
    expect(component.creditCard.invalid).toBeTrue(); // initially invalid
  });

  it('should mark cardNumber invalid when empty', () => {
    const cardNumber = component.creditCard.get('cardNumber');
    cardNumber?.setValue('');
    expect(cardNumber?.valid).toBeFalse();
  });

  it('should make form valid with correct values', () => {
    component.creditCard.setValue({
      cardNumber: '1234567812345678', // 16 digits
      name: 'Venkatesh Kumar',
      expiry: '12/26',
      cvv: '123'
    });

    expect(component.creditCard.valid).toBeTrue();
  });

  it('should select CARD on tab change index 0', () => {
    component.onTabChange(0);
    expect(component.paymentSelectedMethod).toBe('CARD');
  });

  it('should select UPI on tab change index 1 and generate QR', () => {
    component.orderId = 1;
    component.onTabChange(1);
    expect(component.paymentSelectedMethod).toBe('UPI');
    expect(mockPaymentService.genearteQrCode).toHaveBeenCalledWith(1);
  });

  it('should select CASH on tab change index 2', () => {
    component.onTabChange(2);
    expect(component.paymentSelectedMethod).toBe('CASH');
  });

  it('should process payment and navigate on confirmPayment', () => {
    component.orderId = 1;
    component.paymentSelectedMethod = 'CARD';
    component.confirmPayment();

    expect(mockPaymentService.ProcessPayment).toHaveBeenCalled();
    expect(mockToastr.success).toHaveBeenCalled();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/payment-success'], {
      queryParams: { orderId: 1 }
    });
  });

});