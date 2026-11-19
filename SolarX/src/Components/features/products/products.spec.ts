import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductsHome } from './products';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ToastrModule } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../../MaterialModule';
import { MatSliderModule } from '@angular/material/slider';
import { HomeService } from '../../../Shared/home-category';
import { ChangeDetectorRef } from '@angular/core';

describe('ProductsHome', () => {
  let component: ProductsHome;
  let fixture: ComponentFixture<ProductsHome>;
  let homeServiceMock: jasmine.SpyObj<HomeService>;
  let routerMock: jasmine.SpyObj<Router>;
  let cdrMock: jasmine.SpyObj<ChangeDetectorRef>;

  const activatedRouteMock = { queryParams: of({ slug: 'some-slug' }) };

  beforeEach(async () => {
    homeServiceMock = jasmine.createSpyObj('HomeService', ['getHomeContent', 'getCategoriesBySlug']);
    homeServiceMock.getHomeContent.and.returnValue(of({ status: true, data: [] }));
    homeServiceMock.getCategoriesBySlug.and.returnValue(of({ status: true, data: { productsDto: [], children: [] } }));

    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    cdrMock = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);

    await TestBed.configureTestingModule({
      imports: [
        ProductsHome,
        HttpClientTestingModule,
        ToastrModule.forRoot(),
        ReactiveFormsModule,
        MaterialModule,
        MatSliderModule
      ],
      providers: [
        { provide: HomeService, useValue: homeServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: ChangeDetectorRef, useValue: cdrMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductsHome);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate when parent category changes', () => {
    component.onParentCategoryChange('slug-test');
    expect(routerMock.navigate).toHaveBeenCalledWith(['/products'], { queryParams: { slug: 'slug-test' } });
  });

  it('should filter products by price', () => {
    component.allProducts = [{ price: 100 } as any, { price: 200 } as any];
    component.priceForm.patchValue({ priceRange: 150 });
    component.applyFilters();
    expect(component.filteredProducts.length).toBe(1);
  });

  it('should filter by child', () => {
    const child = { productsDto: [{ price: 50 }, { price: 100 }] };
    component.filterByChild(child as any);
    expect(component.selectedChild).toBe(child);
  });

  it('should update paged products on page change', () => {
    component.filteredProducts = Array.from({ length: 20 }, (_, i) => ({ price: i } as any));
    component.pageSize = 5;
    component.onPageChange({ pageIndex: 1, pageSize: 5 } as any);
    expect(component.pagedProducts.length).toBe(5);
  });

  it('should return correct image URLs', () => {
    expect(component.getImageUrl('http://example.com/img.png')).toBe('http://example.com/img.png');
    expect(component.getImageUrl('img.png')).toBe(component.getImageUrl('img.png')); // prefixed by environment.imageUrl
    expect(component.getImageUrl(undefined)).toBe('Assets/Themes/panelimage1.png');
  });

  it('should get product image', () => {
    const product = { productImages: [{ imageUrl: 'img.png' }] } as any;
    expect(component.getProductImage(product)).toBe(component.getImageUrl('img.png'));
  });

  it('should return early if viewProductById called with undefined', () => {
    component.viewProductById(undefined);
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should navigate to product details', () => {
    component.viewProductById(5);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/view-deatils'], { queryParams: { productId: 5 } });
  });

  it('should load categories by slug', () => {
    component.loadCategoriesBySlug('slug-test');
    expect(homeServiceMock.getCategoriesBySlug).toHaveBeenCalled();
  });
});