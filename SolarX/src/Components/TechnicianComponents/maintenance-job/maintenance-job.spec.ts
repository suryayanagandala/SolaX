
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MaintenanceJob } from './maintenance-job';
import { MaintenanceService } from '../../../Shared/maintenance-service';
import { TokenService } from '../../../Shared/token';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ChangeDetectorRef } from '@angular/core';
import { of } from 'rxjs';

describe('MaintenanceJob', () => {
  let component: MaintenanceJob;
  let fixture: ComponentFixture<MaintenanceJob>;

  let serviceSpy: jasmine.SpyObj<MaintenanceService>;
  let tokenSpy: jasmine.SpyObj<TokenService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {

    serviceSpy = jasmine.createSpyObj('MaintenanceService', ['technicianMaintenanceJobs']);
    tokenSpy = jasmine.createSpyObj('TokenService', ['GetUserId']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    tokenSpy.GetUserId.and.returnValue('USER123');

    serviceSpy.technicianMaintenanceJobs.and.returnValue(
      of({
        status: true,
        data: [{ id: 'REQ1' }, { id: 'REQ2' }]
      })
    );

    await TestBed.configureTestingModule({
      imports: [MaintenanceJob],
      providers: [
        { provide: MaintenanceService, useValue: serviceSpy },
        { provide: TokenService, useValue: tokenSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ToastrService, useValue: toastrSpy },
        ChangeDetectorRef
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MaintenanceJob);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get userId from token on init', () => {
    expect(tokenSpy.GetUserId).toHaveBeenCalled();
    expect(component.userId).toBe('USER123');
  });

  it('should load assigned jobs', () => {
    component.getAssignedJobs('USER123');

    expect(serviceSpy.technicianMaintenanceJobs).toHaveBeenCalledWith('USER123');
    expect(component.dto.length).toBe(2);
  });

  it('should navigate to report form with requestId', () => {

    component.reportForm('REQ123');

    expect(routerSpy.navigate).toHaveBeenCalledWith(
      ['/panel/maintenance-form'],
      {
        queryParams: {
          requestId: 'REQ123'
        }
      }
    );
  });

});

