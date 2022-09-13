import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { DatabasesServiceProxy, DecoysServiceProxy, ShortSearchServiceProxy, PageID, GetAllDecoysInput, MailerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import { SelectItem } from 'primeng/api';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateOrEditDecoyModalComponent } from './create-or-edit-decoy-modal.component';
import { ModalDefaults } from '../../../../shared/costants/modal-contants';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './decoy-mailers.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class DecoyMailersComponent extends AppComponentBase {

    @ViewChild('createOrEditDecoyrModal', { static: true }) createOrEditDecoyModal: CreateOrEditDecoyModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    selectedDatabase: number;
    databaseList: SelectItem[] = [];
    filterText: string = '';
    isSave: boolean = false;

    pageId: PageID = PageID.Decoys;
    helpData: any = {
        header: "Search Options:",
        examples: []
    };
    isHelpDisabled: boolean = true;
    iIsActiveFilter = -1;
    decoyDto: GetAllDecoysInput = new GetAllDecoysInput();

    constructor(
        injector: Injector,
        private _decoysServiceProxy: DecoysServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private _shortSearchServiceProxy: ShortSearchServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
            .subscribe(result => {
                this.helpData = result;
                this.isHelpDisabled = false;
            });
        this.fillDatabaseDropdown();
    }

    fillDatabaseDropdown() {
        this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
            this.databaseList = result;
            this.selectedDatabase = this.databaseList[0].value;
            this.getDecoyMailers();
        });
    }

    getDecoyMailers(event?: LazyLoadEvent) {

        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;        

        if (this.selectedDatabase > 0) {
            this.primengTableHelper.showLoadingIndicator();
            this._decoysServiceProxy.getAllDecoyMailers(
                this.filterText.trim(),
                this.selectedDatabase,
                0,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
        }
    }   

    createDecoy(record: MailerDto): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditDecoyModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.componentInstance.mailerId = record.id;
        modalRef.componentInstance.mailerCompany = record.cCompany;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (record.id) 
                    this.isSave = true;
                this.getDecoyMailers();
            }
        });
    }

    allExportToExcel(decoyIdForExcel: number): void {     
        this.decoyDto.filter = this.filterText.trim();
        this.decoyDto.mailerId = decoyIdForExcel;
        this.decoyDto.selectedDatabase = this.selectedDatabase;
        this.decoyDto.sorting = null;
        this.decoyDto.skipCount = 0;
        this.decoyDto.maxResultCount = 2147483640;

        this._decoysServiceProxy.exportToExcel(this.decoyDto)
            .subscribe(result => 
                this._fileDownloadService.downloadFile(result)
            );
    }

    updateDecoyCount(decoyUpdated: boolean): void {
        if (decoyUpdated) {
            this.isSave = true;
            this.getDecoyMailers();
        }
    }
}
