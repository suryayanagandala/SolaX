import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RaiseRequestContent } from './raise-request-content';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

describe('RaiseRequestContent', () => {
  let component: RaiseRequestContent;
  let fixture: ComponentFixture<RaiseRequestContent>;
  const matDialogRefMock = {
    close: jasmine.createSpy('close')
  };

  const matDialogDataMock = {
    installationId: 'INST123',
    quantity: 2
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RaiseRequestContent],
      providers: [
        { provide: MatDialogRef, useValue: matDialogRefMock },
        { provide: MAT_DIALOG_DATA, useValue: matDialogDataMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RaiseRequestContent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.data).toEqual(matDialogDataMock);
  });

  it('should trim input correctly', () => {
    component.description = '   some text   ';
    component.trimInput();
    expect(component.description).toBe('some text');
  });

  it('should validate description correctly', () => {
    component.description = 'short';
    expect(component.isValidReason()).toBeFalse();

    component.description = 'This is valid description';
    expect(component.isValidReason()).toBeTrue();
  });

  it('should close dialog without data', () => {
    component.close();
    expect(matDialogRefMock.close).toHaveBeenCalledWith();
  });

  it('should submit description if valid', () => {
    component.description = 'Valid description';
    component.submit();
    expect(matDialogRefMock.close).toHaveBeenCalledWith({ description: 'Valid description' });
  });

  it('should not submit if description is too short', () => {
    component.description = 'short';
    component.submit();
    expect(matDialogRefMock.close).not.toHaveBeenCalledWith({ description: 'short' });
  });
});