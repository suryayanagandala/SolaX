import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Dashboard } from './dashboard';
import { RouterTestingModule } from '@angular/router/testing';
import { SolarPerformanceService } from '../../../Shared/solar-performance';
import { TokenService } from '../../../Shared/token';
import { NotificationSignalrService } from '../../../Shared/signal-r';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('Dashboard', () => {

  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;

  const solarServiceMock = {
    getPerformance: jasmine.createSpy().and.returnValue(of({
      status: true,
      data: [
        { generated_at: new Date(), power: 120 }
      ]
    })),
    getMonthlyUpdates: jasmine.createSpy().and.returnValue(of({
      status: true,
      data: [
        { date: new Date(), totalPower: 1000 }
      ]
    })),
    solarSummaryDashboard: jasmine.createSpy().and.returnValue(of({
      status: true,
      data: {}
    })),
    getActivatedSolarPanles: jasmine.createSpy().and.returnValue(of({
      status: true,
      data: [{ panelId: 1 }]
    })),
    getPanelDetails: jasmine.createSpy().and.returnValue(of({
      status: true,
      data: { efficiency: 85 }
    }))
  };

  const tokenServiceMock = {
    GetUserId: jasmine.createSpy().and.returnValue('1')
  };

  const signalMock = {
    onReceivePerformance: jasmine.createSpy(),
    stopConnection: jasmine.createSpy()
  };

  const toastrMock = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy(),
    warning: jasmine.createSpy()
  };

  beforeEach(async () => {

    await TestBed.configureTestingModule({
      imports: [
        Dashboard,
        RouterTestingModule
      ],
      providers: [
        { provide: SolarPerformanceService, useValue: solarServiceMock },
        { provide: TokenService, useValue: tokenServiceMock },
        { provide: NotificationSignalrService, useValue: signalMock },
        { provide: ToastrService, useValue: toastrMock }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize empty chart', () => {
    component.initializeEmptyChart();
    expect(component.chartOptions).toBeTruthy();
  });

  it('should load initial performance data', () => {
    component.loadInitialData();
    expect(solarServiceMock.getPerformance).toHaveBeenCalled();
  });

  it('should calculate monthly updates', () => {
    component.loadMonthlyUpdates(1);
    expect(solarServiceMock.getMonthlyUpdates).toHaveBeenCalled();
  });

  it('should load activated panels', () => {
    component.getActivatedSolarPanles();
    expect(solarServiceMock.getActivatedSolarPanles).toHaveBeenCalled();
  });

});