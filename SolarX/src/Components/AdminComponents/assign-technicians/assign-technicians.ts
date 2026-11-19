import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { Auth } from '../../../Shared/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-assign-technicians',
  standalone: true,
  imports: [MaterialModule, ReactiveFormsModule, CommonModule],
  templateUrl: './assign-technicians.html',
  styleUrl: './assign-technicians.scss'
})
export class AssignTechnicians implements OnInit {

  technicianForm!: FormGroup;
  userId!: string;
  assignedTechnican: any = {};

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
    private service: Auth
  ) { }

  specializations = [
    { value: 1, label: 'Installer' },
    { value: 2, label: 'Maintenance' },
    { value: 3, label: 'Electrician' },
    { value: 4, label: 'Supervisor' }
  ];

  qualifications = [
    { value: 1, label: 'Diploma' },
    { value: 2, label: 'BTech' },
    { value: 3, label: 'ITI' }
  ];

  disciplines = [
    { value: 1, label: 'Electrical' },
    { value: 2, label: 'Mechanical' },
    { value: 3, label: 'Civil' },
    { value: 4, label: 'Other' }
  ];

  ngOnInit(): void {

    
     this.route.queryParams.subscribe(params => {
      if (params['userId']) {
        this.userId = String(params['userId']);
        
        
      }})

   
    this.initializeForm();

    
    if (this.userId) {
      this.loadTechnicianDetails();
    }

    
    this.technicianForm.valueChanges.subscribe(values => {
      this.assignedTechnican = { ...values }
    });

  }

  initializeForm() {

    this.technicianForm = this.fb.group({

      fullName: ['', [Validators.required, Validators.pattern(/^[A-Za-z]+( [A-Za-z]+)*$/)]],

      emailId: ['', [Validators.required, Validators.email]],

      phoneNumber: ['', [Validators.required, Validators.pattern(/^[6-9]\d{9}$/)]],

      address: ['', [Validators.required, Validators.pattern(/\S+/)]],

      specialization: ['', Validators.required],

      qualification: ['', Validators.required],

      discipline: ['', Validators.required],

      experienceYears: ['', [Validators.required, Validators.min(0)]],

      salary: ['', [Validators.required, Validators.min(0)]],

      servicePrice: ['', [Validators.required, Validators.min(0)]]

    });

  }

  disableFieldsForEdit() {

    this.technicianForm.get('fullName')?.disable();
    this.technicianForm.get('emailId')?.disable();
    this.technicianForm.get('phoneNumber')?.disable();
    this.technicianForm.get('address')?.disable();
    this.technicianForm.get('qualification')?.disable();
    this.technicianForm.get('specialization')?.disable();
    this.technicianForm.get('discipline')?.disable();

  }

  loadTechnicianDetails() {

    this.service.getTechnicianData(this.userId).subscribe({
      next: (res: any) => {

        if (res.status) {

          this.technicianForm.patchValue(res.data);

          // disable fields after patch
          this.disableFieldsForEdit();

        }

      },
      error: () => {
        this.toastr.error("Failed to load technician");
      }
    });

  }

  assign() {

    if (this.technicianForm.invalid) {
      this.toastr.error("Please fill all required fields");
      return;
    }

    const dto = {
      ...this.technicianForm.getRawValue()
    };

    if (this.userId) {

      // update technician
      this.service.updateTechnicianProfile(this.userId, dto).subscribe({
        next: () => {
          this.toastr.success("Technician updated successfully");
          this.router.navigate(['admin/admin-dashboard']);
        },
        error: () => {
          this.toastr.error("Update failed");
        }
      });

    } else {

      // assign technician
      this.service.AssignedTechnician(dto).subscribe({
        next: () => {
          this.toastr.success("Technician assigned successfully");
          this.router.navigate(['admin/admin-dashboard']);
        },
        error: () => {
          this.toastr.error("Failed to assign technician");
        }
      });

    }

  }

  getSpecializationLabel(value: any) {
    const s = this.specializations.find(x => x.value == value);
    return s ? s.label : '';
  }

  getQualificationLabel(value: any) {
    const q = this.qualifications.find(x => x.value == value);
    return q ? q.label : '';
  }

  getDisciplineLabel(value: any) {
    const d = this.disciplines.find(x => x.value == value);
    return d ? d.label : '';
  }

}