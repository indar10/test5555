import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FastCountDashboardComponent } from './fast-count-dashboard.component';

describe('FastCountDashboardComponent', () => {
  let component: FastCountDashboardComponent;
  let fixture: ComponentFixture<FastCountDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FastCountDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FastCountDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
