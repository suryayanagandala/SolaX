import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../../MaterialModule';
import { Router, RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HomeService } from '../../../Shared/home-category';
import { CartDto, Product } from '../../../Models/Models';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { MatTabsModule } from '@angular/material/tabs';
import { TokenService } from '../../../Shared/token';
import { localMessages } from '../../../localMessages';
import { CartService } from '../../../Shared/cart-service';
import { RecommendationResponseDto } from '../../../Models/Models';
import { MatDialog } from '@angular/material/dialog';
import { CompareProduct } from '../compare-product/compare-product';
import { WhyThis } from '../why-this/why-this';
import { catchError, concatMap, forkJoin, from, of } from 'rxjs';
@Component({
  selector: 'app-view-product-details',
  imports: [MaterialModule, CommonModule, MatTabsModule, RouterLink],
  templateUrl: './view-product-details.html',
  styleUrl: './view-product-details.scss',
})
export class ViewProductDetails implements OnInit {
  productDetails!: Product;
  selectedImage: any | null = null;
  quantity = 1;
  stock = 0;
  selectedTabIndex: number | null = null;
  expectedDate = new Date();
  today = new Date();
  productAnalyticsData: any;
  recommendations!: RecommendationResponseDto;
  products!: any;
  displayedText: string = '';
  isTyping: boolean = true;
  showExplaination: boolean = true;
  aiText: string = '';
  productId?: number;
  recommendedText: string = '';
  isImageLoaded: boolean = false;
  isRecommendedTyping: boolean = false;
  showCompareModal: boolean = false;
  compareBatteries: any[] = [];
  compareInverters: any[] = [];
  aiCompareResult: string = '';
  comboDto!: any;
  typedReason: string = '';
  private messages = localMessages;

