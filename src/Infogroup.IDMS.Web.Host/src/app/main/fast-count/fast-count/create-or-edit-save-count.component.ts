import { Component, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import {
    CampaignsServiceProxy,
    CampaignGeneralDto,
    CreateOrEditSegmentOutputDto,
    CreateOrEditSegmentDto,
    ExportLayoutsServiceProxy,
    SegmentsServiceProxy,
    GetCampaignMaxPerForViewDto,
    CampaignLevelMaxPerDto,
    CampaignOESSDto,
    ActionType,
    CreateOrEditCampaignDto,
    CampaignBillingDto,
    EditCampaignsOutputDto,
    GetDecoyForViewDto,
    ReportsServiceProxy,
    GetCampaignXTabReportsListForView
} from '@shared/service-proxies/service-proxies';
import { SelectItem } from 'primeng/api';
import { NgForm } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { CampaignStatus } from '../../campaigns/shared/campaign-status.enum';
import { Dropdown } from 'primeng/dropdown';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { CreateOrEditResult } from '../../campaigns/shared/campaign-models';
import { CampaignUiHelperService } from '../../campaigns/shared/campaign-ui-helper.service';
import { CampaignAction } from '../../campaigns/shared/campaign-action.enum';
import { Table } from 'primeng/components/table/table';
import * as _ from 'lodash';
import { PrimengTableHelper } from '@shared/helpers/PrimengTableHelper';
import {Constants} from "@app/main/fast-count/constants";

interface SelectionDetails {
    buildId: number;
    build: number; // tblBuild.cBuild
    campaignId: number;
    databaseId: number;
    divisionId: number;
    mailerId: number;
    segmentId: number;
    campaignDescription: string;
    splitType: number;
    databaseName: string;
}

@Component({
    selector: 'app-create-or-edit-save-count',
    templateUrl: './create-or-edit-save-count.component.html',
    styleUrls: ['./create-or-edit-save-count.component.css']
})

export class CreateOrEditSaveCountComponent extends AppComponentBase {
    [x: string]: any;
    active = false;
    saving = false;
    @Input() activeForm: string;
    @Input() campaignId: number;
    @Input() databaseId: number;
    @Input() divisionId: number;
    @Input() cDescription: string;
    @Input() divisionalDatabase: boolean;
    @Input() fastCountConfig: any;
    @Input() channelType;
    @Input() segmentId;
    @Input() Maxper: boolean;
    @Input() buildId;
    activeNav = 1;
    channelTypeEnable = true;
    mailers: any[] = [];
    campaign!: CampaignGeneralDto | undefined;
    billing: CampaignBillingDto = new CampaignBillingDto();
    decoyKeyText: string;
    decoyKey1Text: string;
    decoyGroupList: SelectItem[] = [];
    decoyAddEditText: string;
    decoyGroupText: string;
    decoyKeyMethod: number;
    MinimumMaxPerQuantity: any;
    MaximumMaxPerQuantity: any;
    CampaignLevelMaxPerField = '';
    campaignSaved = false;
    CampaignStatus: number;
    editedDescription = '';
    oessData: CampaignOESSDto = new CampaignOESSDto();
    launchOESSFlag = false;
    outputLayouts: SelectItem[] = [];
    selectedOutputLayout: string;
    selectedOutputSort: string;
    selectedOutputType: string;
    selectedPgpKey: string;
    headerRowCheck = true;
    dataFileOnlyChecked = false;
    unzippedChecked = false;
    selectedOutputShipTo: string;
    shipToCC = '';
    shipToSubject = '';
    shipToNotes = '';
    ftpSite: string;
    userName: string;
    selectedMedia: any;
    fileLabel: any;
    fileNotes: any;
    selectedSplitOption: any;
    selectedPart: any;
    companyId: number;
    isDecoyRecords = false;
    isAuto: boolean;
    isDecoyGroup: boolean;
    decoyGroupValue: string;
    decoyGroupByForEdit: SelectItem[] = [];
    tempDecoyGroupByForEdit: SelectItem[] = [];
    segment: CreateOrEditSegmentDto = new CreateOrEditSegmentDto();
    isEdit = false;
    requiredQuantityValue: string;
    allAvailableText = 'All Available';
    email = '';
    outputShipToValues: SelectItem[] = [];
    outputTypes: SelectItem[] = [];
    headerRowDisabled = true;
    prevSelectedOutputType: string;
    totalOutputQuantity: number;
    isLoading: boolean = true;
    selectedBuild: any;
    buildsList: SelectItem[] = [];
    OutputLayoutId: any;
    layoutId: any;
    isCampaign: boolean = true;
    layoutList: any = [];
    list1: any = [];
    list2: any = [];
    maxPerGroup: string;
    dbArray: any;
    maxPerDrop: SelectItem[] = [];
    currentCampaignStatus = '';
    saveDisabled = true;
    outputQuantityDisabled = true;
    maxperCount: number = 1;

    //Variable Declartion For Count Reports
    XFields: SelectItem[] = [];
    YFields: SelectItem[] = [];
    Levels: SelectItem[] = [{ label: 'Campaign', value: false }, { label: 'Segment', value: true }];
    Types: SelectItem[] = [{ label: 'Net', value: 'N' }, { label: 'Gross', value: 'G' }];
    multiColumnFields: SelectItem[] = [];
    MaxPerFieldsSegmentlevel: SelectItem[] = [];
    @Input() IsMaxFieldEmpty: boolean;
    fastCountConfigXFields: SelectItem[] = [];
    fastCountConfigYFields: SelectItem[] = [];
    isMailerIdUpdated: boolean = false;
    isEditLayout: boolean = false;
    isAdd: boolean = true;
    selectedAvailableFields: any;
    addedAVailableFields: any;
    maintainanceBuildId = 0;
    layoutID = 0;
    currentStatus = 0;
    DbId = 0;
    primengTableHelperDataReports: PrimengTableHelper;
    maxPerFieldsFastCountlevel: any = [];
    Maxperdropdown: boolean = false;
    maxperdata: any;
    maxperConfig: boolean = false;
    showDownLoad: boolean = false;
    reportDataToShowOnUI: any[];
    instantBreakdownData: any;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _orderServiceProxy: CampaignsServiceProxy,
        private _segmentsServiceProxy: SegmentsServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
        private _campaignUiHelperService: CampaignUiHelperService,
        private _reportService: ReportsServiceProxy
    ) {
        super(injector);
        this.primengTableHelperDataReports = new PrimengTableHelper();
    }

    // tslint:disable-next-line:use-lifecycle-interface
    ngOnInit() {

        this.GetMaxPerOrderConfigValuesforDropdown();
        this.campaign = new CampaignGeneralDto();
        if (!this.Maxper) {
            this._exportLayoutServiceProxy.getBuildsByDatabase(this.databaseId).subscribe(result => {
                this.buildsList = result;
                this.selectedBuild = this.buildsList[0].value;
            });
        }
    }

    ngAfterViewInit() {
        setTimeout(() => {
            this.show();
        });
    }

    getCampaignForEdit() {
        this.isLoading = true;
        this._orderServiceProxy.getCampaignForEdit(this.campaignId, this.databaseId, this.divisionId).subscribe(result => {
            this.campaign = result.generalData;
            this.campaign.mailer = result.generalData.mailer;
            this.channelTypeEnable = result.isChannelTypeVisible;
            this.billing.init(result.billingData);
            this.decoyKeyText = result.decoyData.decoyKey;
            this.decoyKey1Text = result.decoyData.decoyKey1;
            this.outputLayouts = result.getOutputData.layoutlist;
            this.selectedOutputLayout = result.getOutputData.layout ? result.getOutputData.layout : (this.fastCountConfig['default-output-layout'] ? String(this.fastCountConfig['default-output-layout']) : '');
            this.layoutId = result.getOutputData.layoutId;
            this.selectedOutputSort = result.getOutputData.sort;
            this.selectedOutputType = result.getOutputData.type.trim();
            this.selectedPgpKey = result.getOutputData.pgpKey;
            this.headerRowCheck = result.getOutputData.isHeaderRow;
            this.dataFileOnlyChecked = result.getOutputData.isDataFileOnly;
            this.selectedOutputShipTo = result.getOutputData.shipTo;
            this.shipToCC = result.getOutputData.shipCCEmail;
            this.shipToSubject = result.getOutputData.shipSubject;
            this.shipToNotes = result.getOutputData.shipNotes;
            this.ftpSite = result.getOutputData.ftpSite;
            this.userName = result.getOutputData.userName;
            this.selectedMedia = result.getOutputData.media === '' ? 'E' : result.getOutputData.media;
            this.fileLabel = result.getOutputData.fileLabel;
            this.fileNotes = result.getOutputData.fileNotes;
            this.selectedPart = result.getOutputData.splitIntoNParts;
            this.selectedSplitOption = result.getOutputData.splitType;
            this.primengTableHelperDataReports.records = result.reportsData.xtabRecords;
            this.primengTableHelperMultiDimension.records = result.multiReportsData.multidimensionalRecords;
            this.multiColumnFields = result.multiReportsData.fieldsDropdown;
            this.MinimumMaxPerQuantity = result.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity;
            this.MaximumMaxPerQuantity = result.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity;
            this.XFields = result.reportsData.xFieldDropdown;
            this.YFields = result.reportsData.yFieldDropdown;
            this.setReportXandYFields();
            //  Maxper
            if (this.Maxper) {
                this.maxperdata = result.maxPerData.getCampaignLevelMaxPerData;
                if (this.Maxperdropdown) {
                    this.CampaignLevelMaxPerField = this.maxperdata.cMaxPerFieldOrderLevel;
                } else {
                    for (let index = 0; index < this.maxPerFieldsFastCountlevel.length; index++) {
                        let maxPerOrderLevel = this.maxPerFieldsFastCountlevel[index].value;
                        if (this.maxperdata.cMaxPerFieldOrderLevel == maxPerOrderLevel.cMaxPerFieldOrderLevel && this.maxperdata.cMaximumQuantity == maxPerOrderLevel.cMaximumQuantity) {
                            this.CampaignLevelMaxPerField = maxPerOrderLevel;
                        }
                    }
                }
            } else {
                this.CampaignLevelMaxPerField = result.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel.toUpperCase();
            }

            this.isLoading = false;

            if (this.CampaignLevelMaxPerField === '') {
                //  this.IsMaxFieldEmpty = true;
                this.MinimumMaxPerQuantity = '';
                this.MaximumMaxPerQuantity = '';
            }
            this.primengTableHelperMaxPer.records = result.maxPerData.getSegmentLevelMaxPerData;
            this.primengTableHelperDecoy.records = result.decoyData.listOfDecoys;
            this.isAuto = result.decoyData.isDecoyKeyMethod === 1 ? true : false;
            this.decoyKeyText = result.decoyData.decoyKey;
            this.decoyKey1Text = result.decoyData.decoyKey1;
            this.isDecoyRecords = result.decoyData.listOfDecoys.length <= 0;
            this.decoyGroupList = result.decoyData.listOfDecoyGroup;
            this.decoyKeyMethod = result.decoyData.isDecoyKeyMethod;
            this.decoyAddEditText = this.l('Add');
            this.isDecoyGroup = true;
            this.tempDecoyGroupByForEdit = result.decoyData.listOfGroupsForEdit;
            this.decoyGroupByForEdit = [];
            if (this.decoyGroupList.length > 0) {
                this.decoyGroupText = this.decoyGroupList[0].value;
            }

            this.billing.init(result.billingData);
            this.oessData.init(result.oessData);
            this.email = result.getOutputData.shipTo;
            this.outputShipToValues = result.getOutputData.shipToList;
            this.selectedOutputType = result.getOutputData.type.trim();
            this.prevSelectedOutputType = result.getOutputData.type.trim();
            this.totalOutputQuantity = result.getOutputData.totalOutputQuantity;
            this.outputTypes = result.getOutputData.typeList;
            if (this.outputTypes.length > 0) {
                this.selectedOutputType = this.outputTypes[0].value;
            }
            if (!this.Maxper) {
                this.getSelectedFields(null, this.selectedOutputLayout);
            }

        });
    }

    setReportXandYFields() {
        this.fastCountConfigXFields.push({ value: '', label: '' });
        this.fastCountConfigYFields.push({ value: '', label: '' });
        for (const key in this.fastCountConfig['selection-fields']) {
            const value = this.fastCountConfig['selection-fields'][key];
            this.XFields.filter(element => {
                if (element.value === value['xtab-report']) {
                    element.label = value['fc-description'];
                    this.fastCountConfigXFields.push(element);
                }
            });
            this.YFields.filter(element => {
                if (element.value === value['xtab-report']) {
                    element.label = value['fc-description'];
                    this.fastCountConfigYFields.push(element);
                }
            });
        }
        if (this.fastCountConfigXFields.length == 1) {
            this.fastCountConfigXFields = this.XFields;
        }
        if (this.fastCountConfigYFields.length == 1) {
            this.fastCountConfigYFields = this.YFields;
        }
    }

    show() {
        this.getCampaignForEdit();
        this.active = true;
        if (this.activeForm === 'PlaceOrder') {
            this.isEdit = true;
            this._segmentsServiceProxy.getSegmentForEdit(this.segmentId).subscribe(result => {
                this.segment = result;
                if (result.iRequiredQty === 0) {
                    this.requiredQuantityValue = this.allAvailableText;
                } else {
                    this.requiredQuantityValue = result.iRequiredQty.toString();
                }
            });
        }
    }

    searchMailers(event) {
        this._orderServiceProxy.getSearchResultForMailer(event.query, this.databaseId, this.divisionalDatabase)
            .subscribe(result => {
                this.mailers = result;
            });
    }

    save(form: NgForm, isSaveSegmentRequired: boolean): void {
        let isAdd = true;
        this.isLoading = true;
        let input = new CreateOrEditCampaignDto();
        let orderStatus = 10;
        input.listOfXTabRecords = [];
        input.listOfMultidimensionalRecords = [];
        input.billingData = CampaignBillingDto.fromJS(this.billing);
        input.maxPerData = new GetCampaignMaxPerForViewDto();
        input.maxPerData.getCampaignLevelMaxPerData = new CampaignLevelMaxPerDto();
        input.id = this.campaignId;

        input.maxPerData.getSegmentLevelMaxPerData = this.primengTableHelperMaxPer.records ? this.primengTableHelperMaxPer.records.filter(r => r.segmentLevelAction !== ActionType.None) : null;
        if (this.campaignId) {
            if (this.Maxper) {
                if (this.Maxperdropdown) {
                    input.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity = this.MaximumMaxPerQuantity && this.MaximumMaxPerQuantity !== '' ? this.MaximumMaxPerQuantity : 0;
                    input.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity = this.MinimumMaxPerQuantity && this.MinimumMaxPerQuantity !== '' ? this.MinimumMaxPerQuantity : 0;
                    input.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel = this.CampaignLevelMaxPerField;
                } else {
                    input.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity = this.MaximumMaxPerQuantity && this.MaximumMaxPerQuantity !== '' ? this.MaximumMaxPerQuantity : 0;
                    input.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity = this.MinimumMaxPerQuantity && this.MinimumMaxPerQuantity !== '' ? this.MinimumMaxPerQuantity : 0;
                    input.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel = this.CampaignLevelMaxPerFieldforFastcount;
                }
            } else {
                input.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity = this.MaximumMaxPerQuantity && this.MaximumMaxPerQuantity !== '' ? this.MaximumMaxPerQuantity : 0;
                input.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity = this.MinimumMaxPerQuantity && this.MinimumMaxPerQuantity !== '' ? this.MinimumMaxPerQuantity : 0;
                input.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel = this.CampaignLevelMaxPerField;
            }
        }
        input.maxPerData.campaignId = this.campaignId;
        input.decoyData = new GetDecoyForViewDto();
        input.decoyData.listOfDecoys = this.primengTableHelperDecoy.records;
        input.decoyData.decoyKey = this.decoyKeyText;
        input.decoyData.decoyKey1 = this.decoyKey1Text;
        input.decoyData.isDecoyKeyMethod = this.decoyKeyMethod;
        this.campaign.broker = this.fastCountConfig.broker;
        input.generalData = CampaignGeneralDto.fromJS(this.campaign);
        input.listOfXTabRecords = this.primengTableHelperDataReports.records ? this.primengTableHelperDataReports.records.filter(r => r.action !== ActionType.None) : null;
        if (input.listOfXTabRecords && input.listOfXTabRecords.length) {
            input.listOfXTabRecords.forEach(item => {
                if (!item.cXField && !item.cYField && item.id) {
                    item.action = ActionType.Delete;
                }
            })
        }
        input.listOfMultidimensionalRecords = this.primengTableHelperMultiDimension.records ? this.primengTableHelperMultiDimension.records.filter(r => r.action !== ActionType.None) : null;
        input.campaignOutputDto = this.campaignId !== undefined ? this.editCampaignOutput() : null;
        if (isSaveSegmentRequired) {
            this.SaveSegment();
        }

        if (this.activeForm === 'PlaceOrder' && this.layoutId != this.selectedOutputLayout) {
            this._orderServiceProxy.copyOrderExportLayout(this.campaignId, parseInt(this.selectedOutputLayout), this.appSession.idmsUser.idmsUserName, this.CampaignStatus)
                .subscribe(() => {
                    this.isLayoutDownloadEnabled = false;
                    this.camapaignSaved = true;
                });
        }
        this._orderServiceProxy.createOrEdit(input, orderStatus)
            .pipe(finalize(() => {
                this.saving = false;
            }))
            .subscribe(result => {
                if (this.activeForm === 'SaveCount') {
                    this._orderServiceProxy.updateFastCountMailerId(input.id).subscribe(result => {
                        this.isMailerIdUpdated = true;
                    });
                }
                if (this.active && this.activeForm === 'SaveCount') {
                    isAdd = !this.campaignId;
                    this.campaignSaved = true;
                    this.CampaignStatus = result.status;
                    if (this.campaignId === undefined) {

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
                        if (!this.isEditLayout) {
                            this.activeModal.close({ isSave: true, isAdd: isAdd, selectionDetails, isMailerIdUpdated: true });
                        }
                    } else {
                        this.editedDescription = result.campaignDescription;
                        if (!this.launchOESSFlag) {
                            this.oessData.databaseId = this.databaseId;
                            this.oessData.campaignDescription = this.cDescription;
                            this.oessData.iBillingQty = this.oessData.iBillingQty.toString() === '' ? 0 : this.oessData.iBillingQty;
                            this._orderServiceProxy.launchOESS(this.oessData).subscribe();
                            this.notify.info(this.l('SavedSuccessfully'));
                            if (!this.isEditLayout) {
                                this.close();
                            }
                        }
                    }
                    this.isLoading = false;
                }
                if (this.activeForm === 'CountReport') {
                    this.GetReport('XTAB', result.campaignId);
                }

                if (this.activeForm === 'Maxper') {
                    this.saving = true;
                    this.isLoading = false;
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                }
            }, () => {
                this.saving = false;
                this.isLoading = false;
            });
    }

    downloadDataReport(): void {
        this.isLoading = true;
        this._reportService.generateExcelReport(this.instantBreakdownData)
            .subscribe(result => {
                this._fileDownloadService.downloadFile(result);
                this.isLoading = false;
            }, () => { this.isLoading = false; });

    }

    GetReport(typeReport: string, campaignId: number): void {
        this._reportService.getInstantBreakDown(typeReport, this.segmentId, campaignId)
            .subscribe(result => {
                    this.showDownLoad = true;
                    this.isLoading = false;
                    this.instantBreakdownData = result;
                    const reportData = this.primengTableHelperDataReports.records.filter((item: GetCampaignXTabReportsListForView) => item.cXField || item.cYField);
                    const singleDimensionReport: GetCampaignXTabReportsListForView[] = reportData.filter((item: GetCampaignXTabReportsListForView) => (item.cXField && !item.cYField) || (!item.cXField && item.cYField));
                    const multiDimensionReport: GetCampaignXTabReportsListForView[] = reportData.filter((item: GetCampaignXTabReportsListForView) => (item.cXField && item.cYField));
                    let singleDimensionReportCount = 0;
                    let multiDimensionReportCount = 0;
                    let reportNo = 1;
                    this.reportDataToShowOnUI = [];
                    for (const key in result) {
                        const reportArray: any[] = result[key];
                        let rowOrCol;
                        let rowOrColHeader;

                        if (singleDimensionReport.length && singleDimensionReportCount < singleDimensionReport.length) {
                            let singleData = singleDimensionReport[singleDimensionReportCount];
                            rowOrCol = (singleData.cXField ? singleData.cXField : singleData.cYField).split('.')[1].toLowerCase();
                            rowOrColHeader = (singleData.cXField ? singleData.cXField : singleData.cYField).split('.')[1];
                        }
                        let row;
                        let rowHeader;
                        let col;
                        let columnHeader;
                        if (multiDimensionReport.length && multiDimensionReportCount < multiDimensionReport.length) {
                            let multiData = multiDimensionReport[multiDimensionReportCount];
                            row = multiData.cXField.split('.')[1].toLowerCase();
                            col = multiData.cYField.split('.')[1].toLowerCase();
                            rowHeader = multiData.cXField.split('.')[1];
                            columnHeader = multiData.cYField.split('.')[1];
                        }
                        const propertiesInLowerCase = Object.getOwnPropertyNames(reportArray[0]).map(property => property.toLowerCase());
                        if (propertiesInLowerCase.some(item => item == row) && propertiesInLowerCase.some(item => item == col)) {
                            const items: any[] = [];
                            const groupedValues = _.groupBy(reportArray, (item) => item[Object.keys(item).find(key => key.toLowerCase() == row)]);
                            for (const key in groupedValues) {
                                const values = groupedValues[key];
                                values.forEach(colValues => {
                                    items.push({
                                        label: key,
                                        colLabel: colValues[Object.keys(colValues).find(key => key.toLowerCase() == col)],
                                        value: (+colValues["count"]).toLocaleString(),
                                    });
                                });
                            }
                            this.reportDataToShowOnUI.push({
                                reportName: `Report ${reportNo}`,
                                description: this.getFieldDescription(rowHeader) + ' vs ' + this.getFieldDescription(columnHeader),
                                labelHeader: this.getFieldDescription(rowHeader),
                                columnHeader: this.getFieldDescription(columnHeader),
                                isSingleDimension: false,
                                rowCount: (+reportArray[0][Object.keys(reportArray[0]).find(key => key.toLowerCase() == "rowcount")]),
                                items
                            });
                            multiDimensionReportCount = multiDimensionReportCount + 1;
                        } else if (propertiesInLowerCase.some(item => item == rowOrCol)) {

                            this.reportDataToShowOnUI.push({
                                reportName: `Report ${reportNo}`,
                                description: this.getFieldDescription(rowOrColHeader),
                                labelHeader: this.getFieldDescription(rowOrColHeader),
                                isSingleDimension: true,
                                rowCount: (+reportArray[0][Object.keys(reportArray[0]).find(key => key.toLowerCase() == "rowcount")]),
                                items: reportArray.map(reportItem => {
                                    return {
                                        label: reportItem[Object.keys(reportItem).find(key => key.toLowerCase() == rowOrCol)],
                                        value: (+reportItem["count"]).toLocaleString(),
                                        rowCount: (+reportItem[Object.keys(reportItem).find(key => key.toLowerCase() == "rowcount")])
                                    };
                                })
                            });
                            singleDimensionReportCount = singleDimensionReportCount + 1;
                        }
                        reportNo = reportNo + 1;
                    }
                },
                () => { this.isLoading = false; });
    }


    SaveSegment() {
        let orderStatus = 10;
        let formControls: any;
        let saveSegment: CreateOrEditResult;
        if (this.requiredQuantityValue.toUpperCase() === this.allAvailableText.toUpperCase()) {
            this.segment.iRequiredQty = 0;
        } else {
            this.segment.iRequiredQty = parseInt(this.requiredQuantityValue);
        }

        this.segment.orderId = this.campaignId;
        this.segment.cMaxPerGroup = this.maxPerGroup;

        this.saving = true;
        this._segmentsServiceProxy.createOrEdit(this.segment, orderStatus)
            .pipe(finalize(() => {
                if (this.segment.id !== undefined) {
                    this.saving = false;
                }
            }))
            .subscribe((result: CreateOrEditSegmentOutputDto) => {
                saveSegment = { isEdit: this.isEdit, isSave: true, ...result };
                this.saving = true;
                this.notify.info(this.l('SavedSuccessfully'));
                if (!this.isEditLayout) {
                    this.activeModal.close(saveSegment);
                    this.notify.info(this.l('SavedSuccessfully'));
                }
            });
    }

    editCampaignOutput(): EditCampaignsOutputDto {

        let input = new EditCampaignsOutputDto();
        input.sort = this.selectedOutputSort;
        input.type = this.selectedOutputType;
        input.pgpKey = this.selectedPgpKey;
        input.isHeaderRow = this.headerRowCheck;
        input.isDataFileOnly = this.dataFileOnlyChecked;
        if (this.unzippedChecked == null) {
            input.isUnzipped = false;
        } else {
            input.isUnzipped = this.unzippedChecked;
        }
        input.shipTo = this.selectedOutputShipTo;
        input.shipCCEmail = this.shipToCC;
        input.shipSubject = this.shipToSubject;
        input.shipNotes = Constants.fastCountShippingNote;
        input.ftpSite = this.ftpSite;
        input.userName = this.userName;
        if (!isNaN(this.companyId)) {
            input.companyId = this.companyId;
        }

        if (this.activeForm === 'PlaceOrder') {
            if (!isNaN(parseInt(this.selectedOutputLayout))) {
                input.layout = this.selectedOutputLayout;
                input.layoutId = parseInt(this.selectedOutputLayout);
                for (let i = 0; i < this.outputLayouts.length; i++) {
                    if (this.outputLayouts[i].value === this.selectedOutputLayout) {
                        input.layoutDescription = this.outputLayouts[i].label.substring(this.outputLayouts[i].label.indexOf(':') + 1, this.outputLayouts[i].label.length).trim();
                        break;
                    } else {
                        input.layoutDescription = '';
                    }
                }
            }
        }

        input.media = this.selectedMedia;
        input.fileNotes = this.fileNotes;
        input.fileLabel = this.fileLabel;
        input.splitIntoNParts = this.selectedPart;
        input.splitType = this.selectedSplitOption;
        return input;
    }

    getEmailAddress() {
        let shipTo = this.selectedOutputShipTo;
        this._orderServiceProxy.getDetailsByCompanyIdAndOrderId(this.campaignId, parseInt(this.selectedOutputShipTo)).subscribe(
            (result: any) => {

                this.ftpSite = result.ftpSite;
                this.userName = result.userName;
                if (result.emailAddress === null || result.emailAddress === undefined) {
                    this.selectedOutputShipTo = shipTo;
                } else {
                    if (this.email === '') {
                        this.email = result.emailAddress;
                    } else {
                        if (result.emailAddress !== '') {
                            this.email += ';' + result.emailAddress;
                        }
                    }

                    this.selectedOutputShipTo = this.email;
                }
            });
    }

    shipToKeyDown(event): void {
        if (event.key === 'Enter') {
            if (!isNaN(this.selectedOutputShipTo as any)) {
                this.companyId = parseInt(this.selectedOutputShipTo);
                this.getEmailAddress();
            }
        }

    }

    onShipToDropdownChange(event): void {
        if (event.originalEvent) {
            if (!isNaN(this.selectedOutputShipTo as any) && this.selectedOutputShipTo.trim() !== '') {
                this.companyId = parseInt(this.selectedOutputShipTo);
                this.getEmailAddress();
            } else {
                this.email = event.value;
            }

        }
    }

    onTypeDropdownChange(skip: boolean, dropdown: Dropdown): void {
        if (this.selectedOutputType === 'FF') {
            this.headerRowCheck = true;
            this.headerRowDisabled = true;
            this.prevSelectedOutputType = this.selectedOutputType;
        } else if (this.selectedOutputType === 'EE') {
            if (this.CampaignStatus === CampaignStatus.CampaignCompleted ||
                this.CampaignStatus > CampaignStatus.CampaignFailed) {
                if (this.totalOutputQuantity > 1000000 && !skip) {
                    this.message.error(this.l('ExportFileFormatError'));
                    this.selectedOutputType = this.prevSelectedOutputType;
                    dropdown.selectedOption = dropdown.options.find(x => x.value === this.selectedOutputType);
                } else {
                    this.headerRowCheck = true;
                    this.headerRowDisabled = true;
                    this.prevSelectedOutputType = this.selectedOutputType;
                }
            } else {
                this.headerRowCheck = true;
                this.headerRowDisabled = true;
                this.prevSelectedOutputType = this.selectedOutputType;
            }
        } else {
            this.headerRowCheck = true;
            this.headerRowDisabled = true;
            this.prevSelectedOutputType = this.selectedOutputType;
        }
    }

    getOutputLayoutFloatStyle() {
        return 'right';
    }

    DownloadExcel(): void {

        this._orderServiceProxy.downloadOutputLayoutTemplateExcelTest(this.campaignId).subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
        });

    }

    onRequiredQuantityKeyDown(event) {
        if (event.key === '.') {
            return false;
        } else {
            return true;
        }

    }

    onLayoutDropdownChange(): void {
        let userName = this.appSession.idmsUser.idmsUserName;
        this.loadingLayoutSelections = true;
        if (this.CampaignStatus >= 40) {
            if (this.CampaignStatus === 50) {
                this.CampaignStatus = 10;
            } else {
                this.CampaignStatus = 40;
            }
        } else {
            this.CampaignStatus = 10;
        }
        this._exportLayoutServiceProxy.getExportLayoutSelectedFields(undefined,false, Number(this.selectedOutputLayout), this.maintainanceBuildId).subscribe(           
            result => {
                this.layoutList = result;
                this.primengTableHelper.records = result;
                this.primengTableHelper.totalRecordsCount = result.length;
                this.maxOrder = result.length;
                this.loadingLayoutSelections = false;
            }, error => {
                this.loadingLayoutSelections = false;
            });
    }

    getLayoutSelectedFields(event?: LazyLoadEvent, layoutID?): void {
        this.loadingLayoutSelections = true;
        if (event === null && layoutID) {
            this.OutputLayoutId = layoutID;
        }
        this._exportLayoutServiceProxy.getExportLayoutSelectedFields(this.campaignId, this.isCampaign, this.OutputLayoutId, this.selectedBuild).subscribe(
            result => {
                this.loadingLayoutSelections = false;
                this.layoutList = result;
                let length = this.layoutList.length / 2;
                this.list1 = this.layoutList.slice(0, length);
                this.list2 = this.layoutList.slice(length, this.layoutList.length);
            },
            error => {
                this.loadingLayoutSelections = false;
            }
        );
        if (this.selectedAvailableFields == '' || this.selectedAvailableFields == undefined) {
            this.isAdd = true;
        } else {
            this.isAdd = false;
        }
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }

    getMaxPerGroups(): void {
        this._segmentsServiceProxy.getMaxPerGroups(this.campaignId, this.segmentId).subscribe(
            (result: any) => {

                this.dbArray = result.segmentDropDown;

                for (let entry of this.dbArray) {

                    this.maxPerDrop.push({ label: entry.label, value: entry.value });
                }
                if (!this.segmentId) {
                    this.maxPerGroup = result.defaultMaxPerValue;
                } else {
                    this.maxPerGroup = this.segment.cMaxPerGroup;
                }
                this.CampaignStatus = result.currentStatus;
                this.currentCampaignStatus = this._campaignUiHelperService.getStatusDescription(result.currentStatus);
                this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(CampaignAction.SaveSelection, result.currentStatus);
                if (result.currentStatus === CampaignStatus.CampaignCompleted && this.selectedSplitOption !== undefined && this.selectedSplitOption !== 4) {
                    this.outputQuantityDisabled = false;
                }
                if (result.currentStatus === CampaignStatus.OutputCompleted || result.currentStatus === CampaignStatus.OutputFailed) {
                    this.outputQuantityDisabled = false;
                }
                if (result.currentStatus === CampaignStatus.CampaignCompleted && this.segment.iOutputQty !== undefined && this.segment.iOutputQty === null) {
                    this.segment.iOutputQty = 0;
                }
            });
    }

    downloadXtabReport(campaignId: any, event: any): void {
        event.preventDefault();
        event.stopPropagation();
        if (this.CampaignStatus !== CampaignStatus.CampaignCreated &&
            this.CampaignStatus !== CampaignStatus.CampaignSubmitted &&
            this.CampaignStatus !== CampaignStatus.CampaignRunning &&
            this.CampaignStatus !== CampaignStatus.CampaignFailed) {
            this._orderServiceProxy.downloadXtabReport(campaignId).subscribe(result => {
                this._fileDownloadService.downloadDocumentAttachment(result);
            });
        }
    }

    //Change Events For XtabReports
    onXFieldChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperDataReports.records[relativeIndex];
        record.cXDesc = this.fastCountConfigXFields.find(x => x.value === event.value).label;
        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    onYFieldChange(relativeIndex: number, event: any): void {

        let record = this.primengTableHelperDataReports.records[relativeIndex];
        record.cYDesc = this.fastCountConfigYFields.find(x => x.value === event.value).label;
        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else if (record.id === 0 && (record.cYDesc !== '' || record.cYDesc != null)) {
            record.action = ActionType.Add;
        }
    }

    onLevelChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperDataReports.records[relativeIndex];
        record.isXTab = this.Levels.find(x => x.value === event.value).label;

        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    onXTabSegmentNumberChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperDataReports.records[relativeIndex];
        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    onMultiSegmentNumberChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    onMultiLevelChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        record.isMulti = this.Levels.find(x => x.value === event.value).label;

        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    onMultiFieldChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        let fieldDescription = '';
        for (let i = 0; i < event.value.length; i++) {
            fieldDescription += this.multiColumnFields.find(x => x.value === event.value[i]).label + ',';
        }
        record.cFieldName = event.value.join(',');
        record.cFieldDescription = fieldDescription.replace(/,\s*$/, '');
        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    onTypeChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperDataReports.records[relativeIndex];
        record.cTypeName = this.Types.find(x => x.value === event.value).label;
        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    onMultiTypeChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMultiDimension.records[relativeIndex];
        record.cTypeName = this.Types.find(x => x.value === event.value).label;
        if (record.id !== 0) {
            record.action = ActionType.Edit;
        } else {
            record.action = ActionType.Add;
        }
    }

    //onMaxPerFieldSegmentLevel Change
    onMaxPerFieldSegmentLevelChange(relativeIndex: number, event: any): void {
        let record = this.primengTableHelperMaxPer.records[relativeIndex];
        record.cMaxPerFieldDescription = this.MaxPerFieldsSegmentlevel.find(x => x.value === event.value).label;
        record.segmentLevelAction = ActionType.Edit;
    }

    //onMaxPerQuantityChange
    onMaxPerTextQuantityChange(absoluteIndex: number) {
        let record = this.primengTableHelperMaxPer.records[absoluteIndex];
        record.segmentLevelAction = ActionType.Edit;
    }

    // //onMaxPerFieldCampaignLevel Change
    onMaxPerFieldCampaignLevelChange(event: any): void {
        if (event.value === '') {
            this.MinimumMaxPerQuantity = '';
            this.MaximumMaxPerQuantity = '';
            this.IsMaxFieldEmpty = true;
        } else {
            this.IsMaxFieldEmpty = false;
        }
    }

    //Delete Xtab
    deleteXtab(relativeIndex: number): void {

        this.message.confirm(
            this.l(''),
            (isConfirmed) => {
                if (isConfirmed) {
                    let record = this.primengTableHelperDataReports.records[relativeIndex];
                    record.action = ActionType.Delete;
                    this.isDelete = true;
                }
            }
        );
    }

    selectNext(tabNo: number) {
        this.activeNav = tabNo;
        this.editFlag = false;
    }

    editLayout() {
        this.isEditLayout = true;
        this.save(null, true);
        this._exportLayoutServiceProxy.getBuildsByDatabase(this.databaseId).subscribe(result => {
            this.buildsList = result;
            this.selectedBuild = this.buildsList[0].value;
            this.maintainanceBuildId = this.selectedBuild;
            this._exportLayoutServiceProxy.getTableDescriptionDropDownValues(this.campaignId, this.maintainanceBuildId, this.isCampaign, this.databaseId).subscribe(result => {
                this.tableDropDownValues = result;
                this.gridTableDropDownValues = result;
                this.selectedTableValue = this.tableDropDownValues.find(x => x.label.includes('(tblMain)')).value;
                this._exportLayoutServiceProxy.getExportLayoutAddField(this.selectedTableValue, this.campaignId, true, parseInt(this.selectedOutputLayout),
                    this.maintainanceBuildId, this.databaseId).subscribe(result => { this.availableFields = result; });
            });
        });

    }

    onAddFieldChange(event) {

        if (event.value.length == 0) {
            this.isAdd = true;
        } else {
            this.isAdd = false;
        }
    }
    AddFields(table: Table) {
        let selectedAvialableFields = '';
        for (let x = 0; x < this.selectedAvailableFields.length; x++) {
            for (let y = 0; y < this.availableFields.length; y++) {
                if (this.availableFields[y].value == this.selectedAvailableFields[x]) {

                    selectedAvialableFields += this.availableFields[y].label + ',';
                    if (!this.checkForFields(this.selectedAvailableFields[x])) {
                        this.availableFields.splice(y, 1);
                    }
                }
            }
        }
        selectedAvialableFields = selectedAvialableFields.slice(0, -1);
        this.selectedAvailableFields = '';
        this.isAdd = true;
        this._exportLayoutServiceProxy.addNewSelectedFields(selectedAvialableFields, this.selectedTableValue, this.campaignId, this.maintainanceBuildId, this.isCampaign, this.layoutID, this.DbId, this.CampaignStatus).subscribe(
            result => {

                for (let x = 0; x < result.length; x++) {
                    this.primengTableHelper.records.push(result[x]);
                }
                this.maxOrder = this.primengTableHelper.records.reduce((max, b) => Math.max(max, b.order), this.primengTableHelper.records[0].order);
                this.getSelectedFields(this.campaignId, table);

            }
        );
    }

    getSelectedFields(event, table) {

        this._exportLayoutServiceProxy.getExportLayoutSelectedFields(this.campaignId, this.isCampaign, this.layoutID, this.maintainanceBuildId).subscribe(
            result => {
                this.layoutList = result;
                this.primengTableHelper.records = result;
                this.primengTableHelper.totalRecordsCount = result.length;
                this.maxOrder = result.length;
            }
        );
    }

    checkForFields(selectedField: string): boolean {

        switch (selectedField.toUpperCase()) {
            case '': return true;
            case 'TBLSEGMENT.IDMSNUMBER':
            case 'TBLSEGMENT.CKEYCODE1':
            case 'TBLSEGMENT.CKEYCODE2':
            case 'TBLSEGMENT.DISTANCE':
            case 'TBLSEGMENT.SPECIALSIC':
            case 'TBLSEGMENT.SICDESCRIPTION': return true;
            default: return false;
        }
    }

    deleteSelectedItem(fieldId, table: Table) {
        console.log(fieldId);
        let recordIds: number[] = [];
        recordIds.push(fieldId);
        this._exportLayoutServiceProxy.deleteExportLayoutRecord(recordIds, this.campaignId, this.isCampaign, this.exportLayoutId, this.currentStatus).subscribe(any => {
            this.getSelectedFields(null, null);
            this._exportLayoutServiceProxy.getExportLayoutAddField(this.selectedTableValue, this.campaignId, this.isCampaign, this.exportLayoutId, this.maintainanceBuildId, this.databaseId).subscribe(
                result => {
                    this.availableFields = result;
                });
            this.notify.info(this.l('Deleted Successfully'));
        });

        this.getSelectedFields(event, table);
    }

    GetMaxPerOrderConfigValuesforDropdown() {
        if (this.fastCountConfig['max-per'] && this.fastCountConfig['max-per']['options']) {
            let fastCountFieldsList = this.fastCountConfig['max-per']['options'];

            for (let index = 0; index < fastCountFieldsList.length; index++) {
                this.maxPerFieldsFastCountlevel[index] = { label: fastCountFieldsList[index].name, value: fastCountFieldsList[index].value };
            }
            this.Maxperdropdown = false;
        } else {
            this._orderServiceProxy.getMaxPerFieldDropdownData(this.databaseId,
                this.buildId,
                '').subscribe(result => {
                this.maxPerFieldsFastCountlevel.push({
                    'label': '',
                    'value': ''
                });
                for (let index = 0; index < result.length; index++) {
                    this.maxPerFieldsFastCountlevel.push({label: result[index].label, value: result[index].value});
                }
                this.Maxperdropdown = true;
            });
        }
    }
    onMaxPerFieldFastCountLevelChange(event: any): void {
        if (event.value == '') {
            this.MinimumMaxPerQuantity = '';
            this.MaximumMaxPerQuantity = '';
            this.CampaignLevelMaxPerFieldforFastcount = '';
        } else {
            this.MinimumMaxPerQuantity = event.value.cMinimumQuantity;
            this.MaximumMaxPerQuantity = event.value.cMaximumQuantity;
            this.CampaignLevelMaxPerFieldforFastcount = event.value.cMaxPerFieldOrderLevel;
        }
    }
    goBackToLayout(layoutflag: boolean) {
        this.isEditLayout = layoutflag;
    }
    getFieldDescription(selection: string): string {
        const field = this.fastCountConfig['selection-fields'][selection];
        return field ? field['fc-description'] : selection;
    }
}
