import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoryList } from './category-list';
import { CategorieService } from '../../../Shared/categories';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('CategoryList', () => {
  let component: CategoryList;
  let fixture: ComponentFixture<CategoryList>;
  let serviceSpy: jasmine.SpyObj<CategorieService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let dialogSpy: jasmine.SpyObj<MatDialog>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj('CategorieService', [
      'getAllCategories',
      'deleteCategory',
      'restoreCategory',
      'deleteParentCategory',
      'restoreParentCategory'
    ]);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error', 'info']);
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CategoryList],
      providers: [
        { provide: CategorieService, useValue: serviceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: Router, useValue: routerSpy },
        // <-- Add this mock for ActivatedRoute
        { provide: ActivatedRoute, useValue: { queryParams: of({}) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryList);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load categories on init', () => {
    const mockCategories = [{ categoryId: 1, name: 'Solar' }];
    serviceSpy.getAllCategories.and.returnValue(of({ status: true }));

    component.ngOnInit();

    expect(serviceSpy.getAllCategories).toHaveBeenCalled();
    expect(component.loading).toBeFalse();
    expect(toastrSpy.info).toHaveBeenCalledWith('Category List');
  });

  it('should call deleteCategory and show success', () => {
    const afterClosedSpy = jasmine.createSpyObj('afterClosed', ['subscribe']);
    afterClosedSpy.subscribe.and.callFake((fn: any) => fn('reason'));
    dialogSpy.open.and.returnValue({ afterClosed: () => afterClosedSpy } as any);

    serviceSpy.deleteCategory.and.returnValue(of({ status: true }));
    serviceSpy.getAllCategories.and.returnValue(of({ status: true, data: [] }));

    component.deleteCategory(1);

    expect(dialogSpy.open).toHaveBeenCalled();
    expect(serviceSpy.deleteCategory).toHaveBeenCalledWith({ categoryId: 1, reason: 'reason' });
    expect(toastrSpy.success).toHaveBeenCalledWith('successfully deactivated');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/category-list']);
  });

  it('should call restoreCategory and show success', () => {
    serviceSpy.restoreCategory.and.returnValue(of({ status: true }));
    serviceSpy.getAllCategories.and.returnValue(of({ status: true, data: [] }));

    component.restoreCategory(1);

    expect(serviceSpy.restoreCategory).toHaveBeenCalledWith(1);
    expect(toastrSpy.success).toHaveBeenCalledWith('activated successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/category-list']);
  });

  it('should call deleteParentCategory and show success', () => {
    const afterClosedSpy = jasmine.createSpyObj('afterClosed', ['subscribe']);
    afterClosedSpy.subscribe.and.callFake((fn: any) => fn('reason'));
    dialogSpy.open.and.returnValue({ afterClosed: () => afterClosedSpy } as any);

    serviceSpy.deleteParentCategory.and.returnValue(of({ status: true }));

    component.deleteParentCategory(2);

    expect(dialogSpy.open).toHaveBeenCalled();
    expect(serviceSpy.deleteParentCategory).toHaveBeenCalledWith({ categoryId: 2, reason: 'reason' });
    expect(toastrSpy.success).toHaveBeenCalledWith('successfully de-activated parent category');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/category-list']);
  });

  it('should call restoreParentCategory and show success', () => {
    serviceSpy.restoreParentCategory.and.returnValue(of({ status: true }));

    component.restoreParentCategory(2);

    expect(serviceSpy.restoreParentCategory).toHaveBeenCalledWith(2);
    expect(toastrSpy.success).toHaveBeenCalledWith('successfully activated');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/category-list']);
  });
});