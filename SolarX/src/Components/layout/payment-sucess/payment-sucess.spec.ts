import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentSucess } from './payment-sucess';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService } from '../../../Shared/payment';
import { of } from 'rxjs';

describe('PaymentSucess', () => {
  let component: PaymentSucess;
  let fixture: ComponentFixture<PaymentSucess>;

  let mockPaymentService: any;
  let mockRouter: any;
  let mockToastr: any;

  beforeEach(async () => {

    mockPaymentService = {
      getInvoice: jasmine.createSpy('getInvoice').and.returnValue(
        of({
          status: true,
          data: {
            payment: {
              amount: 500,
              paymentMethod: 'UPI'
            }
          }
        })
      ),

      invoicePdf: jasmine.createSpy('invoicePdf').and.returnValue(
        of({
          status: true,
          data: btoa('dummy pdf data')
        })
      )
    };

    mockRouter = {
      navigate: jasmine.createSpy('navigate')
    };

    mockToastr = {};

    await TestBed.configureTestingModule({
      imports: [PaymentSucess],
      providers: [
        { provide: PaymentService, useValue: mockPaymentService },
        { provide: Router, useValue: mockRouter },
        { provide: ToastrService, useValue: mockToastr },
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({ orderId: 10 })
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PaymentSucess);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch invoice on init', () => {
    expect(mockPaymentService.getInvoice).toHaveBeenCalledWith(10);
  });

  it('should set amount and paymentMethod', () => {

    component.getInvoice(10);

    expect(component.amount).toBe(500);
    expect(component.paymentMethod).toBe('UPI');
  });

  it('should call invoicePdf when openInvoice called', () => {

    component.orderId = 10;

    component.openInvoice(true);

    expect(mockPaymentService.invoicePdf).toHaveBeenCalledWith(10);
  });

  it('should return if orderId is null', () => {

    component.orderId = null;

    component.openInvoice(true);

    expect(mockPaymentService.invoicePdf).not.toHaveBeenCalled();
  });

});