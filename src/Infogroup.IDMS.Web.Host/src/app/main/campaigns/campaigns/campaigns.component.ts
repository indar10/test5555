import {
    Component,
    Input,
    Injector,
    ViewChild,
    ElementRef,
    EventEmitter,
    Output,
    ViewChildren,
    QueryList
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    IdmsUserLoginInfoDto,
    CampaignsServiceProxy,
    GetCampaignsListForView,
    CampaignFavouritesServiceProxy,
    SegmentSelectionsServiceProxy,
    CampaignFavouriteDtoForView,
    PageID,
    ShortSearchServiceProxy,
    CampaignActionInputDto,

    ExportLayoutsServiceProxy
} from "@shared/service-proxies/service-proxies";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { LazyLoadEvent } from "primeng/components/common/lazyloadevent";
import { finalize } from "rxjs/operators";
import { Paginator } from "primeng/components/paginator/paginator";
import { Table } from "primeng/components/table/table";
import * as moment from "moment";
import {
    NgbModal,
    NgbModalOptions,
    NgbModalRef
} from "@ng-bootstrap/ng-bootstrap";
import { SelectItem } from "primeng/api";
import { IDMSDateTimeService } from "@app/shared/common/timing/idms-date-time.service";
import { CreateOrEditCampaignModalComponent } from "../campaigns/create-or-edit-campaign-modal.component";
import { FileDownloadService } from "@shared/utils/file-download.service";
import { CreateOrEditSegmentModalComponent } from "../segments/create-or-edit-segment-modal.component";
import { CampaignUiHelperService } from "../shared/campaign-ui-helper.service";
import { CampaignAction } from "../shared/campaign-action.enum";
import { CampaignStatus } from "../shared/campaign-status.enum";
import {
    NewStatusInfo,
    SaveSegment,
    SelectionDetails,
    EditSegmentCompleteResult,
    CreateOrEditResult,
} from "../shared/campaign-models";
import { ModalDefaults } from "@shared/costants/modal-contants";
import { CopyCampaignComponent } from "./copy-campaign.component";
import { ReshipCampaignModalComponent } from "./reship-campaign/reship-campaign-modal.component";
import { ScheduleCampaignComponent } from "./schedule-campaign/schedule-campaign-modal.component";
import { SegmentsComponent } from "../segments/segments.component";
import { Router } from "@angular/router";

declare var $: any;

@Component({
    selector: "campaigns",
    templateUrl: "./campaigns.component.html",
    styleUrls: ["./campaigns.component.css"],
    animations: [appModuleAnimation()]
})
export class CampaignsComponent extends AppComponentBase {
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @ViewChild("CampaignDateRangePicker", { static: true })
    dateRangePickerElement: ElementRef;
    @ViewChild("createOrEditCampaignModal", { static: true })
    createOrEditCampaignModal: CreateOrEditCampaignModalComponent;

    @Output() openSelection = new EventEmitter<any>();
    @Output() editCampaignFromSegment = new EventEmitter<any>();
    @Output() editCampaignCompleted = new EventEmitter<any>();
    @ViewChildren(SegmentsComponent) segmentGrids: QueryList<SegmentsComponent>;
    @Input() outsideClick: false;
    pageId: PageID = PageID.Campaigns;
    helpData: any = {
        header: "Search Options:",
        examples: []
    };
    isHelpDisabled: boolean = true;
    advancedFiltersAreShown = false;
    campaignQueueGridShown = false;
    favouritesDisabled: boolean = true;
    filterText = "";
    statusTemp: any[];
    statusdrop: SelectItem[] = [];
    idFilter: "";
    statusFilterValue: string;
    descriptionFilterText: "";
    databaseNameFilter: string;
    initialStartDate: moment.Moment = moment()
        .add(-6, "days")
        .startOf("day");
    initialEndDate: moment.Moment = moment().endOf("day");
    selectedDateRange: Date[] = [
        this.initialStartDate.toDate(),
        this.initialEndDate.toDate()
    ];
    idmsUserData: IdmsUserLoginInfoDto;
    dateRanges = [
        "Today",
        "Yesterday",
        "Last7Days",
        "Last30Days",
        "ThisMonth",
        "LastMonth"
    ];
    customDateHeader = this.l("Last7Days");
    actionType = CampaignAction;
    statusType = CampaignStatus;
    databaseNameFilterText: string;
    customerNameFilterText: string;
    buildNameFilterText: string;
    favouriteCampaigns: CampaignFavouriteDtoForView[] = [];
    customerVisible = true;
    databaseVisible = true;
    advancedSerachVisible = true;
    rowCount: number = 0;
    notifyAndReload = (id: number, message: string) => {
        this.notify.info(this.l(message));
        this.refreshCampaignStatus(id);
    };
    @Output() editExportLayout = new EventEmitter<any>();
    layoutName: any;

