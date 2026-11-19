import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { CartService } from '../../../Shared/cart-service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ToastrService } from 'ngx-toastr';
import { TokenService } from '../../../Shared/token';
import { Router, RouterModule } from '@angular/router';
import { CartDto } from '../../../Models/Models';
import { ChangeDetectorRef } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { environment } from '../../../environments/environment';
import { localMessages } from '../../../localMessages';

@Component({
  selector: 'app-cart',
  standalone:true,
  imports: [MaterialModule,MatProgressSpinnerModule,CommonModule,DecimalPipe,RouterModule],
  templateUrl: './cart.html',
  styleUrl: './cart.scss',
})
export class Cart implements OnInit {

  cart: CartDto | null = null;

  userId:string|null=null;;

  subTotal = 0;
  shipping = 100;
  total = 0;

  private messages=localMessages

  constructor(
    private service: CartService,
    private toastr: ToastrService,
    private authService: TokenService,
    private router: Router,private cd:ChangeDetectorRef,
    private cdr:ChangeDetectorRef
  ) {}

  ngOnInit(): void {
   this.userId = this.authService.GetUserId();

    if (!this.userId) {
      this.toastr.warning(this.messages.warnings.login);
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
    } else {
      this.getUserCartDetails(this.userId);
    }
  }

  getUserCartDetails(userId: string) {
    this.service.getUserCart(userId).subscribe(res => {
      if (res.status) {
        this.cart = res.data;
        this.cdr.detectChanges();
        console.log('cart-details',this.cart);
        this.calculateTotalAmount();
        
      }
    });
  }

  calculateTotalAmount(){
    if(!this.cart){
      return;
    }

    this.subTotal=this.cart.items?.reduce((sum,item)=>sum+(item.quantity*item.price),0)??0;
    this.total=this.subTotal+this.shipping;
  }

  increaseQty(cartItemId:number){
  if(!this.cart) return;

  const item = this.cart.items?.find(x=>x.cartItemId==cartItemId);
  if(!item) return;

  const dto={
    cartItemId:item.cartItemId,
    quantity:item.quantity + 1   
  };

  this.service.updateCart(dto).subscribe((res)=>{
    if(res.status){
      this.getUserCartDetails(this.userId!); 
    }
  });
}

 decreaseQty(i:number){
  if(!this.cart) return;

  const item=this.cart.items![i];
  if(item.quantity <= 1) return;

  const dto={
    cartItemId:item.cartItemId,
    quantity:item.quantity - 1   
  };

  this.service.updateCart(dto).subscribe((res)=>{
    if(res.status){
      this.getUserCartDetails(this.userId!); 
    }
  });
}


 removeCart(cartItemId:number){
  if(!this.cart) return;

  this.service.removeCart(cartItemId).subscribe((res)=>{
    if(res.status){
      this.getUserCartDetails(this.userId!);  
    }
  });
}

   getImageUrl(url?: string): string {
      if (!url) {
        return 'Assets/Themes/panelimage1.png';
      }
  
      if (url.startsWith('http')) {
        return url;
      }
  
      return environment.imageUrl + url;
    }

}
