import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Model2 } from './model-2';

describe('Model2', () => {
  let component: Model2;
  let fixture: ComponentFixture<Model2>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Model2]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Model2);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
