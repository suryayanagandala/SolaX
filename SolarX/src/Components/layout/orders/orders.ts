import { CommonModule } from '@angular/common';
import { Component, AfterViewInit, OnInit } from '@angular/core';
import * as L from 'leaflet';

delete (L.Icon.Default.prototype as any)._getIconUrl;

L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'assets/leaflet/marker-icon-2x.png',
  iconUrl: 'assets/leaflet/marker-icon.png',
  shadowUrl: 'assets/leaflet/marker-shadow.png',
});
import { OrderService } from '../../../Shared/order-service';
import { TokenService } from '../../../Shared/token';
import { CartService } from '../../../Shared/cart-service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { MaterialModule } from '../../../MaterialModule';
import { FormBuilder, ReactiveFormsModule, FormGroup, Validators } from '@angular/forms';
import { CartDto } from '../../../Models/Models';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { OrderCreateDto } from '../../../Models/Models';
import { localMessages } from '../../../localMessages';


@Component({
  selector: 'app-orders',
  imports: [CommonModule, MaterialModule, ReactiveFormsModule,],
  templateUrl: './orders.html',
  styleUrls: ['./orders.scss'],
})
export class Orders implements OnInit, AfterViewInit {

  private map!: L.Map;
  private marker!: L.Marker;
  private messages=localMessages

  buyNowItem: any = null;
  // orderDto!:OrderCreateDto;
  isBuyNow = false;

  suggestions: any[] = [];
  cart: CartDto | null = null;
  userId: string | null = null;
  totalAmount: number | null = null;
  shipping = 100;
  discount: number = 0;
  amount = 0;

  addressForm!: FormGroup;

  private searchSubject = new Subject<string>();

  constructor(
    private builder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private orderService: OrderService,
    private cartService: CartService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    this.userId = this.tokenService.GetUserId();

    if (!this.userId) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
      return;
    }

    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras.state || history.state;

    if (state?.buyNowItem) {
      this.isBuyNow = true;
      this.buyNowItem = state.buyNowItem;

      this.cart = {
        items: [this.buyNowItem]
      } as any;

      this.calculateTotalAmount();
    } else {
      this.isBuyNow = false;
      this.cartSummary(this.userId);
    }
    
    this.addressForm = this.builder.group({
      deliveryName: ['', Validators.required],
      deliveryNumber: ['', Validators.required],
      fullAddress: ['', Validators.required],
      lat: [null, Validators.required],
      lng: [null, Validators.required]
    });

    this.orderService.getUserDetails(this.userId).subscribe((res)=>{
      this.addressForm.patchValue({
        deliveryName:res.data.fullName,
        deliveryNumber:res.data.phoneNumber
      })
    })

    // Debounced search 
    this.searchSubject.pipe(debounceTime(400),distinctUntilChanged())
      .subscribe(query => {
        this.fetchAddressSuggestions(query);
      });
  }

  ngAfterViewInit(): void {
    this.initMap();
  }

  initMap() {
    this.map = L.map('map').setView([17.385044, 78.486671], 13);

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);

    // Click on map → marker + reverse geocode
    this.map.on('click', (e: L.LeafletMouseEvent) => {
      this.setMarkerAndFetchAddress(e.latlng.lat, e.latlng.lng);
    });
  }

  setMarkerAndFetchAddress(lat: number, lng: number) {
    if (this.marker) {
      this.map.removeLayer(this.marker);
    }

    this.marker = L.marker([lat, lng]).addTo(this.map);

    fetch(
      `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}&countrycodes=in`
    )
      .then(res => res.json())
      .then(data => {
        this.addressForm.patchValue({
          fullAddress: data.display_name,
          lat,
          lng
        });
      });
  }

  onAddressInput() {
    const query = this.addressForm.get('fullAddress')?.value;

    if (!query || query.length < 3) {
      this.suggestions = [];
      return;
    }

    this.searchSubject.next(query);
  }

  fetchAddressSuggestions(query: string) {
    fetch(
      `https://nominatim.openstreetmap.org/search?format=json&q=${query}&countrycodes=in&addressdetails=1`
    )
      .then(res => res.json())
      .then(data => {
        this.suggestions = data.slice(0, 5);
      });
  }

  selectSuggestions(item: any) {
    const lat = +item.lat;
    const lng = +item.lon;

    this.addressForm.patchValue({
      fullAddress: item.display_name,
      lat,
      lng
    });

    this.suggestions = [];

    this.map.setView([lat, lng], 15);

    if (this.marker) {
      this.map.removeLayer(this.marker);
    }

    this.marker = L.marker([lat, lng]).addTo(this.map);
  }



  placeOrder() {
    if (this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();
      this.toastr.error(this.messages.order.address);
      return;
    }

    const dto:OrderCreateDto = {
      userId: this.userId!,
      isBuyNow:this.isBuyNow,
      deliveryAddress: this.addressForm.value.fullAddress,
      deliveryName: this.addressForm.value.deliveryName,
      deliveryPhone: this.addressForm.value.deliveryNumber
    };

    if (this.isBuyNow && this.buyNowItem) {
    dto.isBuyNow = true;
    dto.productId = this.buyNowItem.productId;
    dto.quantity = this.buyNowItem.quantity;
  }


    this.orderService.ceratePendingOrder(dto).subscribe((res)=>{
      if(res.status){
        this.router.navigate(['/payment'],{
          state:{
            orderId:res.data.orderId,
            amount:this.totalAmount
          }
        })
      }
      else{
        console.log(res.error);
        this.toastr.error(this.messages.order.error)
      }
    })

    
  }

  cartSummary(userId: string) {
    this.cartService.getUserCart(userId).subscribe(res => {
      if (res.status) {
        this.cart = res.data;
        this.calculateTotalAmount();
      }
    });
  }

  calculateTotalAmount() {
    this.amount = this.cart?.items?.reduce((sum, item) => sum + (item.quantity * item.price), 0) ?? 0;
    const shipping = this.shipping ?? 0;
    const discount = this.discount ?? 0;
    this.totalAmount = this.amount + shipping - discount
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
