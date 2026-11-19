import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminDashboard } from './admin-dashboard';
import { Auth } from '../../../Shared/auth';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('AdminDashboard', () => {
  let component: AdminDashboard;
  let fixture: ComponentFixture<AdminDashboard>;

  // Mocks
  const authMock = {
    getStats: jasmine.createSpy('getStats').and.returnValue(of({ status: true, data: {} })),
    getProductionData: jasmine.createSpy('getProductionData').and.returnValue(of({ status: true, data: { changePercent: 0, monthlyTrend: [], dailyTrend: [] } })),
    getProductionTrend: jasmine.createSpy('getProductionTrend').and.returnValue(of({ status: true, data: [] })),
    getSoldProductsData: jasmine.createSpy('getSoldProductsData').and.returnValue(of({ status: true, data: [[]] })),
    getProductionUpdate: jasmine.createSpy('getProductionUpdate').and.returnValue(of({ status: true, data: { offlinePanels: 0, lowProductionPanels: 0, lowProductStock: [] } }))
  };

  const toastrMock = {
    error: jasmine.createSpy('error')
  };

  const routerMock = {
    navigate: jasmine.createSpy('navigate')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDashboard],
      providers: [
        { provide: Auth, useValue: authMock },
        { provide: ToastrService, useValue: toastrMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});