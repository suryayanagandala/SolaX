import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { InstallationService } from '../../../Shared/installation-service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { localMessages } from '../../../localMessages';

@Component({
  selector: 'app-report-form',
  imports: [MaterialModule,ReactiveFormsModule,CommonModule,RouterModule],
  templateUrl: './report-form.html',
  styleUrl: './report-form.scss',
})
export class ReportForm implements OnInit{
  reportForm!:FormGroup;
  installationId!:string;
  slectedImages:File[]=[];
  previewImages:string[]=[];
  private messages=localMessages

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
  mandatoryKeys = [
  'PanelMounted',
  'InverterInstalled',
  'WiringChecked',
  'EarthingDone',
  'DCIsolationChecked',
  'ACIsolationChecked'
];

  constructor(private toastr:ToastrService,private router:Router,private route:ActivatedRoute,
    private service:InstallationService,private builder:FormBuilder,private cd:ChangeDetectorRef
  ){}
  ngOnInit(): void {
    this.route.queryParams.subscribe(params=>{
      const id=params['installationId'];
      if(id){
        this.installationId=id;
        
      }
      else{
        this.router.navigate(['/installations'])
      }
    })

    this.reportForm=this.builder.group({
      installationId:this.builder.control(this.installationId,Validators.required),
      notes:this.builder.control('',[Validators.required,Validators.minLength(20),this.noOnlySpaces]),
      images: [null, Validators.required],
      checkList:this.builder.group(
        this.checkListItems.reduce((acc:any,item)=>{
          acc[item.key]=[false];
          return acc;
        },{}),
        {validators:this.checkMandatoryItems.bind(this)}
      )
    })
  }

  noOnlySpaces(control: AbstractControl) {
  if (!control.value) return null;

  const isOnlySpaces = control.value.trim().length === 0;
  return isOnlySpaces ? { onlySpaces: true } : null;
}

  checkMandatoryItems(form:FormGroup){
    const values=form.value;
    const allMandatoryCompleted=this.mandatoryKeys.every(key=>values[key]===true);

    return allMandatoryCompleted?null:{mandatoryIncomplete:true}
  }
  

  onImageSelected(event:Event){
    const input=event.target as HTMLInputElement;
    if(!input.files||input.files.length===0){
      this.toastr.error("please select image")
      this.clearImages()
      return;
    }
     
    this.slectedImages=Array.from(input.files);

    this.slectedImages.forEach(item=>{
      const reader=new FileReader();
      reader.onload=()=>{
        this.previewImages.push(reader.result as string)
      };
      reader.readAsDataURL(item);
    });

    this.reportForm.get('images')?.setValue(this.slectedImages);
    this.reportForm.get('images')?.markAsTouched();
  }

  submitForm(){
    if(this.reportForm.invalid){
       this.reportForm.markAllAsTouched();
      return;
    }

    const formData=new FormData();

    formData.append('InstallationId',this.reportForm.value.installationId);
    formData.append('notes',this.reportForm.value.notes);
    formData.append('checkList',this.reportForm.value.checkList)

    this.slectedImages.forEach(img=>{
      formData.append('images',img)
    })
    
   
    this.service.uploadReport(formData).subscribe((res)=>{
      if(res.status){
        console.log(res.data);
        this.toastr.success(this.messages.installation.success);
        this.router.navigate(['/panel/installations'])
        this.cd.detectChanges();
        
      }
        else{
        console.log(res.error);
        this.toastr.error(this.messages.error.fail)
      }
    })
  }
  removeImage(index:number){
   this.slectedImages.splice(index,1);
   this.previewImages.splice(index,1);
   this.reportForm.get('images')?.setValue(
    this.slectedImages.length?this.slectedImages:null
   )
  }

  clearImages() {
  this.slectedImages = [];
  this.previewImages = [];
  this.reportForm.get('images')?.setValue(null);
}


}
