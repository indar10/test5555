import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateOrEditListoflistComponent } from './create-or-edit-listoflist.component';

describe('CreateOrEditListoflistComponent', () => {
  let component: CreateOrEditListoflistComponent;
  let fixture: ComponentFixture<CreateOrEditListoflistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateOrEditListoflistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateOrEditListoflistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
