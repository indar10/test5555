import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateOrEditExternaDatabaseLinksComponent } from './create-or-edit-externa-database-links.component';

describe('CreateOrEditExternaDatabaseLinksComponent', () => {
  let component: CreateOrEditExternaDatabaseLinksComponent;
  let fixture: ComponentFixture<CreateOrEditExternaDatabaseLinksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateOrEditExternaDatabaseLinksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateOrEditExternaDatabaseLinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
