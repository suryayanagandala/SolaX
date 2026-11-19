import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WhyThis } from './why-this';

describe('WhyThis', () => {
  let component: WhyThis;
  let fixture: ComponentFixture<WhyThis>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WhyThis]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WhyThis);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
