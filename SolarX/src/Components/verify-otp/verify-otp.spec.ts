import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { VerifyOtp } from './verify-otp';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Auth } from '../../Shared/auth';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('VerifyOtp Component', () => {

  let component: VerifyOtp;
  let fixture: ComponentFixture<VerifyOtp>;

  let authServiceMock: jasmine.SpyObj<Auth>;
  let routerMock: jasmine.SpyObj<Router>;
  let toastrMock: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {

    authServiceMock = jasmine.createSpyObj('Auth', ['verifyOtp', 'sendOtp']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    toastrMock = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    sessionStorage.setItem('emailId', 'test@gmail.com');

    await TestBed.configureTestingModule({
      imports: [
        VerifyOtp,
        ReactiveFormsModule
      ],
      providers: [
        FormBuilder,
        { provide: Auth, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ToastrService, useValue: toastrMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(VerifyOtp);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize otp form', () => {
    expect(component.otpForm).toBeDefined();
  });

  it('should start timer on init', fakeAsync(() => {

    component.timer = 30;

    component.startTimer();

    tick(1000);

    expect(component.timer).toBe(29);
  }));

  it('should expire timer when reaches zero', fakeAsync(() => {

    component.timer = 1;

    component.startTimer();

    tick(2000);

    expect(component.timer).toBe(0);
    expect(component.expired).toBeTrue();
  }));

  it('should format timer display correctly', () => {

    component.timer = 65;

    const display = component.timerDisplay;

    expect(display).toBe('1:05');
  });

  it('should call verifyOtp when otp length is 6', () => {

    spyOn(component, 'verifyOtp');

    component.otpForm.setValue({ otpText: '123456' });

    component.onOtpChange();

    expect(component.verifyOtp).toHaveBeenCalled();
  });

  it('should not verify otp when expired', () => {

    component.expired = true;

    spyOn(component, 'verifyOtp');

    component.otpForm.setValue({ otpText: '123456' });

    component.onOtpChange();

    expect(component.verifyOtp).not.toHaveBeenCalled();
  });

  it('should show error if email not found in session', () => {

    sessionStorage.removeItem('emailId');

    component.otpForm.setValue({ otpText: '123456' });

    component.verifyOtp();

    expect(toastrMock.error).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should show error if otp invalid', () => {

    component.otpForm.setValue({ otpText: '' });

    component.verifyOtp();

    expect(toastrMock.error).toHaveBeenCalled();
  });

  it('should verify otp successfully', () => {

    component.otpForm.setValue({ otpText: '123456' });

    authServiceMock.verifyOtp.and.returnValue(
      of({ status: true, data: 'token123' })
    );

    component.verifyOtp();

    expect(authServiceMock.verifyOtp).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/resetpassword'], { replaceUrl: true });
  });

  it('should show error if otp verification fails', () => {

    component.otpForm.setValue({ otpText: '123456' });

    authServiceMock.verifyOtp.and.returnValue(
      of({ status: false, error: 'Invalid OTP' })
    );

    component.verifyOtp();

    expect(toastrMock.error).toHaveBeenCalled();
  });

  it('should handle service error during verification', () => {

    component.otpForm.setValue({ otpText: '123456' });

    authServiceMock.verifyOtp.and.returnValue(
      throwError(() => new Error('Server Error'))
    );

    component.verifyOtp();

    expect(authServiceMock.verifyOtp).toHaveBeenCalled();
  });

  it('should resend otp successfully', () => {

    authServiceMock.sendOtp.and.returnValue(
      of({ status: true, data: 'OTP Sent' })
    );

    component.resendOtp();

    expect(authServiceMock.sendOtp).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalled();
    expect(component.timer).toBe(30);
    expect(component.expired).toBeFalse();
  });

});