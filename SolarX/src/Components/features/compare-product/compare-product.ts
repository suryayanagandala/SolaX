import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { MatDialogRef,MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-comapre-product',
  imports: [MaterialModule,CommonModule],
  templateUrl: './compare-product.html',
  styleUrl: './compare-product.scss',
})
export class CompareProduct implements OnInit{
  displayedText='';
  isTyping:boolean=false;
constructor(private dialogRef:MatDialogRef<CompareProduct>,
  //inject predefined objcet
  @Inject(MAT_DIALOG_DATA) public data:any
){}
ngOnInit(): void {
  this.startTypingEffect(this.data.aiResult,(text)=>{
    this.displayedText=text;
  })
}
close(){
  this.dialogRef.close();
}
 startTypingEffect(text:string,callback:(val:string)=>void){
  let index=0;
  this.isTyping = true; 
  const interval=setInterval(()=>{
    callback(text.substring(0,index+1))
    index++;

    if(index===text.length){
      clearInterval(interval);
      this.isTyping=false;
    }
  },30)
 }
}
