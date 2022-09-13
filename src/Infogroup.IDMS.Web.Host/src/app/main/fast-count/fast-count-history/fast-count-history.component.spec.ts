import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FastCountHistoryComponent } from './fast-count-history.component';

describe('FastCountHistoryComponent', () => {
  let component: FastCountHistoryComponent;
  let fixture: ComponentFixture<FastCountHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FastCountHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FastCountHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
