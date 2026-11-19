import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServicesBanner } from './services';

describe('Services', () => {
  let component: ServicesBanner;
  let fixture: ComponentFixture<ServicesBanner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ServicesBanner]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServicesBanner);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
