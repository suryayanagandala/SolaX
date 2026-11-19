import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductForm } from './product-form';
import { ProductService } from '../../../Shared/product';
import { CategorieService } from '../../../Shared/categories';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';

describe('ProductForm', () => {
  let component: ProductForm;
  let fixture: ComponentFixture<ProductForm>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let categoryServiceSpy: jasmine.SpyObj<CategorieService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    productServiceSpy = jasmine.createSpyObj('ProductService', [
      'getProductDetalById',
      'createProduct',
      'updateProduct'
    ]);

    categoryServiceSpy = jasmine.createSpyObj('CategorieService', [
      'getAllCategories'
    ]);

    toastrSpy = jasmine.createSpyObj('ToastrService', [
      'success',
      'error'
    ]);

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ProductForm, RouterTestingModule],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: CategorieService, useValue: categoryServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: ActivatedRoute, useValue: { queryParams: of({ productId: 1 }) } },
        { provide: Router, useValue: routerSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    categoryServiceSpy.getAllCategories.and.returnValue(of({
      status: true,
      
    }));

    productServiceSpy.getProductDetalById.and.returnValue(of({
      status: true,
      data: {
        productName: 'Test Product',
        price: 100,
        description: 'A very long description to pass validation...',
        capacity: '100W',
        capacityValue: 100,
        stock: 10,
        type: 'Mono',
        brand: 'BrandX',
        warranty_Years: 2,
        categoryId: 1
      }
    }));

    productServiceSpy.createProduct.and.returnValue(of({ status: true }));
    productServiceSpy.updateProduct.and.returnValue(of({ status: true }));

    fixture = TestBed.createComponent(ProductForm);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load categories on init', () => {
    expect(component.categories.length).toBe(1);
    expect(component.categories[0].name).toBe('Solar');
  });

  it('should call create when adding new product', () => {
    component.isEditMode = false;
    component.productForm.patchValue({
      productName: 'New Product',
      price: 100,
      description: 'A very long description to pass validation...',
      capacity: '200W',
      capacityValue: 200,
      stock: 5,
      type: 'Mono',
      brand: 'BrandY',
      warranty_Years: 1,
      categoryId: 1
    });

    component.submit();

    expect(productServiceSpy.createProduct).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalledWith('Successfully added');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['admin/product-details']);
  });

  it('should call update when editing', () => {
    component.isEditMode = true;
    component.productId = 1;
    component.productForm.patchValue({
      productName: 'Updated Product',
      price: 150,
      description: 'A very long description to pass validation...',
      capacity: '150W',
      capacityValue: 150,
      stock: 8,
      type: 'Mono',
      brand: 'BrandZ',
      warranty_Years: 2,
      categoryId: 1
    });

    component.submit();

    expect(productServiceSpy.updateProduct).toHaveBeenCalledWith(1, jasmine.any(Object));
    expect(toastrSpy.success).toHaveBeenCalledWith('Successfully updated');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['admin/product-details']);
  });

});