import { Component, Injector, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, NavigationExtras } from '@angular/router';
import { CampaignStatus } from '@app/main/campaigns/shared/campaign-status.enum';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CampaignsServiceProxy, DatabaseDto, DatabasesServiceProxy, GetCampaignsListForView, UserListDto, UserServiceProxy,
  CampaignCopyDto, SegmentSelectionsServiceProxy, CampaignGeneralDto, IdmsUserLoginInfoDto, IDMSConfigurationsServiceProxy
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { Table } from 'primeng/table';
import { CreateOrEditSaveCountComponent } from "../fast-count/create-or-edit-save-count.component";
import { ModalDefaults } from "../../../../shared/costants/modal-contants";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { FcPlaceorderComponent } from '../fast-count/create-or-edit/fc-placeorder/fc-placeorder.component';

@Component({
  selector: 'app-fast-count-history',
  templateUrl: './fast-count-history.component.html',
  styleUrls: ['./fast-count-history.component.css']
})

export class FastCountHistoryComponent extends AppComponentBase {
  maxResultCount: number = 500;
  skipCount: number = 0;
  databases: DatabaseDto[] = [];
  users: UserListDto[] = [];
  historyDto: HistoryDTO;
  @ViewChild("dataTable", { static: true }) dataTable: Table;
  @ViewChild("fcHistoryFilters", { static: true }) historyFilters: NgForm;
  records: GetCampaignsListForView[] = [];
  selectedTab = FastCountTabs;
  copyCampaigns: CampaignCopyDto = new CampaignCopyDto();
  isCopyCompleted: boolean = false;
  copying: boolean = false;
  cpySegmentId: number;
  campaign!: CampaignGeneralDto | undefined;
  idmsUserData: IdmsUserLoginInfoDto;
  Edit: number = 1;
  isSave = false;
  fastCountConfig: any;
  isLoading: boolean = true;
  hasSalesPermission: boolean;

  constructor(injector: Injector, private _userServiceProxy: UserServiceProxy, private _databasesServiceProxy: DatabasesServiceProxy, private _campaignServiceProxy: CampaignsServiceProxy, private _orderServiceProxy: CampaignsServiceProxy,
    private _segmentSelectionServiceProxy: SegmentSelectionsServiceProxy, public router: Router, private modalService: NgbModal, private _idmsConfig: IDMSConfigurationsServiceProxy) {
    super(injector);
    this.setDefaultFilters();
    this.searchByFilters();
    this.idmsUserData = this.appSession.idmsUser;
  }

  ngOnInit(): void {
    this.hasSalesPermission = !this.isGranted("Pages.PlaceOrder");
    if (this.hasSalesPermission) {
      this.historyDto.selectedTab = FastCountTabs.All;
    }
  }

  setDefaultFilters() {
    this.historyDto = {
      databases: [],
      users: [UserListDto.fromJS({
        id: this.appSession.user.id,
        userName: this.appSession.user.userName
      })],
      selectedTab: FastCountTabs.All,
      search: '',
      selectedDateRange: [],
    }
  }

  searchDatabases(event) {
    this._databasesServiceProxy.getAllDatabases(
      event.query,
      this.appSession.idmsUser.idmsUserID,
      undefined,
      this.skipCount,
      this.maxResultCount
    ).subscribe(result => this.databases = result.items
    );
  }

  searchUsers(event) {
    this._userServiceProxy.getUsers(
      event.query,
      undefined,
      undefined,
      false,
      undefined,
      this.maxResultCount,
      this.skipCount
    ).subscribe(result => this.users = result.items);
  }

  changeTab(tab: FastCountTabs) {
    this.historyDto.selectedTab = tab;
    this.searchByFilters();
  }

  clearfcHistoryFilters(): void {
    const isDirty = this.historyFilters.dirty;
    this.historyFilters.resetForm();
    this.setDefaultFilters();
    if (isDirty) this.searchByFilters();
  }

  searchByFilters(): void {
    this.isLoading = true;
    const statusArray = [];
    if (this.historyDto.selectedTab === FastCountTabs.Orders) {
      statusArray.push(...[
        CampaignStatus.OutputSubmitted,
        CampaignStatus.OutputRunning,
        CampaignStatus.OutputCompleted,
        CampaignStatus.OutputFailed,
        CampaignStatus.ApprovedforShipping,
        CampaignStatus.WaitingtoShip,
        CampaignStatus.Shipped,
        CampaignStatus.ShippingFailed,
        CampaignStatus.Cancelled,
      ]);
    } else if (this.historyDto.selectedTab === FastCountTabs.Saved) {
      statusArray.push(...[
        CampaignStatus.CampaignCreated,
        CampaignStatus.CampaignSubmitted,
        CampaignStatus.CampaignRunning,
        CampaignStatus.CampaignCompleted,
        CampaignStatus.CampaignFailed,
        CampaignStatus.ReadytoOutput,
      ]);
    }

    this._campaignServiceProxy.getAllFastCountCampaignsList(
      this.historyDto.search,
      statusArray,
      this.historyDto.databases.map(item => `${item.id}`),
      this.historyDto.users.map(item => `${item.id}`),
      this.historyDto.selectedDateRange,
      undefined,
      0,
      1000
    )
      .subscribe(result => {
        this.records = result.items != null ? result.items : [];
        this.isLoading = false;
      }, () => this.isLoading = false);
  }

