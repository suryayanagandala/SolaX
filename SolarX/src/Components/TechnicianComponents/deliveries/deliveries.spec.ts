import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Deliveries } from './deliveries';
import { TechnicianService } from '../../../Shared/technician-service';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { TokenService } from '../../../Shared/token';
import { of } from 'rxjs';

describe('Deliveries', () => {

  let component: Deliveries;
  let fixture: ComponentFixture<Deliveries>;

  let technicianServiceSpy: jasmine.SpyObj<TechnicianService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let tokenSpy: jasmine.SpyObj<TokenService>;

  beforeEach(async () => {

    technicianServiceSpy = jasmine.createSpyObj('TechnicianService', [
      'GetDeliveries',
      'markAsDelivered',
      'markAsDeliveryStataus'
    ]);

    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    routerSpy = jasmine.createSpyObj('Router', ['navigate'], { url: '/panel/deliveries' });

    tokenSpy = jasmine.createSpyObj('TokenService', ['GetUserId']);

    const activatedRouteMock = {};

    tokenSpy.GetUserId.and.returnValue('USER1');

    technicianServiceSpy.GetDeliveries.and.returnValue(
      of({
        status: true,
        data: [
          { deliveryId: 1, status: 'Pending' },
          { deliveryId: 2, status: 'OutForDelivery' }
        ]
      })
    );

    technicianServiceSpy.markAsDelivered.and.returnValue(
      of({ status: true })
    );

    technicianServiceSpy.markAsDeliveryStataus.and.returnValue(
      of({ status: true })
    );

    await TestBed.configureTestingModule({
      imports: [Deliveries],
      providers: [
        { provide: TechnicianService, useValue: technicianServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: TokenService, useValue: tokenSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Deliveries);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load deliveries on init', () => {
    expect(technicianServiceSpy.GetDeliveries).toHaveBeenCalled();
    expect(component.deliveryDto.length).toBe(2);
  });

  it('should call markAsDelivered when status is OutForDelivery', () => {

    const item = { deliveryId: 2, status: 'OutForDelivery' };

    component.changeStatus(item);

    expect(technicianServiceSpy.markAsDelivered).toHaveBeenCalledWith(2);

  });

  it('should call changeDeliveryStatus when status is not OutForDelivery', () => {

    const item = { deliveryId: 1, status: 'Pending' };

    component.changeStatus(item);

    expect(technicianServiceSpy.markAsDeliveryStataus).toHaveBeenCalledWith(1);

  });

  it('should navigate to login if userId not found', () => {

    tokenSpy.GetUserId.and.returnValue(null as any);

    component.ngOnInit();

    expect(routerSpy.navigate).toHaveBeenCalledWith(
      ['/login'],
      {
        queryParams: {
          returnUrl: routerSpy.url
        }
      }
    );

  });

});

