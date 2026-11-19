import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { MatDialogRef,MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject } from '@angular/core';

@Component({
  selector: 'app-why-this',
  imports: [MaterialModule],
  templateUrl: './why-this.html',
  styleUrl: './why-this.scss',
})
export class WhyThis implements OnInit{
  isTyping:boolean=false;
  displayedText='';

  constructor(private dialogRef:MatDialogRef<WhyThis>,@Inject(MAT_DIALOG_DATA)public data:any){}
 
  ngOnInit(): void {
    this.startTypingEffect(this.data.aiResult,(val)=>{
      this.displayedText=val;
    })
    
  }

  startTypingEffect(text:string,callback:(val:string)=>void){
    this.isTyping=true;
    this.displayedText='';
    let index=0;

    const interval=setInterval(()=>{
     callback(text.substring(0,index+1))
     index++;
     if(index===text.length){
      clearInterval(interval);
      this.isTyping=false;
     }
    },40)
  }

  close(){
    this.dialogRef.close();
  }
}
