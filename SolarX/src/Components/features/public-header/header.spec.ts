import { ComponentFixture, TestBed } from '@angular/core/testing';
import { publicHeader } from './header';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of, BehaviorSubject } from 'rxjs';

import { TokenService } from '../../../Shared/token';
import { NotificationSignalrService } from '../../../Shared/signal-r';
import { NotificatoinState } from '../../../Shared/notificatoin-state';
import { CartService } from '../../../Shared/cart-service';
import { Notification } from '../../../Shared/notification';
import { ChangeDetectorRef } from '@angular/core';

describe('publicHeader', () => {
  let component: publicHeader;
  let fixture: ComponentFixture<publicHeader>;
  let router: Router;

  // Mock Subjects
  const cartCountSubject = new BehaviorSubject<number>(0);
  const notificationSubject = new BehaviorSubject<any[]>([]);
  const unreadSubject = new BehaviorSubject<number>(0);

  // Mock Services
  const tokenServiceMock = {
    GetUsername: jasmine.createSpy().and.returnValue('testuser@gmail.com'),
    GetUserId: jasmine.createSpy().and.returnValue('123'),
    getToken: jasmine.createSpy().and.returnValue('token123'),
    clear: jasmine.createSpy()
  };

  const signalServiceMock = {
    cartCount$: cartCountSubject
  };

  const notificationStateMock = {
    notification$: notificationSubject,
    unreadnotification$: unreadSubject,
    markAllRead: jasmine.createSpy()
  };

  const cartServiceMock = {
    cartCount: jasmine.createSpy().and.returnValue(of({ status: true, data: 5 }))
  };

  const notificationServiceMock = {
    markAllAsRead: jasmine.createSpy().and.returnValue(of({})),
    getAllMessages: jasmine.createSpy().and.returnValue(
      of({ status: true, data: [] })
    ),
    markAsRead: jasmine.createSpy().and.returnValue(of({}))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        publicHeader,
        RouterTestingModule
      ],
      providers: [
        { provide: TokenService, useValue: tokenServiceMock },
        { provide: NotificationSignalrService, useValue: signalServiceMock },
        { provide: NotificatoinState, useValue: notificationStateMock },
        { provide: CartService, useValue: cartServiceMock },
        { provide: Notification, useValue: notificationServiceMock },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(publicHeader);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);

    spyOn(router, 'navigate');

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize username correctly', () => {
    expect(component.username).toBe('testus..');
  });

  it('should return true if logged in', () => {
    expect(component.isLoggedIn()).toBeTrue();
  });

  it('should logout and navigate to home', () => {
    component.logout();

    expect(tokenServiceMock.clear).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should navigate to login', () => {
    component.goToLogin();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should navigate to register', () => {
    component.goToRegister();
    expect(router.navigate).toHaveBeenCalledWith(['/register']);
  });

  it('should emit menu click event', () => {
    spyOn(component.menuClick, 'emit');

    component.toggleMenu();

    expect(component.menuClick.emit).toHaveBeenCalled();
  });

  it('should toggle notification dropdown', () => {
    const event = new MouseEvent('click');

    component.toggleNotification(event);

    expect(component.isDropdownOpen).toBeTrue();
  });

  it('should close dropdown when clicking outside', () => {
    component.isDropdownOpen = true;

    component.closeDropdown();

    expect(component.isDropdownOpen).toBeFalse();
  });

  it('should toggle dark theme', () => {
    component.toggleTheme();

    expect(component.isDark).toBeTrue();
  });

  it('should navigate to notifications page', () => {
    component.goToNotifications();

    expect(router.navigate).toHaveBeenCalledWith(['/notification']);
  });

  it('should call markAsRead when opening unread notification', () => {
    const notification = {
      notificationId: '1',
      isRead: false
    } as any;

    component.openNotification(notification);

    expect(notificationServiceMock.markAsRead).toHaveBeenCalledWith('1');
  });

  it('should not call markAsRead if notification already read', () => {
    const notification = {
      notificationId: '1',
      isRead: true
    } as any;

    component.openNotification(notification);

    expect(notificationServiceMock.markAsRead).not.toHaveBeenCalled();
  });

  it('should update cart count from service', () => {
    component.cartCount('123');

    expect(cartServiceMock.cartCount).toHaveBeenCalledWith('123');
  });

});