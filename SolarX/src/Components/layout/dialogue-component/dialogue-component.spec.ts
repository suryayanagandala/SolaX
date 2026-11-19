import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConfirmDialogComponent } from './dialogue-component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';

describe('ConfirmDialogComponent', () => {
  let component: ConfirmDialogComponent;
  let fixture: ComponentFixture<ConfirmDialogComponent>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<ConfirmDialogComponent>>;

  beforeEach(async () => {
    //  spy
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [ConfirmDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: { orderId: 123 } } 
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should close with true when "Yes, Cancel" button is clicked', () => {
    const yesButton = fixture.debugElement.queryAll(By.css('button'))[1];
    yesButton.triggerEventHandler('click', null);

    expect(dialogRefSpy.close).toHaveBeenCalledWith(true);
  });

  it('should close with false when "No" button is clicked', () => {
    const noButton = fixture.debugElement.queryAll(By.css('button'))[0];
    noButton.triggerEventHandler('click', null);

    expect(dialogRefSpy.close).toHaveBeenCalledWith(false);
  });

  it('should close with correct value when close() is called directly', () => {
    component.close(true);
    expect(dialogRefSpy.close).toHaveBeenCalledWith(true);

    component.close(false);
    expect(dialogRefSpy.close).toHaveBeenCalledWith(false);
  });

  it('should have correct injected data', () => {
    expect(component.data).toEqual({ orderId: 123 });
  });
});