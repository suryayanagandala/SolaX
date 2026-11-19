import { Component, Inject } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-raise-request-content',
  standalone: true,
  imports: [MaterialModule, FormsModule,CommonModule],
  templateUrl: './raise-request-content.html',
  styleUrl: './raise-request-content.scss',
})
export class RaiseRequestContent {

  description: string = '';

  constructor(
    private matdialogRef: MatDialogRef<RaiseRequestContent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  trimInput() {
    if (this.description) {
      this.description = this.description.trim();
    }
  }

  isValidReason(): boolean {
  return !!this.description &&
         this.description.trim().length >= 10;
}

  close() {
    this.matdialogRef.close();
  }

  submit() {

    const cleaned = this.description.trim();

    if (cleaned.length < 10) {
      return;
    }

    this.matdialogRef.close({
      description: cleaned
    });
  }
}