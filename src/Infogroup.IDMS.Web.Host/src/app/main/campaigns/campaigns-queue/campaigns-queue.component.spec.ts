import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignsQueueComponent } from './campaigns-queue.component';

describe('CampaignsQueueComponent', () => {
  let component: CampaignsQueueComponent;
  let fixture: ComponentFixture<CampaignsQueueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CampaignsQueueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignsQueueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
