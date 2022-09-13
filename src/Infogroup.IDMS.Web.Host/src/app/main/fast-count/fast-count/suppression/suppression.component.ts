import { Component, Injector, Input, ViewChild } from "@angular/core";
import { NgForm } from '@angular/forms';
import { CampaignAction } from '@app/main/campaigns/shared/campaign-action.enum';
import { CampaignStatus } from '@app/main/campaigns/shared/campaign-status.enum';
import { CampaignUiHelperService } from '@app/main/campaigns/shared/campaign-ui-helper.service';
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
  ActionType,
  CampaignActionInputDto,
  CampaignsServiceProxy,
  CreateOrEditSegmentPrevOrdersDto,
  GetCampaignsListForView,
  GetSegmentPrevOrdersForViewDto,
  SegmentPrevOrdersDto,
  SegmentPrevOrdersesServiceProxy,
  UserListDto,
  UserServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";

@Component({
  selector: "app-suppression",
  templateUrl: "./suppression.component.html",
  styleUrls: ["./suppression.component.css"],
})
export class SuppressionComponent extends AppComponentBase {
  @Input() segmentId: number;
  @Input() campaignId: number;
  @Input() buildId: number;
  @Input() databaseId: number;
  @Input() isPlaceOrder: boolean;
  @ViewChild("suppressionFilters", { static: true }) historyFilters: NgForm;
  selectedCampaigns: GetCampaignsListForView[] = [];
  fcCampaigns: GetCampaignsListForView[] = [];
  searchText: string;
  isLoading: boolean = true;
  users: UserListDto[] = [];
  selectedUsers: UserListDto[] = [];
  prevOrdersCampaigns: GetSegmentPrevOrdersForViewDto[] = [];
  actionType = CampaignAction;
  statusType = CampaignStatus;

  constructor(
    injector: Injector,
    private activeModal: NgbActiveModal,
    private _campaignServiceProxy: CampaignsServiceProxy,
    private _userServiceProxy: UserServiceProxy,
    private _segmentPrevOrdersesServiceProxy: SegmentPrevOrdersesServiceProxy,
    private _campaignUiHelperService: CampaignUiHelperService,
  ) {
    super(injector);
    const currentUser = UserListDto.fromJS({
      userName: this.appSession.idmsUser.idmsUserName,
      id: this.appSession.userId
    });
    this.selectedUsers = [currentUser];
  }

