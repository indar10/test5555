import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { MaintainanceContainerComponent } from './maintainance-container.component';


describe('MaintainanceTabContainerNGXComponent', () => {
    let component: MaintainanceContainerComponent;
    let fixture: ComponentFixture<MaintainanceContainerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [MaintainanceContainerComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(MaintainanceContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
