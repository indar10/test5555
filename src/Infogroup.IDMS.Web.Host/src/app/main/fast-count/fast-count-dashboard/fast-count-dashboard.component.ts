import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    CampaignGeneralDto,
    CampaignLevelMaxPerDto,
    CampaignsServiceProxy,
    CampaignCopyDto,
    IdmsUserLoginInfoDto,
    SegmentSelectionsServiceProxy, DropdownOutputDto,
    IDMSConfigurationsServiceProxy,
    CreateOrEditCampaignDto, GetCampaignMaxPerForViewDto, GetDecoyForViewDto, EditCampaignsOutputDto
} from '@shared/service-proxies/service-proxies';
import { SelectItem } from 'primeng/api';
import { DatePipe } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Router, NavigationExtras } from '@angular/router';
import { CampaignStatus } from '@app/main/campaigns/shared/campaign-status.enum';
import {Constants} from "@app/main/fast-count/constants";

@Component({
    selector: 'app-fast-count-dashboard',
    templateUrl: './fast-count-dashboard.component.html',
    styleUrls: ['./fast-count-dashboard.component.css'],
    providers: [DatePipe, NgbActiveModal],
    animations: [appModuleAnimation()]
})
export class FastCountDashboardComponent extends AppComponentBase implements OnInit {
    campaign!: CampaignGeneralDto | undefined;
    copyCampaigns: CampaignCopyDto = new CampaignCopyDto();
    maxPer!: CampaignLevelMaxPerDto | undefined;
    databases: any[] = [];
    buildId: number;
    databaseId: number;
    isLoading: boolean;
    channelData: SelectItem[] = [{ label: 'Email', value: 'E' }, { label: 'Postal', value: 'P' }, { label: 'Tele-marketing', value: 'T' }];
    copying: boolean = false;
    isCopyCompleted: boolean = false;
    idmsUserData: IdmsUserLoginInfoDto;
    cpySegmentId: number;
    otherDatabases: any[] = [];
    databaseName: string;
    selectedChannelTypeForOtherDb: string = "P"; // used for other databases in HTML

    constructor(injector: Injector, private _orderServiceProxy: CampaignsServiceProxy, public activeModal: NgbActiveModal, public router: Router, private _segmentSelectionServiceProxy: SegmentSelectionsServiceProxy, private _idmsConfig: IDMSConfigurationsServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.isLoading = true;
        this.getDatabases();
        this.campaign = new CampaignGeneralDto();
        this.maxPer = new CampaignLevelMaxPerDto();
        this.idmsUserData = this.appSession.idmsUser;
    }

    getFastCountConfiguration(databaseID, channelType: string) {
        this._idmsConfig.getConfigurationValue("FastCount", databaseID).subscribe(result => {
            const fastCountConfig = JSON.parse(result.mValue);
            if ("campaign-reference" in fastCountConfig && channelType in fastCountConfig["campaign-reference"]) {
                this.getCampaignForCopy(fastCountConfig["campaign-reference"][channelType], channelType);
            } else {
                this.createNewCampaign(databaseID, channelType, fastCountConfig);
            }
        });
    }

    createNewCampaign(databaseID: number, channelType: string, fastCountConfig: any) {
        this._orderServiceProxy.getGenericMailerForDB(databaseID).subscribe((result) => {
            const input = new CreateOrEditCampaignDto();
            input.generalData = new CampaignGeneralDto();
            input.generalData.cDescription = "FastCount_" + this.idmsUserData.idmsUserName + "_" + new Date().toLocaleString();
            input.generalData.databaseID = databaseID;
            input.generalData.buildID = this.buildId;
            input.generalData.divisionalDatabase = false;
            input.generalData.cChannelType = channelType;
            input.generalData.cOrderType = "N";
            input.generalData.broker = new DropdownOutputDto();
            input.generalData.broker.value = fastCountConfig["broker"]["value"];
            input.generalData.broker.label = fastCountConfig["broker"]["label"];
            input.generalData.mailer = new DropdownOutputDto();
            input.generalData.mailer.value = -result;
            input.maxPerData = new GetCampaignMaxPerForViewDto();
            input.maxPerData.getCampaignLevelMaxPerData = new CampaignLevelMaxPerDto();
            input.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel = "";
            input.isStatusChangeRequired = true;
            if (!input.campaignOutputDto) {
                input.campaignOutputDto = new EditCampaignsOutputDto();
            }
            input.campaignOutputDto.shipNotes = Constants.fastCountShippingNote;
            this._orderServiceProxy.createOrEdit(input, CampaignStatus.CampaignCreated)
                .subscribe(result => {
                    this.notify.info(this.l('Order Created Successfully'));
                    this.router.navigateByUrl('app/main/fast-count/fast-count?Edit=1&&campaignId=' + result.campaignId + '&databaseId=' + result.databaseID + '&mailerId=' + result.mailerId + '&segmentId=' + result.segmentID + '&buildId=' + result.buildID + '&divisionId=' + result.divisionId);
                    this.isLoading = false;
                }, () => this.isLoading = false);
        }, () => {
            this.isLoading = false;
        });
    }

