import { Component } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { FormBuilder,FormGroup,Validators } from '@angular/forms';
@Component({
  selector: 'app-model-2',
  imports: [MaterialModule],
  templateUrl: './model-2.html',
  styleUrl: './model-2.scss',
})
export class Model2 {
 quickForm!:FormGroup;
 platformOptions = ['CMF', 'Joint Recon', 'Trauma'];
 constructor(private builder:FormBuilder){
  this.quickForm=this.builder.group(({
   customerName: [''],
   platforms: [[]]
  }))
 }

 generateAndProceed(){
  const selectedPlatforms=this.quickForm.value.platforms;
  const platForm=selectedPlatforms.map((x:string)=>{
    let base=100;
    if(x==='Trauma') base=70;
    if(x==='Joint ') base=80;
    
  })

 }

 getSegments(p:string){
  if (p === 'CMF') return ['Dental', 'Neuro'];
  if (p === 'Trauma') return ['Spine'];
  return ['General']
 }

 getDefaultDates(){
  return {
     start: '2025-02',
    end: '2026-01'
  }
 }
}
