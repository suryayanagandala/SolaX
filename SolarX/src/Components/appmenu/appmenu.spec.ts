import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Appmenu } from './appmenu';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { TokenService } from '../../Shared/token';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { NotificationSignalrService } from '../../Shared/signal-r';
import { ToastrModule } from 'ngx-toastr';

import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('Appmenu', () => {

  let component: Appmenu;
  let fixture: ComponentFixture<Appmenu>;
  let router: Router;

  const tokenServiceMock = {
    GetUsername: jasmine.createSpy().and.returnValue('testuser@gmail.com'),
    GetUserId: jasmine.createSpy().and.returnValue('123'),
    clear: jasmine.createSpy()
  };

  const notificationMock = {
    startConnection: jasmine.createSpy(),
    addListener: jasmine.createSpy()
  };

  beforeEach(async () => {

    await TestBed.configureTestingModule({
      imports: [
        Appmenu,
        RouterTestingModule,ToastrModule.forRoot(),HttpClientTestingModule
      ],
      providers: [
        { provide: TokenService, useValue: tokenServiceMock },
        { provide: NotificationSignalrService, useValue: notificationMock }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(Appmenu);
    component = fixture.componentInstance;

    router = TestBed.inject(Router);
    spyOn(router, 'navigate');

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize username on ngOnInit', () => {
    component.ngOnInit();
    expect(component.username).toBe('testus..');
  });

  it('should set userId on ngOnInit', () => {
    component.ngOnInit();
    expect(component.userId).toBe('123');
  });

  it('should toggle sidenav', () => {
    component.sideNavOpen = false;
    component.toggleSideNav();
    expect(component.sideNavOpen).toBeTrue();
  });

  it('should logout and navigate to home', () => {
    component.logout();

    expect(tokenServiceMock.clear).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });

});