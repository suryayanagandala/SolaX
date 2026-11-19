import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Notifications } from './notifications';
import { of, BehaviorSubject } from 'rxjs';
import { NotificatoinState } from '../../Shared/notificatoin-state';
import { Notification } from '../../Shared/notification';
import { TokenService } from '../../Shared/token';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { NotifcationDto } from '../../Models/Models';

describe('Notifications Component', () => {

  let component: Notifications;
  let fixture: ComponentFixture<Notifications>;

  let notificationStateMock: jasmine.SpyObj<NotificatoinState>;
  let notificationServiceMock: jasmine.SpyObj<Notification>;
  let tokenServiceMock: jasmine.SpyObj<TokenService>;

  let notificationSubject: BehaviorSubject<NotifcationDto[]>;

  beforeEach(async () => {

    notificationSubject = new BehaviorSubject<NotifcationDto[]>([]);

    notificationStateMock = jasmine.createSpyObj(
      'NotificatoinState',
      ['markAllRead'],
      { notification$: notificationSubject.asObservable() }
    );

    notificationServiceMock = jasmine.createSpyObj('Notification', ['markAllAsRead']);

    tokenServiceMock = jasmine.createSpyObj('TokenService', ['GetUserId']);

    tokenServiceMock.GetUserId.and.returnValue('user123');

    await TestBed.configureTestingModule({
      imports: [Notifications],
      providers: [
        { provide: NotificatoinState, useValue: notificationStateMock },
        { provide: Notification, useValue: notificationServiceMock },
        { provide: TokenService, useValue: tokenServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(Notifications);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should set userId on init', () => {

    component.ngOnInit();

    expect(component.userId).toBe('user123');
  });

  it('should subscribe and load notifications from state', () => {

    const mockNotifications: NotifcationDto[] = [
      { id: 1, message: 'Test1', isRead: false },
      { id: 2, message: 'Test2', isRead: true }
    ] as any;

    notificationSubject.next(mockNotifications);

    expect(component.notifications.length).toBe(2);
  });

  it('should return all notifications when filter is all', () => {

    component.notifications = [
      { id: 1, message: 'A', isRead: false },
      { id: 2, message: 'B', isRead: true }
    ] as any;

    component.filter = 'all';

    const result = component.filteredNotifications;

    expect(result.length).toBe(2);
  });

  it('should return only unread notifications when filter is unread', () => {

    component.notifications = [
      { id: 1, message: 'A', isRead: false },
      { id: 2, message: 'B', isRead: true }
    ] as any;

    component.filter = 'unread';

    const result = component.filteredNotifications;

    expect(result.length).toBe(1);
    expect(result[0].isRead).toBeFalse();
  });

  it('should call markAllRead service', () => {

    component.userId = 'user123';

    notificationServiceMock.markAllAsRead.and.returnValue(of({status:true}));

    component.markAllRead();

    expect(notificationServiceMock.markAllAsRead).toHaveBeenCalledWith('user123');
  });

  it('should update state after marking all as read', () => {

    component.userId = 'user123';

    notificationServiceMock.markAllAsRead.and.returnValue(of({status: true,}));

    component.markAllRead();

    expect(notificationStateMock.markAllRead).toHaveBeenCalled();
  });

});