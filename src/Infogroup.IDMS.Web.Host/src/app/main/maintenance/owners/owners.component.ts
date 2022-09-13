import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { OwnersServiceProxy, ContactType, GetAllSetupInput, DatabasesServiceProxy  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditOwnerModalComponent } from './create-or-edit-owner-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import { ExportLayoutsServiceProxy} from '@shared/service-proxies/service-proxies';
import { SelectItem } from 'primeng/api';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateOrEditContactModalComponent } from '../contacts/contacts/create-or-edit-contact-modal.component';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './owners.component.html',
    encapsulation: ViewEncapsulation.None,    
    animations: [appModuleAnimation()]
})
export class OwnersComponent extends AppComponentBase {

    @ViewChild('createOrEditOwnerModal', { static: true }) createOrEditOwnerModal: CreateOrEditOwnerModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    selectedDatabase: any;    
    databaseList: SelectItem[] = [];
    advancedFiltersAreShown = false;
    filterText = '';
    contactLastNameFilterText = '';
    contactEmailFilterText = '';
    iIsActiveFilter = -1;
    isSave: boolean = false;
    dto: GetAllSetupInput = new GetAllSetupInput();
    contactType: ContactType = ContactType.Owner;
    permissionName: string = 'Pages.OwnerContacts.Edit';

    constructor(
        injector: Injector,
        private _ownersServiceProxy: OwnersServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.FillDatabaseDropdown();
    }

    FillDatabaseDropdown() {
        this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
            this.databaseList = result;
            this.selectedDatabase = this.databaseList[0].value;
            this.getOwners();
        });
    }

    getOwners(event?: LazyLoadEvent) {
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
            this._ownersServiceProxy.getAllOwners(
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

    createOwner(id): void {        
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditOwnerModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.ownerId = id;
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id) 
                    this.isSave = true;
                this.getOwners();
            }
        }
        );
    }

    createOwnersContact(record): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditContactModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.record = record;
        modalRef.componentInstance.contactId = record.id;
        modalRef.componentInstance.isEdit = true;
        modalRef.componentInstance.contactType = this.contactType;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (record.id) 
                    this.isSave = true;
                this.getOwners();
            }
        }
        );
    }
    ExportToExcel(Id): void {      
        this.AllExportToExcel(Id);
    }

    AllExportToExcel(ownerIdForExcel: number): void {
        if (ownerIdForExcel > 0)
            this.dto.filter = ownerIdForExcel.toString();
        else
        this.dto.filter = this.filterText.trim();
        this.dto.iIsActiveFilter = this.iIsActiveFilter;
        this.dto.selectedDatabase = this.selectedDatabase;
        this.dto.contactLastNameFilterText = this.contactLastNameFilterText.trim();
        this.dto.contactEmailFilterText = this.contactEmailFilterText.trim();
        this.dto.sorting = null;
        this.dto.skipCount = 0;
        this.dto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;

        this._ownersServiceProxy.exportToExcel(this.dto)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result)
            });
    }
}
