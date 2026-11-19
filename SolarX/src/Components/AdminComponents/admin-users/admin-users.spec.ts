import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminUsers } from './admin-users';
import { Auth } from '../../../Shared/auth';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

describe('AdminUsers', () => {
  let component: AdminUsers;
  let fixture: ComponentFixture<AdminUsers>;
  let authSpy: jasmine.SpyObj<Auth>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  const mockUsers = [
    { fullName: 'User1', email: 'u1@test.com', phoneNumber: '111', roleName: 'Admin' },
    { fullName: 'User2', email: 'u2@test.com', phoneNumber: '222', roleName: 'User' }
  ];

  beforeEach(async () => {
    authSpy = jasmine.createSpyObj('Auth', ['GetAllUsers']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['error']);
    authSpy.GetAllUsers.and.returnValue(of({ status: true, data: mockUsers }));

    await TestBed.configureTestingModule({
      imports: [AdminUsers],
      providers: [
        { provide: Auth, useValue: authSpy },
        { provide: ToastrService, useValue: toastrSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminUsers);
    component = fixture.componentInstance;

    // Provide MatPaginator & MatSort to avoid undefined
    component.paginator = { firstPage: jasmine.createSpy('firstPage') } as unknown as MatPaginator;
    component.sort = {} as MatSort;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should filter users correctly', () => {
    component.ApplyFilter('User1');
    expect(component.dataSource.filteredData.length).toBe(1);
  });

  it('should call paginator.firstPage on filter', () => {
    component.ApplyFilter('User1');
    expect(component.paginator.firstPage).toHaveBeenCalled();
  });
});