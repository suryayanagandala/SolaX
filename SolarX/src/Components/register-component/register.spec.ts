import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RegisterComponent } from './register';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { Auth } from '../../Shared/auth';
import { UserService } from '../../Shared/user-service';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;

  let authServiceMock: jasmine.SpyObj<Auth>;
  let userServiceMock: jasmine.SpyObj<UserService>;
  let toastrMock: jasmine.SpyObj<ToastrService>;
  let routerMock: jasmine.SpyObj<Router>;

  const activatedRouteMock = {
    queryParams: of({})
  };

  beforeEach(async () => {

    authServiceMock = jasmine.createSpyObj('Auth', ['regeister']);
    userServiceMock = jasmine.createSpyObj('UserService', ['getUserDetails', 'updateProfile']);
    toastrMock = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        RegisterComponent,
        ReactiveFormsModule
      ],
      providers: [
        FormBuilder,
        { provide: Auth, useValue: authServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: ToastrService, useValue: toastrMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize register form', () => {
    expect(component.registerForm).toBeDefined();
  });

  it('should make form invalid when empty', () => {
    component.registerForm.setValue({
      fullName: '',
      emailId: '',
      password: '',
      phoneNumber: '',
      address: ''
    });

    expect(component.registerForm.invalid).toBeTrue();
  });

  it('should validate email format', () => {
    const email = component.registerForm.controls['emailId'];
    email.setValue('invalidemail');

    expect(email.valid).toBeFalse();

    email.setValue('test@gmail.com');
    expect(email.valid).toBeTrue();
  });

  it('should toggle password visibility', () => {

    const initialValue = component.hidePassword;

    component.togglePassword();

    expect(component.hidePassword).toBe(!initialValue);
  });

  it('should call register service when form is valid', () => {

    component.registerForm.setValue({
      fullName: 'John Doe',
      emailId: 'john@gmail.com',
      password: 'Password@1',
      phoneNumber: '9876543210',
      address: 'Mumbai'
    });

    authServiceMock.regeister.and.returnValue(of({ status: true }));

    component.submit();

    expect(authServiceMock.regeister).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should not call register service if form invalid', () => {

    component.registerForm.patchValue({
      fullName: ''
    });

    component.submit();

    expect(authServiceMock.regeister).not.toHaveBeenCalled();
  });

  it('should show error when register fails', () => {

    component.registerForm.setValue({
      fullName: 'John Doe',
      emailId: 'john@gmail.com',
      password: 'Password@1',
      phoneNumber: '9876543210',
      address: 'Mumbai'
    });

    authServiceMock.regeister.and.returnValue(
      throwError(() => ({ error: { message: 'Registration Failed' } }))
    );

    component.submit();

    expect(toastrMock.error).toHaveBeenCalledWith('Registration Failed');
  });

  it('should load user details in edit mode', () => {

    const response = {
      status: true,
      data: {
        fullName: 'John',
        emailId: 'john@gmail.com',
        phoneNumber: '9876543210',
        address: 'Mumbai'
      }
    };

    userServiceMock.getUserDetails.and.returnValue(of(response));

    component.loadUser('123');

    expect(userServiceMock.getUserDetails).toHaveBeenCalledWith('123');
    expect(component.registerForm.value.fullName).toEqual('John');
  });

  it('should update profile successfully', () => {

    component.isEditMode = true;
    component.currentUserId = '123';

    component.registerForm.patchValue({
      fullName: 'John',
      emailId: 'john@gmail.com',
      phoneNumber: '9876543210',
      address: 'Mumbai'
    });

    userServiceMock.updateProfile.and.returnValue(of({ status: true }));

    component.updateProfile();

    expect(userServiceMock.updateProfile).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should validate password capital letter', () => {

    component.registerForm.controls['password'].setValue('Password@1');

    expect(component.hasCapital()).toBeTrue();
  });

  it('should validate password digit', () => {

    component.registerForm.controls['password'].setValue('Password@1');

    expect(component.hasDigit()).toBeTrue();
  });

  it('should validate password special character', () => {

    component.registerForm.controls['password'].setValue('Password@1');

    expect(component.hasSpecialChar()).toBeTrue();
  });

  it('should validate password min length', () => {

    component.registerForm.controls['password'].setValue('Password@1');

    expect(component.minLength()).toBeTrue();
  });

  it('should validate password max length', () => {

    component.registerForm.controls['password'].setValue('Password@1');

    expect(component.maxLength()).toBeTrue();
  });

});