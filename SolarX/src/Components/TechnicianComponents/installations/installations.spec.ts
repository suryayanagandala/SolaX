import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Installations } from './installations';
import { InstallationService } from '../../../Shared/installation-service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NotificationSignalrService } from '../../../Shared/signal-r';
import { of } from 'rxjs';

describe('Installations', () => {

  let component: Installations;
  let fixture: ComponentFixture<Installations>;

  let installationServiceSpy: jasmine.SpyObj<InstallationService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {

    installationServiceSpy = jasmine.createSpyObj('InstallationService', [
      'getInstallationStatus',
      'updateStatus'
    ]);

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    toastrSpy = jasmine.createSpyObj('ToastrService', [
      'success',
      'error'
    ]);

    const signalMock = {}; 

    installationServiceSpy.getInstallationStatus.and.returnValue(
      of({
        status: true,
        data: [
          { installationId: 'I1' },
          { installationId: 'I2' }
        ]
      })
    );

    installationServiceSpy.updateStatus.and.returnValue(
      of({
        status: true,
       
      })
    );

    await TestBed.configureTestingModule({
      imports: [Installations],
      providers: [
        { provide: InstallationService, useValue: installationServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: NotificationSignalrService, useValue: signalMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Installations);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load installations on init', () => {
    expect(installationServiceSpy.getInstallationStatus).toHaveBeenCalled();
    expect(component.installationDto.length).toBe(2);
  });

  it('should assign installation data on success response', () => {

    component.getInstallations();

    expect(component.installationDto).toEqual([
      { installationId: 'I1' },
      { installationId: 'I2' }
    ]);

  });

  it('should call updateStatus when action is not Mark as Completed', () => {

    const report = {
      installationId: 'I1',
      actions: 'In Progress'
    };

    component.UpdatedInstallationStatus(report);

    expect(installationServiceSpy.updateStatus).toHaveBeenCalledWith('I1');

  });

  it('should navigate to report form when action is Mark as Completed', () => {

    const report = {
      installationId: 'I10',
      actions: 'Mark as Completed'
    };

    component.UpdatedInstallationStatus(report);

    expect(routerSpy.navigate).toHaveBeenCalledWith(
      ['panel/report-form'],
      {
        queryParams: {
          installationId: 'I10'
        }
      }
    );

  });

});