  constructor(private route: ActivatedRoute, private router: Router, private toastr: ToastrService,
    private service: HomeService, private authService: TokenService, private cartService: CartService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const productId = params['productId']

      if (productId) {
        this.loadProductDetails(productId);
        this.getProductAnalytics(productId);
        this.getRecommendedProducts(productId);
        this.comboProducts(productId)
        this.productId = productId;
      }
      else {

        this.toastr.error(this.messages.error.fail);
        return;
      }
    })

    this.expectedDate?.setDate(this.today?.getDate() + 7);
  }


  loadProductDetails(proudctId: number) {
    this.service.viewProductDetails(proudctId).subscribe((res) => {
      if (res.status) {
        // console.log('product Details:', res.data);
        this.productDetails = res.data;
        this.selectedImage = this.productDetails.productImages?.[0]?.imageUrl ?? null;
        this.stock = this.productDetails.stock;
        console.log('res data:', this.productDetails);

      }
      else {
        console.log(res.error);

      }
    })
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

  getMainImage(): string {
    return this.getImageUrl(this.selectedImage);
  }

  selectImage(imageUrl?: string) {
    this.selectedImage = imageUrl ?? null
  }

  increaseQty() {
    if (this.quantity < this.stock) {
      this.quantity++;
    }
  }
  decreaseQty() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  onTabIndex(index: number) {
    this.selectedTabIndex = index;
  }

  addToCart() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
      return;
    }

    const userId = this.authService.GetUserId();
    if (!userId) {
      this.toastr.error(this.messages.warnings.login);
      return;
    }



    const dto: CartDto = {
      userId: userId,
      productId: this.productDetails.productId!,
      quantity: this.quantity
    };

    this.service.addToCart(dto).subscribe((res) => {
      if (res.status) {
        if (userId) {
          this.updateCartCount(userId)
        }
        this.toastr.success(this.messages.cart)
      }
      else {
        this.toastr.error(this.messages.error.fail);
        console.log(res.error);

      }
    })
  }

  buyNow() {

    const imageUrl =
      this.productDetails.productImages?.length
        ? this.getImageUrl(this.productDetails.productImages[0].imageUrl)
        : 'assets/no-image.png';
    const buyNowItem = {
      productId: this.productDetails.productId,
      productName: this.productDetails.productName,
      price: this.productDetails.price,
      quantity: this.quantity,
      imageUrl: imageUrl
    }

    this.router.navigate(['/order'], {
      state: { buyNowItem }
    })
  }

  getProductAnalytics(productId: number) {
    this.service.productAnalytics(productId).subscribe((res) => {
      if (res.status) {
        this.productAnalyticsData = res.data;

      }
    })
  }

  updateCartCount(userId: string) {
    this.cartService.updateCart(userId).subscribe((res) => {
      if (res.status) {

      }
      else {

      }
    })
  }

  onMouseMove(event: MouseEvent) {
    //GET the box
    const container = event.currentTarget as HTMLElement;

    const img = container.querySelector('img') as HTMLImageElement;

    //get all position and size if image 
    const rect = container.getBoundingClientRect();

    //convert mouse into box coordinates
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    //convert into percentage
    const xPercent = (x / rect.width) * 100;
    const yPercent = (y / rect.height) * 100;


    img.style.transformOrigin = `${xPercent}%${yPercent}%`;
    img.style.transform = "scale(2)"
  }

  onMouseLeave() {
    const img = document.querySelector(".image-content img") as HTMLImageElement;
    img.style.transform = "scale(1)";
    img.style.transformOrigin = "center"
  }

  showZoom = false;

  onZoomMove(event: MouseEvent) {
    const container = event.currentTarget as HTMLElement;
    const preview = document.querySelector(".zoom-preview") as HTMLElement;

    this.showZoom = true;

    const rect = container.getBoundingClientRect();

    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    const xPercent = (x / rect.width) * 100;
    const yPercent = (y / rect.height) * 100;

    // Set preview image
    preview.style.backgroundImage = `url(${this.getMainImage()})`;

    // Move zoom area
    preview.style.backgroundPosition = `${xPercent}% ${yPercent}%`;
  }

  onZoomLeave() {
    this.showZoom = false;
  }
  getRecommendedProducts(productId: number) {
    this.service.getRecommendationDetails(productId).subscribe((res) => {
      if (res.status) {
        this.recommendations = res.data;
        console.log('recommended-products', this.recommendations);

        this.products = res.data;
        this.recommendations.batteries?.forEach(b => b.isImageLoaded = false);
        this.recommendations.inverters?.forEach(i => i.isImageLoaded = false);
        this.recommendations.accessories?.forEach(a => a.isImageLoaded = false);
        this.startTypingEffect(this.recommendations.aiExplanation, (text) => {
          this.recommendedText = text;
        });
      }
      else {
        this.isImageLoaded = false;
      }

    })
  }

  startTypingEffect(fullText: string, callback: (text: string) => void) {
    let index = 0;

    const interval = setInterval(() => {
      callback(fullText.substring(0, index + 1));
      index++;

      if (index >= fullText.length) {
        clearInterval(interval);
      }
    }, 20);
  }

  toggleExplaination(item: any) {
    item.showExplaination = !item.showExplaination;

    if (this.productId && item.showExplaination) {
      this.service.getWhyThisExplainations(this.productId, item.productId)
        .subscribe((res) => {
          if (res.status) {
            item.aiText = '';

            this.startTypingEffect(res.data, (text) => {
              item.aiText = text;
            });

          } else {
            this.toastr.error(this.messages.error.data);
          }
        });
    }
  }

  whyThisExplainations(item: any) {
    if (!this.productId) {
      return;
    }
    this.service.getWhyThisExplainations(this.productId, item.productId).subscribe((res) => {
      this.dialog.open(WhyThis, {
        width: '800px',
        data: {
          aiResult: res.data
        }
      })
    })
  }


  toggleCompare(item: any, type: string, event: any) {

    let list = type === 'battery' ? this.compareBatteries : this.compareInverters;
    if (list.includes(item)) {
      list.splice(list.indexOf(item), 1)
    }

    else {
      if (list.length < 2) {
        list.push(item)
        if (list.length === 2) {
          this.compareProduct(type);
        }
      }
      else {
        alert('Only 2 Products allowed')
        event.target.checked = false;
      }
    }
  }

  compareProduct(type: string) {
    let list = type === 'battery' ? this.compareBatteries : this.compareInverters;
    this.service.compareTwoProducts(list[0].productId, list[1].productId).subscribe((res) => {
      if (res.status) {
        // this.aiCompareResult=res.data;
        // this.showCompareModal=true
        this.dialog.open(CompareProduct, {
          data: {
            items: list,
            aiResult: res.data
          }
        })
      }
      else {
        this.toastr.error(this.messages.error.data);
      }
    })
  }

  comboProducts(productId: number) {
    this.typedReason = '';
    this.service.comboProducts(productId).subscribe((res) => {
      if (res.status) {
        this.comboDto = res.data;

        this.startTypingEffect(res.data.reason, (val) => {
          this.typedReason = val;
        })
      }
      else {
        this.toastr.error(this.messages.error.data);
      }
    })
  }


  closeCompare() {
    this.showCompareModal = false;
  }

  buyCombo() {

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
      return;
    }

    const userId = this.authService.GetUserId();
    if (!userId) {
      this.toastr.error(this.messages.warnings.login);
      return;
    }

    const dto1: CartDto = {
      userId: userId,
      productId: this.comboDto.battery.productId,
      quantity: 1
    };

    const dto2: CartDto = {
      userId: userId,
      productId: this.comboDto.inverter.productId,
      quantity: 1
    };


    // forkJoin([
    //   this.service.addToCart(dto1),
    //   this.service.addToCart(dto2)
    // ]).subscribe(([res1,res2])=>{
    //   if(res1.status&&res2.status){
    //     this.updateCartCount(userId);
    //   }
    // })

   from([dto1, dto2]).pipe(
  concatMap(dto =>
    this.service.addToCart(dto).pipe(
      catchError(err => {
        console.error("Error adding item", err);
        return of(null); // continue flow
      })
    )
  )
).subscribe({
  complete: () => {
    this.updateCartCount(userId);
  }
});


    alert(" Full setup added to cart!");
  }

  comboCart(productId: number) {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
      return;
    }

    const userId = this.authService.GetUserId();
    if (!userId) {
      this.toastr.error(this.messages.warnings.login);
      return;
    }



    const dto: CartDto = {
      userId: userId,
      productId: productId,
      quantity: 1
    };

    this.service.addToCart(dto).subscribe((res) => {
      if (res.status) {
        if (userId) {
          this.updateCartCount(userId)
        }
        this.toastr.success(this.messages.cart)
      }
      else {
        this.toastr.error(this.messages.error.fail);
        console.log(res.error);

      }
    })
  }

}
