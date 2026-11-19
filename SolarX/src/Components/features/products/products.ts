import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { HomeService } from '../../../Shared/home-category';
import { ToastrService } from 'ngx-toastr';
import { MaterialModule } from '../../../MaterialModule';
import { MatSliderModule } from '@angular/material/slider';
import { PageEvent } from '@angular/material/paginator';
import { Product } from '../../../Models/Models';
import { CategoryDto } from '../../../Models/Models';
import { localMessages } from '../../../localMessages';

@Component({
  selector: 'app-products',
  imports: [
    CommonModule,
    MaterialModule,
    MatSliderModule,
    ReactiveFormsModule
  ],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class ProductsHome implements OnInit {

  priceForm!: FormGroup;

  parentCategories: CategoryDto[]=[]
  parentCategory: any | null = null;
  childCategories: any[] = [];

  allProducts: Product[] = [];
  filteredProducts: Product[] = [];
  pagedProducts: Product[] = [];

  selectedChild: any | null = null;

  selectedParentSlug: string | null = null;
  private messages=localMessages;

  minPrice = 0;
  maxPrice =20000;

  pageSize = 8;
  pageIndex = 0;

  loading = true;

  constructor(
    private service: HomeService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private cdr:ChangeDetectorRef
  ) {
    this.priceForm = this.fb.group({
      priceRange: [this.maxPrice]
    });
  }

  ngOnInit(): void {
  this.route.queryParams.subscribe(params => {
    const slug = params['slug'];

    // ALWAYS load parent categories first
    this.service.getHomeContent().subscribe(res => {
      if (res.status) {
        this.parentCategories = res.data;

        if (slug) {
          this.selectedParentSlug = slug;
          this.loadCategoriesBySlug(slug);
        } else {
          this.loadParentCategories();
        }
      }
    });
  });
}

  loadParentCategories() {
    this.service.getHomeContent().subscribe((res) => {
      if (res.status) {
       

        this.parentCategories = res.data;
       

        const data = res.data;

        this.parentCategory = data;
        
        this.childCategories=[];
        this.selectedChild = null;
   
      
        const parentProducts = this.parentCategories.flatMap(
      p => p.productsDto ?? []
    );

    const childProducts = this.parentCategories.flatMap(
      p => p.children?.flatMap(c => c.productsDto ?? []) ?? []
    );

    this.allProducts = [...parentProducts, ...childProducts];

    this.maxPrice = Math.max(...this.allProducts.map(p => p.price));

// update slider value
this.priceForm.patchValue({ priceRange: this.maxPrice });
    
    this.applyFilters();
    this.pageIndex = 0;
    this.updatePagedProducts();
    this.loading = false;
      }
      else {
        console.log(res.error);
        this.toastr.error()
        this.loading=true
      }
    })
  }

  loadCategoriesBySlug(slug: string) {
    this.loading = true;
    this.selectedParentSlug = slug; 
    this.service.getCategoriesBySlug(slug).subscribe(res => {
      if (!res.status) {
        this.toastr.error(this.messages.Product.error);
        return;
      }

      const data = res.data;

      this.parentCategory = data;
      this.childCategories = data.children ?? [];
      this.selectedChild = null;

      const parentProducts = data.productsDto ?? [];
      const childProducts = this.childCategories.flatMap(
        c => c.productsDto ?? []
      );

      this.allProducts = [...parentProducts, ...childProducts];
      this.maxPrice = Math.max(...this.allProducts.map(p => p.price));


      this.priceForm.patchValue({ priceRange: this.maxPrice });
      this.cdr.detectChanges();

      this.applyFilters();
      this.pageIndex = 0;
      this.loading = false;

      this.updatePagedProducts();

      
    });
  }

  onParentCategoryChange(slug: string | null) {
    if (!slug) return;
    this.router.navigate(['/products'], {
      queryParams: { slug }
    });
  }

  filterByChild(child: any | null) {
    this.selectedChild = child;
    this.pageIndex = 0;
    this.applyFilters();
  }

  applyFilters() {
    const max = this.priceForm.value.priceRange;

    const baseProducts =
      this.selectedChild
        ? (this.selectedChild.productsDto ?? [])
        : this.allProducts;

    this.filteredProducts = baseProducts.filter((p: Product) => p.price <= max);

    this.updatePagedProducts();
  }

  updatePagedProducts() {
    const start = this.pageIndex * this.pageSize;
    const end = start + this.pageSize;
    this.pagedProducts = this.filteredProducts.slice(start, end);

  }

  onPageChange(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedProducts();
  }

  viewProductById(productId?: number) {
    if (!productId) {
    return;
  }
    this.router.navigate(['/view-deatils'], {
      queryParams: { productId }
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


 getProductImage(product: Product): string {
  const imageUrl = product.productImages?.[0]?.imageUrl;
  return this.getImageUrl(imageUrl);
}

}

