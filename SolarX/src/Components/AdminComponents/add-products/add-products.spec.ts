import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddProducts } from './add-products';
import { ProductService } from '../../../Shared/product';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';

describe('AddProducts', () => {

  let component: AddProducts;
  let fixture: ComponentFixture<AddProducts>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {

    productServiceSpy = jasmine.createSpyObj('ProductService', [
      'getAllProducts',
      'softDelete',
      'restoreProduct'
    ]);

    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);

    toastrSpy = jasmine.createSpyObj('ToastrService', [
      'success',
      'error'
    ]);

    await TestBed.configureTestingModule({
      imports: [AddProducts, RouterTestingModule],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: ToastrService, useValue: toastrSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(AddProducts);
    component = fixture.componentInstance;

  });

  beforeEach(() => {

    const mockResponse = {
      status: true,
      data: [
        { productId: 1, productName: 'Solar Panel' },
        { productId: 2, productName: 'Battery' }
      ]
    };

    productServiceSpy.getAllProducts.and.returnValue(of({status:true}));

    fixture.detectChanges();

  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load products on init', () => {

    expect(productServiceSpy.getAllProducts).toHaveBeenCalled();

    expect(component.dataSource.data.length).toBe(2);

  });

  it('should apply filter', () => {

    component.applyFilter('solar');

    expect(component.dataSource.filter).toBe('solar');

  });

  it('should call softDelete when reason provided', () => {

    const dialogRefSpy = {
      afterClosed: () => of('Outdated')
    };

    dialogSpy.open.and.returnValue(dialogRefSpy as any);

    productServiceSpy.softDelete.and.returnValue(of({ status: true }));

    component.deleteProduct(1);

    expect(productServiceSpy.softDelete).toHaveBeenCalledWith({
      productId: 1,
      reason: 'Outdated'
    });

  });

  it('should not call softDelete when dialog cancelled', () => {

    const dialogRefSpy = {
      afterClosed: () => of(null)
    };

    dialogSpy.open.and.returnValue(dialogRefSpy as any);

    component.deleteProduct(1);

    expect(productServiceSpy.softDelete).not.toHaveBeenCalled();

  });

  it('should restore product', () => {

    productServiceSpy.restoreProduct.and.returnValue(
      of({ status: true })
    );

    component.restoreProduct(1);

    expect(productServiceSpy.restoreProduct).toHaveBeenCalledWith(1);

  });

  it('should open dialog to show reason', () => {

    const dialogRefSpy = {
      afterClosed: () => of(true)
    };

    dialogSpy.open.and.returnValue(dialogRefSpy as any);

    component.showReason('Test reason');

    expect(dialogSpy.open).toHaveBeenCalled();

  });

});