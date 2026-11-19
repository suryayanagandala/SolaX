import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../MaterialModule';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-delete-reason-dialog-component',
  imports: [MaterialModule,FormsModule],
  templateUrl: './delete-reason-dialog-component.html',
  styleUrl: './delete-reason-dialog-component.scss',
})
export class DeleteReasonDialogComponent {
 reason='';

 constructor(@Inject(MAT_DIALOG_DATA) public data: { reason: string },
  private dialog:MatDialogRef<DeleteReasonDialogComponent>){
    this.reason = data.reason; 
  }

 submit(){
  if(!this.reason.trim()){
    return;
  }
  else{
    this.dialog.close(this.reason);
  }
 }

 cancel(){
  this.dialog.close(null);
 }
}