  getExistingPreviousOrders() {
    this.isLoading = true;
    this._segmentPrevOrdersesServiceProxy.getExistingPreviousOrders(
      this.campaignId,
      this.segmentId
    ).pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        if (result.listOfSegmentPrevOrders.length) {
          this.prevOrdersCampaigns = result.listOfSegmentPrevOrders;
          result.listOfSegmentPrevOrders.forEach(item => {
            const data = this.fcCampaigns.find(fcItem => fcItem.campaignId === item.orderID);
            this.selectedCampaigns.push(data);
          });
        }
      });
  }

  close(isSave: boolean = false): void {
    this.activeModal.close({ isSave });
  }

  getFCCampaigns() {
    this.isLoading = true;
    const statusArray = [];
    if (this.isPlaceOrder) {
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
    } else {
      statusArray.push(...[
        CampaignStatus.CampaignCreated,
        CampaignStatus.CampaignSubmitted,
        CampaignStatus.CampaignRunning,
        CampaignStatus.CampaignCompleted,
        CampaignStatus.CampaignFailed,
        CampaignStatus.ReadytoOutput,
      ]);
    }
    this._campaignServiceProxy
      .getAllFastCountCampaignsList(
        this.searchText,
        statusArray,
        [`${this.databaseId}`],

        this.selectedUsers ? this.selectedUsers.map((item) => `${item.id}`) : undefined,
        undefined,
        undefined,
        0,
        1000
      )
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((result) => {
        this.fcCampaigns = result.items.filter(
          (item: GetCampaignsListForView) =>
            this.isPlaceOrder ? item.status >= 70 : item.status < 70 && item.campaignId.toString() != this.campaignId.toString()
        );
        this.getExistingPreviousOrders();
      });
  }

  clearSelections(): void {
    this.selectedCampaigns = [];
  }

  clearSuppressionFilters(): void {
    const isDirty = this.historyFilters.dirty;
    this.historyFilters.resetForm();
    if (isDirty) this.getFCCampaigns();
  }

  searchUsers(event) {
    this._userServiceProxy
      .getUsers(event.query, undefined, undefined, false, undefined, 1000, 0)
      .subscribe((result) => (this.users = result.items));
  }

  save(): void {
    const createEditDto = new CreateOrEditSegmentPrevOrdersDto();
    createEditDto.campaignID = this.campaignId;
    createEditDto.listOfSegmentOrders = [];
    let prevOrderId: number[] = [];
    if (this.prevOrdersCampaigns.length) {
      prevOrderId = this.prevOrdersCampaigns.map(item => item.orderID);
      this.prevOrdersCampaigns.forEach(prevItem => {
        if (!this.selectedCampaigns.some(curItem => curItem.campaignId == prevItem.orderID)) {
          const segPrevOrder = new SegmentPrevOrdersDto();
          segPrevOrder.id = prevItem.previousOrderID;
          segPrevOrder.prevOrderID = prevItem.orderID;
          segPrevOrder.segmentId = this.segmentId;
          segPrevOrder.cPrevSegmentID = prevItem.prevSegmentIDs;
          segPrevOrder.cPrevSegmentNumber = "";
          segPrevOrder.cIncludeExclude = "Exclude";
          segPrevOrder.cCompanyFieldName = "Individual";
          segPrevOrder.action = ActionType.Delete;
          createEditDto.listOfSegmentOrders.push(segPrevOrder);
        }
      })
    }
    this.selectedCampaigns.filter(item => !prevOrderId.includes(item.campaignId)).forEach((item: GetCampaignsListForView) => {
      const segPrevOrder = new SegmentPrevOrdersDto();
      segPrevOrder.id = 0;
      segPrevOrder.prevOrderID = item.campaignId;
      segPrevOrder.segmentId = this.segmentId;
      segPrevOrder.cPrevSegmentID = "";
      segPrevOrder.cPrevSegmentNumber = "";
      segPrevOrder.cIncludeExclude = "Exclude";
      segPrevOrder.cCompanyFieldName = "Individual";
      segPrevOrder.action = ActionType.Add;
      createEditDto.listOfSegmentOrders.push(segPrevOrder);
    });

    if (createEditDto.listOfSegmentOrders.length) {
      this.isLoading = true;
      this._segmentPrevOrdersesServiceProxy.savePreviousOrders(createEditDto)
        .pipe(finalize(() => this.isLoading = false))
        .subscribe(() => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.close(true);
        });
    }
  }

  selectAllCampaign(isSelectAll: boolean) {
    if (isSelectAll) {
      this.selectedCampaigns = [...this.fcCampaigns.filter(item => item.status == this.statusType.CampaignCompleted)];
    }
    else {
      this.selectedCampaigns = [];
    }
  }

  selectSingleCampaign(isChecked: boolean, fcData: GetCampaignsListForView) {
    if (isChecked) {
      this.selectedCampaigns.push(this.fcCampaigns.find(item => item.campaignId == fcData.campaignId));
    } else {
      this.selectedCampaigns = this.selectedCampaigns.filter(item => item.campaignId != fcData.campaignId);
    }
  }

  isChecked(fcData: GetCampaignsListForView) {
    return this.selectedCampaigns.some(item => item.campaignId == fcData.campaignId);
  }

  isShown(action: CampaignAction, status: CampaignStatus): boolean {
    return this._campaignUiHelperService.shouldActionBeEnabled(action, status);
  }

  executeCampaign(campaign: GetCampaignsListForView): void {
    const campaignRow: GetCampaignsListForView = this.fcCampaigns.find(row => row.campaignId == campaign.campaignId);
    if (campaignRow) {
      campaignRow["isExecuting"] = true;
    }

    if (campaign.status === 10 || campaign.status === 50 || campaign.status === 40) {
      const input = CampaignActionInputDto.fromJS({
        campaignId: campaign.campaignId,
        databaseId: campaign.databaseID,
        campaignStatus: campaign.status,
        buildId: campaign.buildID,
        currentBuild: campaign.build,
        isExecute: true
      });
      this._campaignServiceProxy.executeCampaign(input).subscribe(result => {
        if (result.success)
          this.notifyAndReload(campaign.campaignId, result.message);
        else
          this.message.confirm('', this.l(result.message), isConfirmed => {
            if (isConfirmed)
              this._campaignServiceProxy
                .campaignActions(input)
                .subscribe(result => {
                  if (result.success)
                    this.notifyAndReload(campaign.campaignId, result.message);
                });
          });
      });
    }
    else
      this.onCampaignAction(campaign);
  }

  notifyAndReload = (id: number, message: string) => {
    this.notify.info(this.l(message));
    this.refreshCampaignStatus(id);
  };

  refreshCampaignStatus(id: number): void {
    const campaignRow: GetCampaignsListForView = this.fcCampaigns.find(row => row.campaignId == id);
    if (campaignRow) {
      campaignRow['isExecuting'] = true;
      this._campaignServiceProxy.getCampaignStatus(id)
        .subscribe(result => {
          campaignRow.status = result.value;
          campaignRow.statusDescription = result.label;
          campaignRow["isExecuting"] = false;
        });
    }
  }

  onCampaignAction(campaign: GetCampaignsListForView) {
    const input = CampaignActionInputDto.fromJS({
      campaignId: campaign.campaignId,
      databaseId: campaign.databaseID,
      campaignStatus: campaign.status,
      buildId: campaign.buildID,
      currentBuild: campaign.build
    });
    this._campaignServiceProxy
      .campaignActions(input)
      .subscribe(result => {
        if (result.success)
          this.notifyAndReload(campaign.campaignId, result.message);
      });
  }
}
