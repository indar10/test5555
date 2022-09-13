import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { finalize } from 'rxjs/operators';
import { CreateOrEditCampaignDto, CampaignBillingDto, CampaignsServiceProxy, ActionType, EditCampaignsOutputDto, CampaignAttachmentDto, CampaignGeneralDto, GetCampaignMaxPerForViewDto, CampaignLevelMaxPerDto, SegmentSelectionsServiceProxy, EditCampaignExportPartDto, DecoyDto, GetDecoyForViewDto, CampaignFavouritesServiceProxy, CampaignOESSDto, GetCampaignDropdownsDto, DropdownOutputDto } from '@shared/service-proxies/service-proxies';
import { DatePipe } from '@angular/common';
import { SelectItem } from 'primeng/api';
import { CampaignUiHelperService } from '../shared/campaign-ui-helper.service';
import { CampaignAction } from '../shared/campaign-action.enum';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { CampaignStatus } from '../shared/campaign-status.enum';
import { NgForm } from '@angular/forms';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { FileUpload } from 'primeng/fileupload';
import { SelectionDetails } from '../shared/campaign-models';
import { ModalWindowName } from '@shared/costants/modal-contants';
import { Dropdown } from 'primeng/dropdown';

@Component({
    selector: 'createOrEditCampaignModal',
    styleUrls: ['create-or-edit-campaign-modal.component.css'],
    templateUrl: './create-or-edit-campaign-modal.component.html',
    providers: [DatePipe]
})
export class CreateOrEditCampaignModalComponent extends AppComponentBase implements OnInit {
    
    databases: any = [];
    selectedDatabase: number;
    IsMaxFieldIsEmpty: boolean = false;
    saving: boolean = false;
    documentFileSize: number;
    active: boolean = false;
    saveDisabled: boolean = true;
    outputTypes: SelectItem[] = [];
    outputLayouts: SelectItem[] = [];
    outputShipToValues: SelectItem[] = [];
    outputSortValues: SelectItem[] = [];
    outputPgpKeyValues: SelectItem[] = [];
    shipToCC: string = "";
    shipToSubject: string = "";
    shipToNotes: string = "";
    shippedDate: string = "";
    isShipped: boolean = false;
    isGridTextChanged: boolean = false;
    outputMediaList: SelectItem[] = [];
    index: number = null;
    lastIndex = -1;
    selectedMedia: any;
    fileLabel: any;
    fileNotes: any;
    cols: any[];
    attachementTypeCode: any[] = [];

    isDownloadXtabEnabled: boolean = false;
    isDownloadMulticolumnEnabled: boolean = false;
    isLayoutDownloadEnabled: boolean = false;

    selectedItem: any;

    toolTipLayoutDownloadLink: string = "Download Layout";
    ftpSite: string;
    userName: string;
    email: string = "";
    selectedCategories: string[] = ['dataFileOnly'];

    companyId: number;
    CampaignStatus: number;
    
    headerRowCheck: boolean = false;
    dataFileOnlyChecked: boolean = false;
    unzippedChecked: boolean = false;

    datafileOnlydisabled: boolean = false;
    headerRowdisabled: boolean = false;
    unzippeddisabled: boolean = false;

    dbOutputTypeArray: any;
    dbOutputSortArray: any;
    dboutputLayoutArray: any;
    dboutputShipToArray: any;
    dboutputPgpKeyArray: any;

    selectedOutputLayout: string;
    selectedOutputType: string;
    prevSelectedOutputType: string;
    selectedoutputLayoutLabel: string;
    selectedOutputShipTo: string;
    selectedoutputSort: string;
    selectedPgpKey: string;
    isDelete: boolean = false;

    multiColumnFields: SelectItem[] = [];
    isShowTable: boolean = false;


    XFields: SelectItem[] = [];
    YFields: SelectItem[] = [];
    Levels: SelectItem[] = [{ label: 'Campaign', value: false }, { label: 'Segment', value: true }];
    Types: SelectItem[] = [{ label: 'Net', value: 'N' }, { label: 'Gross', value: 'G' }];
    SeedKeyMethodList: SelectItem[] = [{ label: 'Auto', value: 1 }, { label: 'Manual Seed Key', value: 0 }];
    splitOptionList: SelectItem[] =
        [{ label: 'No Split', value: 1 },
        { label: 'Split By Key Code 1', value: 2 },
        { label: 'Split Into N Equal Parts', value: 3 },
        { label: 'Split Into N Parts', value: 4 }
        ];
    selectedSplitOption: any;
    isPartsVisible: boolean = true;
    isSplitDisabled: boolean = false;
    partsList: SelectItem[] = [];
    selectedPart: any;
    classType: any;

    isDecoyRecords: boolean = false;
    isAuto: boolean;
    decoyKeyText: string;
    decoyKey1Text: string;
    decoyGroupList: SelectItem[] = [];
    decoyAddEditText: string;
    decoyGroupText: string;
    decoyKeyMethod: number;
    totalOutputQuantity: number;
    isDecoyGroup: boolean;
    decoyGroupValue: string;
    isSeedRecordsUpdated = false;
    isReportsDataUpdated = false;
    isMultiReportsDataUpdated = false;
    isMaxPerUpdated = false;
    decoyGroupByForEdit: SelectItem[] = [];
    tempdecoyGroupByForEdit: SelectItem[] = [];
    seed: DecoyDto = new DecoyDto();
    seedEdit: DecoyDto = new DecoyDto();
    setAbsoluteIndex: number;

    billing: CampaignBillingDto = new CampaignBillingDto();
    oessData: CampaignOESSDto = new CampaignOESSDto();
    documentsData: CampaignAttachmentDto[] = [];
    @Input() campaignId: number;
    @Input() IsMaxFieldEmpty: boolean;
    MinimumMaxPerQuantity: any;
    MaximumMaxPerQuantity: any;
    CampaignLevelMaxPerField: string = "";
    camapaignSaved: boolean = false;
    MaxPerFieldsSegmentlevel: SelectItem[] = [];
    MaxPerFieldsCampaignlevel: SelectItem[] = [];
    uomData: SelectItem[] = [];
    salesRep: SelectItem[] = [];
    isLayoutFieldDisabled: boolean = false;

    uploadUrl: string = "";
    formData: FormData = new FormData();
    isFavourite: boolean = false;
    isFavouriteChanged: boolean = false;
    infoFetched: boolean = false;
    formattedAmount: any;
    databaseId: number;
    campaignDescription: string;
    launchOESSFlag: boolean = false;
    oessValidationOccured: boolean = false;
    editedDescription: string = '';

    @ViewChild('databaseForm', { static: false }) mainForm: NgForm;
    @ViewChild('PDFFileUpload', { static: true }) pdfFileUpload: FileUpload;   
    builds: any[] = [];
    offers: any[] = [];
    campaign!: CampaignGeneralDto | undefined;
    maxPer!: CampaignLevelMaxPerDto | undefined;
    mailers: any[] = [];
    brokers: any[] = [];
    maxPerFieldsCampaignlevel: any = [];
    brokerVisible = true;
    offerDDVisible = false;
    defaultOfferId: number = -1;    
    rowCount: number;
    customerVisible: boolean = true;
    brokerVisibleOnCount = true;
    offerVisibleOnCount = true;
    channelTypeEnable: boolean = true;
    @ViewChild('mailerInput', { static: false }) mailerInput: any;
    @ViewChild('offerDDInput', { static: false }) offerDDInput: any;
    @ViewChild('brokerInput', { static: false }) brokerInput: any;
    @ViewChild('buildInput', { static: false }) buildInput: any;
    @ViewChild('fieldInput', { static: false }) fieldInput: any;
    divisionId: number;

