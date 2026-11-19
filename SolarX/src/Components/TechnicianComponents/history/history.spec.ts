import { ComponentFixture, TestBed } from '@angular/core/testing';
import { History } from './history';
import { TechnicianService } from '../../../Shared/technician-service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('History', () => {

  let component: History;
  let fixture: ComponentFixture<History>;

  let technicianServiceSpy: jasmine.SpyObj<TechnicianService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {

    technicianServiceSpy = jasmine.createSpyObj('TechnicianService', [
      'getDeliveredOrders'
    ]);

    toastrSpy = jasmine.createSpyObj('ToastrService', [
      'success',
      'error'
    ]);

    routerSpy = jasmine.createSpyObj('Router', [
      'navigate'
    ]);

    technicianServiceSpy.getDeliveredOrders.and.returnValue(
      of({
        status: true,
        data: [
          { orderId: 'O1' },
          { orderId: 'O2' }
        ]
      })
    );

    await TestBed.configureTestingModule({
      imports: [History],
      providers: [
        { provide: TechnicianService, useValue: technicianServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(History);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load delivered orders on init', () => {
    expect(technicianServiceSpy.getDeliveredOrders).toHaveBeenCalled();
    expect(component.deliveredOrdersDto.length).toBe(2);
  });

  it('should assign delivered orders when API returns success', () => {

    component.deliveredOrders();

    expect(component.deliveredOrdersDto).toEqual([
      { orderId: 'O1' },
      { orderId: 'O2' }
    ]);

  });

  it('should show error toast when API fails', () => {

    technicianServiceSpy.getDeliveredOrders.and.returnValue(
      of({
        status: false
      })
    );

    component.deliveredOrders();

    expect(toastrSpy.error).toHaveBeenCalled();

  });

});

