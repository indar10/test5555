import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExternalDatabaseLinksComponent } from './external-database-links.component';

describe('ExternalDatabaseLinksComponent', () => {
  let component: ExternalDatabaseLinksComponent;
  let fixture: ComponentFixture<ExternalDatabaseLinksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExternalDatabaseLinksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExternalDatabaseLinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
