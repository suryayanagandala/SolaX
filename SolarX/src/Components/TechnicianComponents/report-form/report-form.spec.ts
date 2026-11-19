import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReportForm } from './report-form';
import { of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { InstallationService } from '../../../Shared/installation-service';
import { ToastrService } from 'ngx-toastr';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';

describe('ReportForm', () => {
  let component: ReportForm;
  let fixture: ComponentFixture<ReportForm>;

  let routerSpy: jasmine.SpyObj<Router>;
  let installationServiceSpy: jasmine.SpyObj<InstallationService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    installationServiceSpy = jasmine.createSpyObj('InstallationService', ['uploadReport']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    const activatedRouteMock = {
      queryParams: of({ installationId: '123' })
    };

    installationServiceSpy.uploadReport.and.returnValue(
      of({ status: true })
    );

    await TestBed.configureTestingModule({
      imports: [ReportForm, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: InstallationService, useValue: installationServiceSpy },
        { provide: ToastrService, useValue: toastrSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ReportForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize installationId from query params', () => {
    expect(component.installationId).toBe('123');
  });

  it('should invalidate form when required fields missing', () => {
    component.reportForm.patchValue({
      notes: '',
    });

    expect(component.reportForm.invalid).toBeTrue();
  });

  it('should validate mandatory checklist items', () => {

    const checklist = component.reportForm.get('checkList');

    checklist?.patchValue({
      PanelMounted: true,
      InverterInstalled: true,
      WiringChecked: true,
      EarthingDone: true,
      DCIsolationChecked: true,
      ACIsolationChecked: true
    });

    expect(checklist?.valid).toBeTrue();
  });

  it('should call uploadReport on valid submit', () => {

    component.reportForm.patchValue({
      installationId: '123',
      notes: 'This is a valid installation report with enough length',
      images: [new File(['test'], 'test.png')]
    });

    const checklist = component.reportForm.get('checkList');
    checklist?.patchValue({
      PanelMounted: true,
      InverterInstalled: true,
      WiringChecked: true,
      EarthingDone: true,
      DCIsolationChecked: true,
      ACIsolationChecked: true
    });

    component.slectedImages = [new File(['test'], 'test.png')];

    component.submitForm();

    expect(installationServiceSpy.uploadReport).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalled();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/panel/installations']);
  });

  it('should remove image correctly', () => {

    const file = new File(['test'], 'test.png');

    component.slectedImages = [file];
    component.previewImages = ['data:image/png;base64,test'];

    component.removeImage(0);

    expect(component.slectedImages.length).toBe(0);
    expect(component.previewImages.length).toBe(0);
  });

});