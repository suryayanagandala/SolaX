import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ForgetPassword } from './forget-password';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { of, throwError } from 'rxjs';

import { ToastrService } from 'ngx-toastr';
import { Auth } from '../../Shared/auth';

describe('ForgetPassword', () => {
  let component: ForgetPassword;
  let fixture: ComponentFixture<ForgetPassword>;
  let router: Router;

  // Mock Services
  const authServiceMock = {
    sendOtp: jasmine.createSpy()
  };

  const toastrMock = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy()
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ForgetPassword,
        ReactiveFormsModule,
        RouterTestingModule
      ],
      providers: [
        FormBuilder,
        { provide: Auth, useValue: authServiceMock },
        { provide: ToastrService, useValue: toastrMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ForgetPassword);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);

    spyOn(router, 'navigate');

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with email control', () => {
    expect(component.form.contains('email')).toBeTrue();
  });

  it('should make form invalid when email empty', () => {
    component.form.controls['email'].setValue('');
    expect(component.form.invalid).toBeTrue();
  });

  it('should validate email format', () => {
    component.form.controls['email'].setValue('invalidEmail');
    expect(component.form.invalid).toBeTrue();
  });

  it('should show error if form invalid', () => {
    component.form.controls['email'].setValue('');

    component.sendOtp();

    expect(toastrMock.error).toHaveBeenCalledWith('invalid email Id');
  });

  it('should call sendOtp service when form valid', () => {
    authServiceMock.sendOtp.and.returnValue(
      of({ status: true, data: 'OTP Sent' })
    );

    component.form.controls['email'].setValue('test@gmail.com');

    component.sendOtp();

    expect(authServiceMock.sendOtp).toHaveBeenCalledWith('test@gmail.com');
  });

  it('should store email and navigate on success', () => {
    spyOn(sessionStorage, 'setItem');

    authServiceMock.sendOtp.and.returnValue(
      of({ status: true, data: 'OTP Sent' })
    );

    component.form.controls['email'].setValue('test@gmail.com');

    component.sendOtp();

    expect(sessionStorage.setItem).toHaveBeenCalledWith(
      'emailId',
      'test@gmail.com'
    );

    expect(toastrMock.success).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/verify-otp']);
  });

  it('should show error if API returns false', () => {
    authServiceMock.sendOtp.and.returnValue(
      of({ status: false })
    );

    component.form.controls['email'].setValue('test@gmail.com');

    component.sendOtp();

    expect(toastrMock.error).toHaveBeenCalledWith('Invalid emailId');
  });

  it('should handle service error', () => {
    authServiceMock.sendOtp.and.returnValue(
      throwError(() => new Error('API Error'))
    );

    component.form.controls['email'].setValue('test@gmail.com');

    component.sendOtp();

    expect(authServiceMock.sendOtp).toHaveBeenCalled();
  });

});