    constructor(

        injector: Injector,
        private _campaignUiHelperService: CampaignUiHelperService,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
        private _orderServiceProxy: CampaignsServiceProxy,
        private _campaignFavoritesServiceProxy: CampaignFavouritesServiceProxy,
        private _fileDownloadService: FileDownloadService,
        public activeModal: NgbActiveModal,
        private _httpClient: HttpClient,
        private modalService: NgbModal,
        private datePipe: DatePipe,

    ) {
        super(injector);
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/File/campaigns/UploadPDF';

    }
    ngOnInit() {
        this.campaign = new CampaignGeneralDto();
        this.maxPer = new CampaignLevelMaxPerDto();
        if (this.campaignId) 
            this.getDatabases(this.databaseId);
        else {
            this._orderServiceProxy.fetchUserDatabaseMailerBasedOnUser(this.appSession.idmsUser.idmsUserID).subscribe(
                result => {
                    this.getDatabases(result.databaseId);
                }
            );
        }
        this.onUnzippedCheckBoxChecked(null);
        this.show(this.campaignId);
    }

    getDatabases(currentDatabaseId: number) {
        this._orderServiceProxy.getDatabaseWithLatestBuild(currentDatabaseId).subscribe(
            (result1) => {
                this.databases = result1.databases.databases;                
                this.setControlOptions(result1);
                this.setUserDatabaseMailerRecord(result1.isShowCustomer);
                if (!this.campaignId) {
                    this.campaign.databaseID = result1.databases.defaultDatabase;

                }
                else
                    this.campaign.databaseID = this.databaseId;
                this.getMaxPerFields();
                this.campaign.divisionalDatabase = result1.divisionalDatabase;
            }
        );
    }
    show(campaignId: any): void {
        this.unzippedChecked = false;
        this.isLayoutDownloadEnabled = true;
        this.isDownloadXtabEnabled = true;
        this.isDownloadMulticolumnEnabled = true;
        this.active = true;
        this.infoFetched = false;


        if (!campaignId) {
            this.active = true;
            this.saveDisabled = false;
            this.campaign.cDescription = "New Campaign " + this.datePipe.transform(new Date(), "yyyy-MM-dd");
            this.campaign.cOfferName = '';
            this.campaign.cOrderType = 'N';
            this.campaign.cChannelType = 'P';
            this.campaign.mailerDescription = '';
            this.campaign.brokerDescription = '';
        }
        else {
            this._orderServiceProxy.getCampaignForEdit(campaignId, this.databaseId, this.divisionId).subscribe(result => {
                this.campaign = result.generalData;
                this.campaign.mailer = result.generalData.mailer;
                this.campaign.broker = result.generalData.broker;
                this.channelTypeEnable = result.isChannelTypeVisible;
                this.CampaignStatus = result.currentStatus;
                if (this.CampaignStatus == 130) {
                    this.isShipped = true;
                }
                this.isLayoutFieldDisabled = this.CampaignStatus >= 130 ? true : false;
                this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(CampaignAction.SaveSelection, result.currentStatus);
                this.databaseId = result.generalData.databaseID;
                this.campaignDescription = result.generalData.cDescription;
                this.billing.init(result.billingData);
                this.oessData.init(result.oessData);
                this.uomData = result.oessData.uom;
                this.salesRep = result.oessData.salesRep;
                if (campaignId) {
                    this.active = true;
                }
                this.selectedOutputType = result.getOutputData.type.trim();
                this.totalOutputQuantity = result.getOutputData.totalOutputQuantity;
                this.prevSelectedOutputType = result.getOutputData.type.trim();
                this.selectedOutputLayout = result.getOutputData.layout;

                if (result.getOutputData.layout != undefined && result.getOutputData.layout != "") {
                    this.isLayoutDownloadEnabled = false;
                }
                else {
                    this.isLayoutDownloadEnabled = true;
                }
                this.selectedoutputSort = result.getOutputData.sort;
                this.selectedPgpKey = result.getOutputData.pgpKey;
                this.selectedOutputShipTo = result.getOutputData.shipTo;
                this.shipToCC = result.getOutputData.shipCCEmail;
                this.shipToSubject = result.getOutputData.shipSubject;
                this.shipToNotes = result.getOutputData.shipNotes;
                this.shippedDate = result.getOutputData.shippedDate;
                this.email = result.getOutputData.shipTo;
                this.ftpSite = result.getOutputData.ftpSite;
                this.userName = result.getOutputData.userName;
                this.outputTypes = result.getOutputData.typeList;
                this.outputSortValues.push({ label: "Select Sort Field", value: "" });
                this.outputSortValues = this.outputSortValues.concat(result.getOutputData.sortList);
                this.outputLayouts = result.getOutputData.layoutlist;
                this.outputPgpKeyValues.push({ label: "Select PGP Key", value: "" });
                this.outputPgpKeyValues = this.outputPgpKeyValues.concat(result.getOutputData.pgpKeyList);
                this.outputShipToValues = result.getOutputData.shipToList;
                this.outputMediaList = result.getOutputData.mediaList;
                this.selectedMedia = result.getOutputData.media == "" ? "E" : result.getOutputData.media;
                this.fileLabel = result.getOutputData.fileLabel;
                this.fileNotes = result.getOutputData.fileNotes;
                this.onTypeDropdownChange(true,null);
                this.unzippedChecked = result.getOutputData.isUnzipped;
                this.onUnzippedCheckBoxChecked(null);
                this.headerRowCheck = result.getOutputData.isHeaderRow;
                this.dataFileOnlyChecked = result.getOutputData.isDataFileOnly;
                this.XFields = result.reportsData.xFieldDropdown;
                this.YFields = result.reportsData.yFieldDropdown;
                this.multiColumnFields = result.multiReportsData.fieldsDropdown;
                this.primengTableHelper.records = result.reportsData.xtabRecords;
                this.primengTableHelperMultiDimension.records = result.multiReportsData.multidimensionalRecords;
                this.disabledReportsDownloadLink();
                this.selectedPart = result.getOutputData.splitIntoNParts;
                this.selectedSplitOption = result.getOutputData.splitType;
                this.onSplitDropdownChange(null);
                this.onPartDropdownChange(null);
                this.documentsData = result.documentsData;
                this.documentFileSize = result.documentFileSize;
                this.primengTableHelperMaxPer.records = result.maxPerData.getSegmentLevelMaxPerData;
                this.MaxPerFieldsSegmentlevel = result.maxPerData.getMaxPerFieldDropdownData;
                this.MinimumMaxPerQuantity = result.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity;
                this.MaximumMaxPerQuantity = result.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity;
                this.CampaignLevelMaxPerField = result.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel.toUpperCase();
                if (this.CampaignLevelMaxPerField == "") {
                    this.IsMaxFieldEmpty = true;
                    this.MinimumMaxPerQuantity = "";
                    this.MaximumMaxPerQuantity = "";
                }
                this.primengTableHelperDecoy.records = result.decoyData.listOfDecoys;
                this.isAuto = result.decoyData.isDecoyKeyMethod == 1 ? true : false;
                this.decoyKeyText = result.decoyData.decoyKey;
                this.decoyKey1Text = result.decoyData.decoyKey1;
                this.isDecoyRecords = result.decoyData.listOfDecoys.length > 0 ? false : true;
                this.decoyGroupList = result.decoyData.listOfDecoyGroup;
                this.decoyKeyMethod = result.decoyData.isDecoyKeyMethod;
                this.decoyAddEditText = this.l('Add');
                this.isDecoyGroup = true;
                this.tempdecoyGroupByForEdit = result.decoyData.listOfGroupsForEdit;
                this.decoyGroupByForEdit = [];
                if (this.decoyGroupList.length > 0)
                    this.decoyGroupText = this.decoyGroupList[0].value;
                this.lastIndex = 0;
                this.isFavourite = result.isFavouriteCampaign;
                this.infoFetched = true;
                this.updateTotal();
            });
        }
    }

