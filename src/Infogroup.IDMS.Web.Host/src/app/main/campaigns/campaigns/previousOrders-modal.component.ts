import { Component, Injector, Input, OnInit, ViewChild, EventEmitter, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { LazyLoadEvent, SelectItem } from 'primeng/api';
import { IdmsUserLoginInfoDto, SegmentPrevOrdersesServiceProxy, ActionType, SegmentPrevOrdersDto, CreateOrEditSegmentPrevOrdersDto, ShortSearchServiceProxy, PageID } from '@shared/service-proxies/service-proxies';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { NotifyService } from 'abp-ng2-module/dist/src/notify/notify.service';
import { CampaignUiHelperService } from '../shared/campaign-ui-helper.service';
import { CampaignAction } from '../shared/campaign-action.enum';
import { finalize } from 'rxjs/operators';


@Component({
    selector: 'previousOrdersModal',
    styleUrls: ['previousOrders-modal.component.css'],
    templateUrl: './previousOrders-modal.component.html'
})
export class PreviousOrdersModalComponent extends AppComponentBase implements OnInit {

    active: boolean = false;
    campaignStatus: number;
    @Input() segment: any;
    @Input() buildId: any;
    @Input() segmentNo: string;
    @Input() campaignLevel: boolean;
    segmentList: any[] = [];
    addRows: any[] = [];
    notify: NotifyService;
    segmentPrevOrderDto: SegmentPrevOrdersDto[] = [];
    segPrevOrder: SegmentPrevOrdersDto = new SegmentPrevOrdersDto();
    createEditDto: CreateOrEditSegmentPrevOrdersDto = new CreateOrEditSegmentPrevOrdersDto();
    campaignId: number;
    segmentId: number;
    saving: boolean = false;
    idmsUserData: IdmsUserLoginInfoDto;
    advancedFiltersAreShown = false;   
    campaignDescription: string = '';
    segmentDescription: string = '';
    filterText: string = '';
    saveDisabled: boolean = false;
    isSelectedListLoading: boolean = false;
    inclusionexclusionValues: SelectItem[] = [{ label: 'Include', value: 'Include' }, { label: 'Exclude', value: 'Exclude' }];
    individualCompanyValues: SelectItem[] = [{ label: 'Individual', value: 'Individual' }, { label: 'Company', value: 'Company' }];
    selectedSourcesLength: number = 0;
    isSave: boolean = false;
    isDeleteHidden: boolean = false;
    expandedRows = [];
    pageTitle: string = "";

    deleteOrders: any[] = [];
    matchFields: any[] = [];
    pageId: PageID = PageID.CampaignHistory;
    
    helpData: any = {
        header: "Search Options:",
        examples: []
    };
    isHelpDisabled: boolean = true;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @Output() editCampaignFromSegment = new EventEmitter<any>();

    pTogglerChecked: boolean = false;

    constructor(
        injector: Injector,

        private _segmentPrevOrdersesServiceProxy: SegmentPrevOrdersesServiceProxy,
        private _campaignUiHelperService: CampaignUiHelperService,
        private activeModal: NgbActiveModal,
        private _shortSearchServiceProxy: ShortSearchServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {

        this.active = true;
        this.idmsUserData = this.appSession.idmsUser;
        this.isSave = false;
        this.isDeleteHidden = false;        
        this.getExistingPreviousOrders();
        this.addRows = [];
        this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
            .subscribe(result => {
                this.helpData = result;
                this.isHelpDisabled = false;
            });
        this.pageTitle = this.campaignLevel
            ? this.l("PromotionHistory")
            : `${this.l("Segment")} ${this.segmentNo} ${this.l(
                "History"
            )}`;
        //this.getExistingPreviousOrders();
    }

    getExistingPreviousOrders(event?: LazyLoadEvent) {
        
        this._segmentPrevOrdersesServiceProxy.getExistingPreviousOrders(

            this.campaignId,
            this.segmentId            


        ).subscribe(result => {
            this.primengTableHelper.records = result.listOfSegmentPrevOrders;
            this.primengTableHelper.totalRecordsCount = result.listOfSegmentPrevOrders.length;  
            this.saveDisabled = !(this._campaignUiHelperService.shouldActionBeEnabled(CampaignAction.SaveSelection, result.currentStatus)
                && this.permission.isGranted('Pages.SegmentPrevOrderses.Edit'));
            this.primengTableHelper.hideLoadingIndicator();
        });
    }


    searchResults(isFromSearch: boolean, event?: LazyLoadEvent): void {
        this.isDeleteHidden = true;
        this.primengTableHelper.showLoadingIndicator();
        this.expandedRows = [];
        this._segmentPrevOrdersesServiceProxy.getAllPrevOrdersList(
            this.campaignId,
            this.segmentId,
            this.filterText.trim(),
            isFromSearch,
            this.primengTableHelper.getSorting(this.dataTable),
            0,
            1
        ).subscribe(result => {
            this.primengTableHelper.records = result;
            this.primengTableHelper.totalRecordsCount = result.length;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    openSegments(record) {

        this.editCampaignFromSegment.emit({ campaignId: record.campaignId, segmentId: parseInt(record.segmentId), campaignDescription: record.campaignDescription });
    }

    addSegmentRecordInList(record) {
        if (record.segment != undefined) {
            if (this.segmentList.indexOf(record) == -1) {
                this.segmentList.push(record);
                for (let i = 0; i < this.primengTableHelper.records.length; i++) {
                    if (this.primengTableHelper.records[i].orderID == record.segment.orderId) {
                        if (this.addRows.indexOf(this.primengTableHelper.records[i]) == -1)
                            this.addRows = this.addRows.concat(this.primengTableHelper.records[i]);
                    }
                }
            }
        }

    }
    removeSegmentRecordInList(record) {
        if (record.segment != undefined) {
            for (let i = 0; i < this.segmentList.length; i++) {
                if (this.segmentList[i].segment.description === record.segment.description) {
                    this.segmentList.splice(i, 1);
                }
            }
        }
    }

    changeIncludeExclude(IsIncludeExclude: number, index, record) {

        if (IsIncludeExclude == 0) {
            this.primengTableHelper.records[index].cIncludeOrExclude = "Exclude";

        }
        else {
            this.primengTableHelper.records[index].cIncludeOrExclude = "Include";

        }
        if (this.primengTableHelper.records[index].previousOrderID != 0) {
            if (this.addRows.indexOf(record) == -1) {
                this.addRows = this.addRows.concat(record);
            }
            else {

                for (let i = 0; i < this.addRows.length; i++) {
                    if (this.addRows[i].orderID == record.orderID) {
                        this.addRows[i] = record;
                    }
                }
            }
            this.pTogglerChecked = false;
            this.primengTableHelper.records[index].action = ActionType.Edit;
        }
    }
    changeIndividualCompany(IsIndividualCompany: number, index: number, record) {

        if (IsIndividualCompany == 0) {
            this.primengTableHelper.records[index].cIndividualOrCompany = "Company";

        }
        else {
            this.primengTableHelper.records[index].cIndividualOrCompany = "Individual";

        }
        if (this.primengTableHelper.records[index].previousOrderID != 0) {
            if (this.addRows.indexOf(record) == -1) {
                this.addRows = this.addRows.concat(record);
            }

            else {

                for (let i = 0; i < this.addRows.length; i++) {
                    if (this.addRows[i].orderID == record.orderID) {
                        this.addRows[i] = record;
                    }
                }
            }
            this.pTogglerChecked = false;
            this.primengTableHelper.records[index].action = ActionType.Edit;
        }
    }
    deletePrevOrder() {
        if (this.addRows.length > 0) {            
            this.message.confirm(
                "",
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        for (let i = 0; i < this.addRows.length; i++) {
                            this.addRows[i].action = ActionType.Delete;
                            this.deleteOrders = this.deleteOrders.concat(this.addRows[i]);
                        }
                    }  
                }
            );
        }

    }
    close(): void {
        this.segmentList = [];
        this.activeModal.close({ isSave: this.isSave });
        this.active = false;
        this.isDeleteHidden = false;
        this.expandedRows = [];
        this.saving = false;

    }
    save(): void {
        var rows = this.addRows;
        this.expandedRows = [];
        if (this.deleteOrders.length > 0) {
            for (let i = 0; i < this.deleteOrders.length; i++) {
                rows.push(this.deleteOrders[i]);
            }
        }
        this.segmentPrevOrderDto = [];
        for (let i = 0; i < rows.length; i++) {
            this.segPrevOrder = new SegmentPrevOrdersDto();
            this.segPrevOrder.id = rows[i].previousOrderID;
            this.segPrevOrder.prevOrderID = rows[i].orderID;
            this.segPrevOrder.segmentId = this.segmentId;
            this.segPrevOrder.cPrevSegmentID = this.segmentList.filter(x => x.segment.orderId == rows[i].orderID).map(x => x.segment.id).join(',').toString();
            this.segPrevOrder.cPrevSegmentNumber = this.segmentList.filter(x => x.segment.orderId == rows[i].orderID).map(x => x.segment.iDedupeOrderSpecified).join(',').toString();
            this.segPrevOrder.cIncludeExclude = rows[i].cIncludeOrExclude;
            this.segPrevOrder.cCompanyFieldName = rows[i].cIndividualOrCompany;
            this.segPrevOrder.cMatchFieldName = rows[i].cMatchFieldName;

            if (rows[i].previousOrderID == 0 &&
                rows[i].action != ActionType.Delete) {
                this.segPrevOrder.action = ActionType.Add;
            }
            else {
                this.segPrevOrder.action = rows[i].action;
            }
            this.segmentPrevOrderDto.push(this.segPrevOrder);
        }
        this.createEditDto.campaignID = this.campaignId;
        this.createEditDto.listOfSegmentOrders = this.segmentPrevOrderDto;
        this.saving = true;
        this._segmentPrevOrdersesServiceProxy.savePreviousOrders(this.createEditDto)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(result => {
            this.getExistingPreviousOrders();
            if (this.segmentPrevOrderDto.length > 0) {                
                this.isSave = true;
                this.saving = false;
                this.notify.info(this.l('SavedSuccessfully'));                
            } 

        });
        this.addRows = [];
        this.deleteOrders = [];
        this.isDeleteHidden = false;
    }

    setPToggler(record) {

        this.pTogglerChecked = true;
    }
    clearFilters() {
        this.filterText = "";
        $("#togBtn").prop("checked", true);
        this.searchResults(true);
    }
    
}

