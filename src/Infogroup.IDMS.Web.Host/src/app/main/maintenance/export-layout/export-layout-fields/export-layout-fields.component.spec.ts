import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportLayoutFieldsComponent } from './export-layout-fields.component';

describe('ExportLayoutFieldsComponent', () => {
  let component: ExportLayoutFieldsComponent;
  let fixture: ComponentFixture<ExportLayoutFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExportLayoutFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExportLayoutFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
