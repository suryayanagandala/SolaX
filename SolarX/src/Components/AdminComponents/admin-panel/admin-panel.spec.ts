import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminPanel } from './admin-panel';
import { RouterTestingModule } from '@angular/router/testing';
import { ToastrService } from 'ngx-toastr';
import { TokenService } from '../../../Shared/token';
import { Notification } from '../../../Shared/notification';
import { NotificatoinState } from '../../../Shared/notificatoin-state';
import { of } from 'rxjs';
import { By } from '@angular/platform-browser';

describe('AdminPanel', () => {
  let component: AdminPanel;
  let fixture: ComponentFixture<AdminPanel>;
  let routerNavigateSpy: jasmine.Spy;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;
  let notificationServiceSpy: jasmine.SpyObj<Notification>;
  let stateSpy: jasmine.SpyObj<NotificatoinState>;

  beforeEach(async () => {
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success']);
    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['GetUserId', 'clear']);
    notificationServiceSpy = jasmine.createSpyObj('Notification', ['markAllAsRead', 'markAsRead']);
    stateSpy = jasmine.createSpyObj('NotificatoinState', ['markAllRead'], {
      notification$: of([]),
      unreadnotification$: of(0)
    });

    await TestBed.configureTestingModule({
      imports: [AdminPanel, RouterTestingModule],
      providers: [
        { provide: ToastrService, useValue: toastrSpy },
        { provide: TokenService, useValue: tokenServiceSpy },
        { provide: Notification, useValue: notificationServiceSpy },
        { provide: NotificatoinState, useValue: stateSpy },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminPanel);
    component = fixture.componentInstance;

    // Spy router.navigate from RouterTestingModule
    routerNavigateSpy = spyOn((component as any).router, 'navigate');
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should toggle theme', () => {
    expect(component.isDark).toBeFalse();
    component.toggleTheme();
    expect(component.isDark).toBeTrue();
    component.toggleTheme();
    expect(component.isDark).toBeFalse();
  });

  it('should logout user', () => {
    component.logout();
    expect(tokenServiceSpy.clear).toHaveBeenCalled();
    expect(routerNavigateSpy).toHaveBeenCalledWith(['/login']);
    expect(toastrSpy.success).toHaveBeenCalledWith('logout successfully!');
  });

  it('should toggle notification dropdown and mark all as read', () => {
    tokenServiceSpy.GetUserId.and.returnValue('user123');
    component.userId = 'user123';

    notificationServiceSpy.markAllAsRead.and.returnValue(of({status:true}));
    component.toggleNotification(new MouseEvent('click'));

    expect(component.isDropdownOpen).toBeTrue();
    expect(notificationServiceSpy.markAllAsRead).toHaveBeenCalledWith('user123');
    expect(stateSpy.markAllRead).toHaveBeenCalled();
  });

  it('should close dropdown on document click', () => {
    component.isDropdownOpen = true;
    component.closeDropdown();
    expect(component.isDropdownOpen).toBeFalse();
  });

  it('should get recent notifications', () => {
    const notifications = [
      { notificationId: 1, isRead: false } as any,
      { notificationId: 2, isRead: true } as any,
      { notificationId: 3, isRead: false } as any,
      { notificationId: 4, isRead: true } as any
    ];
    (stateSpy.notification$ as any) = of(notifications);
    component.notification = notifications;

    expect(component.recentNotifications.length).toBe(3);
    expect(component.recentNotifications).toEqual(notifications.slice(0, 3));
  });

  it('should navigate to notifications page', () => {
    component.goToNotifications();
    expect(routerNavigateSpy).toHaveBeenCalledWith(['/notification']);
    expect(component.isDropdownOpen).toBeFalse();
  });

  it('should mark notification as read if unread', () => {
    const item = { notificationId: 5, isRead: false } as any;
    notificationServiceSpy.markAsRead.and.returnValue(of({status:true}));

    component.openNotification(item);
    expect(notificationServiceSpy.markAsRead).toHaveBeenCalledWith(5);
  });

  it('should not mark notification as read if already read', () => {
    const item = { notificationId: 5, isRead: true } as any;
    component.openNotification(item);
    expect(notificationServiceSpy.markAsRead).not.toHaveBeenCalled();
  });
});