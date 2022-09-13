import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { DatabasesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDatabaseModalComponent } from './create-or-edit-database-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './databases.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class DatabasesComponent extends AppComponentBase {

    @ViewChild('createOrEditDatabaseModal', { static: true }) createOrEditDatabaseModal: CreateOrEditDatabaseModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    filterText = '';   
    entityHistoryEnabled = false;
    isSave: boolean = false;

    constructor(
        injector: Injector,
        private _databasesServiceProxy: DatabasesServiceProxy,      
        private _fileDownloadService: FileDownloadService,
        private modalService: NgbModal
    ) {
        super(injector);
    } 

    getDatabases(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;

        this.primengTableHelper.showLoadingIndicator();

        this._databasesServiceProxy.getAllDatabases(
            this.filterText.trim(),
            this.appSession.idmsUser.idmsUserID,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createOrEditDatabase(id): void {       
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditDatabaseModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.databaseId = id;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id) 
                    this.isSave = true;
                this.getDatabases();
            }
        }
        );
    }

    ExportToExcel(databaseId, databaseName) {
        this._databasesServiceProxy.exportDatabaseExcel(
            databaseId,
            databaseName)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result)
            });
    }
}
