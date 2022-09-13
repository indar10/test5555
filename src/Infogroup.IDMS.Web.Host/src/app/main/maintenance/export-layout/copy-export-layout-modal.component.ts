import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Paginator } from 'primeng/components/paginator/paginator';
import { SelectItem, LazyLoadEvent } from 'primeng/api';
import { CopyAllExportLayoutDto, ExportLayoutsServiceProxy, DatabasesServiceProxy } from '@shared/service-proxies/service-proxies';
import { Table } from 'primeng/components/table/table';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './copy-export-layout-modal.component.html',
    animations: [appModuleAnimation()]
})
export class CopyExportLayoutComponent extends AppComponentBase implements OnInit {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() databaseLists: SelectItem[] = [];
    saving: boolean = false;
    canAdd: boolean = false;
    isDisabled: boolean = true;
    CopyExportLayout: any;
    databaseList: SelectItem[] = [];
    groupFromList: SelectItem[] = [];
    groupToList: SelectItem[] = [];

    copyExportLayoutDto: CopyAllExportLayoutDto = new CopyAllExportLayoutDto();

    constructor(
        injector: Injector,
        private activeModal: NgbActiveModal,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy

    ) {
        super(injector);
    }

    ngOnInit() {
        this.databaseList.push({ label: this.l("SelectDatabase"), value: "" });
        this.databaseList = this.databaseList.concat(this.databaseLists);
        this.groupFromList.unshift({ label: this.l("SelectGroup"), value: "" });
        this.groupToList.unshift({ label: this.l("SelectGroup"), value: "" }); 
    }    

    getAllExportLayout(event?: LazyLoadEvent) {
        if (this.copyExportLayoutDto.groupFromId != 0 && this.copyExportLayoutDto.groupFromId != undefined) {
            this.primengTableHelper.showLoadingIndicator();
            this._exportLayoutServiceProxy.getAllExportLayout(
                this.copyExportLayoutDto.databaseFromId,
                this.copyExportLayoutDto.groupFromId,
                this.primengTableHelper.getSorting(this.dataTable)
            ).subscribe(result => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
        }
    }

    close(): void {
        this.activeModal.close({ isSave: false });
    }

    copyExportLayout(): void {
        this.message.confirm(
            this.l(""),
            this.l(""),
            isConfirmed => {
                if (isConfirmed) {
                    this.saving = true;
                    this.copyExportLayoutDto.layouts = this.CopyExportLayout;
                    this._exportLayoutServiceProxy.copyAllExportLayout(this.copyExportLayoutDto)
                        .pipe(finalize(() => { this.saving = false; }))
                        .subscribe(() => {
                            this.notify.info(this.l('SavedSuccessfully'));
                            this.activeModal.close({ isSave: this.saving, isSelectedDatabase: this.copyExportLayoutDto.databaseToId });
                        });
                }
                else {
                    this.saving = false;
                }
            });
        
    }

    canSave(): void {
        if (this.CopyExportLayout.length !== 0)
            this.canAdd = true;
        else
            this.canAdd = false;
    }  


    onDatabaseFromChange(event): void {
        this.copyExportLayoutDto.databaseFromId = event.value;        
        this.setGroupFromOptions(event.value);  
    }

    onDatabaseToChange(event): void {
        this.copyExportLayoutDto.databaseToId = event.value;
        this.setGroupToOptions(event.value);
    }

    onGroupFromChange(event): void {
        if (event.value != '') {
            this.isDisabled = false;
            this.copyExportLayoutDto.groupFromId = event.value;
            this.getAllExportLayout();
        }
        else {
            this.isDisabled = true;
            this.primengTableHelper.records = null;
            this.primengTableHelper.totalRecordsCount = 0;
        }
    }

    onGroupToChange(event): void {
        this.copyExportLayoutDto.groupToId = event.value;
    }

    setGroupFromOptions(selectedFromDatabase: number) {
        this._exportLayoutServiceProxy.getGroupNames(selectedFromDatabase, 1)
            .subscribe(result => {
                if (result.length == 0) {
                    this.groupFromList = [];
                    this.groupFromList.push({ label: this.l("SelectGroup"), value: "" });
                    this.copyExportLayoutDto.groupFromId = null;
                    this.isDisabled = true;
                    this.primengTableHelper.records = null;
                    this.primengTableHelper.totalRecordsCount = 0;
                }
                else {
                    this.groupFromList = [];
                    this.groupFromList = result;   
                    this.groupFromList.unshift({ label: this.l("SelectGroup"), value: "" });  
                }                
            });
    }

    setGroupToOptions(selectedToDatabase: number) {
        this._exportLayoutServiceProxy.getGroupNames(selectedToDatabase, 1)
            .subscribe(result => {
                if (result.length == 0) {
                    this.groupToList = [];
                    this.groupToList.push({ label: this.l("SelectGroup"), value: "" });
                    this.copyExportLayoutDto.groupToId = null;
                }
                else {
                    this.groupToList = [];
                    this.groupToList = result; 
                    this.groupToList.unshift({ label: this.l("SelectGroup"), value: "" });
                } 
            });
    }
}