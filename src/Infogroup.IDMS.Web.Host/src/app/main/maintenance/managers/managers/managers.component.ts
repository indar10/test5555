import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ManagersServiceProxy, ManagerDto, ExportLayoutsServiceProxy, ContactType, GetAllSetupInput, DatabasesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { CreateOrEditManagerModalComponent } from './create-or-edit-manager-modal.component';
import { SelectItem } from 'primeng/api';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateOrEditContactModalComponent } from '../../contacts/contacts/create-or-edit-contact-modal.component';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './managers.component.html',
    encapsulation: ViewEncapsulation.None,    
    animations: [appModuleAnimation()]
})
export class ManagersComponent extends AppComponentBase {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    selectedDatabase: any;
    databaseList: SelectItem[] = [];
    advancedFiltersAreShown = false;
    filterText = '';
    iIsActiveFilter = -1;
    contactLastNameFilterText = '';
    contactEmailFilterText = '';
    databasecDatabaseNameFilter = '';
    isSave: boolean = false;
    setupDto: GetAllSetupInput = new GetAllSetupInput();
    contactType: ContactType = ContactType.Manager;
    permissionName: string = 'Pages.ManagerContacts.Edit';

    constructor(
        injector: Injector,
        private _managersServiceProxy: ManagersServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.fillDatabaseDropdown();
    }

    fillDatabaseDropdown(): void {
        this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
            this.databaseList = result;
            this.selectedDatabase = this.databaseList[0].value;
            this.getManagers();
        });
    }

    getManagers(event?: LazyLoadEvent): void {
        let isActiveChecked = $('#iIsActiveFilterSelect:checkbox:checked').length > 0;

        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;

        if (isActiveChecked)
            this.iIsActiveFilter = 1;
        else
            this.iIsActiveFilter = 0;
        if (this.selectedDatabase > 0) {
            this.primengTableHelper.showLoadingIndicator();

            this._managersServiceProxy.getAllManagers(
                this.filterText.trim(),
                this.iIsActiveFilter,
                this.selectedDatabase,
                this.contactLastNameFilterText.trim(),
                this.contactEmailFilterText.trim(),
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            ).subscribe(result => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
        }
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createOrEditManager(id: number): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditManagerModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.Id = id;
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id)
                this.isSave = true;
                this.getManagers();
            }
        }
        );
    }

    createManagersContact(record: any): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditContactModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.record = record;
        modalRef.componentInstance.contactId = record.id;
        modalRef.componentInstance.isEdit = true;
        modalRef.componentInstance.contactType = this.contactType;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (record.id)
                    this.isSave = true;
                this.getManagers();
            }
        }
        );
    }

    exportContactAssignmentsToExcel(): void {
        if (this.selectedDatabase > 0) {
            this.getSetupDto();
            this._managersServiceProxy.exportContactAssignmentsToExcel(this.setupDto).subscribe(result => {
                this._fileDownloadService.downloadTempFile(result)
            });
        }
    }

    allExportToExcel(): void {
        if (this.selectedDatabase > 0) {
            this.getSetupDto();
            this._managersServiceProxy.exportManagersToExcel(this.setupDto).subscribe(result => {
                this._fileDownloadService.downloadTempFile(result)
            });
        }
    }

    getSetupDto(): void {
        this.setupDto.filter = this.filterText.trim();
        this.setupDto.iIsActiveFilter = this.iIsActiveFilter;
        this.setupDto.selectedDatabase = this.selectedDatabase;
        this.setupDto.contactLastNameFilterText = this.contactLastNameFilterText.trim();
        this.setupDto.contactEmailFilterText = this.contactEmailFilterText.trim();
        this.setupDto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;
    }
}
