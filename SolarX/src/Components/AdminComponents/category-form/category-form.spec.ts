import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoryFormComponent } from './category-form';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../../MaterialModule';
import { CategorieService } from '../../../Shared/categories';
import { CategoryDto } from '../../../Models/Models';

describe('CategoryFormComponent', () => {
  let component: CategoryFormComponent;
  let fixture: ComponentFixture<CategoryFormComponent>;
  let serviceMock: jasmine.SpyObj<CategorieService>;
  let routerMock: jasmine.SpyObj<Router>;
  let toastr: ToastrService;

  beforeEach(async () => {
    // Mock CategorieService
    serviceMock = jasmine.createSpyObj('CategorieService', [
      'getAllCategories',
      'getCategoryById',
      'createCategory',
      'updateCategory',
      'updateParentCategory'
    ]);

    // Return dummy observables
    serviceMock.getAllCategories.and.returnValue(of({ status: true, data: [] }));
    serviceMock.getCategoryById.and.returnValue(of({ status: true,  }));
    serviceMock.createCategory.and.returnValue(of({ status: true }));
    serviceMock.updateCategory.and.returnValue(of({ status: true }));
    serviceMock.updateParentCategory.and.returnValue(of({ status: true}));

    // Mock Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        CategoryFormComponent,
        ReactiveFormsModule,
        MaterialModule,
        HttpClientTestingModule,
        ToastrModule.forRoot()
      ],
      providers: [
        { provide: CategorieService, useValue: serviceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: { queryParams: of({ id: null }) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryFormComponent);
    component = fixture.componentInstance;
    toastr = TestBed.inject(ToastrService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form initially', () => {
    expect(component.categoryform.valid).toBeFalse();
  });

  it('should call submit and create category when not edit mode', () => {
    component.categoryform.patchValue({ name: 'Test', description: 'Desc', parentId: null });
    component.isEditMode = false;

    component.submit();

    expect(serviceMock.createCategory).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/category-list']);
  });

  it('should call submit and update parent category when edit mode and parentId null', () => {
    component.categoryform.patchValue({ name: 'Test', description: 'Desc', parentId: null });
    component.isEditMode = true;
    component.categoryId = 1;

    component.submit();

    expect(serviceMock.updateParentCategory).toHaveBeenCalledWith(1, jasmine.any(Object));
    expect(routerMock.navigate).toHaveBeenCalledWith(['/admin/category-list']);
  });

  it('should call submit and update child category when edit mode and parentId set', () => {
    component.categoryform.patchValue({ name: 'Test', description: 'Desc', parentId: 2 });
    component.isEditMode = true;
    component.categoryId = 1;

    component.submit();

    expect(serviceMock.updateCategory).toHaveBeenCalledWith(1, jasmine.any(Object));
    expect(routerMock.navigate).toHaveBeenCalledWith(['/admin/category-list']);
  });

  it('should patch form when loadCategoryById is called', () => {
    component.loadCategoryById(1);
    expect(component.categoryform.value.name).toBe('Test');
  });

  it('should load categories on init', () => {
    component.loadCategories();
    expect(serviceMock.getAllCategories).toHaveBeenCalled();
    expect(component.categories.length).toBe(0);
  });
});