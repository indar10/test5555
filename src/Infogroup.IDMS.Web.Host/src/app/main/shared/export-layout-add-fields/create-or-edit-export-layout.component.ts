import { Component, Injector, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ExportLayoutsServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { NgbActiveModal,NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';
import { SelectItem } from 'primeng/api';
import { UploadLayoutModalComponent } from './upload-layout-modal/upload-layout-modal.component';
declare var $: any;

@Component({
    selector: 'CreateOrEditExportLayout',
    templateUrl: './create-or-edit-export-layout.component.html',
    styleUrls: ['./create-or-edit-export-layout.component.css'],
    animations: [appModuleAnimation()]
})

export class CreateOrEditExportLayoutComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;   
    layoutDescription: string = "";
    active: boolean = true;;
    selectedTableValue: any;
    selectedOutputCase: any;
    availableFields: any;
    selectedAvailableFields: any;
    isDescription: any;
    isDisplayOrder: any;
    isAdd: boolean = true;
    title: string = "";
    tableDropDownValues: SelectItem[] = [];
    gridTableDropDownValues: SelectItem[] = [];
    gridTbaleDropdownValues
    oututCaseValues: SelectItem[] = [];
    isDelete: boolean = false;
    @Input() maintainanceBuildId: number = 0;
    @Input() isCampaign: boolean = false;
    @Input() databaseId: number = 0;
    @Input() exportLayoutId: number = 0;
    @Input() layoutName: string = "";
    @Input() currentStatus: number = 0;
    maxOrder: number;
    @Input() campaignId: number;
    advancedFiltersAreShown = false;
    addRows: any[] = [];
    @Output() editExportLayout = new EventEmitter<any>();
    @Output() closeExportLayout = new EventEmitter<any>();    

    constructor(
        injector: Injector,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
        public activeModal: NgbActiveModal,
        private modalService: NgbModal

    ) {
        super(injector);
    }
    ngOnInit() {
        if (this.isCampaign) {
            this._exportLayoutServiceProxy.getCampaignRecordForCampaignId(this.campaignId, this.databaseId).subscribe(result => {

                this.layoutDescription = "Edit " + result.cExportLayout;
                this.selectedOutputCase = result.cOutputCase;
            }
            );
        }
        else {
            this.layoutDescription = "Layout :" + this.layoutName;
        }
        this._exportLayoutServiceProxy.getTableDescriptionDropDownValues(this.campaignId, this.maintainanceBuildId, this.isCampaign, this.databaseId).subscribe(result => {
            this.tableDropDownValues = result;
            this.gridTableDropDownValues = result;
            this.selectedTableValue = this.tableDropDownValues.find(x => x.label.includes("(tblMain)")).value;         
            this._exportLayoutServiceProxy.getExportLayoutAddField(this.selectedTableValue, this.campaignId, this.isCampaign, this.exportLayoutId, this.maintainanceBuildId, this.databaseId).subscribe(result => { this.availableFields = result; });
        });

        this._exportLayoutServiceProxy.getOutputCaseDropDownValues().subscribe(result => {
            this.oututCaseValues = result;

        });
        this._exportLayoutServiceProxy.getExportLayoutSelectedFields(this.campaignId, this.isCampaign, this.exportLayoutId, this.maintainanceBuildId).subscribe(
            result => {
                this.primengTableHelper.records = result;
                this.primengTableHelper.totalRecordsCount = result.length;
                this.isDeleteEnabled();
                if (result.length > 0) {
                    this.maxOrder = result.reduce((max, b) => Math.max(max, b.order), result[0].order);
                }
                else {

                    this.maxOrder = 0;
                }
            }
        );
        if (this.selectedAvailableFields == "" || this.selectedAvailableFields == undefined) {
            this.isAdd = true;
        }
        else {
            this.isAdd = false;
        }
    }



    scroll(table: Table) {
        let body = table.containerViewChild.nativeElement.getElementsByClassName("ui-table-scrollable-body")[0];
        body.scrollTop = body.scrollHeight;
    }

    onAddFieldChange(event) {

        if (event.value.length == 0) {
            this.isAdd = true;
        }
        else {
            this.isAdd = false;
        }
    }

    onGridCheckBoxChange() {
        this.isDeleteEnabled();
    }

    isDeleteEnabled() {
        if (this.primengTableHelper.totalRecordsCount == 0 || this.addRows.length == 0) {
            this.isDelete = true;
        }
        else
            this.isDelete = false;
    }

    editSelectedItem(relativeIndex: number): void {
        let record = this.primengTableHelper.records[relativeIndex];
        record.fieldName = "";
        record.iIsCalculatedField = true;
        record.isFormulaEnabled = true;
        record.formula = "SPACE(" + record.width + ")";
        this._exportLayoutServiceProxy.updateExportLayoutRecords(record, this.isCampaign, this.campaignId, this.currentStatus, this.maintainanceBuildId, this.databaseId, record.tableId).subscribe(any => { this._exportLayoutServiceProxy.getExportLayoutAddField(this.selectedTableValue, this.campaignId, this.isCampaign, this.exportLayoutId, this.maintainanceBuildId, this.databaseId).subscribe(result => { this.availableFields = result; }); });

    }

    reorderOrderIdOnEnterKey(event, id: number, orderId: number, flag, dirty) {
        if (event.key == "Enter") {
            this.reorderOrderId(id, orderId, flag, dirty);
        }
    }

    reorderOrderId(id: number, orderId: number, flag, dirty): void {


        if (flag && dirty) {
            this._exportLayoutServiceProxy.reorderExportLayoutOrderId(id, orderId, this.campaignId, this.isCampaign).subscribe(result => { this.getSelectedFields(null, null); });

        }
    }

    deleteSelectedItem(): void {
        var recordIds: number[] = [];
        if (this.addRows.length > 0 && this.primengTableHelper.records.length > 0)
            this.message.confirm(
                this.l(''),
                (isConfirmed) => {
                    if (isConfirmed) {
                        for (let i = 0; i < this.addRows.length; i++) {
                            recordIds.push(this.addRows[i].id);
                        }
                        this._exportLayoutServiceProxy.deleteExportLayoutRecord(recordIds, this.campaignId, this.isCampaign, this.exportLayoutId, this.currentStatus).subscribe(any => {
                            this.getSelectedFields(null, null);
                            this._exportLayoutServiceProxy.getExportLayoutAddField(this.selectedTableValue, this.campaignId, this.isCampaign, this.exportLayoutId, this.maintainanceBuildId, this.databaseId).subscribe(
                                result => {
                                    this.availableFields = result;
                                });
                            this.notify.info(this.l('Deleted Successfully'));
                        });

                        this.getSelectedFields(null, null);
                    }
                }
            );
    }

    saveOutputCaseLayout() {
        this._exportLayoutServiceProxy.saveLayoutOutputCase(this.campaignId, this.selectedOutputCase, this.currentStatus).subscribe();
    }

    updateOrderExportLayoutForTableDescription(record, flag, controlFlag) {
        if (flag && controlFlag) {
            this._exportLayoutServiceProxy.updateExportLayoutRecords(record, this.isCampaign, this.campaignId, this.currentStatus, this.maintainanceBuildId, this.databaseId, record.tableId).subscribe(any => { this.getSelectedFields(null,null) });
        }
    }

    updateOrderExportLayoutTableFields(record, flag, controlFlag) {

        if (flag && controlFlag) {
            
            this._exportLayoutServiceProxy.updateExportLayoutRecords(record, this.isCampaign, this.campaignId, this.currentStatus, this.maintainanceBuildId, this.databaseId, record.tableId).subscribe();
            
        }
        
    }
    checkForFields(selectedField: string): boolean {

        switch (selectedField.toUpperCase()) {
            case "": return true;
            case "TBLSEGMENT.IDMSNUMBER":
            case "TBLSEGMENT.CKEYCODE1":
            case "TBLSEGMENT.CKEYCODE2":
            case "TBLSEGMENT.DISTANCE":
            case "TBLSEGMENT.SPECIALSIC":
            case "TBLSEGMENT.SICDESCRIPTION": return true;
            default: return false;
        }
    }

    AddFields(table: Table) {
        var selectedAvialableFields = "";
        for (let x = 0; x < this.selectedAvailableFields.length; x++) {
            for (let y = 0; y < this.availableFields.length; y++) {
                if (this.availableFields[y].value == this.selectedAvailableFields[x]) {

                    selectedAvialableFields += this.availableFields[y].label + ",";
                    if (!this.checkForFields(this.selectedAvailableFields[x]))
                        this.availableFields.splice(y, 1);
                }
            }
        }
        selectedAvialableFields = selectedAvialableFields.slice(0, -1);
        this.selectedAvailableFields = "";
        this.isAdd = true;
        this._exportLayoutServiceProxy.addNewSelectedFields(selectedAvialableFields, this.selectedTableValue, this.campaignId, this.maintainanceBuildId, this.isCampaign, this.exportLayoutId, this.databaseId, this.currentStatus).subscribe(
            result => {

                for (let x = 0; x < result.length; x++) {
                    this.primengTableHelper.records.push(result[x]);
                }
                this.maxOrder = this.primengTableHelper.records.reduce((max, b) => Math.max(max, b.order), this.primengTableHelper.records[0].order);
                this.getSelectedFields(this.campaignId, table);

            }
        )
    }

    getSelectedFields(event, table) {

        this._exportLayoutServiceProxy.getExportLayoutSelectedFields(this.campaignId, this.isCampaign, this.exportLayoutId, this.maintainanceBuildId).subscribe(
            result => {
                this.primengTableHelper.records = result;
                this.primengTableHelper.totalRecordsCount = result.length;
                this.maxOrder = result.length;
                this.isDeleteEnabled();
                this.scroll(table);
            }
        );
    }

    setFieldsDropDown(event) {
        this.selectedAvailableFields = null;
        this._exportLayoutServiceProxy.getExportLayoutAddField(this.selectedTableValue, this.campaignId, this.isCampaign, this.exportLayoutId, this.maintainanceBuildId, this.databaseId).subscribe(result => { this.availableFields = result; });

    }

    close(): void {
        this.closeExportLayout.emit();
    }

    closeModal(): void {
        this.active = false;
        this.activeModal.close();
    }
    
    importLayout(){
        this._exportLayoutServiceProxy.getFieldsCounts(this.exportLayoutId).subscribe(
            ()=>{
                const modalRef: NgbModalRef = this.modalService.open(
                    UploadLayoutModalComponent,
                    {
                        backdrop: ModalDefaults.Backdrop,
                        windowClass: ModalDefaults.WindowClass
                    }
                );
                modalRef.componentInstance.exportLayoutId = this.exportLayoutId;
                modalRef.componentInstance.databaseId = this.databaseId;
                modalRef.componentInstance.buildId =  this.maintainanceBuildId;
                modalRef.componentInstance.campaignId = this.campaignId;
                modalRef.componentInstance.isCampaign = this.isCampaign;
                modalRef.componentInstance.databaseId = this.databaseId;              
                modalRef.result.then(params => {
                    if(params.isSave){
                        this.getSelectedFields(null, null);
                    }
                });
            }
        )

    }
    
} 