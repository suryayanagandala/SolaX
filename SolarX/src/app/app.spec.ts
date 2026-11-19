import { TestBed, ComponentFixture } from '@angular/core/testing';
import { App } from './app';
import { TokenService } from '../Shared/token';
import { NotificationSignalrService } from '../Shared/signal-r';
import { Notification } from '../Shared/notification';
import { NotificatoinState } from '../Shared/notificatoin-state';
import { ChangeDetectorRef } from '@angular/core';

// Mock classes
class MockTokenService {
  getToken() { return 'mock-token'; }
  GetUserId() { return 'user123'; }
}
class MockNotificationSignalrService { startConnection(token: string) {} }
class MockNotification {}
class MockNotificatoinState { loadNotifications(userId: string, notificationService: Notification) {} }
class MockChangeDetectorRef { detectChanges() {} }

describe('App Component', () => {
  let fixture: ComponentFixture<App>;
  let component: App;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App], // <-- Standalone components go here
      providers: [
        { provide: TokenService, useClass: MockTokenService },
        { provide: NotificationSignalrService, useClass: MockNotificationSignalrService },
        { provide: Notification, useClass: MockNotification },
        { provide: NotificatoinState, useClass: MockNotificatoinState },
        { provide: ChangeDetectorRef, useClass: MockChangeDetectorRef }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should call startConnection and loadNotifications if token and userId exist', () => {
    const tokenSpy = spyOn(component['token'], 'getToken').and.returnValue('token123');
    const userIdSpy = spyOn(component['token'], 'GetUserId').and.returnValue('user123');
    const signalSpy = spyOn(component['signalService'], 'startConnection');
    const loadSpy = spyOn(component['notificationState'], 'loadNotifications');
    const cdSpy = spyOn(component['cd'], 'detectChanges');

    component.ngOnInit();

    expect(tokenSpy).toHaveBeenCalled();
    expect(userIdSpy).toHaveBeenCalled();
    expect(signalSpy).toHaveBeenCalledWith('token123');
    expect(loadSpy).toHaveBeenCalledWith('user123', component['notificationService']);
    expect(cdSpy).toHaveBeenCalled();
  });

  it('should not call startConnection if token or userId is missing', () => {
    spyOn(component['token'], 'getToken').and.returnValue(null);
    spyOn(component['token'], 'GetUserId').and.returnValue(null);

    const signalSpy = spyOn(component['signalService'], 'startConnection');
    const loadSpy = spyOn(component['notificationState'], 'loadNotifications');

    component.ngOnInit();

    expect(signalSpy).not.toHaveBeenCalled();
    expect(loadSpy).not.toHaveBeenCalled();
  });

  
});