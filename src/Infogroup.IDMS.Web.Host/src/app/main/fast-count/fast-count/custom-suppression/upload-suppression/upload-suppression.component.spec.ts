import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadSuppressionComponent } from './upload-suppression.component';

describe('UploadSuppressionComponent', () => {
  let component: UploadSuppressionComponent;
  let fixture: ComponentFixture<UploadSuppressionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadSuppressionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadSuppressionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
