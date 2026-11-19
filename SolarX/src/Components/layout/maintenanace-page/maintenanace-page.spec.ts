import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MaintenanacePage } from './maintenanace-page';
import { of } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { MaintenanceService } from '../../../Shared/maintenance-service';
import { TokenService } from '../../../Shared/token';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../environments/environment';

describe('MaintenanacePage', () => {
  let component: MaintenanacePage;
  let fixture: ComponentFixture<MaintenanacePage>;

  const mockToastr = { error: jasmine.createSpy('error') };
  const mockService = {
    MaintenanceDataForCustomerPage: jasmine.createSpy('MaintenanceDataForCustomerPage')
      .and.returnValue(of({ status: true, data: [{ installationId: 1, quantity: 5, requestedQuantity: 0 }] })),
    raiseRequest: jasmine.createSpy('raiseRequest').and.returnValue(of({ status: true }))
  };
  const mockToken = { GetUserId: jasmine.createSpy('GetUserId').and.returnValue('user123') };
  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of({ description: 'Fix panel' })
    })
  };
  const mockSnackBar = { open: jasmine.createSpy('open') };
  const mockRouter = { navigate: jasmine.createSpy('navigate') };
  const mockActivatedRoute = { snapshot: {}, params: of({}) }; 

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MaintenanacePage],
      providers: [
        { provide: ToastrService, useValue: mockToastr },
        { provide: MaintenanceService, useValue: mockService },
        { provide: TokenService, useValue: mockToken },
        { provide: MatDialog, useValue: mockDialog },
        { provide: MatSnackBar, useValue: mockSnackBar },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockActivatedRoute } 
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MaintenanacePage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.userId).toBe('user123');
    expect(component.dto.length).toBe(1);
  });

  it('should open dialog and call request', fakeAsync(() => {
    const item = component.dto[0];
    component.openDialogueCompoenet(item);
    tick();
    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockService.raiseRequest).toHaveBeenCalledWith({
      installationId: 1,
      userId: 'user123',
      description: 'Fix panel',
      quantity: 1
    });
    expect(mockSnackBar.open).toHaveBeenCalledWith(
      'Maintenance request submitted successfully',
      'Close',
      jasmine.any(Object)
    );
  }));

  it('should return true for fully requested item', () => {
    const item = { requestedQuantity: 5, quantity: 5 };
    expect(component.isFullyRequested(item)).toBeTrue();
  });

  it('should return default image if url is missing', () => {
    expect(component.getImageUrl()).toBe('Assets/Themes/panelimage1.png');
  });

  it('should return full url if image path is relative', () => {
    expect(component.getImageUrl('panel.jpg')).toBe(environment.imageUrl + 'panel.jpg');
  });

  it('should return the same url if it starts with http', () => {
    expect(component.getImageUrl('http://example.com/panel.jpg')).toBe('http://example.com/panel.jpg');
  });
});