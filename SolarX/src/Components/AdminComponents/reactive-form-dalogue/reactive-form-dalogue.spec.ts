import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormDalogue } from './reactive-form-dalogue';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

describe('ReactiveFormDalogue', () => {
  let component: ReactiveFormDalogue;
  let fixture: ComponentFixture<ReactiveFormDalogue>;

  // Mock MatDialogRef
  const dialogRefMock = {
    close: jasmine.createSpy('close')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormDalogue],
      providers: [
        FormBuilder,
        { provide: MatDialogRef, useValue: dialogRefMock } 
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ReactiveFormDalogue);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should trim input correctly', () => {
    component.reasonForm.get('reason')?.setValue('   test   ');
    component.trimInput();
    expect(component.reasonForm.get('reason')?.value).toBe('test');
  });

  it('should return valid reason only if length >= 10', () => {
    component.reasonForm.get('reason')?.setValue('short');
    expect(component.isValidReason()).toBeFalse();
    component.reasonForm.get('reason')?.setValue('sufficient length');
    expect(component.isValidReason()).toBeTrue();
  });

  it('should call dialog close on submit with valid reason', () => {
    component.reasonForm.get('reason')?.setValue('valid reason text');
    component.submit();
    expect(dialogRefMock.close).toHaveBeenCalledWith({
      reason: 'valid reason text'
    });
  });

  it('should not close dialog on submit if reason too short', () => {
    dialogRefMock.close.calls.reset();
    component.reasonForm.get('reason')?.setValue('short');
    component.submit();
    expect(dialogRefMock.close).not.toHaveBeenCalled();
  });
});