    constructor(
        injector: Injector,
        private _campaignServiceProxy: CampaignsServiceProxy,
        private _campaignUiHelperService: CampaignUiHelperService,
        private _idmsDateTimeService: IDMSDateTimeService,
        private _fileDownloadService: FileDownloadService,
        private _campaignFavoritesServiceProxy: CampaignFavouritesServiceProxy,
        private _segmentSelectionServiceProxy: SegmentSelectionsServiceProxy,
        private modalService: NgbModal,
        private _shortSearchServiceProxy: ShortSearchServiceProxy,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,       
        private _router:Router
    ) {
        super(injector);
    }
    ngOnInit() {
        this.loadFavouriteCampaigns();
        this.statusTemp = this._campaignUiHelperService.getStatusValue();
        this.statusdrop = this.statusTemp.map(status => {
            return { label: status.statusDescription, value: status.id };
        });
        this.statusdrop.splice(0, 0, { label: "Select", value: "" });
        this.statusFilterValue = this.statusdrop[0]["value"];
        this._shortSearchServiceProxy
            .getSearchHelpText(this.pageId)
            .subscribe(result => {
                this.helpData = result;
                this.isHelpDisabled = false;
            });
        this.fetchUserDatabaseMailerRecord(this.appSession.idmsUser.idmsUserID);
    }

    getCampaigns(event?: LazyLoadEvent) {
        this.fetchUserDatabaseMailerRecord(this.appSession.idmsUser.idmsUserID);
        if (this.campaignQueueGridShown) {
            return;
        }
        let myCampaignChecked = $("#togBtn:checkbox:checked").length > 0;
        this.idmsUserData = this.appSession.idmsUser;

        let callAppService = !myCampaignChecked || this.idmsUserData !== undefined;
        if (callAppService) {
            this.createFilters(myCampaignChecked, event);
        } else {
            this.primengTableHelper.totalRecordsCount = 0;
            this.primengTableHelper.records = undefined;
        }
    }

