import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompareProduct } from './compare-product';

describe('ComapreProduct', () => {
  let component: CompareProduct;
  let fixture: ComponentFixture<CompareProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompareProduct]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CompareProduct);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
