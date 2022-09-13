import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CampaignBillingDto, CampaignGeneralDto, CampaignLevelMaxPerDto, CampaignsServiceProxy, CreateOrEditCampaignDto, DropdownOutputDto, EditCampaignsOutputDto, GetCampaignMaxPerForViewDto, GetDecoyForViewDto } from '@shared/service-proxies/service-proxies';
import { SelectItem } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import {Constants} from "@app/main/fast-count/constants";

@Component({
  selector: 'app-fc-maxper',
  templateUrl: './fc-maxper.component.html',
})

export class FcMaxperComponent extends AppComponentBase implements OnInit {
  isLoading: boolean = true;
  // hideQuantityFields: boolean;
  maxPerFieldsFastCountlevel: SelectItem[] = [];
  campaign: CreateOrEditCampaignDto = CreateOrEditCampaignDto.fromJS({
    maxPerData: GetCampaignMaxPerForViewDto.fromJS({
      getCampaignLevelMaxPerData: new CampaignLevelMaxPerDto(),
    }),
  });
  // cMaxPerFieldOrderLevel: any;

  @Input() campaignId: number;
  @Input() databaseId: number;
  @Input() divisionId: number;
  @Input() buildId: number;
  @Input() fastCountConfig: any;


  constructor(injector: Injector, public activeModal: NgbActiveModal, private _orderServiceProxy: CampaignsServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getCampaignForEdit();
    this.getMaxPerValuesforDropdown();
  }

  getCampaignForEdit() {
    this.isLoading = true;
    this._orderServiceProxy.getCampaignForEdit(this.campaignId, this.databaseId, this.divisionId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        this.campaign = result;
        // const campaignLevelMaxPer = this.campaign.maxPerData.getCampaignLevelMaxPerData;
        // if (!this.hideQuantityFields) {
        //   this.cMaxPerFieldOrderLevel = campaignLevelMaxPer.cMaxPerFieldOrderLevel;
        // } else {
        //   const configMaxPer = this.maxPerFieldsFastCountlevel.find(fcItem => fcItem.value.cMaxPerFieldOrderLevel == campaignLevelMaxPer.cMaxPerFieldOrderLevel && campaignLevelMaxPer.cMaximumQuantity == fcItem.value.cMaximumQuantity);
        //   if (configMaxPer) {
        //     this.cMaxPerFieldOrderLevel = configMaxPer.value
        //   }
        // }
      });
  }

  getMaxPerValuesforDropdown() {
    // if (this.fastCountConfig['max-per'] && this.fastCountConfig['max-per']['options']) {
    //   const fastCountFieldsList: any[] = this.fastCountConfig['max-per']['options'];
    //   this.maxPerFieldsFastCountlevel = fastCountFieldsList.map(fcItem => {
    //     return {
    //       label: fcItem.name,
    //       value: fcItem.value
    //     }
    //   });
    //   this.hideQuantityFields = true;
    // } else {
      this._orderServiceProxy.getMaxPerFieldDropdownData(this.databaseId, this.buildId, '').subscribe(result => {
        this.maxPerFieldsFastCountlevel.push({
          label: "All Per",
          value: ""
        });
        for (let index = 0; index < result.length; index++) {
          this.maxPerFieldsFastCountlevel.push({ label: result[index].label, value: result[index].value });
        }
      });
    // }
  }

  onMaxPerChange(event: any): void {
    if (event.value == '') {
      this.campaign.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity = 0;
      this.campaign.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity = 0;
    } else {
      this.campaign.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity = 1;
      this.campaign.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity = undefined;
    }
  }

  getData() {
    return this.maxPerFieldsFastCountlevel;
  }

  save() {
    this.isLoading = true;
    this._orderServiceProxy.createOrEdit(this.getCampaignData(), this.campaign.currentStatus)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(() => {
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
          ...this.campaign.maxPerData.getCampaignLevelMaxPerData
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