    onUnzippedCheckBoxChecked(event): void {

        if (this.unzippedChecked == true) {
            this.datafileOnlydisabled = true;
            this.dataFileOnlyChecked = true;
            this.isSplitDisabled = true;
            this.selectedSplitOption = this.splitOptionList[0].value;
            this.isPartsVisible = true;
            this.classType = "col-md-12";
            this.onSplitDropdownChange(null);
        }
        else {
            this.datafileOnlydisabled = false;
            this.dataFileOnlyChecked = false;
            this.isSplitDisabled = false;
        }
    }

    onDatafileonlyCheckBoxChecked(event): void {
        if (event) {
            this.dataFileOnlyChecked = true;
        }
        else {
            this.dataFileOnlyChecked = false;
        }
    }

    onHeaderRowChecked(event): void {
        if (event) {
            this.headerRowCheck = true;
        }
        else {
            this.headerRowCheck = false;
        }
    }

    onTypeDropdownChange(skip: boolean, dropdown: Dropdown): void {
        if (this.selectedOutputType == "FF") {
            this.headerRowCheck = false;
            this.headerRowdisabled = true;
            this.prevSelectedOutputType = this.selectedOutputType;            
        }
        else if (this.selectedOutputType == "EE") {
            if (this.CampaignStatus == CampaignStatus.CampaignCompleted ||
                this.CampaignStatus > CampaignStatus.CampaignFailed )   {
                if (this.totalOutputQuantity > 1000000 && !skip) {
                    this.message.error(this.l("ExportFileFormatError"));
                    this.selectedOutputType = this.prevSelectedOutputType;
                    dropdown.selectedOption = dropdown.options.find(x => x.value == this.selectedOutputType);
                }
                else {
                    this.headerRowCheck = true;
                    this.headerRowdisabled = true;
                    this.prevSelectedOutputType = this.selectedOutputType;
                }                    
            } else {
                this.headerRowCheck = true;
                this.headerRowdisabled = true;
                this.prevSelectedOutputType = this.selectedOutputType;
            }                    
        }
        else {
            this.headerRowCheck = true;
            this.headerRowdisabled = false;
            this.prevSelectedOutputType = this.selectedOutputType;
        }
    }

    getEmailAddress() {
        var shipTo = this.selectedOutputShipTo;
        this._orderServiceProxy.getDetailsByCompanyIdAndOrderId(this.campaignId, parseInt(this.selectedOutputShipTo)).subscribe(

            (result: any) => {

                this.ftpSite = result.ftpSite;
                this.userName = result.userName;
                if (result.emailAddress == null || result.emailAddress == undefined) {
                    this.selectedOutputShipTo = shipTo;
                }
                else {
                    if (this.email == "" && result.emailAddress != null && result.emailAddress != undefined) {
                        this.email = result.emailAddress;
                    }
                    else {
                        if (result.emailAddress != "" && result.emailAddress != null && result.emailAddress != undefined)
                            this.email += ";" + result.emailAddress;
                    }

                    this.selectedOutputShipTo = this.email;
                }
            }
        );
    }

    shipToKeyDown(event): void {
        if (event.key == "Enter") {
            if (!isNaN(this.selectedOutputShipTo as any)) {
                this.companyId = parseInt(this.selectedOutputShipTo);
                this.getEmailAddress();
            }
        }

    }

    onShipToDropdownChange(event): void {
        if (event.originalEvent != undefined) {
            if (!isNaN(this.selectedOutputShipTo as any) && this.selectedOutputShipTo.trim() !== "") {
                this.companyId = parseInt(this.selectedOutputShipTo);
                this.getEmailAddress();
            }
            else {
                this.email = event.value;
            }

        }
    }

    DownloadExcel(): void {

        this._orderServiceProxy.downloadOutputLayoutTemplateExcelTest(this.campaignId).subscribe(result => {
            this._fileDownloadService.downloadTempFile(result)
        });

    }

    editCampaignExportPart(): EditCampaignExportPartDto[] {

        var campaignExportPart = [];

        for (let i = 0; i < this.primengTableHelperExportPart.records.length; i++) {
            var input = new EditCampaignExportPartDto();
            input.orderId = this.campaignId;
            input.iDedupeOrderSpecified = this.primengTableHelperExportPart.records[i].iDedupeOrderSpecified;
            input.segmentID = this.primengTableHelperExportPart.records[i].segmentID;
            input.iQuantity = this.primengTableHelperExportPart.records[i].iQuantity;
            input.outputQuantity = this.primengTableHelperExportPart.records[i].outputQuantity;
            input.providedQuantity = this.primengTableHelperExportPart.records[i].providedQuantity;
            input.cPartNo = [];
            for (let j = 0; j < this.selectedPart; j++) {
                input.cPartNo.push((j + 1).toString());
            }
            campaignExportPart.push(input);
        }
        return campaignExportPart;
    }

    editCampaignOutput(): EditCampaignsOutputDto {

        var input = new EditCampaignsOutputDto();
        for (let i = 0; i < this.outputLayouts.length; i++) {
            if (this.outputLayouts[i].value == this.selectedOutputLayout) {
                input.layoutDescription = this.outputLayouts[i].label.substring(this.outputLayouts[i].label.indexOf(':') + 1, this.outputLayouts[i].label.length).trim();
                break;
            }
            else {
                input.layoutDescription = "";
            }
        }
        input.sort = this.selectedoutputSort;
        input.type = this.selectedOutputType;
        input.layout = this.selectedOutputLayout;
        input.pgpKey = this.selectedPgpKey;
        input.isHeaderRow = this.headerRowCheck;
        input.isDataFileOnly = this.dataFileOnlyChecked;
        if (this.unzippedChecked == null) {
            input.isUnzipped = false;
        }
        else {
            input.isUnzipped = this.unzippedChecked;
        }
        input.shipTo = this.selectedOutputShipTo;
        input.shipCCEmail = this.shipToCC;
        input.shipSubject = this.shipToSubject;
        input.shipNotes = this.shipToNotes;
        input.ftpSite = this.ftpSite;
        input.userName = this.userName;
        if (!isNaN(this.companyId)) {
            input.companyId = this.companyId;
        }
        if (!isNaN(parseInt(this.selectedOutputLayout))) {
            input.layoutId = parseInt(this.selectedOutputLayout);
        }

        input.media = this.selectedMedia;
        input.fileNotes = this.fileNotes;
        input.fileLabel = this.fileLabel;
        input.splitIntoNParts = this.selectedPart;
        input.splitType = this.selectedSplitOption;
        return input;
    }

