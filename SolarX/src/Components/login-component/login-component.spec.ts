import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login-component';
import { of, throwError } from 'rxjs';

import { Auth } from '../../Shared/auth';
import { TokenService } from '../../Shared/token';
import { Router, ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ToastrService } from 'ngx-toastr';
import { NotificationSignalrService } from '../../Shared/signal-r';
import { Role } from '../../Models/Models';

describe('LoginComponent', () => {

  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  let authSpy: jasmine.SpyObj<Auth>;
  let tokenSpy: jasmine.SpyObj<TokenService>;
  let router: Router;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let signalSpy: jasmine.SpyObj<NotificationSignalrService>;

  beforeEach(async () => {

    authSpy = jasmine.createSpyObj('Auth', ['login']);

    tokenSpy = jasmine.createSpyObj('TokenService', [
      'SaveToken',
      'SaveUser',
      'getToken',
      'GetUserId'
    ]);

    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    signalSpy = jasmine.createSpyObj('NotificationSignalrService', ['startConnection']);
    signalSpy.startConnection.and.returnValue(Promise.resolve());

    await TestBed.configureTestingModule({
      imports: [
        LoginComponent,
        RouterTestingModule
      ],
      providers: [
        { provide: Auth, useValue: authSpy },
        { provide: TokenService, useValue: tokenSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: NotificationSignalrService, useValue: signalSpy },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { queryParams: {} } }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);

    spyOn(router, 'navigate');
    spyOn(router, 'navigateByUrl');

    fixture.detectChanges();
  });

  /* ---------- COMPONENT ---------- */

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  /* ---------- FORM ---------- */

  it('should initialize login form', () => {
    expect(component._loginForm).toBeDefined();
  });

  it('should make form invalid when empty', () => {
    expect(component._loginForm.valid).toBeFalse();
  });

  it('should validate email required', () => {

    const email = component._loginForm.controls['email'];
    email.setValue('');

    expect(email.valid).toBeFalse();
  });

  it('should validate email format', () => {

    const email = component._loginForm.controls['email'];
    email.setValue('invalidEmail');

    expect(email.valid).toBeFalse();
  });

  it('should validate password required', () => {

    const password = component._loginForm.controls['password'];
    password.setValue('');

    expect(password.valid).toBeFalse();
  });

  it('should make form valid when correct values provided', () => {

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    expect(component._loginForm.valid).toBeTrue();
  });

  /* ---------- LOGIN ---------- */

  it('should not call login service if form invalid', () => {

    component.Login();

    expect(authSpy.login).not.toHaveBeenCalled();
  });

  it('should call login service when form valid', () => {

    authSpy.login.and.returnValue(of({ status: true, data: {} }));

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(authSpy.login).toHaveBeenCalled();
  });

  /* ---------- SUCCESS LOGIN ---------- */

  it('should save token and user when login success', () => {

    const response = {
      status: true,
      data: {
        tokenResponse: 'abc123',
        roleName: Role.Admin
      }
    };

    authSpy.login.and.returnValue(of(response));
    tokenSpy.getToken.and.returnValue('abc123');

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(tokenSpy.SaveToken).toHaveBeenCalled();
    expect(tokenSpy.SaveUser).toHaveBeenCalled();
  });

  it('should start SignalR connection after login', () => {

    const response = {
      status: true,
      data: {
        tokenResponse: 'abc123',
        roleName: Role.Admin
      }
    };

    authSpy.login.and.returnValue(of(response));
    tokenSpy.getToken.and.returnValue('abc123');

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(signalSpy.startConnection).toHaveBeenCalledWith('abc123');
  });

  it('should show success toastr on login success', () => {

    const response = {
      status: true,
      data: {
        tokenResponse: 'abc123',
        roleName: Role.Admin
      }
    };

    authSpy.login.and.returnValue(of(response));
    tokenSpy.getToken.and.returnValue('abc123');

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(toastrSpy.success).toHaveBeenCalled();
  });

  /* ---------- ROLE NAVIGATION ---------- */

  it('should navigate to admin dashboard', () => {

    const response = {
      status: true,
      data: {
        tokenResponse: 'abc',
        roleName: Role.Admin
      }
    };

    authSpy.login.and.returnValue(of(response));
    tokenSpy.getToken.and.returnValue('abc');

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(router.navigate).toHaveBeenCalledWith(['/admin']);
  });

  it('should navigate to technician panel', () => {

    const response = {
      status: true,
      data: {
        tokenResponse: 'abc',
        roleName: Role.Technician
      }
    };

    authSpy.login.and.returnValue(of(response));
    tokenSpy.getToken.and.returnValue('abc');
    tokenSpy.GetUserId.and.returnValue('123');

    component._loginForm.setValue({
      email: 'tech@test.com',
      password: '123456'
    });

    component.Login();

    expect(router.navigate).toHaveBeenCalledWith(['/panel'], {
      queryParams: { userId: '123' }
    });
  });

  it('should navigate to home for normal user', () => {

    const response = {
      status: true,
      data: {
        tokenResponse: 'abc',
        roleName: 'Customer'
      }
    };

    authSpy.login.and.returnValue(of(response));
    tokenSpy.getToken.and.returnValue('abc');

    component._loginForm.setValue({
      email: 'user@test.com',
      password: '123456'
    });

    component.Login();

    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });

  /* ---------- RETURN URL ---------- */

  it('should navigate to returnUrl if exists', () => {

    const route = TestBed.inject(ActivatedRoute);
    route.snapshot.queryParams['returnUrl'] = '/checkout';

    const response = {
      status: true,
      data: {
        tokenResponse: 'abc',
        roleName: Role.Admin
      }
    };

    authSpy.login.and.returnValue(of(response));
    tokenSpy.getToken.and.returnValue('abc');

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(router.navigateByUrl).toHaveBeenCalledWith('/checkout');
  });

  /* ---------- ERROR CASE ---------- */

  it('should show error toastr when login fails', () => {

    const response = {
      status: false,
      error: 'Invalid credentials'
    };

    authSpy.login.and.returnValue(of(response));

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(toastrSpy.error).toHaveBeenCalled();
  });

  it('should handle API error', () => {

    authSpy.login.and.returnValue(
      throwError(() => new Error('Server error'))
    );

    component._loginForm.setValue({
      email: 'admin@test.com',
      password: '123456'
    });

    component.Login();

    expect(toastrSpy.error).toHaveBeenCalled();
  });

  /* ---------- NAVIGATION ---------- */

  it('should navigate to register page', () => {

    component.goToRegister();

    expect(router.navigate).toHaveBeenCalledWith(['/register']);
  });

  it('should navigate to forgot password page', () => {

    component.gotoForgotPassword();

    expect(router.navigate).toHaveBeenCalledWith(['/forgotPassword']);
  });

  /* ---------- PASSWORD TOGGLE ---------- */

  it('should toggle password visibility', () => {

    const initial = component.hidePassword;

    component.togglePassword();

    expect(component.hidePassword).toBe(!initial);
  });

});