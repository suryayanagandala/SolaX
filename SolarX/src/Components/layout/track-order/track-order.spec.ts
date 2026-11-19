import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TrackOrder } from './track-order';
import { RouterTestingModule } from '@angular/router/testing';
import { OrderService } from '../../../Shared/order-service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('TrackOrder', () => {

  let component: TrackOrder;
  let fixture: ComponentFixture<TrackOrder>;

  const orderServiceMock = {
    trackOrder: jasmine.createSpy().and.returnValue(of({
      status: true,
      data: [
        { status: 'Completed' },
        { status: 'Active' },
        { status: 'Pending' }
      ]
    }))
  };

  const toastrMock = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy(),
    warning: jasmine.createSpy()
  };

  const activatedRouteMock = {
    queryParams: of({ orderId: 1 })
  };

  beforeEach(async () => {

    await TestBed.configureTestingModule({
      imports: [
        TrackOrder,
        RouterTestingModule
      ],
      providers: [
        { provide: OrderService, useValue: orderServiceMock },
        { provide: ToastrService, useValue: toastrMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(TrackOrder);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call trackOrder on init', () => {
    component.ngOnInit();
    expect(orderServiceMock.trackOrder).toHaveBeenCalledWith(1);
  });

  it('should set tracking data', () => {
    component.trackOrder(1);
    expect(component.trackDto.length).toBeGreaterThan(0);
  });

  it('should return correct step status', () => {

    component.trackDto = [
      { status: 'Completed' } as any,
      { status: 'Active' } as any,
      { status: 'Pending' } as any
    ];

    expect(component.changeStep(0)).toBe('completed');
    expect(component.changeStep(1)).toBe('active');
    expect(component.changeStep(2)).toBe('pending');
  });

});