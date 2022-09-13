import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomSuppressionComponent } from './custom-suppression.component';

describe('CustomSuppressionComponent', () => {
  let component: CustomSuppressionComponent;
  let fixture: ComponentFixture<CustomSuppressionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomSuppressionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomSuppressionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
