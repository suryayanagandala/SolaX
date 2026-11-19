import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Model1 } from './model-1';

describe('Model1', () => {
  let component: Model1;
  let fixture: ComponentFixture<Model1>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Model1]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Model1);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