    onLayoutDropdownChange(): void {
        var userName = this.appSession.idmsUser.idmsUserName;
        if (this.CampaignStatus >= 40) {
            if (this.CampaignStatus === 50) {
                this.CampaignStatus = 10;
            }
            else {
                this.CampaignStatus = 40;
            }           
        }
        else {
            this.CampaignStatus = 10;
        }
        this._orderServiceProxy.copyOrderExportLayout(this.campaignId, parseInt(this.selectedOutputLayout), userName, this.CampaignStatus)
            .subscribe(() => {
                this.isLayoutDownloadEnabled = false;
                this.camapaignSaved = true;
            });
    }

    onSortDropdownChange(event): void {
        this.selectedoutputSort = event.value;
    }

    onPGPKeyDropdownChange(event): void {
        this.selectedPgpKey = event.value;
    }

    save(form: NgForm): void {

        let orderStatus: number = 10;
        let formControls: any;
        const campaignDescriptionControl = form.controls.mainForm.get('cDescription');
        if (this.primengTableHelperDecoy.records != null && this.primengTableHelperDecoy.records != undefined && this.primengTableHelperDecoy.records.length > 0)
            this.isSeedRecordsUpdated = this.primengTableHelperDecoy.records.filter(t => t.action == ActionType.Add || t.action == ActionType.Edit || (t.action == ActionType.Delete && t.id != 0)).length > 0 ? true : false;
        if (this.primengTableHelper.records != null && this.primengTableHelper.records != undefined && this.primengTableHelper.records.length > 0)
            this.isReportsDataUpdated = this.primengTableHelper.records.filter(t => t.action == ActionType.Add || t.action == ActionType.Edit || (t.action == ActionType.Delete && t.id != 0)).length > 0 ? true : false;
        if (this.primengTableHelperMultiDimension.records != null && this.primengTableHelperMultiDimension.records != undefined && this.primengTableHelperMultiDimension.records.length > 0)
            this.isMultiReportsDataUpdated = this.primengTableHelperMultiDimension.records.filter(t => t.action == ActionType.Add || t.action == ActionType.Edit || (t.action == ActionType.Delete && t.id != 0)).length > 0 ? true : false;
        if (this.primengTableHelperMaxPer.records != null && this.primengTableHelperMaxPer.records != undefined && this.primengTableHelperMaxPer.records.length > 0)
            this.isMaxPerUpdated = this.primengTableHelperMaxPer.records.filter(t => t.segmentLevelAction == ActionType.Edit).length > 0 ? true : false;

        let formDirty = form.controls.mainForm.dirty || this.isDelete || this.campaignId === undefined || this.isSeedRecordsUpdated || this.isReportsDataUpdated || this.isMaxPerUpdated || this.isMultiReportsDataUpdated;
        if (this.campaignId && this.permission.isGranted('Pages.Campaigns.OESS')) {
            formDirty = formDirty || form.controls.oessForm.dirty;
        }
        if (formDirty) {
            if (this.campaignId !== undefined && this.CampaignStatus !== 10) {
                formControls = form.controls.mainForm;
                orderStatus = this.getDirtyStatus(formControls.controls);
            }
            const onlyDescriptionChanged: boolean = orderStatus === 0 && campaignDescriptionControl.dirty;
            if (onlyDescriptionChanged || this.oessData.oessStatus == "Invoiced") {
                orderStatus = this.CampaignStatus;
            }

            let isAdd: boolean = true;
            let input = new CreateOrEditCampaignDto();
            input.isStatusChangeRequired = !onlyDescriptionChanged;
            input.maxPerData = new GetCampaignMaxPerForViewDto();
            input.maxPerData.getCampaignLevelMaxPerData = new CampaignLevelMaxPerDto();
            input.id = this.campaignId;

            input.generalData = CampaignGeneralDto.fromJS(this.campaign);
            if (!this.campaignId) {
                var campaignLevelMaxPerData = this.getCampaignLevelMaxPerData();
                input.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity = campaignLevelMaxPerData.cMaximumQuantity;
                input.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity = campaignLevelMaxPerData.cMinimumQuantity;
                input.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel = campaignLevelMaxPerData.cMaxPerFieldOrderLevel;
            }


            input.billingData = CampaignBillingDto.fromJS(this.billing)

            input.listOfXTabRecords = this.primengTableHelper.records ? this.primengTableHelper.records.filter(r => r.action != ActionType.None) : null;
            input.listOfMultidimensionalRecords = this.primengTableHelperMultiDimension.records ? this.primengTableHelperMultiDimension.records.filter(r => r.action != ActionType.None) : null;
            input.campaignOutputDto = this.campaignId != undefined ? this.editCampaignOutput() : null;
            if (this.selectedSplitOption == 4) {
                input.campaignOutputDto.editCampaignExportPart = this.campaignId != undefined ? this.editCampaignExportPart() : [];
            }

            input.maxPerData.getSegmentLevelMaxPerData = this.primengTableHelperMaxPer.records ? this.primengTableHelperMaxPer.records.filter(r => r.segmentLevelAction != ActionType.None) : null;
            if (this.campaignId) {
                input.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity = this.MaximumMaxPerQuantity && this.MaximumMaxPerQuantity != "" ? this.MaximumMaxPerQuantity : 0;
                input.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity = this.MinimumMaxPerQuantity && this.MinimumMaxPerQuantity != "" ? this.MinimumMaxPerQuantity : 0;
                input.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel = this.CampaignLevelMaxPerField;
            }
            input.maxPerData.campaignId = this.campaignId;
            input.decoyData = new GetDecoyForViewDto();
            input.decoyData.listOfDecoys = this.primengTableHelperDecoy.records;
            input.decoyData.decoyKey = this.decoyKeyText;
            input.decoyData.decoyKey1 = this.decoyKey1Text;
            input.decoyData.isDecoyKeyMethod = this.decoyKeyMethod;
            if (!this.launchOESSFlag)
                this.saving = true;
            this._orderServiceProxy.createOrEdit(input, orderStatus)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(result => {
                    isAdd = this.campaignId ? false : true;
                    this.camapaignSaved = true;
                    this.CampaignStatus = result.status;
                    if (this.campaignId == undefined) {

                        this.appSession.idmsUser.currentCampaignDatabaseId = input.generalData.databaseID;
                        this.notify.info(this.l('SavedSuccessfully'));
                        let selectionDetails: SelectionDetails = {
                            campaignId: result.campaignId,
                            segmentId: result.segmentID,
                            databaseId: result.databaseID,
                            divisionId: result.divisionId,
                            databaseName: result.databaseName,
                            buildId: result.buildID,
                            mailerId: result.mailerId,
                            build: result.build,
                            campaignDescription: input.generalData.cDescription,
                            splitType: result.splitType
                        };
                        this.activeModal.close({ isSave: true, isAdd: isAdd, selectionDetails, isFavouriteChanged: this.isFavouriteChanged });
                    }
                    else {
                        this.editedDescription = result.campaignDescription;
                        if (!this.launchOESSFlag) {
                            this.oessData.databaseId = this.databaseId;
                            this.oessData.campaignDescription = this.campaignDescription;
                            this.oessData.iBillingQty = this.oessData.iBillingQty.toString() == "" ? 0 : this.oessData.iBillingQty;
                            this._orderServiceProxy.launchOESS(this.oessData).subscribe();
                            this.notify.info(this.l('SavedSuccessfully'));
                            this.close();
                        }
                    }

                });
        }
        else {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close();
        }
    }

    getOutputLayoutFloatStyle() {
        return 'right';
    }

