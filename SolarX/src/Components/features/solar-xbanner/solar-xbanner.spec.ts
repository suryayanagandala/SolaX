import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SolarXBanner } from './solar-xbanner';

describe('SolarXBanner', () => {
  let component: SolarXBanner;
  let fixture: ComponentFixture<SolarXBanner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SolarXBanner]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SolarXBanner);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
