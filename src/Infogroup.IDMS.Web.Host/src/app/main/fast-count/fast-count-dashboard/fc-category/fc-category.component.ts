import { Component, Injector, OnInit } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CampaignCopyDto, CampaignsServiceProxy, SegmentSelectionsServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from "rxjs/operators";

@Component({
  selector: 'app-fc-category',
  templateUrl: './fc-category.component.html'
})
export class FcCategoryComponent extends AppComponentBase implements OnInit {
  isLoading: boolean = false;
  category: any[] = [];
  constructor(injector: Injector, private _orderServiceProxy: CampaignsServiceProxy, private _segmentSelectionServiceProxy: SegmentSelectionsServiceProxy, public router: Router) {
    super(injector);
  }

  ngOnInit() {
    this.category = [{
      index: "1",
      title: "Cat Owners",
      description: "",
      value: "",
      copyId: 1146402
    }, {
      index: "2",
      title: "Dog Owners",
      description: "",
      value: "",
      copyId: 1146403
    }, {
      index: "3",
      title: "Business Professionals",
      description: "",
      value: "",
      copyId: 1146406
    }];
  }

  getCampaignForCopy(id: number) {
    this.isLoading = true;
    this._orderServiceProxy.getCampaignForCopy(id).subscribe(result => {
      const copyCampaigns = CampaignCopyDto.fromJS(result.campaignCopyData);
      copyCampaigns.mailer.value = -copyCampaigns.mailer.value;
      copyCampaigns.cDescription = "FastCount_" + this.appSession.idmsUser.idmsUserName + "_" + new Date().toLocaleString();
      this.createCopyOfCampaign(copyCampaigns);
    });
  }

  createCopyOfCampaign(campaign: CampaignCopyDto): void {
    this._orderServiceProxy.createCopyCampaign(campaign).subscribe(result => {
      this.notify.info(this.l('Order Created'));
      this._segmentSelectionServiceProxy
        .getSegmentIdForOrderLevel(result[0].campaignId)
        .pipe(finalize(() => this.isLoading = false))
        .subscribe(segId => {
          let fastCountSelectionDetails: NavigationExtras = {
            queryParams: {
              campaignId: result[0].campaignId,
              segmentId: segId,
              databaseId: result[0].databaseID,
              divisionId: result[0].divisionId,
              buildId: result[0].buildID,
              mailerId: result[0].mailerId,
              build: result[0].build,
              campaignDescription: result[0].campaignDescription,
              splitType: result[0].splitType,
              CreatedBy: '',
              divisionalDatabase: campaign.divisionalDatabase,
              channelType: "E"
            }
          };

          this.router.navigateByUrl('app/main/fast-count/fast-count?Edit=1&&campaignId=' + result[0].campaignId + '&databaseId=' + result[0].databaseID + '&mailerId=' + result[0].mailerId + '&segmentId=' + segId + '&buildId=' + result[0].buildID + '&divisionId=' + result[0].divisionId, { state: fastCountSelectionDetails });
        });
    });
  }
}
