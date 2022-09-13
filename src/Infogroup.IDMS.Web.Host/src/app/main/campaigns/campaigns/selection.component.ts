import {
    Component,
    Injector,
    Output,
    EventEmitter,
    Input,
    AfterViewInit,
    OnInit,
    ViewChild
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
declare var $: any;
import { SelectItem } from "primeng/api";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { finalize } from "rxjs/operators";
import {
    CampaignsServiceProxy,
    SegmentsServiceProxy,
    GetQueryBuilderDetails,
    SegmentSelectionsServiceProxy,
    CreateOrEditSegmentDto,
    ExportLayoutsServiceProxy,
    SegmentSelectionSaveDto,
    SegmentSelectionDto,
    CampaignActionInputDto
} from "@shared/service-proxies/service-proxies";
import { CreateOrEditSegmentModalComponent } from "../segments/create-or-edit-segment-modal.component";
import { CampaignUiHelperService } from "../shared/campaign-ui-helper.service";
import { CampaignAction } from "../shared/campaign-action.enum";
import { CampaignStatus } from "../shared/campaign-status.enum";
import { SourcesModalComponent } from "./sources-modal.component";
import { SaveSegment, CopySegment, CreateOrEditResult, EditSegmentCompleteResult } from "../shared/campaign-models";
import { CreateOrEditCampaignModalComponent } from "./create-or-edit-campaign-modal.component";
import { PreviousOrdersModalComponent } from "./previousOrders-modal.component";
import { AppConsts } from "@shared/AppConsts";
import { SelectionAction } from "../shared/selection-action.enum";
import { SegmentDataPreviewComponent } from "../segments/segments-datapreview.compnent";
import { ModalDefaults, ModalSize } from "@shared/costants/modal-contants";
import { CopySegmentComponent } from "../segments/copy-segments.component";
import { SavedSelectionComponent } from "./saved-selection/saved-selection.component";
import { MultiFieldSelectionModalComponent } from "./multiField-selection-modal.component";
import { GeoSearchModalComponent } from "./geo-search/geo-search-modal.component";
import { BulkSegmentModalComponent } from "./bulk-segment-upload-modal.component";
import { FileDownloadService } from "@shared/utils/file-download.service";
import { GlobalChangesModalComponent } from "./global-changes/global-changes-modal.component";
import { AddToFavoritesComponent } from "./add-to-favorites/add-to-favorites-modal.component";
import { SubsetsModalComponent } from "./subsets/subsets-modal.component";
import { ImportSegmentModalComponent } from "../segments/import-segment/import-segment.component";
import { OccupationSelectionModalComponent } from "./occupation-selection/occupation-selection-modal.component";
import { GeoMappingComponent } from "../../shared/geo-mapping/geo-mapping.component";
import { CitySelectionModalComponent } from '@app/main/shared/city-selection-modal/city-selection-modal.component';
import { SICSelectionModalComponent } from '@app/main/shared/sic-selection-modal/sic-selection-modal.component';

enum SelectionType {
    DropDown = "select",
    Checkbox = "checkbox",
    RadioButton = "radio"
}

@Component({
    selector: "selection",
    styleUrls: ["selection.component.css"],
    templateUrl: "./selection.component.html"
})
export class SelectionComponent extends AppComponentBase
    implements OnInit, AfterViewInit {
    @ViewChild('adminEmailDiv', { static: false }) myErrorText: any;
    active = false;
    saving = false;
    validating: boolean = false;
    blockedPanel: boolean = false;
    @Input() segmentId: number;
    @Input() campaignId: number;
    @Input() buildId: number;
    @Input() mailerId: number;
    @Input() build: number;
    @Input() databaseId: number;
    @Input() divisionId: number;
    @Input() splitType: number;
    segments: SelectItem[] = [];
    selectedSegment: number;
    isDataPresent: boolean = false;
    queryString: string;
    @Input() quickCount: string;
    @Output() reloadCampaign = new EventEmitter<any>();
    @Output() favouritesChanged = new EventEmitter();
    @Output() cancelSelection = new EventEmitter<any>();
    @Output() excuteCampaign = new EventEmitter();
    @Output() editExportLayout = new EventEmitter<any>();
    @Output() editCampaignCompleted = new EventEmitter<any>();
    @Output() onEditSegmentComplete = new EventEmitter<EditSegmentCompleteResult>();



    saveOnExecute: boolean = false;
    previousSegmentId = 0;
    isLayout: boolean = false;
    layoutDescription: string = "";
    queryBuilder: string;
    currentStatusDescription: string = "";
    currentStatus: number = 0;
    builder: string;
    result: string;
    reset: string;
    savedRule: any;
    saveDisabled: boolean = true;
    canAdd: boolean;
    canImportSegments: boolean;
    canEdit: boolean;
    canCopy: boolean;
    canDelete: boolean;
    canExecute: boolean;
    showSICSearch: boolean;
    showOccupationSearch: boolean;
    showCitySearch: boolean;
    showGeoRadiusSearch: boolean;
    canEditCampaign: boolean;
    canEditSources: boolean;
    showSubsets: boolean;
    actionsDisabled: boolean = true;
    IsShowCountButton: boolean;
    isQuickCountButtonVisible: boolean;
    showAdvancedFilters: boolean = false;
    layoutName: any;
    IsFileType: boolean = false;
    fileWithOR: number = 0;
    deletedSelections: any;
    isRuleUpdatedOrCreated: boolean = false;
    canDataPreview: boolean;
    showBulkSegmentUpload: boolean;
    dto: SegmentSelectionSaveDto = new SegmentSelectionSaveDto();
    isTextBoxHidden: boolean = true;
    adminEmail: string = "";
    tootlTip: string = "";
    buildLolId: number;
    segmentSources : string [] = [];
    segmentSubSelectsSources : string [] = [];


    @Input() IsShowQuickCountButton: boolean;
    action = SelectionAction;

    generateValueDescription = array =>
        array.reduce((obj, item) => {
            obj[item.cValue + "|||" + item.cDescription] =
                item.cValue + " : " + item.cDescription;
            return obj;
        }, {});

    constructor(
        injector: Injector,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
        private _campaignServiceProxy: CampaignsServiceProxy,
        private _segmentServiceProxy: SegmentsServiceProxy,
        private _campaignUiHelperService: CampaignUiHelperService,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private modalService: NgbModal
    ) {
        super(injector);
    }
    ngOnInit() {
        this.queryBuilder = "queryBuilder-" + this.campaignId;
        this.builder = "builder-" + this.campaignId;
        this.result = "result-" + this.campaignId;
        this.reset = "reset-" + this.campaignId;
        // assign campaign level permissions
        this.showSubsets = this.permission.isGranted("Pages.SubSelects");
        this.canAdd = this.permission.isGranted("Pages.Segments.Create");
        this.canEditCampaign = this.permission.isGranted("Pages.Campaigns.Edit");
        this.canEditSources = this.permission.isGranted("Pages.SegmentLists");
        this.canImportSegments = this.permission.isGranted("Pages.Segments.ImportSegment");
        this.loadSegmentsWithSourcesAndSubsets();
    }

    loadSegmentsWithSourcesAndSubsets(): void {
        this._campaignServiceProxy.getSegmentsWithSourcesAndSubSelect(this.campaignId)
            .subscribe(r => {
                this.segmentSubSelectsSources = r["SUBSELECT"];
                this.segmentSources = r["SOURCE"];
            });
    }
    ngAfterViewInit(): void {
        $("#" + this.queryBuilder).hide();
        if (this.campaignId != 0) {
            if (this.segmentId != 0) this.show(this.campaignId, this.segmentId);
            else this.show(this.campaignId);
        }
        // else this.show();
    }
    BindBuilder() {
        this.quickCount = "";
        this.previousSegmentId = this.selectedSegment;
        if ($("#" + this.queryBuilder).length) {
            $("#" + this.result)
                .addClass("hide")
                .find("pre")
                .empty();
            $("#" + this.queryBuilder).hide();
            $(`#${this.builder}`).queryBuilder("destroy");
        }
        this.refreshActions();
        this._segmentSelectionProxy
            .getSelectionFieldsNew(
                this.selectedSegment,
                "1",
                this.databaseId,
                this.buildId,
                this.mailerId
            )
            .subscribe((data: GetQueryBuilderDetails) => {
                this.buildLolId = data.buildLolId
                this.isDataPresent = data ? true : false;
                let builderData = JSON.parse(data.filterDetails);
                var unmappedfields = JSON.parse(data.unMappedFilters);
                this.QueryBuilerSQL(builderData, unmappedfields);
                this.queryString = data.filterQuery;
                $("#" + this.queryBuilder).show();
                if (this.queryString)
                    $("#" + this.builder).queryBuilder(
                        "setRulesFromSQL",
                        this.queryString,
                        this.campaignId,
                        AppConsts.remoteServiceBaseUrl
                    );
            });
    }
    show(campaignId: number, segmentId = 0): void {
        if (campaignId) {
            this._segmentServiceProxy
                .getAllSegmentForDropdown(campaignId, this.databaseId)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    this.segments = result.segmentDropDown;
                    this.currentStatus = result.currentStatus;
                    this.currentStatusDescription = this._campaignUiHelperService.getStatusDescription(
                        result.currentStatus
                    );
                    this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(
                        CampaignAction.SaveSelection,
                        result.currentStatus
                    );
                    this.canExecute =
                        result.currentStatus == CampaignStatus.CampaignCreated || result.currentStatus == CampaignStatus.CampaignFailed || result.currentStatus == CampaignStatus.CampaignCompleted;
                    this.selectedSegment =
                        segmentId == 0 ? this.segments[0].value : segmentId;
                    this.active = true;
                    this.IsShowQuickCountButton =
                        (result.currentStatus == CampaignStatus.CampaignCreated || result.currentStatus == CampaignStatus.CampaignFailed) &&
                        result.isQuickCountButtonVisible;
                    this.isQuickCountButtonVisible = result.isQuickCountButtonVisible;
                    this.showSICSearch = result.isSICScreenConfigured;
                    this.showOccupationSearch = result.isOccupationScreenConfigured;
                    this.showGeoRadiusSearch = result.isGeoSearchConfigured;
                    this.showCitySearch = result.isCountyCityScreenConfigured;
                    this.BindBuilder();
                });
        } else {
            this.segments = [{ label: "Select", value: 0 }];
            this.selectedSegment = this.segments[0].value;
            this.active = true;
        }
    }
    close(): void {
        this.active = false;
        if ($("#" + this.queryBuilder).is(":visible")) {
            $("#" + this.builder).queryBuilder("reset");
            $("#" + this.result)
                .addClass("hide")
                .find("pre")
                .empty();
            $("#" + this.queryBuilder).hide();
        }
        this.cancelSelection.emit();
    }

    refreshActions(): void {
        let nonCampaignLevel = this.selectedSegment != this.segments[0].value;

        this.canDataPreview =
            this.currentStatus != CampaignStatus.CampaignCreated &&
            this.currentStatus != CampaignStatus.CampaignSubmitted &&
            this.currentStatus != CampaignStatus.CampaignRunning &&
            this.currentStatus != CampaignStatus.CampaignFailed &&
            nonCampaignLevel &&
            this.permission.isGranted("Pages.Segments.DataPreview");
        this.canEdit =
            this.permission.isGranted("Pages.Segments.Edit") && nonCampaignLevel;
        this.canCopy =
            this.permission.isGranted("Pages.Segments.Copy") && nonCampaignLevel;
        this.canDelete =
            this.permission.isGranted("Pages.Segments.Delete") && nonCampaignLevel;
        this.actionsDisabled =
            this.saveDisabled ||
            !(
                this.canAdd ||
                this.canEdit ||
                this.canCopy ||
                this.canDelete ||
                this.canEditCampaign ||
                this.canEditSources || this.showSubsets
            );
        this.showAdvancedFilters =
            !this.saveDisabled &&
            (this.showSICSearch || this.showOccupationSearch ||
                this.showCitySearch ||
                this.permission.isGranted("Pages.SavedSelections"));

        this.showBulkSegmentUpload =
            (this.currentStatus == CampaignStatus.CampaignCreated ||
                this.currentStatus == CampaignStatus.CampaignCompleted) &&
            this.permission.isGranted("Pages.Segments.BulkSegment");
    }

    copySegment(): void {
        this._segmentServiceProxy
            .getSegmentsBasedOnIds(this.selectedSegment)
            .subscribe(result => {
                const modalRef: NgbModalRef = this.modalService.open(
                    CopySegmentComponent,
                    { backdrop: ModalDefaults.Backdrop }
                );
                modalRef.componentInstance.campaignId = this.campaignId;
                modalRef.componentInstance.segmentId = this.selectedSegment;
                modalRef.componentInstance.segment = result;
                modalRef.result.then((resultModal: CopySegment) => {
                    if (resultModal.isCopy) {
                        this.notify.success(this.l("SegmentSuccessfullyCopied"));
                        this.unlockCampaign();
                        this.show(this.campaignId, resultModal.segmentId);
                    }
                });
            });
    }
    openCreateOrEditSegment(isEdit?: boolean): void {
        const modalRef: NgbModalRef = this.modalService.open(
            CreateOrEditSegmentModalComponent,
            { backdrop: ModalDefaults.Backdrop, size: ModalDefaults.Size }
        );
        let segment: CreateOrEditSegmentDto = CreateOrEditSegmentDto.fromJS({
            id: this.selectedSegment
        });
        modalRef.componentInstance.OrderId = this.campaignId;

        modalRef.componentInstance.databaseId = this.databaseId;
        modalRef.componentInstance.buildId = this.buildId;
        modalRef.componentInstance.mailerId = this.mailerId;

        modalRef.componentInstance.splitType = this.splitType;
        if (isEdit) modalRef.componentInstance.segmentId = segment.id;
        modalRef.result.then((result: CreateOrEditResult) => {
            if (result.isSave) {
                this.onEditSegmentComplete.emit({ segmentId: result.id, campaignId: this.campaignId, newStatus: result.newStatus })
                this.show(this.campaignId, result.id);
            }
        });
    }

    openImportSegment(): void {
        const modalRef: NgbModalRef = this.modalService.open(ImportSegmentModalComponent,
            { backdrop: ModalDefaults.Backdrop }
        );
        modalRef.componentInstance.campaignId = this.campaignId;
        modalRef.result.then((result: SaveSegment) => {
            if (result.isSave) {
                this.notify.info(result.segmentId + this.l("SegmentImportedSuccessfully"));
                this.unlockCampaign();
                this.show(this.campaignId, this.selectedSegment);
            }
        });
    }

    deleteSegment(): void {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                this._segmentServiceProxy.delete(this.selectedSegment).subscribe(() => {
                    this.notify.success(this.l("SuccessfullyDeleted"));
                    this.unlockCampaign();
                    this.show(this.campaignId);
                });
            }
        });
    }

    editCampaign(): void {
        let modalRef = this.modalService.open(CreateOrEditCampaignModalComponent, {
            size: ModalDefaults.Size,
            backdrop: ModalDefaults.Backdrop,
            windowClass: ModalDefaults.WindowClass
        });
        modalRef.componentInstance.campaignId = this.campaignId;
        modalRef.componentInstance.databaseId = this.databaseId;
        modalRef.componentInstance.divisionId = this.divisionId;
        modalRef.result.then(result => {
            if (result.isFavouriteChanged) {
                this.favouritesChanged.emit();
            }
            if (result.isSave) {

                if (result.campaignStatus === CampaignStatus.CampaignCreated) {
                    this.unlockCampaign();

                } else {
                    this.currentStatusDescription = this._campaignUiHelperService.getStatusDescription(
                        result.campaignStatus
                    );
                }
                this.isLayout = result.isExportLayoutSelected;
                this.editCampaignCompleted.emit({ id: result.campaignId, description: result.description });
            }
        });
    }
    openSources(): void {
        let segmentID: number = this.selectedSegment;
        if (segmentID != 0) {
            const modalRef: NgbModalRef = this.modalService.open(
                SourcesModalComponent,
                { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop }
            );
            modalRef.componentInstance.segmentID = segmentID;

            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.BindBuilder();
                    this.loadSegmentsWithSourcesAndSubsets();
                }
            });
        }
    }

    openSubsets(): void {
        let segmentId: number = this.selectedSegment;
        if (segmentId != 0) {
            const modalRef: NgbModalRef = this.modalService.open(
                SubsetsModalComponent,
                { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop }
            );
            modalRef.componentInstance.segmentId = segmentId;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.componentInstance.buildId = this.buildId;
            modalRef.componentInstance.mailerId = this.mailerId;
            const description = this.segments.find(segment => segment.value === segmentId).label;
            modalRef.componentInstance.segmentNo = description.split(':')[0].trim();
            modalRef.componentInstance.campaignLevel = segmentId === this.segments[0].value;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.BindBuilder();
                }
                this.loadSegmentsWithSourcesAndSubsets();
            });
        }
    }

    openSICSearch(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                SICSelectionModalComponent,
                {
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: "sicModalClass"
                }
            );
            modalRef.componentInstance.segmentId = this.selectedSegment;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.buildId = this.buildId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.refreshActions();
                    this.BindBuilder();
                }
            });
        }
    }

    openCitySearch(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                CitySelectionModalComponent,
                {
                    size: ModalDefaults.Size,
                    windowClass: ModalDefaults.WindowClass,
                    backdrop: ModalDefaults.Backdrop
                }
            );
            modalRef.componentInstance.segmentId = this.selectedSegment;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.buildId = this.buildId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.BindBuilder();
                }
            });
        }
    }

    openGeoMapping(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                GeoMappingComponent,
                {
                    size: ModalSize.EXTRA_LARGE,
                    windowClass: ModalDefaults.WindowClass,
                    backdrop: ModalDefaults.Backdrop,
                    centered: true,
                }
            );
            modalRef.componentInstance.segmentId = this.selectedSegment;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.buildId = this.buildId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.result.then((params) => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.BindBuilder();
                }
            });
        }
    }

    openOccupationalSearch(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                OccupationSelectionModalComponent,
                {
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: "sicModalClass"
                }
            );
            modalRef.componentInstance.segmentId = this.selectedSegment;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.buildId = this.buildId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.refreshActions();
                    this.BindBuilder();
                }
            });
        }
    }

    openSavedSelection(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                SavedSelectionComponent,
                {
                    size: "lg",
                    windowClass: "savedSelectionModalClass",
                    backdrop: "static"
                }
            );
            modalRef.componentInstance.segmentId = this.selectedSegment;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.BindBuilder();
                }
            });
        }
    }

    openMultiFieldSearch(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                MultiFieldSelectionModalComponent,
                {
                    backdrop: "static",
                    windowClass: "multiFieldModalClass"
                }
            );
            modalRef.componentInstance.segmentId = this.selectedSegment;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.componentInstance.buildId = this.buildId;
            modalRef.componentInstance.mailerId = this.mailerId;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.BindBuilder();
                }
            });
        }
    }

    openGeoRadiusSearch(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                GeoSearchModalComponent,
                {
                    backdrop: ModalDefaults.Backdrop,
                    size: ModalDefaults.Size
                }
            );
            modalRef.componentInstance.segmentId = this.selectedSegment;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.buildId = this.buildId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();
                    this.BindBuilder();
                }
            });
        }
    }

    openGlobalChanges(action: any): void {
        let windowClass = action == SelectionAction.EditSegments ? 'bulk-edit-segment__modal' : 'global-changes__modal'
        const modalRef: NgbModalRef = this.modalService.open(
            GlobalChangesModalComponent,
            {
                backdrop: ModalDefaults.Backdrop,
                windowClass
            }
        );
        const currentSegment = this.selectedSegment;
        modalRef.componentInstance.sourceSegmentId = currentSegment;
        modalRef.componentInstance.campaignId = this.campaignId;
        modalRef.componentInstance.currentStatus = this.currentStatus;
        modalRef.componentInstance.splitType = this.splitType;
        modalRef.componentInstance.buildId = this.buildId;
        modalRef.componentInstance.mailerId = this.mailerId;
        modalRef.componentInstance.databaseId = this.databaseId;
        modalRef.componentInstance.divisionId = this.divisionId;
        modalRef.componentInstance.selectedAction = action;
        modalRef.result.then(params => {
            if (params.isSave) {
                if (action == SelectionAction.DeleteSegments) {
                    this.reloadCampaign.emit();
                    this.show(this.campaignId);
                }
                else if (action == SelectionAction.EditSegments) {
                    this.reloadCampaign.emit();
                    this.show(this.campaignId, currentSegment);
                }
                else {
                    if (this.currentStatus != CampaignStatus.CampaignCreated)
                        this.unlockCampaign();
                    this.BindBuilder();
                }
            }
        });
    }

    openAddToFavorites(): void {
        const modalRef: NgbModalRef = this.modalService.open(
            AddToFavoritesComponent,
            {
                backdrop: ModalDefaults.Backdrop,
                windowClass: ModalDefaults.Size
            }
        );
        const currentSegment = this.selectedSegment;
        modalRef.componentInstance.sourceSegmentId = currentSegment;
        modalRef.componentInstance.campaignId = this.campaignId;
        modalRef.componentInstance.databaseId = this.databaseId;
    }

    unlockCampaign(): void {
        if (this.currentStatus != 10) {
            this.reloadCampaign.emit();
        }
        this.currentStatusDescription = this._campaignUiHelperService.getStatusDescription(
            CampaignStatus.CampaignCreated
        );
        this.currentStatus = 10;
        this.saveDisabled = false;
        this.canExecute = true;
        this.IsShowQuickCountButton = this.isQuickCountButtonVisible;
    }

    QueryBuilerSQL(inputQuery, unMappedFilters) {
        $("#" + this.builder).queryBuilder({
            plugins: [
                //'bt-tooltip-errors': { delay: 100 },
                // 'bt-selectpicker'
                "chosen-selectpicker",
                //'bt-checkbox',
                "sortable"
                //'filter-description',
                //'unique-filter'
                //'not-group': null
            ],
            filters: inputQuery,
            unMappedFilters: unMappedFilters,
            icons: {
                add_group: "fa fa-plus-square",
                add_rule: "fa fa-plus-circle",
                remove_group: "fa fa-minus-square",
                remove_rule: "fa fa-minus-circle",
                error: "fa fa-exclamation-triangle"
            }
        });

        // reset builder
        $("#" + this.reset).on("click", () => {
            if (this.queryString !== '{"condition":"AND","rules":[]}') {
                this.message.confirm(this.l(""), isConfirmed => {
                    if (isConfirmed) {
                        this._segmentSelectionProxy
                            .deleteAll(this.selectedSegment, this.campaignId)
                            .subscribe(() => {
                                this.notify.info(this.l("FilterDeleted"));
                                this.queryString = '{"condition":"AND","rules":[]}';
                                $("#" + this.builder).queryBuilder("reset");
                                this.unlockCampaign();
                                this.quickCount = "";
                            });
                    }
                });
            } else {
                this.message.confirm(this.l(""), isConfirmed => {
                    if (isConfirmed) {
                        this.notify.info(this.l("FilterDeleted"));
                        $("#" + this.builder).queryBuilder("reset");
                    }
                });
            }
        });

        $("#" + this.builder).on("triggerFileUploadBlock.queryBuilder", (rule?, isBlocked?) => {
            if (isBlocked)
                this.blockedPanel = true;
            else
                this.blockedPanel = false;
        });

        $("#" + this.builder).on(
            "afterUpdateRuleFilter.queryBuilder",
            (rule, previousFilter) => {
                let inputType: SelectionType;
                let $ruleValueContainer;
                $("#" + previousFilter.id)
                    .children(".rule-header")
                    .children(".btn-group.pull-right.rule-actions.btnSelect")
                    .css("display", "none");
                $("#" + previousFilter.id)
                    .children(".rule-header")
                    .children(".btn-group.pull-right.rule-actions.btnTextbox")
                    .css("display", "none");
                $("#" + previousFilter.id)
                    .children(".rule-header")
                    .children(".btn-group.pull-right.rule-actions.btnFile")
                    .css("display", "none");
                // set the operator width
                $("#" + previousFilter.id)
                    .children(".rule-operator-container")
                    .children(".chosen-container.chosen-container-single")
                    .css("width", "180px");
                $("#" + previousFilter.id)
                    .children(".rule-filter-container")
                    .children(".chosen-container.chosen-container-single")
                    .css("width", "300px");
                let data = previousFilter.filter.data;
                let cModePrevious = previousFilter.cModeType;
                var $input;
                // get the element where select box is to be added
                $ruleValueContainer = $("#" + previousFilter.id).children(
                    ".rule-value-container"
                );
                $input = $("#" + previousFilter.id)
                    .children(".rule-value-container")
                    .children(".form-control");
                if (data.iShowListBox == true) {
                    this._segmentSelectionProxy
                        .getFieldValues(
                            previousFilter.filter.id,
                            this.buildLolId
                        )
                        .subscribe((ddValues: any) => {
                            // Convert Object array to key:value array
                            previousFilter.filter.values = this.generateValueDescription(
                                ddValues
                            );
                            // Checking Input Type
                            if (ddValues.length > 7) inputType = SelectionType.DropDown;
                            else if (ddValues.length <= 7)
                                inputType = SelectionType.Checkbox;
                            switch (inputType) {
                                case SelectionType.Checkbox:
                                    if (cModePrevious != "F" && cModePrevious != "T") {
                                        previousFilter.filter.input = inputType;

                                        var val = [];
                                        var prevVal = null;
                                        if (
                                            previousFilter.value !== null &&
                                            previousFilter.value !== undefined
                                        ) {
                                            if (previousFilter.value === "") {
                                                prevVal = [""];
                                            } else {
                                                prevVal =
                                                    previousFilter.value.length > 0
                                                        ? previousFilter.value.split(",")
                                                        : null;
                                            }
                                        }
                                        $.each(ddValues, function () {
                                            if (prevVal != null) {
                                                prevVal.forEach(item => {
                                                    if (item === this.cValue) {
                                                        val.push(`${item + "|||" + this.cDescription}`);
                                                    }
                                                });
                                            }
                                        });
                                        previousFilter.value = val;

                                        $("#" + this.builder).queryBuilder(
                                            "createRuleInput",
                                            previousFilter
                                        );

                                        // for ishowdefault
                                        if (
                                            previousFilter.cModeType === 1 ||
                                            previousFilter.cModeType === null ||
                                            previousFilter.cModeType === undefined
                                        ) {
                                            if (
                                                previousFilter.filter.data.iShowDefault === 1 &&
                                                previousFilter.filter.data.iShowTextBox === true
                                            ) {
                                                $("#" + this.builder).queryBuilder(
                                                    "toRuleTextbox",
                                                    previousFilter
                                                );
                                            }
                                            if (
                                                previousFilter.filter.data.iShowDefault === 3 &&
                                                previousFilter.filter.data.iFileOperations === true
                                            ) {
                                                $("#" + this.builder).queryBuilder(
                                                    "forFileUpload",
                                                    previousFilter
                                                );
                                            }
                                        }
                                    }
                                    break;
                                case SelectionType.DropDown:
                                    if (cModePrevious != "F" && cModePrevious != "T") {
                                        var options = [];

                                        var val = [];
                                        var prevVal = null;
                                        if (
                                            previousFilter.value !== null &&
                                            previousFilter.value !== undefined
                                        ) {
                                            if (previousFilter.value === "") {
                                                prevVal = [""];
                                            } else {
                                                prevVal =
                                                    previousFilter.value.length > 0
                                                        ? previousFilter.value.split(",")
                                                        : null;
                                            }
                                        }

                                        $.each(ddValues, function () {
                                            options.push(
                                                `<option value="${this.cValue +
                                                "|||" +
                                                this.cDescription}"> ${this.cValue} : ${
                                                    this.cDescription
                                                }</option>`
                                            );

                                            if (prevVal != null) {
                                                prevVal.forEach(item => {
                                                    if (item === this.cValue) {
                                                        val.push(`${item + "|||" + this.cDescription}`);
                                                    }
                                                });
                                            }
                                        });
                                        // clear the previous values are append new values
                                        $input
                                            .find("option")
                                            .remove()
                                            .end()
                                            .append(options);
                                        previousFilter.value = val;

                                        // Set the values
                                        $input.val(previousFilter.value);

                                        // Replace the listbox control with Chosen drop down
                                        $("#" + previousFilter.id)
                                            .children(".rule-value-container")
                                            .children(".form-control")
                                            .removeClass("form-control")
                                            .addClass("chosen");

                                        $(".chosen").chosen();

                                        // for ishowdefault
                                        if (
                                            previousFilter.cModeType === 1 ||
                                            previousFilter.cModeType === null ||
                                            previousFilter.cModeType === undefined
                                        ) {
                                            if (
                                                previousFilter.filter.data.iShowDefault === 1 &&
                                                previousFilter.filter.data.iShowTextBox === true
                                            ) {
                                                $("#" + this.builder).queryBuilder(
                                                    "toRuleTextbox",
                                                    previousFilter
                                                );
                                            }
                                            if (
                                                previousFilter.filter.data.iShowDefault === 3 &&
                                                previousFilter.filter.data.iFileOperations === true
                                            ) {
                                                $("#" + this.builder).queryBuilder(
                                                    "forFileUpload",
                                                    previousFilter
                                                );
                                            }
                                        }
                                    }

                                    break;
                            }
                        });
                } else {
                    previousFilter.value = "";
                }
                //$ruleValueContainer.css('width', '100%');
                $input.css("width", "100%");
                if (data.iShowTextBox) {
                    // Show the textbox button
                    $("#" + previousFilter.id)
                        .children(".rule-header")
                        .children(".btn-group.pull-right.rule-actions.btnTextbox")
                        .css("display", "block");
                }

                if (data.iShowListBox) {
                    // show the select button
                    $("#" + previousFilter.id)
                        .children(".rule-header")
                        .children(".btn-group.pull-right.rule-actions.btnSelect")
                        .css("display", "block");
                }

                if (data.iFileOperations) {
                    // show the upload button
                    $("#" + previousFilter.id)
                        .children(".rule-header")
                        .children(".btn-group.pull-right.rule-actions.btnFile")
                        .css("display", "block");
                }
            }
        );
    }
    onSegmentChange(action: any) {
        if (this.previousSegmentId != this.selectedSegment) {
            this.saveWhereClause(action);
        }
    }
    saveWhereClause(action: any) {
        if (document.querySelectorAll("[id*='_group']").length < 1) {
            this.selectAction(action);
            if (SelectionAction.SegmentChange)
                this.selectedSegment = this.previousSegmentId;
            return;
        }
        var segmentSelectionlist = $("#" + this.builder).queryBuilder(
            "getSQLtblsegmentSelection",
            action != SelectionAction.SaveOnly
        );
        var isError = segmentSelectionlist != null ? segmentSelectionlist.pop() : 0;

        if (
            segmentSelectionlist === null ||
            segmentSelectionlist.length === 0 ||
            isError > 0
        ) {
            if (isError === -1) {
                this.selectAction(action);
            }
            if (SelectionAction.SegmentChange)
                this.selectedSegment = this.previousSegmentId;
            return;
        }
        let countActions =
            action == SelectionAction.Execute || action == SelectionAction.QuickCount;
        this.validating = countActions;
        $("#" + this.result).addClass("hide");
        var segmentID = SelectionAction.SegmentChange == action ? this.previousSegmentId : this.selectedSegment;
        var countFalse = 0;
        //Get SegmentSelectionList

        this.saveDisabled =
            action != null &&
            !this._campaignUiHelperService.shouldActionBeEnabled(
                CampaignAction.SaveSelection,
                this.currentStatus
            );
        var error =
            segmentSelectionlist == null ||
            segmentSelectionlist == undefined ||
            isError > 0;
        this.validating = this.validating && !error;
        this.saveDisabled = this.saveDisabled && !error;

        //Get deleted rules
        var deletedSelections =
            segmentSelectionlist != null || segmentSelectionlist != undefined
                ? segmentSelectionlist.pop()
                : null;

        //Pop Header Condition
        var headerCondition =
            segmentSelectionlist != null ? segmentSelectionlist.pop() : null;

        //Pop Group Occurence counts from SegmentSelectionList
        var groupCount =
            segmentSelectionlist != null ? segmentSelectionlist.pop() : null;

        let campaignLevel = SelectionAction.SegmentChange ? this.previousSegmentId === this.segments[0].value : this.selectedSegment === this.segments[0].value ;
        if (campaignLevel) {
            if (Object.keys(groupCount).length > 1 && headerCondition === "OR") {
                this.message.error(this.l("OrNotAllowedAtCampaignLevel"));
                this.validating = false;
                if (SelectionAction.SegmentChange)
                    this.selectedSegment = this.previousSegmentId;
                return;
            }
            else if (Object.keys(groupCount).length === 1 && headerCondition === "OR") {
                headerCondition = "AND";
            }
        }

        //Adding additional required fields in SegmentSelectionList
        if (segmentSelectionlist != null) {
            var prevGroupNo = 0;
            var curGroupNo = 0;
            var flag = 0,
                i = 0;
            var groupNo = 1;
            var loopTimes = 0;
            var ruleType = 0;

            segmentSelectionlist.forEach(value => {
                if (value.isRuleUpdated === 0 || value.isRuleUpdated === 1) {
                    this.isRuleUpdatedOrCreated = true;

                    if (
                        (value.cValueOperator.toUpperCase() === "LIKE" ||
                            value.cValueOperator.toUpperCase() === "NOT LIKE") &&
                        value.cValues.search("%") === -1
                    )
                        countFalse++;
                }

                if (
                    action == SelectionAction.QuickCount &&
                    (value.cValueMode === "F" ||
                        value.cQuestionDescription === "Zip Radius")
                ) {
                    this.IsFileType = true;
                }

                var cGrouping = value.cGrouping;

                (

                    value.segmentId = segmentID
                ),
                    (value.cGrouping =
                        groupCount[value.iGroupNumber.toString()] > 1 ? "Y" : "N");

                if (value.isRuleUpdated > 1)
                    value.isRuleUpdated =
                        value.cGrouping !== cGrouping ? 1 : value.isRuleUpdated;

                var tempGroupNum = value.iGroupNumber;
                if (Object.keys(groupCount)[i] == tempGroupNum) flag = 1;
                else {
                    flag = 0;
                    i++;
                }
                if (prevGroupNo == 0) {
                    value.iGroupNumber = curGroupNo + 1;
                    curGroupNo++;
                    value.cJoinOperator = headerCondition;
                } else if (prevGroupNo != 0 && value.cGrouping == "Y") {
                    if (flag == 1) {
                        value.iGroupNumber = curGroupNo;
                        curGroupNo = value.iGroupNumber;
                    } else {
                        value.iGroupNumber = ++curGroupNo;
                        curGroupNo = value.iGroupNumber;
                        value.cJoinOperator = headerCondition;
                    }
                } else if (prevGroupNo != 0 && value.cGrouping == "N") {
                    value.iGroupNumber = ++curGroupNo;
                    value.cJoinOperator = headerCondition;
                }
                prevGroupNo = value.iGroupNumber;

                if (groupNo !== value.iGroupNumber) {
                    groupNo = value.iGroupNumber;
                    loopTimes = 0;
                    ruleType = 0;
                }

                if (value.cGrouping == "Y") {
                    if ((value.cValueMode === "F" || value.cQuestionDescription === "Zip Radius") && value.cJoinOperator === "OR" && (value.iGroupNumber === 2 || loopTimes > 0)) {
                        this.fileWithOR = 2;
                    }
                    else if ((value.cValueMode === "F" || value.cQuestionDescription === "Zip Radius") && value.cJoinOperator === "OR" && loopTimes === 0) {
                        value.cJoinOperator = "AND";
                    }

                    if (loopTimes === 0) {
                        if (value.cValueMode === "F" || value.cQuestionDescription === "Zip Radius")
                            ruleType = 1;
                        else
                            ruleType = 2;
                    }
                    else {
                        if ((value.cValueMode === "F" || value.cQuestionDescription === "Zip Radius") && ruleType === 2)
                            this.fileWithOR = 1;
                        else if (value.cValueMode !== "F" && value.cQuestionDescription !== "Zip Radius" && ruleType === 1)
                            this.fileWithOR = 1;
                    }
                    loopTimes++;
                }
                else {
                    if ((value.cValueMode === "F" || value.cQuestionDescription === "Zip Radius") && value.cJoinOperator === "OR" && value.iGroupNumber === 2) {
                        this.fileWithOR = 2;
                    }
                    else if ((value.cValueMode === "F" || value.cQuestionDescription === "Zip Radius") && value.cJoinOperator === "OR") {
                        value.cJoinOperator = "AND";
                    }
                }
            });
        }

        if (this.fileWithOR > 0) {
            if (this.fileWithOR === 1) {
                this.message.error(this.l("SingleRuleAllowed"));
            }
            else {
                this.message.error(this.l("ConditionRestriction"));
            }
            this.fileWithOR = 0;
            this.IsFileType = false;
            this.validating = false;
            if (SelectionAction.SegmentChange)
                this.selectedSegment = this.previousSegmentId;
            this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(
                CampaignAction.SaveSelection,
                this.currentStatus);
            return;
        }

        if (this.IsFileType) {
            this.message.error(this.l("QuickCountFileValidation"));
            this.IsFileType = false;
            this.validating = false;
            this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(
                CampaignAction.SaveSelection,
                this.currentStatus
            );

            return;

        }

        this.saveOnExecute = this.isRuleUpdatedOrCreated || deletedSelections.length > 0;
        if ((this.isRuleUpdatedOrCreated || deletedSelections.length > 0) && !this.saveDisabled) {
            // if (countFalse > 0) {
            //     this.message.confirm(
            //         this.l("ValidateLikeOperatorValue"),
            //         this.l("AreYouSure"),
            //         isConfirmed => {
            //             console.log("isConfirmed",isConfirmed);
            //             if (isConfirmed) {
            //                 this.saveFilters(segmentSelectionlist, action, deletedSelections);
            //             } else {
            //                 this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(
            //                     CampaignAction.SaveSelection,
            //                     this.currentStatus
            //                 );
            //                 if (countActions) {
            //                     this.validating = false;
            //                     this.unlockCampaign();
            //                 }
            //                 if (SelectionAction.SegmentChange)
            //                     this.selectedSegment = this.previousSegmentId;
            //             }
            //         }
            //     );
            // } else {
            this.saveFilters(segmentSelectionlist, action, deletedSelections);
            //    }
        } else {
            this.executeAction(action, false);
        }
        this.isRuleUpdatedOrCreated = false;
    }

    openPreviousOrders(campaignid): void {
        let segmentID: number = this.selectedSegment;
        if (segmentID) {
            const modalRef = this.modalService.open(PreviousOrdersModalComponent, {
                size: ModalSize.EXTRA_LARGE,
                backdrop: ModalDefaults.Backdrop
            });
            modalRef.componentInstance.segmentId = segmentID;
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.buildId = this.buildId;
            const description = this.segments.find(segment => segment.value === segmentID).label;
            modalRef.componentInstance.segmentNo = description.split(':')[0].trim();
            modalRef.componentInstance.campaignLevel = segmentID === this.segments[0].value;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.unlockCampaign();

                }
            });
        }
    }
    openSegmentDataPreview(): void {
        let segmentID: number = this.selectedSegment;
        if (segmentID) {
            const modalRef = this.modalService.open(SegmentDataPreviewComponent, {
                size: ModalDefaults.Size,
                backdrop: ModalDefaults.Backdrop,
                windowClass: ModalDefaults.WindowClass
            });
            modalRef.componentInstance.segmentId = segmentID;
        }
    }
    openCreateOrEditExportLayout(): void {
        this._exportLayoutServiceProxy
            .getCampaignRecordForCampaignId(this.campaignId, this.databaseId)
            .subscribe(result => {
                this.layoutName = result.cExportLayout;
                this.editExportLayout.emit({
                    campaignId: this.campaignId,
                    layoutName: this.layoutName,
                    isCampaign: true,
                    currentStatus: this.currentStatus
                });
            });
    }

    saveFilters(segmentSelectionlist, action: any, deletedSelections): void {
        this.dto.selections = segmentSelectionlist.map(selection =>
            SegmentSelectionDto.fromJS(selection)
        );
        this.dto.campaignId = this.campaignId;
        this.dto.databaseId = this.databaseId;
        this.dto.deletedSelections =
            deletedSelections !== undefined ? deletedSelections : null;
        this.saving = true;
        this._segmentSelectionProxy
            .createSegmentSelectionDetails(this.dto, true)
            .pipe(
                finalize(() => {
                    this.saving = false;
                    this.saveOnExecute = false;
                })
            )
            .subscribe(() => {
                    if (action !== SelectionAction.SegmentChange) {
                        this.unlockCampaign();
                        this.BindBuilder();
                    }
                    this.executeAction(action, true);
                },
                () => {
                    this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(
                        CampaignAction.SaveSelection,
                        this.currentStatus
                    );
                }
            );
    }

    executeAction(action, isSelectionChanged: boolean = false): void {
        this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(
            CampaignAction.SaveSelection,
            this.currentStatus
        );
        this.selectAction(action, true, isSelectionChanged);
    }

    selectAction(action, isexecuteAction: boolean = false, isSelectionChanged: boolean = false): void {
        switch (action) {
            case SelectionAction.Execute:
                this.execute();
                break;
            case SelectionAction.QuickCount:
                if (this.isRuleUpdatedOrCreated && isexecuteAction)
                    this.reloadCampaign.emit();
                this.GetQuickCount(isSelectionChanged);
                break;
            case SelectionAction.SelectSIC:
                this.openSICSearch();
                break;
            case SelectionAction.SelectOccupation:
                this.openOccupationalSearch();
                break;
            case SelectionAction.SelectCountyCity:
                this.openCitySearch();
                break;
            case SelectionAction.SavedSelection:
                this.openSavedSelection();
                break;
            case SelectionAction.SelectMultiField:
                this.openMultiFieldSearch();
                break;
            case SelectionAction.SearchGeoRadius:
                this.openGeoRadiusSearch();
                break;
            case SelectionAction.BulkSegmentUpload:
                this.openBulkSegment();
                break;
            case SelectionAction.AppendRules:
            case SelectionAction.BulkCampaignHistory:
            case SelectionAction.DeleteRules:
            case SelectionAction.FindReplace:
            case SelectionAction.EditSegments:
            case SelectionAction.DeleteSegments:
                this.openGlobalChanges(action);
                break;
            case SelectionAction.AddToFavorites:
                this.openAddToFavorites();
                break;
            case SelectionAction.CopySegment:
                this.copySegment();
                break;
            case SelectionAction.NewSegment:
                this.openCreateOrEditSegment();
                break;
            case SelectionAction.EditSegment:
                this.openCreateOrEditSegment(true);
                break;
            case SelectionAction.DeleteSegment:
                this.deleteSegment();
                break;
            case SelectionAction.EditCampaign:
                this.editCampaign();
                break;
            case SelectionAction.ImportSegments:
                this.openImportSegment();
                break;
            case SelectionAction.Sources:
                this.openSources();
                break;
            case SelectionAction.Subsets:
                this.openSubsets();
                break;
            case SelectionAction.CampaignHistory:
                this.openPreviousOrders(this.campaignId);
                break;
            case SelectionAction.EditOutputLayout:
                this.openCreateOrEditExportLayout();
                break;
            case SelectionAction.DataPreview:
                this.openSegmentDataPreview();
                break;
            case SelectionAction.Print:
                this.printCampaign(this.campaignId,this.databaseId);
                break;
            case SelectionAction.SegmentChange:
                if (this.saveOnExecute && !this.saveDisabled)
                    this.unlockCampaign();
                this.BindBuilder();
                break;
            case SelectionAction.SelectGeoMapping:
                this.openGeoMapping();
                break;
            default:
                if (isexecuteAction) {
                    this.notify.info(this.l("FilterSaved"));
                    if (this.isRuleUpdatedOrCreated) {
                        this.reloadCampaign.emit();
                    }
                    break;
                }
        }
    }

    execute(): void {
        const input = CampaignActionInputDto.fromJS({
            campaignId: this.campaignId,
            databaseId: this.databaseId,
            campaignStatus: this.currentStatus,
            buildId: this.buildId,
            currentBuild!: this.build,
            isExecute: true
    });
        this._campaignServiceProxy.executeCampaign(input).subscribe(result => {
                if (result.success)
                    this.onExecuteCompleted(result.message);
                else {
                    this.validating = false;
                    this.message.confirm('', this.l(result.message), isConfirmed => {
                        if (isConfirmed) {
                            this.validating = true;
                            this._campaignServiceProxy
                                .campaignActions(input)
                                .subscribe(result => {
                                        if (result.success)
                                            this.onExecuteCompleted(result.message);
                                    },
                                    () => this.onExecuteError()
                                );
                        }
                        else this.cancelSelection.emit();
                    });
                }
            },
            () => this.onExecuteError()
        );
    }
    onExecuteCompleted = (message: string) => {
        this.validating = false;
        this.notify.info(this.l(message));
        this.excuteCampaign.emit();
    }

    onExecuteError = () => {
        this.validating = false;
        this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(
            CampaignAction.SaveSelection,
            this.currentStatus
        );
        if (this.saveOnExecute) {
            this.BindBuilder();
            this.saveOnExecute = false;
        }
    }
    openBulkSegment(): void {
        if (this.selectedSegment) {
            const modalRef: NgbModalRef = this.modalService.open(
                BulkSegmentModalComponent,
                {
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass
                }
            );
            modalRef.componentInstance.campaignId = this.campaignId;
            modalRef.componentInstance.databaseId = this.databaseId;
            modalRef.result.then(params => {
                if (params.isSave) {
                    this.notify.success(this.l("SegmentSuccessfullyUploaded"));
                    this.unlockCampaign();
                    this.show(this.campaignId, this.selectedSegment);
                }
            });
        }
    }

    GetQuickCount(isSelectionChanged: boolean): void {
        this._segmentServiceProxy
            .getQuickCount(this.selectedSegment)
            .subscribe(result => {
                    this.validating = false;
                    this.quickCount = "Available Quantity" + " : " + result;
                    if (isSelectionChanged) this.unlockCampaign();
                },
                () => { this.validating = false; });
    }

    onSelectionActionButtonClick() {
        this._segmentSelectionProxy
            .getLayoutNameFromCampaignId(this.campaignId)
            .subscribe(result => {
                this.layoutDescription = result;

                if (this.layoutDescription != "") this.isLayout = true;
                else this.isLayout = false;
            });
    }

    printCampaign(campaignId: any, databaseID: number) {
        this._campaignServiceProxy
            .printDetailsReport(campaignId, databaseID)
            .subscribe(result => {
                this._fileDownloadService.downloadDocumentAttachment(result);
            });
    }
    onDivFocus() {
        this.isTextBoxHidden = true;
    }
    onIconClick() {
        this.isTextBoxHidden = false;
        this._segmentSelectionProxy.getAdminEmailAddress(this.databaseId).subscribe(result => {
            setTimeout(() => {
                    this.adminEmail = result;
                    const selBox = document.createElement('textarea');
                    selBox.style.position = 'fixed';
                    selBox.style.left = '0';
                    selBox.style.top = '0';
                    selBox.style.opacity = '0';
                    selBox.value = this.adminEmail;
                    document.body.appendChild(selBox);
                    selBox.focus();
                    selBox.select();
                    document.execCommand('copy');
                    document.body.removeChild(selBox);
                    document.getElementById('adminEmailDiv').focus();
                    this.notify.info(this.l('CopyAdminEmailAddress'));
                },
                500);
        });

    }
}
