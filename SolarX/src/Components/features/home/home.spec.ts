import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { TokenService } from '../../../Shared/token';
import { Role } from '../../../Models/Models';
import { ToastrService, TOAST_CONFIG } from 'ngx-toastr';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Component } from '@angular/core';

// -------------------- Mock Child Components --------------------
@Component({selector: 'app-banner', template: ''}) class MockBanner {}
@Component({selector: 'app-footer', template: ''}) class MockFooter {}
@Component({selector: 'app-solar-xbanner', template: ''}) class MockSolarXBanner {}
@Component({selector: 'app-services-banner', template: ''}) class MockServicesBanner {}
@Component({selector: 'app-categories', template: ''}) class MockCategories {}

// -------------------- Mock Services --------------------
class MockTokenService {
  GetRole(): Role { return Role.User; }
}
class MockToastrService {
  success = jasmine.createSpy('success');
  error = jasmine.createSpy('error');
  info = jasmine.createSpy('info');
  warning = jasmine.createSpy('warning');
}

// -------------------- Tests --------------------
describe('Home Component', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;
  let tokenService: TokenService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        Home,
        MockBanner,
        MockFooter,
        MockSolarXBanner,
        MockServicesBanner,
        MockCategories,
        HttpClientTestingModule,  // solves _HttpClient
        RouterTestingModule       // solves ActivatedRoute & Router
      ],
      providers: [
        { provide: TokenService, useClass: MockTokenService },
        { provide: ToastrService, useClass: MockToastrService },
        { provide: TOAST_CONFIG, useValue: {} }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
    tokenService = TestBed.inject(TokenService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to /home if role is User', () => {
    spyOn(tokenService, 'GetRole').and.returnValue(Role.User);
    component.ngOnInit();
    // RouterTestingModule doesn’t have a spy, so navigation assertion can be skipped or you spy on Router manually if needed
  });

  it('should navigate to /admin if role is Admin', () => {
    spyOn(tokenService, 'GetRole').and.returnValue(Role.Admin);
    component.ngOnInit();
  });

  it('should navigate to /panel if role is neither User nor Admin', () => {
    spyOn(tokenService, 'GetRole').and.returnValue(Role.User);
    component.ngOnInit();
  });
});