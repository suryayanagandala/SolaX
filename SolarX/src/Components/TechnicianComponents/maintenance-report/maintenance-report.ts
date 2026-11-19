import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InstallationService } from '../../../Shared/installation-service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { localMessages } from '../../../localMessages';
import { MaintenanceService } from '../../../Shared/maintenance-service';

@Component({
  selector: 'app-report-form',
  standalone: true,
  imports: [MaterialModule, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './maintenance-report.html',
  styleUrl: './maintenance-report.scss',
})
export class MaintenanceReport implements OnInit {

  reportForm!: FormGroup;
  requestId!: string;
  selectedImages: File[] = [];
  previewImages: string[] = [];
  private messages = localMessages;

  checkListItems = [
    { key: 'PanelMounted', label: 'Panel Mounted' },
    { key: 'InverterInstalled', label: 'Inverter Installed' },
    { key: 'WiringChecked', label: 'Wiring Checked' },
    { key: 'EarthingDone', label: 'Earthing Done' },
    { key: 'NetMeterInstalled', label: 'Net Meter Installed' },
    { key: 'DCIsolationChecked', label: 'DC Isolation Checked' },
    { key: 'ACIsolationChecked', label: 'AC Isolation Checked' },
    { key: 'CableTied', label: 'Cables Tied' },
    { key: 'PanelCleaned', label: 'Panels Cleaned' },
    { key: 'SiteCleaned', label: 'Site Cleaned' }
  ];

  constructor(
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
    private service: MaintenanceService,
    private builder: FormBuilder
  ) { }

  ngOnInit(): void {

    this.route.queryParams.subscribe(params => {
      const id = params['requestId'];
      if (id) {
        this.requestId = id;
      } else {
        this.router.navigate(['/panel/maintenanace-jobs']);
      }
    });

    this.reportForm = this.builder.group({
      requestId: [this.requestId, Validators.required],
      notes: ['', [Validators.required, Validators.minLength(20),this.noOnlySpaces]],
      images: [null, Validators.required],
      replacedParts: this.builder.group(
        this.checkListItems.reduce((acc: any, item) => {
          acc[item.key] = [false];
          return acc;
        }, {}),
        { validators: this.atLeastOneChecked }
      )
    });
  }
  noOnlySpaces(control: AbstractControl) {
  if (!control.value) return null;

  const isOnlySpaces = control.value.trim().length === 0;
  return isOnlySpaces ? { onlySpaces: true } : null;
}
  
  atLeastOneChecked(form: FormGroup) {
    const values = form.value;
    const oneSelected = Object.values(values).some(value => value === true);
    return oneSelected ? null : { atleastOneRequired: true };
  }

  onImageSelected(event: Event) {
  const input = event.target as HTMLInputElement;

  if (!input.files || input.files.length === 0) {
    return;
  }

  this.selectedImages = Array.from(input.files);
  this.previewImages = [];

  this.selectedImages.forEach(file => {
    const reader = new FileReader();
    reader.onload = () => {
      this.previewImages.push(reader.result as string);
    };
    reader.readAsDataURL(file);
  });

  const imageControl = this.reportForm.get('images');
  imageControl?.setValue(this.selectedImages);
  imageControl?.updateValueAndValidity();
}


  removeImage(index: number) {
  this.selectedImages.splice(index, 1);
  this.previewImages.splice(index, 1);

  const imageControl = this.reportForm.get('images');

  if (this.selectedImages.length === 0) {
    imageControl?.setValue(null);
    imageControl?.markAsTouched();
  } else {
    imageControl?.setValue(this.selectedImages);
    imageControl?.updateValueAndValidity();
  }
}


  submitForm() {

    if (this.reportForm.invalid) {
      this.reportForm.markAllAsTouched();
      return;
    }

    const formData = new FormData();

    formData.append('requestId', this.reportForm.value.requestId);
    formData.append('notes', this.reportForm.value.notes);
    formData.append('replacedParts', JSON.stringify(this.reportForm.value.replacedParts));

    this.selectedImages.forEach(img => {
      formData.append('Images', img);
    });

    this.service.uploadReport(formData).subscribe(res => {
      if (res.status) {
        this.toastr.success(this.messages.installation.success);
        this.router.navigate(['/panel/maintenanace-jobs']);
      } else {
        this.toastr.error(this.messages.error.fail);
        console.log(res.error);
        
      }
    });
  }

}