    getDirtyStatus(formControls): number {
        var newStatus = [];        
        let listGeneralTab = new Array("cOffer", "cOrderType", "campaignLevelDrop", "FromQty", "ToQty");
        let listOutputTab = new Array("outputType", "outputMedia", "fileLabel", "fileNotes", "outputLayout", "outputSort", "outputPgpKey", "outputShipTo", "shipCC", "shipSubject", "headerRow", "dataFileOnly", "unzipped",
            "isNetUse", "isNoUsage", "lvaOrderNo", "nextMarkOrderNo", "sanNumber", "brokerPONo", "outputSplit", "outputPart", "seedKey", "seedKey1Lable", "seedKeyLable");
        let shipNotes = "shipNotes";
        let mailer = "mailer";
        let broker = "broker";
        let channelType = "cChannelType";
        if (this.isReportsDataUpdated || this.isMultiReportsDataUpdated || this.isMaxPerUpdated)
            return 10;
        for (let x = 0; x < listGeneralTab.length; x++) {
            if (formControls[listGeneralTab[x]]) {
                var status = formControls[listGeneralTab[x]]["dirty"];
                if (status) {
                    return 10;
                }
            }
        }

        if (formControls[channelType]) {
            var channelTypeStatus = formControls[channelType]["dirty"];
            if (channelTypeStatus) {
                newStatus.push(this.CampaignStatus);
            }
        }

        if (formControls[mailer]) {
            var mailerStatus = formControls[mailer]["dirty"];
            if (mailerStatus) {
                if (this.CampaignStatus === 90) { 
                    newStatus.push(90);
                }
                else if (this.CampaignStatus === 40) {
                    newStatus.push(40);
                }
                else if (this.CampaignStatus === 100) {
                    newStatus.push(40);
                }
                else if (this.CampaignStatus === 50) {
                    return 10;
                }
                else {
                    return 10;
                }
            }
        }

        if (formControls[broker]) {
            var brokerStatus = formControls[broker]["dirty"];
            if (brokerStatus) {
                if (this.CampaignStatus === 90) {
                    newStatus.push(90);
                }
                else if (this.CampaignStatus === 40) {
                    newStatus.push(40);
                }
                else if (this.CampaignStatus === 100) {
                    newStatus.push(40);
                }
                else if (this.CampaignStatus === 50) {
                    return 10;
                }
                else {
                    return 10;
                }
            }
        }


        for (let x = 0; x < listOutputTab.length; x++) {
            if (formControls[listOutputTab[x]]) {
                var status = formControls[listOutputTab[x]]["dirty"];
                if (status || this.isGridTextChanged || this.isSeedRecordsUpdated) {
                    if (this.CampaignStatus === 50) {
                        newStatus.push(50);
                    }
                    newStatus.push(40);
                }
            }
        }
        var shipToNotesStatus = formControls[shipNotes]["dirty"];
        if (shipToNotesStatus) {
            if (this.CampaignStatus === 90) {
                newStatus.push(90);
            }
            else if (this.CampaignStatus === 50) {
                newStatus.push(50);
            }
            else {
                newStatus.push(40);
            }
        }

        if (newStatus.length === 0) 
            return 0;        
        else 
            return Math.min(...newStatus);      
               
    }  
    
    disabledReportsDownloadLink() {
        if (this.CampaignStatus == CampaignStatus.CampaignCreated ||
            this.CampaignStatus == CampaignStatus.CampaignSubmitted ||
            this.CampaignStatus == CampaignStatus.CampaignRunning ||
            this.CampaignStatus == CampaignStatus.CampaignFailed) {
            this.isDownloadXtabEnabled = true;
            this.isDownloadMulticolumnEnabled = true;
        }

        else {
            this.isDownloadXtabEnabled = false;
            for (let i = 0; i < this.primengTableHelperMultiDimension.records.length; i++) {
                if (this.primengTableHelperMultiDimension.records[i].cFieldDescription != "") {


                    this.isDownloadMulticolumnEnabled = false;
                    break;
                }
                else {
                    this.isDownloadMulticolumnEnabled = true;
                }
            }

        }
    }

    close(): void {
        this.active = false;
        this.isDelete = false;
        this.isGridTextChanged = false;
        this.activeModal.close({
            isSave: this.camapaignSaved,
            campaignId: this.campaignId,
            description : this.editedDescription,
            isExportLayoutSelected: !this.isLayoutDownloadEnabled,
            campaignStatus: this.CampaignStatus,
            isFavouriteChanged: this.isFavouriteChanged
        });
    }

