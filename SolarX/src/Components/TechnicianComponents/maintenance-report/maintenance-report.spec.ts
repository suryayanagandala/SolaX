import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MaintenanceReport } from './maintenance-report';
import { ActivatedRoute, Router } from '@angular/router';
import { MaintenanceService } from '../../../Shared/maintenance-service';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';

describe('MaintenanceReport', () => {
  let component: MaintenanceReport;
  let fixture: ComponentFixture<MaintenanceReport>;

  let routerSpy: jasmine.SpyObj<Router>;
  let serviceSpy: jasmine.SpyObj<MaintenanceService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    serviceSpy = jasmine.createSpyObj('MaintenanceService', ['uploadReport']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    const activatedRouteMock = {
      queryParams: of({ requestId: 'REQ123' })
    };

    serviceSpy.uploadReport.and.returnValue(
      of({ status: true })
    );

    await TestBed.configureTestingModule({
      imports: [MaintenanceReport, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: MaintenanceService, useValue: serviceSpy },
        { provide: ToastrService, useValue: toastrSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MaintenanceReport);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize requestId from query params', () => {
    expect(component.requestId).toBe('REQ123');
  });

  it('should invalidate form when required fields missing', () => {
    component.reportForm.patchValue({
      notes: ''
    });

    expect(component.reportForm.invalid).toBeTrue();
  });

  it('should validate replacedParts checklist (at least one required)', () => {

    const replacedParts = component.reportForm.get('replacedParts');

    replacedParts?.patchValue({
      PanelMounted: true
    });

    expect(replacedParts?.valid).toBeTrue();
  });

  it('should remove image correctly', () => {

    const file = new File(['test'], 'test.png');

    component.selectedImages = [file];
    component.previewImages = ['data:image/png;base64,test'];

    component.removeImage(0);

    expect(component.selectedImages.length).toBe(0);
    expect(component.previewImages.length).toBe(0);
  });

  it('should call uploadReport on valid submit', () => {

    component.reportForm.patchValue({
      requestId: 'REQ123',
      notes: 'Maintenance completed successfully with detailed report',
      images: [new File(['test'], 'test.png')]
    });

    const replacedParts = component.reportForm.get('replacedParts');
    replacedParts?.patchValue({
      PanelMounted: true
    });

    component.selectedImages = [new File(['test'], 'test.png')];

    component.submitForm();

    expect(serviceSpy.uploadReport).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalled();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/panel/maintenanace-jobs']);
  });

  it('should show error toast when API fails', () => {

    serviceSpy.uploadReport.and.returnValue(
      of({ status: false, error: 'Failed' })
    );

    component.reportForm.patchValue({
      requestId: 'REQ123',
      notes: 'Maintenance completed successfully with detailed report',
      images: [new File(['test'], 'test.png')]
    });

    const replacedParts = component.reportForm.get('replacedParts');
    replacedParts?.patchValue({
      PanelMounted: true
    });

    component.selectedImages = [new File(['test'], 'test.png')];

    component.submitForm();

    expect(toastrSpy.error).toHaveBeenCalled();
  });

});