  openFastCountView(record: GetCampaignsListForView, isEdit: string) {
    const url = this.router.serializeUrl(
      this.router.createUrlTree(['app/main/fast-count/fast-count'], {
        queryParams: {
          "Edit": isEdit,
          "campaignId": record.campaignId,
          "databaseId": record.databaseID,
          "mailerId": record.mailerId,
          "segmentId": record.segmentID,
          "buildId": record.buildID,
          "divisionId": record.divisionId
        }
      })
    );
    window.open(url, '_blank');
  }

  duplicateAndEdit(Id) {
    this._orderServiceProxy.getCampaignForCopy(Id).subscribe(result => {
      this.copyCampaigns.init(result.campaignCopyData);
      this.copyCampaigns.mailer.value = -this.copyCampaigns.mailer.value;
      this.copyCampaigns.cDescription = "FastCount_" + this.idmsUserData.idmsUserName + "_" + new Date().toLocaleString();
      this.createCopyOfCampaign(this.copyCampaigns);
      this.isCopyCompleted = false;
    });
  }

  createCopyOfCampaign(campaign): void {
    this.copying = true;
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
              databaseName: ltstCampaign[0].databaseName,
              buildId: ltstCampaign[0].buildID,
              mailerId: ltstCampaign[0].mailerId,
              build: ltstCampaign[0].build,
              campaignDescription: ltstCampaign[0].campaignDescription,
              splitType: ltstCampaign[0].splitType,
              CreatedBy: '',
              divisionalDatabase: ltstCampaign[0].databaseID,
              channelType: 'E'
            }
          };

          this.router.navigateByUrl('app/main/fast-count/fast-count?Edit=1&&campaignId=' + ltstCampaign[0].campaignId + '&databaseId=' + ltstCampaign[0].databaseID + '&mailerId=' + ltstCampaign[0].mailerId + '&segmentId=' + this.cpySegmentId + '&buildId=' + ltstCampaign[0].buildID + '&divisionId=' + ltstCampaign[0].divisionId, { state: fastCountSelectionDetails });
        });
    });
  }
  getFastCountConfig(campaign, activeForm) {
    this._idmsConfig.getConfigurationValue("FastCount", campaign.databaseID).subscribe(result => {
      this.fastCountConfig = JSON.parse(result.mValue);
      if (this.fastCountConfig)
      this._campaignServiceProxy.getDatabaseWithLatestBuild(campaign.databaseID)
      .subscribe((result) => {
        campaign.divisionalDatabase = result.divisionalDatabase,
        this.openModalPopup(campaign, activeForm, this.fastCountConfig);
      });      
    });
  }

  openModalPopup(campaign, activeForm: string, fastCountConfig): void {
    const modalRef: NgbModalRef = this.modalService.open(
      FcPlaceorderComponent,
      {
        size: ModalDefaults.Size,
        backdrop: ModalDefaults.Backdrop,
        windowClass: ModalDefaults.WindowClass,
      }
    );   
      modalRef.componentInstance.campaignId =
          campaign.campaignId;
      modalRef.componentInstance.databaseId =
          campaign.databaseID;
      modalRef.componentInstance.divisionId =
          campaign.divisionId;
      modalRef.componentInstance.cDescription =
          campaign.campaignDescription;
      modalRef.componentInstance.divisionalDatabase =
          campaign.divisionalDatabase;
      modalRef.componentInstance.channelType =
          campaign.channelType;
      modalRef.componentInstance.activeForm = activeForm;
      modalRef.componentInstance.fastCountConfig = fastCountConfig;
      modalRef.componentInstance.buildId = campaign.buildId;
    
    if (activeForm === "PlaceOrder" || activeForm === "CountReport") {
      modalRef.componentInstance.segmentId =
        campaign.segmentId;
    }
    modalRef.result.then((result) => {
      if (result.saving) {
        if (campaign.campaignId) {
          this.isSave = true;
        }
      }
    });
  }
}

enum FastCountTabs {
  "All",
  "Orders",
  "Saved"
}

interface HistoryDTO {
  search: string;
  databases: DatabaseDto[];
  users: UserListDto[];
  selectedTab: FastCountTabs;
  selectedDateRange: moment.Moment[];
}