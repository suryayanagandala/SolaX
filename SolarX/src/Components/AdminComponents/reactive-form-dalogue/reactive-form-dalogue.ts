import { Component } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-reactive-form-dalogue',
  imports: [MaterialModule, ReactiveFormsModule, CommonModule],
  templateUrl: './reactive-form-dalogue.html',
  styleUrl: './reactive-form-dalogue.scss',
})
export class ReactiveFormDalogue {

  reasonForm!: FormGroup;

  constructor(
    private builder: FormBuilder,
    private dialog: MatDialogRef<ReactiveFormDalogue>
  ) {
    this.reasonForm = this.builder.group({
      reason: ['', [Validators.required, Validators.minLength(5)]]
    });
  }

  trimInput() {
    const value = this.reasonForm.get('reason')?.value || '';
    this.reasonForm.get('reason')?.setValue(value.trim());
  }

  isValidReason(): boolean {
    const reason = this.reasonForm.get('reason')?.value || '';
    return reason.trim().length >= 10;
  }

  close() {
    this.dialog.close();
  }

  submit() {

    const reason = this.reasonForm.get('reason')?.value || '';
    const cleaned = reason.trim();

    if (cleaned.length < 10) {
      return;
    }

    this.dialog.close({
      reason: cleaned
    });
  }
}