import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Banner } from './banner';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from '../../../MaterialModule';

describe('Banner', () => {
  let component: Banner;
  let fixture: ComponentFixture<Banner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        Banner,
        MaterialModule,           
        RouterTestingModule,      
        BrowserAnimationsModule   
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Banner);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});