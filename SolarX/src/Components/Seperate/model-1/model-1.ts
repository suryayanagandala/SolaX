import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-model-1',
  imports: [MaterialModule, ReactiveFormsModule, CommonModule],
  templateUrl: './model-1.html',
  styleUrl: './model-1.scss',
})
export class Model1 implements OnInit {

  stepForm1!: FormGroup;
  stepForm2!: FormGroup
  stepForm3!: FormGroup
  stepForm4!: FormGroup
  platformOptions = ['CMF', 'Joint Recon', 'Trauma', 'Dental'];
  usageModes = ['Admin', 'Customer', 'Internal'];
  platforms = ['CMF', 'Joint Recon', 'Trauma'];

  segmentsList = ['Other', 'Dental', 'Spine', 'Neuro'];
  constructor(private toastr: ToastrService, private builder: FormBuilder) {


    this.stepForm1 = this.builder.group({
      platformTools: [[]],
      summaryTools: [[]],
      removeListPrice: [false],
      showSplit: [true],
      showDps: [true]
    });

    this.stepForm2 = this.builder.group({
      customerName: ['Apollo Hospital'],
      showFooter: [false],
      notes: [{ value: 'Preferred customer configuration for demo', disabled: true }]
    });

    this.stepForm3 = this.builder.group({
      showTam: [true],
      showDates: [true],
      tamValue: ['$0.00'],
      startDate: ['2025-02'],
      endDate: ['2026-01']
    });

    this.stepForm4 = this.builder.group({
      platforms: this.builder.array([])
    })


  }

  ngOnInit(): void {
    this.platforms.forEach(x => {
      this.platformArray.push(
        this.builder.group({
          name: [x],
          segments: [[]],
          customerReported: [0],
          adminEstimated: [0],
          fileReported: [0]
        })
      )
    })

    const value = this.stepForm2.get('showFooter');

    value?.valueChanges.subscribe(val => {
      if (val) {
        this.stepForm2.get('notes')?.enable({ emitEvent: false });
      }
      else {
        this.stepForm2.get('notes')?.disable({ emitEvent: false });
      }
    })
  }




  get platformArray() {
    return this.stepForm4.get('platforms') as any;
  }
  removePlatform(platform: string) {
    const updated = (this.stepForm1.value.platformTools || [])
      .filter((p: string) => p !== platform);

    this.stepForm1.get('platformTools')?.setValue(updated);
  }
  removeSummary(summary: string) {
    const updated = (this.stepForm1.value.summaryTools || []).filter((x: string) => x !== summary);
    this.stepForm1.get('summaryTools')?.setValue(updated);
  }

 removeSegments(index: number, segment: string) {
  const platformGroup = this.platformArray.at(index);

  const current = platformGroup.get('segments')?.value || [];

  const updated = current.filter((s: string) => s !== segment);

  platformGroup.get('segments')?.setValue(updated);
}

 get summaryData(){
  return {
    platformSetup: this.stepForm1.value,
    customer: this.stepForm2.value,
    global: this.stepForm3.value,
    platforms: this.stepForm4.value.platforms
  };
 }

 submit(){
  this.toastr.success('data saved succesfully');
 }

}
