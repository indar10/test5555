import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessQueueDatabasesComponent } from './process-queue-databases.component';

describe('ProcessQueueDatabasesComponent', () => {
  let component: ProcessQueueDatabasesComponent;
  let fixture: ComponentFixture<ProcessQueueDatabasesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcessQueueDatabasesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcessQueueDatabasesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
