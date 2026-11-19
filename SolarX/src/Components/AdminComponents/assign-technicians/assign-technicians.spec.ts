import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AssignTechnicians } from './assign-technicians';
import { Auth } from '../../../Shared/auth';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { of } from 'rxjs';

describe('AssignTechnicians', () => {
  let component: AssignTechnicians;
  let fixture: ComponentFixture<AssignTechnicians>;
  let authSpy: jasmine.SpyObj<Auth>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let routeStub: Partial<ActivatedRoute>;

  beforeEach(async () => {
    authSpy = jasmine.createSpyObj('Auth', [
      'getTechnicianData',
      'updateTechnicianProfile',
      'AssignedTechnician'
    ]);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    routeStub = { queryParams: of({ userId: 'T123' }) };

    await TestBed.configureTestingModule({
      imports: [AssignTechnicians],
      providers: [
        { provide: Auth, useValue: authSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: routeStub },
        FormBuilder
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AssignTechnicians);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load technician details if userId exists', () => {
    const mockData = { fullName: 'John', emailId: 'john@test.com' };
    authSpy.getTechnicianData.and.returnValue(of({ status: true, data: mockData }));

    component.ngOnInit();

    expect(authSpy.getTechnicianData).toHaveBeenCalledWith('T123');
    expect(component.technicianForm.get('fullName')?.value).toBe('John');
  });

  it('should assign technician when userId is not set', () => {
    authSpy.AssignedTechnician.and.returnValue(of({ status: true }));

    component.userId = '';
    component.initializeForm();
    component.technicianForm.patchValue({
      fullName: 'Test', emailId: 'test@test.com', phoneNumber: '9876543210',
      address: 'Some address', specialization: 1, qualification: 1,
      discipline: 1, experienceYears: 2, salary: 50000, servicePrice: 200
    });

    component.assign();

    expect(authSpy.AssignedTechnician).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalledWith('Technician assigned successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['admin/admin-dashboard']);
  });

  it('should update technician when userId exists', () => {
    authSpy.updateTechnicianProfile.and.returnValue(of({ status: true }));

    component.userId = 'T123';
    component.initializeForm();
    component.technicianForm.patchValue({
      fullName: 'Test', emailId: 'test@test.com', phoneNumber: '9876543210',
      address: 'Some address', specialization: 1, qualification: 1,
      discipline: 1, experienceYears: 2, salary: 50000, servicePrice: 200
    });

    component.assign();

    expect(authSpy.updateTechnicianProfile).toHaveBeenCalledWith('T123', jasmine.any(Object));
    expect(toastrSpy.success).toHaveBeenCalledWith('Technician updated successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['admin/admin-dashboard']);
  });

  it('should disable fields after patching technician details', () => {
    const mockData = { fullName: 'John', emailId: 'john@test.com', phoneNumber: '9876543210', address: 'Addr', specialization: 1, qualification: 1, discipline: 1, experienceYears: 2, salary: 1000, servicePrice: 200 };
    authSpy.getTechnicianData.and.returnValue(of({ status: true, data: mockData }));

    component.ngOnInit();

    expect(component.technicianForm.get('fullName')?.disabled).toBeTrue();
    expect(component.technicianForm.get('emailId')?.disabled).toBeTrue();
  });
});