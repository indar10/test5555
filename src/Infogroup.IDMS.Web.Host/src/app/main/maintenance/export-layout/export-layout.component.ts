import { Component, Injector, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { NgbActiveModal, NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { SelectItem, LazyLoadEvent} from 'primeng/api';
import { CreateExportLayoutModalComponent } from './create-export-layout-modal.component';
import { CreateOrEditExportLayoutComponent } from '@app/main/shared/export-layout-add-fields/create-or-edit-export-layout.component';
import { IdmsUserLoginInfoDto, ExportLayoutsServiceProxy, DatabasesServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ModalDefaults } from '@shared/costants/modal-contants';
import { CopyExportLayoutComponent } from './copy-export-layout-modal.component';
import { finalize } from 'rxjs/operators';


declare var $: any;

@Component({
    selector: 'ExportLayout',
    templateUrl: './export-layout.component.html',
    styleUrls: ['./export-layout.component.css'],
    animations: [appModuleAnimation()]
})

export class ExportLayoutComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() outsideClick: false;

    layoutDescription: string = "";
    databaseList: SelectItem[] = [];
    oututCaseValues: SelectItem[] = [];
    oututCaseValuesForGrid: SelectItem[] = [];
    groupNameValues: SelectItem[] = [];
    buildsList: SelectItem[] = [];
    selectedDatabase: any;
    selectedOutputCase: any;
    selectedGroupName: any;
    selectedBuild: any;
    advancedFiltersAreShown: any;
    fieldDescription: any;

    active: boolean = true;;
    idmsUserData: IdmsUserLoginInfoDto;
    filterText = '';

    @Output() editExportLayout = new EventEmitter<any>();
    exportLayoutId: any;
    maintainanceBuildId: any;
    databaseId: any;
    layoutName: any;
    deleteFlag: boolean;

    constructor(
        injector: Injector,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private modalService: NgbModal,
        private _fileDownloadService: FileDownloadService,


    ) {
        super(injector);
    }
    ngOnInit() {
        this.FillDatabaseDropdown();
        this._exportLayoutServiceProxy.getOutputCaseDropDownValues().subscribe(result => {
            this.oututCaseValues.push({ label: "", value: "" });
            this.oututCaseValues = this.oututCaseValues.concat(result);            
            this.selectedOutputCase = this.oututCaseValues[0].value;
            this.oututCaseValuesForGrid = result;
        });
    }

    getExportLayouts(event?: LazyLoadEvent) {       
        this.idmsUserData = this.appSession.idmsUser;
        let callAppService = this.idmsUserData !== undefined;
        if (callAppService) {
            this.createFilters(event);
           
        }
        else {
            this.primengTableHelper.totalRecordsCount = 0;
            this.primengTableHelper.records = undefined;
        }
    }

    FillDatabaseDropdown() {
        this._databaseServiceProxy.getExportLayoutDatabasesForUser().subscribe(result => {
            this.databaseList = result;
            this.selectedDatabase = this.databaseList[0].value;
            this._exportLayoutServiceProxy.getGroupNames(this.selectedDatabase, 1).subscribe(result => { this.groupNameValues = result; });
            this._exportLayoutServiceProxy.getBuildsByDatabase(this.selectedDatabase).subscribe(result => { this.buildsList = result; this.selectedBuild = this.buildsList[0].value; this.getExportLayouts();});
        });
    }

    createFilters(event?: LazyLoadEvent): void {


        if (this.primengTableHelper.shouldResetPaging(event) && !this.deleteFlag) {
            this.paginator.changePage(0);
            return;
        }
        this.deleteFlag = false;
        this.primengTableHelper.showLoadingIndicator();
        this._exportLayoutServiceProxy.getAllExportLayoutsList(
            this.selectedOutputCase,
            this.fieldDescription,
            this.selectedDatabase,
            this.filterText,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event))
        .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
        .subscribe(result => {
            this._exportLayoutServiceProxy.getBuildsByDatabase(this.selectedDatabase).subscribe(result => { this.buildsList = result; this.selectedBuild = this.buildsList[0].value; });
            this._exportLayoutServiceProxy.getGroupNames(this.selectedDatabase, 1).subscribe(result => { this.groupNameValues = result; });
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
        });
    }

    onGroupNameChange(event, i) {
        let record = this.primengTableHelper.records[i];

        var fieldDescription = this.groupNameValues.find(x => x.value == event.value).label;

        record.cGroupName = fieldDescription;
        record.groupID = event.value;
        this.updateExportLayoutTableFields(record, true);

    }

    onOutputCaseChange(event, i) {

        let record = this.primengTableHelper.records[i];
        var fieldDescription = this.oututCaseValues.find(x => x.value == event.value).label;
        record.cOutputCase = fieldDescription;
        record.cOutputCaseCode = event.value;
        this.updateExportLayoutTableFields(record, true);
    }

    onHasKeyCodeChange(event, i) {
        let record = this.primengTableHelper.records[i];
        record.iHasKeyCode = event;
        this.updateExportLayoutTableFields(record, true);

    }

    onHasPhoneChange(event, i) {
        let record = this.primengTableHelper.records[i];
        record.iHasPhone = event;
        this.updateExportLayoutTableFields(record, true);

    }

    createExportLayout(): void {
        let modalRef: NgbModalRef = null;
        modalRef = this.modalService.open(CreateExportLayoutModalComponent, { backdrop: 'static', windowClass: '.app-modal-window .modal-dialog' });
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.result.then(result => {

            if (result.isSave)
                this.getExportLayouts();
        }
        );


    }
    updateExportLayoutTableFields(record, controlFlag) {


        if (controlFlag && record.cDescription != "") {
            this._exportLayoutServiceProxy.updateMaitainanceExportLayoutRecords(record).subscribe();
        }

    }

    deleteExportLayout(id) {
        this.message.confirm(
            this.l(''),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._exportLayoutServiceProxy.deleteMaintainanceExportLayoutRecord(id).subscribe();
                    this.notify.info(this.l('Deleted Successfully'));
                    this.deleteFlag = true;
                    this.getExportLayouts();
                }
            }
        );
    }

    copyExporyLayout(record) {
        this._exportLayoutServiceProxy.copyExportLayout(record).subscribe(result => { this.notify.info(this.l('Copied Successfully')); this.getExportLayouts(); });
    }

    openAddFieldsPopUp(exportLayout, flag) {
        if (flag) {            
            this.exportLayoutId = exportLayout.id;
            this.maintainanceBuildId = this.selectedBuild;
            this.databaseId = this.selectedDatabase;
            this.layoutName = exportLayout.cDescription;
            this.editExportLayout.emit({ exportLayoutId: this.exportLayoutId, maintainanceBuildId: this.maintainanceBuildId, isCampaign: false, databaseId: this.databaseId, layoutName: this.layoutName});
        }

    }
    ExportToExcel(record) {
        this._exportLayoutServiceProxy.downloadExportLayoutExcel(record.id, this.selectedDatabase, this.selectedBuild, 1).subscribe(result => { this._fileDownloadService.downloadTempFile(result) });
    }

    copyAllExportLayout(): void {
        let modalRef: NgbModalRef = null;
        modalRef = this.modalService.open(
            CopyExportLayoutComponent,
            {
                size: ModalDefaults.Size,
                backdrop: ModalDefaults.Backdrop,
                windowClass: ModalDefaults.WindowClass
            });
        
        modalRef.componentInstance.databaseLists = this.databaseList;

        modalRef.result.then(result => {
            if (result.isSave && result.isSelectedDatabase == this.selectedDatabase)
                this.getExportLayouts();
        });
    }
}