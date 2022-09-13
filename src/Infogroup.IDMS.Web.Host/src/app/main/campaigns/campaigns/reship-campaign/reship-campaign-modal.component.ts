import { Component, Input, Injector, ViewChild, ElementRef } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { finalize } from "rxjs/operators";
import { CampaignsServiceProxy } from "@shared/service-proxies/service-proxies";

@Component({
  selector: "reship-campaign",
  templateUrl: "./reship-campaign-modal.component.html"
})
export class ReshipCampaignModalComponent extends AppComponentBase {
  @Input() campaignId: number;
  @Input() databaseId: number;
  @Input() campaignDescription: string = '';
  reshipEmail: string = '';
  @ViewChild('reshipEmailInput', { read: ElementRef, static: true }) emailInput: any;

  saving: boolean = false;
  constructor(
    injector: Injector,
    private activeModal: NgbActiveModal,
    private _campaignsServiceProxy: CampaignsServiceProxy
  ) {
    super(injector);
  }

  ngAfterViewInit() {
    this.emailInput.nativeElement.focus();
  }

  save(): void {
    this._campaignsServiceProxy.reshipCampaign
      (this.campaignId, this.databaseId, this.reshipEmail)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.success(this.l('Campaign_Submit_ReShip', this.campaignId));
        this.activeModal.close({ isSave: true });
      }
      );
  }

  close(): void {
    this.activeModal.close({ isSave: false });
  }
}
