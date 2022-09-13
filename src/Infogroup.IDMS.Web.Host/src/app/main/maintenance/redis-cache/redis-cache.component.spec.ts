import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RedisCacheComponent } from './redis-cache.component';

describe('RedisCacheComponent', () => {
  let component: RedisCacheComponent;
  let fixture: ComponentFixture<RedisCacheComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RedisCacheComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RedisCacheComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
