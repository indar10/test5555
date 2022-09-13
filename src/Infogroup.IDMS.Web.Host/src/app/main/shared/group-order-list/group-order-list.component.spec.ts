import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupOrderListComponent } from './group-order-list.component';

describe('GroupOrderListComponent', () => {
  let component: GroupOrderListComponent;
  let fixture: ComponentFixture<GroupOrderListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GroupOrderListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupOrderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
