import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../MaterialModule';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  template: `
    <h2 mat-dialog-title>Confirm Cancellation</h2>

    <mat-dialog-content>
      <p>Are you sure you want to cancel this order?</p>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="close(false)">No</button>
      <button mat-raised-button color="warn" (click)="close(true)">
        Yes, Cancel
      </button>
    </mat-dialog-actions>
  `
})
export class ConfirmDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  close(result: boolean) {
    this.dialogRef.close(result);
  }
}