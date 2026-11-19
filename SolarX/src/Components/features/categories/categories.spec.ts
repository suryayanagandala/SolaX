import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Categories } from './categories';
import { HomeService } from '../../../Shared/home-category';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('Categories', () => {

  let component: Categories;
  let fixture: ComponentFixture<Categories>;

  let homeServiceSpy: jasmine.SpyObj<HomeService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {

    homeServiceSpy = jasmine.createSpyObj('HomeService', ['getHomeCategories']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['error']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Categories],
      providers: [
        { provide: HomeService, useValue: homeServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Categories);
    component = fixture.componentInstance;
  });

 

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  

  it('should call loadHomeCategories on init', () => {

    const spy = spyOn(component, 'loadHomeCategories');

    component.ngOnInit();

    expect(spy).toHaveBeenCalled();
  });

 

  it('should load categories successfully', () => {

    const mockResponse = {
      status: true,
      data: [
        { id: 1, name: 'Commercial', slug: 'commercial' },
        { id: 2, name: 'Residential', slug: 'residential-solar-panels' }
      ]
    };

    homeServiceSpy.getHomeCategories.and.returnValue(of({status:true}));

    component.loadHomeCategories();

    expect(homeServiceSpy.getHomeCategories).toHaveBeenCalled();
    expect(component.categories.length).toBe(0);
    expect(component.loading).toBeFalse();
  });

  

  it('should show error when API fails', () => {

    const mockResponse = {
      status: false,
      error: 'Something went wrong'
    };

    homeServiceSpy.getHomeCategories.and.returnValue(of(mockResponse));

    component.loadHomeCategories();

    expect(component.loading).toBeTrue();
    expect(toastrSpy.error).toHaveBeenCalled();
  });



  it('should return correct category image', () => {

    const image = component.getCategoryImage('commercial');

    expect(image).toBe('Assets/Themes/Commercial-Image.jpg');
  });

  

  it('should return default image if slug not found', () => {

    const image = component.getCategoryImage('unknown-category');

    expect(image).toBe('Assets/Themes/Banner Image.png');
  });

});