    clearFilters(event?: LazyLoadEvent) {
        this.filterText = "";
        this.idFilter = "";
        this.descriptionFilterText = "";
        this.statusFilterValue = this.statusdrop[0].value;
        this.databaseNameFilterText = "";
        this.buildNameFilterText = "";
        this.customerNameFilterText = "";
        let ranges = this._idmsDateTimeService.createDateRangePickerOptions();
        this.customDateHeader = this.l("Last7Days");
        this.selectedDateRange = ranges[this.l("Last7Days")];
        $("#togBtn").prop("checked", true);
        this.getCampaigns();
    }
    createFilters(myCampaignChecked: boolean, event?: LazyLoadEvent): void {
        let orderIdFilter = "";
        let descriptionFilter = "";
        let databaseNameFilter = "";
        let customerNameFilter = "";
        let buildNameFilter = "";
        let statusFilter = "";
        let userNameFilter = this.idmsUserData.idmsUserName;
        let userIDFilter = this.idmsUserData.idmsUserID;
        let isApplySelectedDateRangeFilter = true;
        let selectedDateRangeFilter: moment.Moment[] = [
            this.initialStartDate,
            this.initialEndDate
        ];

        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        orderIdFilter = this.idFilter;
        descriptionFilter = this.descriptionFilterText;
        statusFilter = this.statusFilterValue;

        if (!myCampaignChecked) {
            userNameFilter = "";
        }
        if (
            this.filterText != "" ||
            (orderIdFilter != "" && orderIdFilter != undefined) ||
            (descriptionFilter != "" && descriptionFilter != undefined) ||
            statusFilter != ""
        ) {
            isApplySelectedDateRangeFilter = false;
        }

        databaseNameFilter = this.databaseNameFilterText;
        customerNameFilter = this.customerNameFilterText;
        buildNameFilter = this.buildNameFilterText;
        if (
            (databaseNameFilter != "" && databaseNameFilter != undefined) ||
            (customerNameFilter != "" && customerNameFilter != undefined) ||
            (buildNameFilter != "" && buildNameFilter != undefined)
        ) {
            isApplySelectedDateRangeFilter = false;
        }


        selectedDateRangeFilter[0] = moment(this.selectedDateRange[0]);
        selectedDateRangeFilter[1] = moment(this.selectedDateRange[1]).endOf("day");
        if (isApplySelectedDateRangeFilter == false) {
            selectedDateRangeFilter = null;
        }
        // Call the service method
        this.primengTableHelper.showLoadingIndicator();

        this._campaignServiceProxy
            .getAllCampaignsList(
                this.filterText,
                orderIdFilter,
                descriptionFilter,
                statusFilter,
                selectedDateRangeFilter,
                userIDFilter,
                userNameFilter,
                databaseNameFilter,
                customerNameFilter,
                buildNameFilter,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe(result => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
            });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    openCreateOrEditCampaign(campaignId?: number, databaseId?: number, divisionId?: number): void {
        let modalRef: NgbModalRef = null;
        if (campaignId == 0 || campaignId == undefined || campaignId == null) {
            modalRef = this.modalService.open(
                CreateOrEditCampaignModalComponent,
                {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass
                });

        } else {
            modalRef = this.modalService.open(CreateOrEditCampaignModalComponent, {
                size: ModalDefaults.Size,
                backdrop: ModalDefaults.Backdrop,
                windowClass: ModalDefaults.WindowClass
            });
        }
        modalRef.componentInstance.campaignId = campaignId;
        modalRef.componentInstance.databaseId = databaseId;
        modalRef.componentInstance.divisionId = divisionId;

        modalRef.result.then(params => {
            if (params.isFavouriteChanged) {
                this.loadFavouriteCampaigns();
            }
            if (params.isSave) {
                if (params.isAdd) {
                    this.getCampaigns();
                    this.openSelection.emit({ ...params.selectionDetails });
                }
                else
                    this.editCampaignCompleted.emit({ id: params.campaignId, description: params.description });
            }
        });
    }

    onOrderDateChange(dates: any): void {
        if (this.campaignQueueGridShown) {
            return;
        }
        if (!dates || !dates[0] || !dates[1]) {
            return;
        }
        this.selectedDateRange[0] = dates[0];
        this.selectedDateRange[1] = dates[1];
        if (!this.advancedFiltersAreShown) this.getCampaigns();
    }
    onCustomDateRangeSelect(dateRange) {
        if (this.campaignQueueGridShown) {
            return;
        }
        let ranges = this._idmsDateTimeService.createDateRangePickerOptions();
        this.customDateHeader = dateRange;
        this.selectedDateRange = ranges[dateRange];
    }

    printCampaign(campaignId: any, databaseID: any) {
        this._campaignServiceProxy
            .printDetailsReport(campaignId,databaseID)
            .subscribe(result => {
                this._fileDownloadService.downloadDocumentAttachment(result);
            });
    }
    copyCampaign(campaignId: any) {
        const modalRef: NgbModalRef = this.modalService.open(
            CopyCampaignComponent,
            { backdrop: ModalDefaults.Backdrop }
        );
        modalRef.componentInstance.campaignId = campaignId;
        modalRef.result.then(params => {
            if (params.isSave) {
                this.reloadPage();
            }
        });
    }

    openSelectionScreen(record: GetCampaignsListForView, segmentId: number) {
        let selectionDetails: SelectionDetails = {
            campaignId: record.campaignId,
            databaseId: record.databaseID,
            buildId: record.buildID,
            databaseName: record.databaseName,
            divisionId: record.divisionId,
            campaignDescription: record.campaignDescription,
            build: record.build,
            mailerId: record.mailerId,
            segmentId: segmentId,
            splitType: record.splitType
        };
        if (!segmentId) {
            this._segmentSelectionServiceProxy
                .getSegmentIdForOrderLevel(record.campaignId)
                .subscribe(result => {
                    selectionDetails.segmentId = result;
                    this.openSelection.emit({ ...selectionDetails });
                });
        } else {
            this.openSelection.emit({ ...selectionDetails });
        }
    }

    createSegment(campaign: GetCampaignsListForView): void {
        const modalRef: NgbModalRef = this.modalService.open(
            CreateOrEditSegmentModalComponent,
            { backdrop: "static", size: ModalDefaults.Size }
        );
        modalRef.componentInstance.OrderId = campaign.campaignId;
        modalRef.componentInstance.databaseId = campaign.databaseID;
        modalRef.componentInstance.buildId = campaign.buildID;
        modalRef.componentInstance.mailerId = campaign.mailerId;

        modalRef.result.then((result: CreateOrEditResult) => {
            if (result.isSave) {
                let targetGrid = this.segmentGrids.find(grid => grid.orderId == campaign.campaignId);
                if (targetGrid) {
                    targetGrid.paginator.first = 0;
                    targetGrid.getSegment();
                }
                else {
                    this.refreshCampaignStatus(campaign.campaignId);
                }
            }
        });
    }

    isShown(action: CampaignAction, status: CampaignStatus): boolean {
        return this._campaignUiHelperService.shouldActionBeEnabled(action, status);
    }

    onScheduleCampaignAction(campaign: GetCampaignsListForView) {
        const input = CampaignActionInputDto.fromJS({
            campaignId: campaign.campaignId,
            databaseId: campaign.databaseID,
            campaignStatus: campaign.status,
            buildId: campaign.buildID,
            currentBuild: campaign.build
        });
        this._campaignServiceProxy.campaignScheduleActionsValidations(input).
            subscribe(result => {
                const modalRef: NgbModalRef = this.modalService.open(
                    ScheduleCampaignComponent,
                    {
                        backdrop: ModalDefaults.Backdrop,
                        windowClass: ModalDefaults.Size
                    }
                );
                modalRef.componentInstance.campaign = campaign;
                modalRef.result.then(params => {
                    if (params.saving) {
                        this.refreshCampaignStatus(campaign.campaignId);
                    }
                });
            });
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

    onStatusUpdate(updatedStatus: NewStatusInfo) {
        let campaign: GetCampaignsListForView = this.primengTableHelper.records.find(
            cmp => cmp.campaignId == updatedStatus.campaignId
        );
        if (campaign && campaign.status != updatedStatus.status) {
            campaign.status = updatedStatus.status;
            campaign.statusDescription = this._campaignUiHelperService.getStatusDescription(
                updatedStatus.status
            );
        }
    }

    onCampaignInvoked(value: boolean) {
        this.campaignQueueGridShown = value;
        this.getCampaigns();
    }

    loadFavouriteCampaigns(): void {
        this.favouritesDisabled = true;
        this._campaignFavoritesServiceProxy
            .getAllFavouriteCampaigns()
            .subscribe(result => {
                this.favouriteCampaigns = result;
                this.favouritesDisabled = result.length === 0;
            });
    }

    fetchUserDatabaseMailerRecord(userid: number) {
        this._campaignServiceProxy.fetchUserDatabaseMailerBasedOnUser(userid).subscribe(result => {
            this.rowCount = result.count;

            if (this.rowCount <= 0) {
                this.databaseVisible = true;
                this.customerVisible = true;
                this.advancedSerachVisible = true;
            }
            else if (this.rowCount == 1) {
                this.databaseVisible = false;
                this.customerVisible = false;
                this.advancedSerachVisible = false;

            }
            else {
                this.databaseVisible = true;
                this.customerVisible = false;
                this.advancedSerachVisible = false;
            }

        });
    }
    openReship(campaign: GetCampaignsListForView): void {
        this.message.confirm(this.l("ReshipConfirmationText"), this.l("ReshipConfirmationTitle", campaign.campaignId), isConfirmed => {
            if (isConfirmed) {
                const modalRef: NgbModalRef = this.modalService.open(
                    ReshipCampaignModalComponent,
                    {
                        backdrop: ModalDefaults.Backdrop,
                        windowClass: ModalDefaults.Size
                    }
                );
                modalRef.componentInstance.campaignDescription = campaign.campaignDescription;
                modalRef.componentInstance.campaignId = campaign.campaignId;
                modalRef.componentInstance.databaseId = campaign.databaseID;
                modalRef.result.then(params => {
                    if (params.isSave) {
                        this.refreshCampaignStatus(campaign.campaignId);
                    }
                });
            }
        });
    }
    isDeleteVisible(campaign: GetCampaignsListForView): boolean {
        return this.permission.isGranted("Pages.Campaigns.Delete")
            && (campaign.createdBy.trim().toLowerCase() == this.appSession.user.userName.toLowerCase())
            && this.isShown(CampaignAction.Delete, campaign.status)
    }

    deleteCampaign(id: number): void {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                this._campaignServiceProxy.deleteCampaign(id).subscribe(() => {
                    this.notify.success(this.l("SuccessfullyDeleted"));
                    if (this.favouriteCampaigns.some(campaign => campaign.campaignId === id))
                        this.loadFavouriteCampaigns();
                    this.getCampaigns();
                });
            }
        });
    }

    isCancelVisible(campaign: GetCampaignsListForView): boolean {
        return this.permission.isGranted("Pages.Campaigns.Cancel")
            && (campaign.createdBy.trim().toLowerCase() == this.appSession.user.userName.toLowerCase())
            && this.isShown(CampaignAction.CancelCampaign, campaign.status)
    }
    cancelCampaign(id: number): void {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                this._campaignServiceProxy.cancelCampaign(id).subscribe(() => {
                    this.notify.success(this.l("CampaignSuccessfullyCancelled"));
                    this.refreshCampaignStatus(id);
                });
            }
        });
    }

    resetCampaign(id: number): void {        
        this._campaignServiceProxy.resetCampaign(id).subscribe(() => {
            this.notify.success(this.l("CampaignSuccessfullyReset"));
            this.refreshCampaignStatus(id);
        });                  
    }

    executeCampaign(campaign: GetCampaignsListForView): void {
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

    onScheduleCampaignActionClick(campaign: GetCampaignsListForView): void {
        if (campaign.status === 10) {
            const input = CampaignActionInputDto.fromJS({
                campaignId: campaign.campaignId,
                databaseId: campaign.databaseID,
                campaignStatus: campaign.status,
                buildId: campaign.buildID,
                currentBuild: campaign.build
            });
            this._campaignServiceProxy.checkLatestBuildForScheduleCampaign(input).subscribe(result => {
                if (result.success)
                    this.onScheduleCampaignAction(campaign);
                else
                    this.message.confirm('', this.l(result.message), isConfirmed => {
                        if (isConfirmed)
                            this.onScheduleCampaignAction(campaign);
                    });
            });
        }
        else
            this.onScheduleCampaignAction(campaign);
    }

    openCreateOrEditExportLayout(campaignId: number, currentStatus: any, databaseID: number): void {
        this._exportLayoutServiceProxy
            .getCampaignRecordForCampaignId(campaignId, databaseID)
            .subscribe(result => {
                this.layoutName = result.cExportLayout;
                this.editExportLayout.emit({
                    campaignId: campaignId,
                    layoutName: this.layoutName,
                    isCampaign: true,
                    currentStatus: currentStatus
                });
            });
    }


    refreshCampaignById(id: number): void {
        let campaignRow: GetCampaignsListForView = this.primengTableHelper.records.find(row => row.campaignId == id);
        if (campaignRow) {
            this.primengTableHelper.showLoadingIndicator();
            this._campaignServiceProxy.getCampaignForView(id)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    if (result.campaignId)
                        campaignRow.init(result);
                });
        }
    }

    refreshCampaignStatus(id: number): void {
        let campaignRow: GetCampaignsListForView = this.primengTableHelper.records.find(row => row.campaignId == id);
        if (campaignRow) {
            this.primengTableHelper.showLoadingIndicator();
            this._campaignServiceProxy.getCampaignStatus(id)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    campaignRow.status = result.value;
                    campaignRow.statusDescription = result.label;
                });
        }
    }

    refreshSegment(segmentData: EditSegmentCompleteResult): void {
        if (segmentData.newStatus > 0)
            this.onStatusUpdate({ campaignId: segmentData.campaignId, status: segmentData.newStatus });
        if (this.segmentGrids) {
            let targetCampaign = this.segmentGrids.find(grid => grid.orderId == segmentData.campaignId);
            if (targetCampaign) {
                targetCampaign.refreshSegmentById(segmentData.segmentId);
            }
        }
    }

}
