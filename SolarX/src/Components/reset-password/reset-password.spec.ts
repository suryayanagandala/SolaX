import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ResetPassword } from './reset-password';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { Auth } from '../../Shared/auth';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ResetPassword Component', () => {

  let component: ResetPassword;
  let fixture: ComponentFixture<ResetPassword>;

  let authServiceMock: jasmine.SpyObj<Auth>;
  let routerMock: jasmine.SpyObj<Router>;
  let toastrMock: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {

    authServiceMock = jasmine.createSpyObj('Auth', ['resetPassword']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    toastrMock = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    sessionStorage.setItem('tokenResponse', 'mock-token');

    await TestBed.configureTestingModule({
      imports: [
        ResetPassword,
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

    fixture = TestBed.createComponent(ResetPassword);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should create component', () => {
    expect(component).toBeTruthy();
  });


  it('should initialize reset form', () => {
    expect(component.resetForm).toBeDefined();
  });


  it('should redirect to login if token not found', () => {

    sessionStorage.removeItem('tokenResponse');

    component.ngOnInit();

    expect(toastrMock.error).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });


  it('should make form invalid when empty', () => {

    component.resetForm.setValue({
      newPassword: '',
      confirmPassword: ''
    });

    expect(component.resetForm.invalid).toBeTrue();
  });


  it('should validate password pattern', () => {

    const password = component.resetForm.controls['newPassword'];

    password.setValue('weak');
    expect(password.valid).toBeFalse();

    password.setValue('Password@1');
    expect(password.valid).toBeTrue();
  });


  it('should toggle password visibility', () => {

    const initialValue = component.hidePassword;

    component.togglePassword();

    expect(component.hidePassword).toBe(!initialValue);
  });


  it('should not call service if passwords do not match', () => {

    component.resetForm.setValue({
      newPassword: 'Password@1',
      confirmPassword: 'Password@2'
    });

    component.resetPassword();

    expect(toastrMock.error).toHaveBeenCalled();
    expect(authServiceMock.resetPassword).not.toHaveBeenCalled();
  });


  it('should call resetPassword service when form valid', () => {

    component.resetForm.setValue({
      newPassword: 'Password@1',
      confirmPassword: 'Password@1'
    });

    authServiceMock.resetPassword.and.returnValue(
      of({ status: true, data: 'Password Reset Success' })
    );

    component.resetPassword();

    expect(authServiceMock.resetPassword).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });


  it('should show error when reset password fails', () => {

    component.resetForm.setValue({
      newPassword: 'Password@1',
      confirmPassword: 'Password@1'
    });

    authServiceMock.resetPassword.and.returnValue(
      of({ status: false, error: 'Reset Failed' })
    );

    component.resetPassword();

    expect(toastrMock.error).toHaveBeenCalled();
  });


  it('should handle service error response', () => {

    component.resetForm.setValue({
      newPassword: 'Password@1',
      confirmPassword: 'Password@1'
    });

    authServiceMock.resetPassword.and.returnValue(
      throwError(() => new Error('Server Error'))
    );

    component.resetPassword();

    expect(authServiceMock.resetPassword).toHaveBeenCalled();
  });

});