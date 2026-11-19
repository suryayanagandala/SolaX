import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ImagesForm } from './images-form';
import { ProductService } from '../../../Shared/product';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ImagesForm', () => {
  let component: ImagesForm;
  let fixture: ComponentFixture<ImagesForm>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let activatedRouteStub: Partial<ActivatedRoute>;

  beforeEach(async () => {
    productServiceSpy = jasmine.createSpyObj('ProductService', [
      'updateProductImage',
      'addProductImages'
    ]);

    toastrSpy = jasmine.createSpyObj('ToastrService', [
      'success',
      'error',
      'warning'
    ]);

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  
    activatedRouteStub = {
      queryParams: of({ productId: 1, imageId: 1 })
    };

    await TestBed.configureTestingModule({
      imports: [ImagesForm],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteStub }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ImagesForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load productId and imageId on init', () => {
    expect(component.productId).toBe(1);
    expect(component.imageId).toBe(1);
    expect(component.isEditMode).toBeTrue();
  });

  it('should warn if no files selected on submit', () => {
    component.selectedFiles = [];
    component.submit();
    expect(toastrSpy.warning).toHaveBeenCalledWith('Please select image');
  });

  it('should call updateProductImage when in edit mode', fakeAsync(() => {
    component.isEditMode = true;
    component.productId = 1;
    component.imageId = 1;

    const file = new File(['dummy content'], 'update.png', { type: 'image/png' });
    component.selectedFiles = [file];

    productServiceSpy.updateProductImage.and.returnValue(of({ status: true }));

    component.submit();

    tick(800);

    expect(productServiceSpy.updateProductImage).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalledWith('successfully update image');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/viewImages'], { queryParams: { productId: 1 } });
  }));

  it('should call addProductImages when not in edit mode', fakeAsync(() => {
    component.isEditMode = false;
    component.productId = 1;

    const file = new File(['dummy content'], 'add.png', { type: 'image/png' });
    component.selectedFiles = [file];

    productServiceSpy.addProductImages.and.returnValue(of({ status: true }));

    component.submit();

    tick(800); 

    expect(productServiceSpy.addProductImages).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalledWith('successfully add images');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/viewImages'], { queryParams: { productId: 1 } });
  }));
});