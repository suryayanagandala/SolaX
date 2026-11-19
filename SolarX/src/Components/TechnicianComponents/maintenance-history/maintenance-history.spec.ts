import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MaintenanceHistory } from './maintenance-history';
import { MaintenanceService } from '../../../Shared/maintenance-service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('MaintenanceHistory', () => {
  let component: MaintenanceHistory;
  let fixture: ComponentFixture<MaintenanceHistory>;

  let serviceSpy: jasmine.SpyObj<MaintenanceService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {

    serviceSpy = jasmine.createSpyObj('MaintenanceService', ['getCompletedMaintenances']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    serviceSpy.getCompletedMaintenances.and.returnValue(
      of({
        status: true,
        data: [{ id: 'M1' }, { id: 'M2' }]
      })
    );

    await TestBed.configureTestingModule({
      imports: [MaintenanceHistory],
      providers: [
        { provide: MaintenanceService, useValue: serviceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MaintenanceHistory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load completed maintenances on init', () => {
    expect(serviceSpy.getCompletedMaintenances).toHaveBeenCalled();
    expect(component.dto.length).toBe(2);
  });

  it('should assign maintenance data on success response', () => {

    component.getCompletedTasks();

    expect(component.dto).toEqual([{ id: 'M1' }, { id: 'M2' }]);
  });

  it('should show error toast on failure response', () => {

    serviceSpy.getCompletedMaintenances.and.returnValue(
      of({ status: false })
    );

    component.getCompletedTasks();

    expect(toastrSpy.error).toHaveBeenCalled();
  });

});