    getDatabases() {
        this._idmsConfig
            .getConfigurationValue("FastCount", 0).subscribe(item => {
            this._orderServiceProxy.getDatabaseWithLatestBuild(undefined).subscribe((result) => {
                const fastCountConfig = JSON.parse(item.mValue);
                this.databases = result.databases.databases.filter(k => fastCountConfig["dashboard"]["databases"].includes(k.value));
                this.databases.forEach(k => k.channelType = this.channelData[1].value);
                this.campaign.divisionalDatabase = result.divisionalDatabase;
                this.otherDatabases = result.databases.databases.filter(k => !fastCountConfig["dashboard"]["databases"].includes(k.value));
                this.otherDatabases.forEach(k => k.channelType = this.channelData[1].value);
                this.isLoading = false;
            }, () => {
                this.isLoading = false;
            });
        }, () => {
            this.isLoading = false;
        });
    }

    onSaveCompaign(databaseSelected: any) {
        this.databaseId = databaseSelected.value;
        this.isLoading = true;
        this._orderServiceProxy.getDatabaseWithLatestBuild(this.databaseId).subscribe(
            (result1) => {
                this.buildId = result1.builds.defaultSelection ? result1.builds.defaultSelection : result1.builds.buildDropDown[0].value;
                this.getFastCountConfiguration(this.databaseId, databaseSelected.channelType);
            },
            () => {
                this.isLoading = false;
            }
        );
    }

    getCampaignForCopy(Id, channelType: string) {
        this._orderServiceProxy.getCampaignForCopy(Id).subscribe(result => {
            this.copyCampaigns.init(result.campaignCopyData);
            this.copyCampaigns.mailer.value = -this.copyCampaigns.mailer.value;
            this.copyCampaigns.cDescription = "FastCount_" + this.idmsUserData.idmsUserName + "_" + new Date().toLocaleString();
            this.createCopyOfCampaign(this.copyCampaigns, channelType);
            this.isCopyCompleted = false;
        });
    }

    createCopyOfCampaign(campaign, channelType: string): void {
        this.copying = true;
        campaign.buildId = this.buildId;
        this._orderServiceProxy.createCopyCampaign(campaign).subscribe(result => {
            this.notify.info(this.l('Order Created'));
            this.isCopyCompleted = true;
            let ltstCampaign = result;
            this._segmentSelectionServiceProxy
                .getSegmentIdForOrderLevel(ltstCampaign[0].campaignId)
                .subscribe(result => {
                    this.cpySegmentId = result;
                    let fastCountSelectionDetails: NavigationExtras = {
                        queryParams: {
                            campaignId: ltstCampaign[0].campaignId,
                            segmentId: this.cpySegmentId,
                            databaseId: ltstCampaign[0].databaseID,
                            divisionId: ltstCampaign[0].divisionId,
                            databaseName: this.databaseName,
                            buildId: this.buildId,
                            mailerId: ltstCampaign[0].mailerId,
                            build: ltstCampaign[0].build,
                            campaignDescription: ltstCampaign[0].campaignDescription,
                            splitType: ltstCampaign[0].splitType,
                            CreatedBy: '',
                            divisionalDatabase: this.campaign.divisionalDatabase,
                            channelType: channelType,
                        }
                    };

                    this.router.navigateByUrl('app/main/fast-count/fast-count?Edit=1&&campaignId=' + ltstCampaign[0].campaignId + '&databaseId=' + ltstCampaign[0].databaseID + '&mailerId=' + ltstCampaign[0].mailerId + '&segmentId=' + this.cpySegmentId + '&buildId=' + this.buildId + '&divisionId=' + ltstCampaign[0].divisionId + '&databaseName=' + this.databaseName, { state: fastCountSelectionDetails });
                    this.isLoading = false;
                });
        });
    }
}

