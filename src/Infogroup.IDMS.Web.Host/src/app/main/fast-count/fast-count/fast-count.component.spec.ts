import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FastCountComponent } from './fast-count.component';

describe('FastCountComponent', () => {
  let component: FastCountComponent;
  let fixture: ComponentFixture<FastCountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FastCountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FastCountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
