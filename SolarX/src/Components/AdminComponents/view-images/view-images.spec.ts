import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViewImages } from './view-images';
import { ProductService } from '../../../Shared/product';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';

describe('ViewImages', () => {
  let component: ViewImages;
  let fixture: ComponentFixture<ViewImages>;

  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    productServiceSpy = jasmine.createSpyObj('ProductService', [
      'getProductImagesById',
      'deleteImage'
    ]);

    toastrSpy = jasmine.createSpyObj('ToastrService', [
      'success',
      'error',
      'show'
    ]);

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ViewImages, RouterTestingModule], // RouterTestingModule provides ActivatedRoute
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        {
          provide: ActivatedRoute,
          useValue: { queryParams: of({ productId: 1 }) }
        },
        { provide: Router, useValue: routerSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ViewImages);
    component = fixture.componentInstance;

  
    productServiceSpy.getProductImagesById.and.returnValue(of({
      status: true,
      
    }));

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load productId from queryParams', () => {
    expect(component.productId).toBe(1);
  });

  it('should load images on init', () => {
    expect(component.images.length).toBe(2);
    expect(toastrSpy.show).toHaveBeenCalledWith('show images for product1');
  });

  it('should return correct image URL', () => {
    const url = component.getImageUrl('test.jpg');
    expect(url).toContain('test.jpg');
  });

  it('should delete an image', () => {
    spyOn(window, 'confirm').and.returnValue(true); // simulate confirm

    productServiceSpy.deleteImage.and.returnValue(of({ status: true }));

    component.deleteProduct(1);

    expect(productServiceSpy.deleteImage).toHaveBeenCalledWith(1);
    expect(component.images.some(x => x.imageId === 1)).toBeFalse();
    expect(toastrSpy.success).toHaveBeenCalledWith('image with 1 id sucessfully removed');
  });

  it('should not delete an image if cancelled', () => {
    spyOn(window, 'confirm').and.returnValue(false); // simulate cancel

    component.deleteProduct(1);

    expect(productServiceSpy.deleteImage).not.toHaveBeenCalled();
  });
});