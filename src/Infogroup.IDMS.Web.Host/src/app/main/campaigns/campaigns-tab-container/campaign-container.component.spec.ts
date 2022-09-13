import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CampaignContainerComponent } from './campaign-container.component';


describe('CampaignsTabContainerNGXComponent', () => {
  let component: CampaignContainerComponent;
  let fixture: ComponentFixture<CampaignContainerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CampaignContainerComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
