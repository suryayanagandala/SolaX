import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TechnicianPanel } from './technician-panel';
import { of, BehaviorSubject } from 'rxjs';
import { TechnicianService } from '../../../Shared/technician-service';
import { TokenService } from '../../../Shared/token';
import { NotificatoinState } from '../../../Shared/notificatoin-state';
import { Notification } from '../../../Shared/notification';
import { NotificationSignalrService } from '../../../Shared/signal-r';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('TechnicianPanel Component', () => {

  let component: TechnicianPanel;
  let fixture: ComponentFixture<TechnicianPanel>;

  let technicianServiceMock: jasmine.SpyObj<TechnicianService>;
  let tokenServiceMock: jasmine.SpyObj<TokenService>;
  let notificationStateMock: jasmine.SpyObj<NotificatoinState>;
  let notificationServiceMock: jasmine.SpyObj<Notification>;
  let signalServiceMock: jasmine.SpyObj<NotificationSignalrService>;
  let routerMock: jasmine.SpyObj<Router>;
  let toastrMock: jasmine.SpyObj<ToastrService>;

  let notificationSubject: BehaviorSubject<any>;
  let unreadSubject: BehaviorSubject<number>;

  beforeEach(async () => {

    notificationSubject = new BehaviorSubject<any[]>([]);
    unreadSubject = new BehaviorSubject<number>(0);

    technicianServiceMock = jasmine.createSpyObj('TechnicianService',
      ['getTechnicianData', 'TechinicianDashbaord']);

    tokenServiceMock = jasmine.createSpyObj('TokenService',
      ['GetUser']);

    notificationStateMock = jasmine.createSpyObj(
      'NotificatoinState',
      ['markAllRead'],
      {
        notification$: notificationSubject.asObservable(),
        unreadnotification$: unreadSubject.asObservable()
      }
    );

    notificationServiceMock = jasmine.createSpyObj('Notification',
      ['markAllAsRead']);

    signalServiceMock = jasmine.createSpyObj('NotificationSignalrService',
      ['listen']);

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    toastrMock = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    tokenServiceMock.GetUser.and.returnValue({
      userId: 'tech123',
      specifications: 'Solar'
    });

    technicianServiceMock.getTechnicianData.and.returnValue(
      of({ status: true, data: { specifications: 'Solar' } })
    );

    technicianServiceMock.TechinicianDashbaord.and.returnValue(
      of({ status: true, data: {} })
    );

    notificationServiceMock.markAllAsRead.and.returnValue(
      of({ status: true })
    );

    await TestBed.configureTestingModule({
      imports: [TechnicianPanel],
      providers: [
        { provide: TechnicianService, useValue: technicianServiceMock },
        { provide: TokenService, useValue: tokenServiceMock },
        { provide: NotificatoinState, useValue: notificationStateMock },
        { provide: Notification, useValue: notificationServiceMock },
        { provide: NotificationSignalrService, useValue: signalServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: {} },
        { provide: ToastrService, useValue: toastrMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(TechnicianPanel);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize userId on init', () => {
    component.ngOnInit();
    expect(component.userId).toBe('tech123');
  });

  it('should load technician data', () => {
    component.getTechnicinaData('tech123');
    expect(technicianServiceMock.getTechnicianData).toHaveBeenCalledWith('tech123');
  });

  it('should call dashboard API', () => {
    component.GetDashboard();
    expect(technicianServiceMock.TechinicianDashbaord).toHaveBeenCalled();
  });

  it('should toggle theme', () => {

    const initial = component.isDark;

    component.toggleTheme();

    expect(component.isDark).toBe(!initial);
  });

  it('should logout and navigate to login', () => {

    spyOn(sessionStorage, 'clear');
    spyOn(localStorage, 'clear');

    component.logout();

    expect(sessionStorage.clear).toHaveBeenCalled();
    expect(localStorage.clear).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should subscribe notifications', () => {

    notificationSubject.next([{ message: 'test' }]);

    expect(component.notification.length).toBe(1);
  });

  it('should subscribe unread notification count', () => {

    unreadSubject.next(5);

    expect(component.notificationCount).toBe(5);
  });

  it('should mark all notifications as read', () => {

    component.userId = 'tech123';

    component.markAllread();

    expect(notificationServiceMock.markAllAsRead).toHaveBeenCalledWith('tech123');
    expect(notificationStateMock.markAllRead).toHaveBeenCalled();
  });

  it('should toggle notification dropdown', () => {

    component.userId = 'tech123';

    const event = new MouseEvent('click');
    spyOn(event, 'stopPropagation');

    component.toggleNotification(event);

    expect(component.isDropdownOpen).toBeTrue();
    expect(event.stopPropagation).toHaveBeenCalled();
  });

  it('should close dropdown on document click', () => {

    component.isDropdownOpen = true;

    component.closeDropdown();

    expect(component.isDropdownOpen).toBeFalse();
  });

});