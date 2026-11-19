import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PricingModel } from './pricing-model';

describe('PricingModel', () => {
  let component: PricingModel;
  let fixture: ComponentFixture<PricingModel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PricingModel]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PricingModel);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
