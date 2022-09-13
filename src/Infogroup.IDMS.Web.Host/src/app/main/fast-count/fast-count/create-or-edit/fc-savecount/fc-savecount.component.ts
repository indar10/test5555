import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CampaignBillingDto, CampaignGeneralDto, CampaignLevelMaxPerDto, CampaignsServiceProxy, CreateOrEditCampaignDto, DropdownOutputDto, EditCampaignsOutputDto, GetCampaignMaxPerForViewDto, GetCampaignsOutputDto, GetDecoyForViewDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import {Constants} from "@app/main/fast-count/constants";

@Component({
  selector: 'app-fc-savecount',
  templateUrl: './fc-savecount.component.html'
})
export class FcSavecountComponent extends AppComponentBase implements OnInit  {
  isLoading: boolean = true;
  campaign: CreateOrEditCampaignDto = CreateOrEditCampaignDto.fromJS({
    generalData: new CampaignGeneralDto(),
    getOutputData: new GetCampaignsOutputDto()
  });

  @Input() campaignId: number;
  @Input() databaseId: number;
  @Input() divisionId: number;
  @Input() fastCountConfig: any;

  constructor(injector: Injector, public activeModal: NgbActiveModal, private _orderServiceProxy: CampaignsServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getCampaignForEdit();
  }

  getCampaignForEdit() {
    this.isLoading = true;
    this._orderServiceProxy.getCampaignForEdit(this.campaignId, this.databaseId, this.divisionId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => this.campaign = result);
  }

  save() {
    this.isLoading = true;
    this._orderServiceProxy.createOrEdit(this.getCampaignData(), this.campaign.currentStatus)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(() => {
      this._orderServiceProxy.updateFastCountMailerId(this.campaign.id).subscribe(r => console.log("mailer updated"));
      this.notify.info(this.l('SavedSuccessfully'));
      this.close(true);
    });
  }
  
  getCampaignData(): CreateOrEditCampaignDto {
    const campaignDto = CreateOrEditCampaignDto.fromJS({
      id: this.campaign.id,
      generalData: CampaignGeneralDto.fromJS(this.campaign.generalData),
      listOfXTabRecords: [],
      listOfMultidimensionalRecords: [],
      campaignOutputDto: {
        ...EditCampaignsOutputDto.fromJS(this.campaign.getOutputData),
        layoutDescription: undefined,
        layout: undefined,
        email: undefined,
        layoutId: undefined,
        shippedDate: undefined,
      },
      billingData: CampaignBillingDto.fromJS(this.campaign.billingData),
      maxPerData: GetCampaignMaxPerForViewDto.fromJS({
        campaignId: this.campaign.id,
        getSegmentLevelMaxPerData: [],
        getCampaignLevelMaxPerData: CampaignLevelMaxPerDto.fromJS({
          cMaximumQuantity: this.campaign.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity,
          cMinimumQuantity: this.campaign.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity,
          cMaxPerFieldOrderLevel: this.campaign.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel
        }),
      }),
      decoyData: GetDecoyForViewDto.fromJS({
        listOfDecoys: [],
        isDecoyKeyMethod: this.campaign.decoyData.isDecoyKeyMethod,
        decoyKey: this.campaign.decoyData.decoyKey,
        decoyKey1: this.campaign.decoyData.decoyKey1
      }),
    });
    if (!campaignDto.generalData.broker || !campaignDto.generalData.broker.value) {
      campaignDto.generalData.broker = DropdownOutputDto.fromJS({
        label: this.fastCountConfig["broker"]["label"],
        value: this.fastCountConfig["broker"]["value"]
      });
      campaignDto.generalData.brokerDescription = this.fastCountConfig["broker"]["label"];
    }
    campaignDto.campaignOutputDto.shipNotes = Constants.fastCountShippingNote;
    return campaignDto;
  }

  close(saving: boolean = false): void {
    this.activeModal.close({ saving });
  }
}
