import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SolarAdvisor } from './solar-advisor';

describe('SolarAdvisor', () => {
  let component: SolarAdvisor;
  let fixture: ComponentFixture<SolarAdvisor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SolarAdvisor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SolarAdvisor);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
