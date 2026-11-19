import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TechnicianDashboard } from './technician-dashboard';
import { TechnicianService } from '../../../Shared/technician-service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { TechnicianRole } from '../../../Models/Models';

describe('TechnicianDashboard Component', () => {

  let component: TechnicianDashboard;
  let fixture: ComponentFixture<TechnicianDashboard>;

  let technicianServiceMock: jasmine.SpyObj<TechnicianService>;
  let toastrMock: jasmine.SpyObj<ToastrService>;
  let routerMock: jasmine.SpyObj<Router>;

  beforeEach(async () => {

    technicianServiceMock = jasmine.createSpyObj('TechnicianService', [
      'TechinicianDashbaord',
      'getWorkStats',
      'getMonthlyUpdates',
      'getEmployeeRevenue',
      'getEmployeeEfficinecy'
    ]);

    toastrMock = jasmine.createSpyObj('ToastrService', ['error']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    technicianServiceMock.TechinicianDashbaord.and.returnValue(
      of({
        status: true,
        data: {
          pendingCount: 2,
          outForDeliveryCount: 1,
          deliveredTodayCount: 3,
          totalActiveDeliveries: 5,
          completedCount: 4,
          specilization: TechnicianRole.Installer
        }
      })
    );

   
     technicianServiceMock.getWorkStats.and.returnValue(
      of({
        status:true
      })
    );

    technicianServiceMock.getMonthlyUpdates.and.returnValue(
      of({
        status: true,
        data: [
          { month: 'Jan', completedJobs: 5 },
          { month: 'Feb', completedJobs: 8 }
        ]
      })
    );

    technicianServiceMock.getEmployeeRevenue.and.returnValue(
      of({
        status: true,
        data: [
          { month: 'Jan', revenue: 2000 },
          { month: 'Feb', revenue: 3500 }
        ]
      })
    );

    technicianServiceMock.getEmployeeEfficinecy.and.returnValue(
      of({
        status: true,
        data: {
          efficiency: 75,
          totalJobs: 20,
          completedJobs: 15
        }
      })
    );

    await TestBed.configureTestingModule({
      imports: [TechnicianDashboard],
      providers: [
        { provide: TechnicianService, useValue: technicianServiceMock },
        { provide: ToastrService, useValue: toastrMock },
        { provide: Router, useValue: routerMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(TechnicianDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should load dashboard data', () => {

    component.getDashboardDat();

    expect(technicianServiceMock.TechinicianDashbaord).toHaveBeenCalled();
    expect(component.dashboard).toBeDefined();
  });

  it('should set role from dashboard data', () => {

    component.getDashboardDat();

    expect(component.role).toBe(TechnicianRole.Installer);
  });

  it('should show error when dashboard API fails', () => {

    technicianServiceMock.TechinicianDashbaord.and.returnValue(
      of({ status: false })
    );

    component.getDashboardDat();

    expect(toastrMock.error).toHaveBeenCalled();
  });

  it('should calculate work status chart', () => {

  technicianServiceMock.getWorkStats.and.returnValue(
    of({
      status: true,
      data: {
        assigned: 5,
        pending: 3,
        completed: 10
      }
    })
  );

  component.getWorkStatus();

  expect(technicianServiceMock.getWorkStats).toHaveBeenCalled();
  expect(component.assigned).toBe(5);
  expect(component.pending).toBe(3);
  expect(component.completed).toBe(10);

});

  it('should load monthly completed jobs chart', () => {

    component.monthlyChart();

    expect(technicianServiceMock.getMonthlyUpdates).toHaveBeenCalled();
    expect(component.monthlyOptions).toBeDefined();
  });

  it('should load revenue chart', () => {

    component.getRevenue();

    expect(technicianServiceMock.getEmployeeRevenue).toHaveBeenCalled();
    expect(component.monthlyRevenue).toBeDefined();
  });

  it('should load efficiency gauge', () => {

    component.getEfficiency();

    expect(technicianServiceMock.getEmployeeEfficinecy).toHaveBeenCalled();
    expect(component.efficiencyOptions).toBeDefined();
  });

  it('should calculate efficiency stats', () => {

    component.getEfficiency();

    expect(component.totalJobs).toBe(20);
    expect(component.completedJobs).toBe(15);
  });

  it('should check card visibility', () => {

    component.role = TechnicianRole.Installer;

    const card = {
      roles: [TechnicianRole.Installer]
    };

    const result = component.isCardVisible(card);

    expect(result).toBeTrue();
  });

});