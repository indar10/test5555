import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ContactType, BrokersServiceProxy, GetAllBrokersInput, DatabasesServiceProxy  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import { ExportLayoutsServiceProxy } from '@shared/service-proxies/service-proxies';
import { SelectItem } from 'primeng/api';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateOrEditContactModalComponent } from '../contacts/contacts/create-or-edit-contact-modal.component';
import { CreateOrEditBrokerModalComponent } from './create-or-edit-broker-modal.component';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './brokers.component.html',
    encapsulation: ViewEncapsulation.None,   
    animations: [appModuleAnimation()]
})
export class BrokersComponent extends AppComponentBase {
   
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    selectedDatabase: any;    
    databaseList: SelectItem[] = [];
    advancedFiltersAreShown = false;
    filterText = '';
    contactLastNameFilterText = '';
    contactEmailFilterText = '';
    iIsActiveFilter = -1;
    getBroker:GetAllBrokersInput= new GetAllBrokersInput();
    contactType: ContactType = ContactType.Broker;
    permissionName: string = "Pages.ContactsBrokers.Edit";
    isSave: boolean = false;

    constructor(
        injector: Injector,
        private _brokersServiceProxy: BrokersServiceProxy,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private _fileDownloadService: FileDownloadService,
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
            this.getBrokers();
        });
    }

    getBrokers(event?: LazyLoadEvent) {
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
            this._brokersServiceProxy.getAllBrokers(
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

    createOrEditBroker(id:number): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditBrokerModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.id = id;
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id)
                    this.isSave = true;
                this.getBrokers();
            }
        }
        );
    }

    createBrokersContact(record): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditContactModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass});
        modalRef.componentInstance.record = record;
        modalRef.componentInstance.contactId = record.id;
        modalRef.componentInstance.isEdit = true;
        modalRef.componentInstance.contactType = this.contactType;
        modalRef.componentInstance.permissionName = this.permissionName;
        
        modalRef.result.then((result) => {
            if (result.saving) {
                if (record.id)
                    this.isSave = true;
                this.getBrokers();
            }
        }
        );
    }
    

    ExportToExcel(ownerIdForExcel: number): void {       

        this.getBroker.filter = ownerIdForExcel > 0 ? ownerIdForExcel.toString().trim() : this.filterText.trim();
        this.getBroker.iIsActiveFilter = this.iIsActiveFilter;
        this.getBroker.selectedDatabase = this.selectedDatabase;
        this.getBroker.contactLastNameFilterText = this.contactLastNameFilterText.trim();
        this.getBroker.contactEmailFilterText = this.contactEmailFilterText.trim();
        this.getBroker.sorting = null;
        this.getBroker.skipCount = 0;
        this.getBroker.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;

        this._brokersServiceProxy.exportToExcel(this.getBroker)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result)
            });
    }
}