    //Change Events For XtabReports
    onXFieldChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelper.records[relativeIndex];
        record.cXDesc = this.XFields.find(x => x.value == event.value).label;
        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    onYFieldChange(relativeIndex: number, event: any): void {

        let record = this.primengTableHelper.records[relativeIndex];
        record.cYDesc = this.YFields.find(x => x.value == event.value).label;
        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0 && (record.cYDesc != '' || record.cYDesc != null))
            record.action = ActionType.Add;
    }

    onLevelChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelper.records[relativeIndex];
        record.isXTab = this.Levels.find(x => x.value == event.value).label;

        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    onXTabSegmentNumberChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelper.records[relativeIndex];
        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    onMultiSegmentNumberChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    onMultiLevelChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        record.isMulti = this.Levels.find(x => x.value == event.value).label;

        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    onMultiFieldChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        var fieldDescription = "";
        for (let i = 0; i < event.value.length; i++) {
            fieldDescription += this.multiColumnFields.find(x => x.value == event.value[i]).label + ",";
        }
        record.cFieldName = event.value.join(",");
        record.cFieldDescription = fieldDescription.replace(/,\s*$/, "");
        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    onTypeChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelper.records[relativeIndex];
        record.cTypeName = this.Types.find(x => x.value == event.value).label;
        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    onMultiTypeChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        record.cTypeName = this.Types.find(x => x.value == event.value).label;
        if (record.id != 0)
            record.action = ActionType.Edit;
        else if (record.id == 0)
            record.action = ActionType.Add;
    }

    //onMaxPerFieldSegmentLevel Change
    onMaxPerFieldSegmentLevelChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMaxPer.records[relativeIndex];
        record.cMaxPerFieldDescription = this.MaxPerFieldsSegmentlevel.find(x => x.value == event.value).label;
        record.segmentLevelAction = ActionType.Edit;
    }

    //onMaxPerQuantityChange
    onMaxPerTextQuantityChange(absoluteIndex: number) {
        let record = this.primengTableHelperMaxPer.records[absoluteIndex];
        record.segmentLevelAction = ActionType.Edit;
    }

    // //onMaxPerFieldCampaignLevel Change
    onMaxPerFieldCampaignLevelChange(event: any): void {
        if (event.value == "") {
            this.MinimumMaxPerQuantity = "";
            this.MaximumMaxPerQuantity = "";
            this.IsMaxFieldEmpty = true;
        }
        else
            this.IsMaxFieldEmpty = false;


    }

    //Download Xtab
    downloadXtabReport(campaignId: any, event: any): void {
        event.preventDefault();
        event.stopPropagation();
        if (this.CampaignStatus != CampaignStatus.CampaignCreated &&
            this.CampaignStatus != CampaignStatus.CampaignSubmitted &&
            this.CampaignStatus != CampaignStatus.CampaignRunning &&
            this.CampaignStatus != CampaignStatus.CampaignFailed) {

            this._orderServiceProxy.downloadXtabReport(campaignId).subscribe(result => {
                this._fileDownloadService.downloadDocumentAttachment(result);
            });
        }
    }

    downloadMultiColumnReport(campaignId: any, event: any): void {

        event.preventDefault();

        event.stopPropagation();
        if (this.CampaignStatus != CampaignStatus.CampaignCreated &&
            this.CampaignStatus != CampaignStatus.CampaignSubmitted &&
            this.CampaignStatus != CampaignStatus.CampaignRunning &&
            this.CampaignStatus != CampaignStatus.CampaignFailed) {

            this._orderServiceProxy.downloadMulticolumnReport(campaignId).subscribe(result => {

                this._fileDownloadService.downloadDocumentAttachment(result);
            });
        }
    }

    //Delete Xtab
    deleteXtab(relativeIndex: number): void {

        this.message.confirm(
            this.l(''),
            (isConfirmed) => {
                if (isConfirmed) {
                    let record = this.primengTableHelper.records[relativeIndex];
                    record.action = ActionType.Delete;
                    this.isDelete = true;
                }
            }
        );
    }

    deleteMulti(relativeIndex: number): void {
        this.message.confirm(
            this.l(''),
            (isConfirmed) => {
                if (isConfirmed) {
                    let record = this.primengTableHelperMultiDimension.records[relativeIndex];
                    record.action = ActionType.Delete;
                    this.isDelete = true;
                }
            }
        );
    }

    deleteOrderAttachment(id) {
        if (id > 0) {
            this.message.confirm(
                this.l(''),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._orderServiceProxy.deleteCampaignAttachment(id).subscribe(any => { this._orderServiceProxy.fetchAttachmentsData(this.campaignId).subscribe(result => { this.documentsData = result; }); });
                    }
                }
            );
        }
    }

    uploadFile(event, i, fileUploadControl, isDisabled): void {
        if (isDisabled) {
            return;
        }
        const formData: FormData = new FormData();
        const file = event.files[0];
        if (file.size > (this.documentFileSize * 1024 * 1024)) {
            this.message.error(this.l("File_Document_Warn_SizeLimit", this.documentFileSize.toString()));
            fileUploadControl.clear();
            return;
        }
        formData.append('file', file, file.name);
        formData.append('code', this.documentsData[i].code);
        this.attachementTypeCode.push(this.documentsData[i].code);
        var currentAttachementType = this.documentsData[i].code;
        this.documentsData[i].isDisabled = true;
        var uploadUrl = AppConsts.remoteServiceBaseUrl + '/File/UploadAttachmentFile?campaignId=' + this.campaignId;
        this._httpClient
            .post<any>(uploadUrl, formData)
            .subscribe(response => {
                var attachment = this.documentsData.find(x => x.code == response.result.code);
                if (attachment) {
                    attachment.cFileName = response.result.cFileName;
                    attachment.realFileName = response.result.realFileName;
                }
                if (response.success) {
                    this.notify.success("Upload Successful");
                    this._orderServiceProxy.uploadFile(attachment.cFileName, attachment.realFileName, attachment.code, attachment.id, this.campaignId)
                        .subscribe(any => {
                            this._orderServiceProxy.fetchAttachmentsData(this.campaignId)
                                .subscribe(result => {
                                    this.documentsData = result;
                                    this.modifyIsDisabled(currentAttachementType); 
                                },
                                    () => {
                                        this.notify.error("Upload Unsuccessful");
                                        this.modifyIsDisabled(currentAttachementType);
                                    });
                        });
                } else if (response.error != null) {
                    this.notify.error("Upload Unsuccessful");
                    this.modifyIsDisabled(currentAttachementType);
                }
            });
    }

    modifyIsDisabled(currentAttachementType: string): void {
        this.attachementTypeCode.forEach(value => {
            if (value !== currentAttachementType) {
                this.documentsData.find(x => x.code == value).isDisabled = true;
            }
        });
        var indexOfType = this.attachementTypeCode.indexOf(currentAttachementType);
        if (indexOfType !== -1) {
            this.attachementTypeCode.splice(indexOfType, 1);
        }
    }

    onUploadExcelError(event): void {
        this.notify.error("Upload Unsuccessful");
    }

    onPartDropdownChange(event): void {

        if (event != null || event != undefined) {
            this.selectedPart = event.value;
        }
        if (this.selectedSplitOption == 4 || this.selectedSplitOption == 3) {
            if (this.selectedPart == "" || this.selectedPart == undefined || this.selectedPart == null) {
                this.selectedPart = 1;
            }
        }
        if (this.selectedSplitOption == 4) {
            this._orderServiceProxy.getExportParts(this.campaignId, this.selectedPart).subscribe(result => {
                this.cols = [];
                for (let i = 0; i < this.selectedPart; i++) {

                    this.cols.push({ field: i, header: (i + 1).toString() })
                }
                if (result.length > 0) {
                    this.isShowTable = false;
                    this.primengTableHelperExportPart.records = result;
                }
                else {
                    this.isShowTable = true;
                    this.primengTableHelperExportPart.records = [];
                }
            });
        }
    }

    onSplitDropdownChange(event): void {

        if (this.selectedSplitOption == 3 || this.selectedSplitOption == 4) {
            if (this.selectedSplitOption == 3) {
                this.classType = "col-md-6";
                this.partsList = [];
                this.isPartsVisible = false;
                for (let i = 1; i <= 20; i++) {
                    this.partsList.push({ label: i.toString(), value: i });
                }
            }
            if (this.selectedSplitOption == 4) {
                this.classType = "col-md-6";
                this.partsList = [];
                this.isPartsVisible = false;
                for (let i = 1; i <= 10; i++) {

                    this.partsList.push({ label: i.toString(), value: i });
                }
            }
            this._orderServiceProxy.getSplitPartForCampaignId(this.campaignId).subscribe(result => {


                var splitOption = 0;
                var splitPart = 0;
                splitOption = result.iSplitType;
                splitPart = result.iSplitIntoNParts;

                if (splitOption == this.selectedSplitOption) {
                    this.selectedPart = splitPart;
                }
                else {
                    this.selectedPart = 1;
                }
                this.onPartDropdownChange(null);
            });
        }
        else {
            this.selectedPart = 0;
            this.isPartsVisible = true;
            this.classType = "col-md-12";
        }
    }

    quantityValueChanged() {
        this.isGridTextChanged = true;
    }

    onSeedKeyChange(event): void {
        if (event.value == 1) {
            this.isAuto = true;
            this.decoyKeyText = '';
        }
        else {
            this.isAuto = false;
        }
    }

    openAddEditSeed(isEdit, absoluteIndex, isNotPermissionForEdit): void {
        this.setAbsoluteIndex = absoluteIndex;
        if (isEdit) {
            if (!isNotPermissionForEdit) {
                this.decoyAddEditText = this.l('EditSeed');
                this.seed = DecoyDto.fromJS(this.primengTableHelperDecoy.records[absoluteIndex]);

                if (this.seed.cDecoyGroup != "" && this.seed.cDecoyGroup != null && this.seed.cDecoyGroup != undefined) {
                    this.decoyGroupByForEdit = this.tempdecoyGroupByForEdit;
                    this.isDecoyGroup = false
                }
                else {
                    this.isDecoyGroup = true;
                    this.decoyGroupByForEdit = [];
                }
                this.lastIndex = 1;
            }
        }
        else {
            this.decoyAddEditText = this.l('Add');
            this.isDecoyGroup = true;
            this.decoyGroupByForEdit = [];
            this.seed = new DecoyDto();
            this.lastIndex = 1;
        }

    }

    resetPanels(): void {
        this.lastIndex = 0;
        this.isDecoyRecords = this.primengTableHelperDecoy.records.length > 0 ? false : true;
        this.decoyAddEditText = this.l('Add');
        this.decoyGroupByForEdit = [];
    }

    validateSeed(seedForm): void {
        this._orderServiceProxy.validateDecoys(this.seed).subscribe(response => {
            if (response) {
                this.message.confirm(
                    this.l('ValidateDecoyKey', '<##DK##>'),
                    (isConfirmed) => {
                        if (isConfirmed) {
                            this.saveSeed(seedForm);
                        }
                    });
            }
            else
                this.saveSeed(seedForm);
        });
    }

    saveSeed(seedForm): void {
        if ((this.seed.id > 0 && this.seed.id != undefined && this.decoyAddEditText == this.l('EditSeed')) || (this.seed.action == ActionType.Add && this.decoyAddEditText == this.l('EditSeed'))) {
            if (this.seed.action != ActionType.Add)
                this.seed.action = ActionType.Edit;
            if (this.seed != null)
                this.mapAddressAndAddressLine(this.seed);
            this.primengTableHelperDecoy.records[this.setAbsoluteIndex] = DecoyDto.fromJS(this.seed);
            seedForm.reset();
            this.resetPanels();
        }
        else {
            this.seed.action = ActionType.Add;
            this.seed.cDecoyType = "M";
            this.mapAddressAndAddressLine(this.seed);
            let record = DecoyDto.fromJS(this.seed);
            this.primengTableHelperDecoy.records.push(record);
            seedForm.reset();
            this.resetPanels();
        }
    }

    mapAddressAndAddressLine(seedData: DecoyDto): void {
        seedData.cName = seedData.cFirstName + " " + seedData.cLastName;
        if (seedData.cZip4 == null || seedData.cZip4 == undefined || seedData.cZip4 == '' || seedData.cZip4 == "0")
            seedData.cAddress = seedData.cCity + "," + seedData.cState + " " + seedData.cZip;
        else
            seedData.cAddress = seedData.cCity + "," + seedData.cState + " " + seedData.cZip + "-" + seedData.cZip4;
    }

    deleteSeed(absoluteIndex: number, isNotPermissionForDelete: boolean): void {
        if (!isNotPermissionForDelete) {
            this.message.confirm(
                this.l(''),
                (isConfirmed) => {
                    if (isConfirmed) {
                        let record = this.primengTableHelperDecoy.records[absoluteIndex];
                        record.action = ActionType.Delete;
                        this.isDecoyRecords = this.primengTableHelperDecoy.records.filter(t => t.action != ActionType.Delete).length > 0 ? false : true;
                    }
                }
            );
        }
    }

    onDecoyKeyChange(event): void {
        this.decoyGroupText = event.value;
    }

    onSaveGroup(): void {
        this._orderServiceProxy.getDecoyGroupList(this.campaignId, this.decoyGroupText).subscribe(response => {
            if (response != null && response.length > 0) {
                response.forEach(groupResponse => {
                    groupResponse.action = ActionType.Add;
                    this.primengTableHelperDecoy.records.push(groupResponse);
                    this.isDecoyRecords = this.primengTableHelperDecoy.records.length > 0 ? false : true;
                    this.lastIndex = 0;
                });
            }
        });
    }

    onTabOpen(e) {
        this.lastIndex = e.index;
    }

    addOrRemoveFavourites(): void {
        this.isFavouriteChanged = !this.isFavouriteChanged;
        this._campaignFavoritesServiceProxy.addOrRemoveFavouriteCampaigns(this.campaignId, this.isFavourite)
            .subscribe(() => {
                if (!this.isFavourite)
                    this.notify.info(this.l('Added to Favorites Successfully'));
                else
                    this.notify.info(this.l('Removed from Favorites Successfully'));
                this.isFavourite = !this.isFavourite;
            });
    }

    onDiscountChange(event): void {
        if (this.CampaignStatus < CampaignStatus.Shipped) {
            if (event != "") {
                if (this.IsNumeric(event)) {
                    this.saveDisabled = false;
                }
                else {
                    this.saveDisabled = true;
                }
            }
            else {
                this.saveDisabled = false;
            }
        }
        this.oessData.nDiscountPercentage = event;
        this.updateEffectiveUnitPrice();
    }

    onUnitPriceChange(event): void {
        if (this.CampaignStatus < CampaignStatus.Shipped) {
            if (event != "") {
                if (this.IsNumeric(event)) {
                    this.saveDisabled = false;
                }
                else {
                    this.saveDisabled = true;
                }
            }
            else {
                this.saveDisabled = false;
            }
        }
        this.oessData.nUnitPrice = event;
        this.updateEffectiveUnitPrice();
    }

    IsNumeric(input: string): boolean {
        if ((/^\d{1,}(\.\d{1,})?$/).test(input))
            return true;
        else
            return false;
    }

    onQuantityChange(event): void {
        if (this.CampaignStatus < CampaignStatus.Shipped) {
            if (event != "") {
                if (this.IsNumeric(event)) {
                    this.saveDisabled = false;
                }
                else {
                    this.saveDisabled = true;
                }
            }
            else {
                this.saveDisabled = false;
            }
        }
        this.oessData.iBillingQty = event;

        this.updateTotal();
    }

    onShippingChargeChange(event): void {
        if (this.CampaignStatus < CampaignStatus.Shipped) {
            if (event != "") {
                if (this.IsNumeric(event)) {
                    this.saveDisabled = false;
                }
                else {
                    this.saveDisabled = true;
                }
            }
            else {
                this.saveDisabled = false;
            }
        }
        this.oessData.nShippingCharge = event;
        this.updateTotal();
    }

    onEffectiveUnitPriceChange(event): void {
        if (this.CampaignStatus < CampaignStatus.Shipped) {
            if (event != "") {
                if (this.IsNumeric(event)) {
                    this.saveDisabled = false;
                }
                else {
                    this.saveDisabled = true;
                }
            }
            else {
                this.saveDisabled = false;
            }
        }
        this.oessData.nEffectiveUnitPrice = event;
    }

    updateEffectiveUnitPrice(): void {
        this.oessData.nDiscountPercentage = this.oessData.nDiscountPercentage == "" ? "0" : this.oessData.nDiscountPercentage;
        var effUnitPrice = parseFloat(this.oessData.nUnitPrice) * (1 - parseFloat(this.oessData.nDiscountPercentage) / 100);
        this.oessData.nEffectiveUnitPrice = isNaN(effUnitPrice) ? '' : effUnitPrice.toFixed(3);

        this.updateTotal();
    }

    updateTotal(): void {
        var shippingCharge = this.oessData.nShippingCharge == "" ? 0 : this.oessData.nShippingCharge;
        var total = 0;
        switch (this.oessData.lK_BillingUOM) {
            case 'E': //each (E)
                if (this.oessData.iBillingQty == null)
                    this.oessData.iBillingQty = 0;
                total = parseFloat(this.oessData.nEffectiveUnitPrice) * parseFloat(this.oessData.iBillingQty.toString()) + parseFloat(shippingCharge.toString());
                break;
            case 'F': //flat (F)
                total = parseFloat(this.oessData.nEffectiveUnitPrice) + parseFloat(shippingCharge.toString());
                break;
            case 'T': // per thousand (T)
                if (this.oessData.iBillingQty == null)
                    this.oessData.iBillingQty = 0;
                total = parseFloat(this.oessData.nEffectiveUnitPrice) * (parseFloat(this.oessData.iBillingQty.toString()) / 1000) + parseFloat(shippingCharge.toString());
                break;
            default:
                break;
        }
        this.oessData.iTotalPrice = isNaN(total) ? '' : total.toFixed(3);
    }

    setLaunchOESSFlag(form: NgForm): void {
        this.launchOESSFlag = true;
        this.launchOESS();
        if (!this.oessValidationOccured && this.CampaignStatus < CampaignStatus.Shipped) {
            this.save(form);
        }

    }

    launchOESS(): void {
        this.oessValidationOccured = false;
        if (this.CampaignStatus != CampaignStatus.OutputCompleted) {
            this.message.error(this.l('OutputCompletedValidation'));
            this.oessValidationOccured = true;
            this.launchOESSFlag = false;
            //this.saving = false;
            return;
        }

        // check if there is already a row in tblOrderBillingStatus, do not allow to re-submit
        if (this.oessData.oessStatus != 'New') {
            this.message.error(this.l('BillingAlreadyInitiated'));
            this.oessValidationOccured = true;
            this.launchOESSFlag = false;
            //this.saving = false;
            return;
        }

        this.oessData.databaseId = this.databaseId;
        this.oessData.campaignDescription = this.campaignDescription;
        this._orderServiceProxy.launchOESS(this.oessData).subscribe(response => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close();
            this.launchOESSFlag = false;
            var width = 1250;
            var height = 750;
            var center_left = (screen.width / 2) - (width / 2);
            var center_top = (screen.height / 2) - (height / 2);
            var param = 'left=' + center_left + ',top=' + center_top + 'center:yes;resizable:yes;scroll:yes;help:no;status:no;dialogWidth:' + width + 'px;dialogHeight:' + height + 'px;';
            window.open(response, ModalWindowName.BLANK, param);

        });

    }
    downloadAttachment(code: any) {
        this._orderServiceProxy
            .downloadFile(this.campaignId, code, this.databaseId)
            .subscribe(result => {
                this._fileDownloadService.downloadDocumentAttachment(result);
            });
    }

    onDatabaseChange(event): void {
        this.resetControls();
        this._orderServiceProxy.getFieldsOnDatabaseChange(
            event.value
        ).subscribe(result => {
            this.setControlOptions(result);
            this.getMaxPerFields();            
            this.setUserDatabaseMailerRecord(result.isShowCustomer);
        });
    }

    resetControls(): void {
        this.campaign.mailer = null;
        this.builds = [];
        this.campaign.buildID = -1;
        if (this.mailerInput)
            this.mailerInput.control.markAsPristine();
        if (this.buildInput)
            this.buildInput.control.markAsPristine();
        if (this.campaign.divisionalDatabase) {
            this.campaign.broker = null;
            if (this.brokerInput)
                this.brokerInput.control.markAsPristine();
        }
        else {
            this.offers = [];
            this.campaign.offerID = -1;
            this.defaultOfferId = -1;
            if (this.offerDDInput)
                this.offerDDInput.control.markAsPristine();
        }
    }

    setControlOptions(result: GetCampaignDropdownsDto): void {
        this.builds = result.builds.buildDropDown;
        this.changeFormType(result.divisionalDatabase);
        if (!result.divisionalDatabase) {
            if (!this.campaignId) {
                this.campaign.mailer = result.defaultMailer;
                this.defaultOfferId = !result.offerID ? -1 : result.offerID;
            }
            if (typeof this.campaign.mailer !== "undefined" && this.campaign.mailer !== null && this.campaign.mailer.value > 0) this.onMailerSelect({ value: this.campaign.mailer.value });
        }
        else {
            if (!this.campaignId) {
                this.campaign.broker = result.defaultBroker;
                this.campaign.mailer = result.defaultMailer;
            }
        }
        if (!this.campaignId) {
            this.campaign.buildID = result.builds.defaultSelection;
        }
    }

    changeFormType(divisionalDatabase: boolean): void {
        this.campaign.divisionalDatabase = divisionalDatabase;
        this.brokerVisible = divisionalDatabase;
        this.offerDDVisible = !divisionalDatabase;
    }

    onMailerSelect(event): void {
        if (!this.campaign.divisionalDatabase && event.value > 0) {
            this.offers = [];
            this._orderServiceProxy.getOffersDDForMailer(
                event.value
            ).subscribe(result => {
                if (!this.campaign.divisionalDatabase) {
                    this.offers = result.slice();;
                    this.offers.splice(0, 0, DropdownOutputDto.fromJS({ label: "Select Offer" }));
                    if (this.defaultOfferId > -1) {
                        this.campaign.offerID = this.defaultOfferId;
                    }
                    else {
                        if (this.offers.length > 2) {
                            this.campaign.offerID = this.offers[0].value;
                        }
                        else {
                            this.campaign.offerID = this.offers[1].value;
                        }
                    }
                }
            });
        }
    }

    getMaxPerFields(): void {
        if (this.campaign.databaseID) {
            this._orderServiceProxy.getMaxPerFieldDropdownData(
                undefined,
                this.campaign.buildID,
                ""
            ).subscribe(
                result => {
                    this.maxPerFieldsCampaignlevel = [];
                    this.maxPerFieldsCampaignlevel.push({ label: "", value: "" });
                    this.maxPerFieldsCampaignlevel = this.maxPerFieldsCampaignlevel.concat(result);

                });
        }
    }
    
    setUserDatabaseMailerRecord(isShowCustomer: boolean) {
        if (this.campaign.divisionalDatabase) {
            this.customerVisible = isShowCustomer;
            this.brokerVisibleOnCount = isShowCustomer;
            this.offerVisibleOnCount = true;
        }
        else {
            if (this.campaignId) {
                this.customerVisible = false;                
                this.offerVisibleOnCount = false;
            }
            else {
                this.customerVisible = isShowCustomer;
                this.offerVisibleOnCount = true;
            }
            this.brokerVisible = false;
        }
    }


    searchMailers(event): void {
        if (this.campaign.databaseID) {
            this._orderServiceProxy.getSearchResultForMailer(
                event.query,
                this.campaign.databaseID,
                this.campaign.divisionalDatabase
            ).subscribe(result => { this.mailers = result });
        }
    }

    searchBrokers(event): void {
        this._orderServiceProxy.getDivisionBrokers(
            event.query,
            this.campaign.databaseID,
            this.campaign.divisionalDatabase
        ).subscribe(result => { this.brokers = result });
    }

    onBuildChange(event): void {
        this.getMaxPerFields();
    }   

    onMaxPerQuantityChange(): void {
        if (this.MaximumMaxPerQuantity > 0)
            this.MinimumMaxPerQuantity = 1;
        else
            this.MinimumMaxPerQuantity = 0;
    }

    getCampaignLevelMaxPerData(): CampaignLevelMaxPerDto {
        if (this.CampaignLevelMaxPerField == "") {
            this.MaximumMaxPerQuantity = "";
            this.MinimumMaxPerQuantity = "";
        };
        this.maxPer.cMaximumQuantity = this.MaximumMaxPerQuantity && this.MaximumMaxPerQuantity != "" ? this.MaximumMaxPerQuantity : 0;
        this.maxPer.cMinimumQuantity = this.MinimumMaxPerQuantity && this.MinimumMaxPerQuantity != "" ? this.MinimumMaxPerQuantity : 0;
        this.maxPer.cMaxPerFieldOrderLevel = this.CampaignLevelMaxPerField;
        return this.maxPer;
    }

}