import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { MailersServiceProxy, ExportLayoutsServiceProxy, ContactType, GetAllSetupInput, DatabasesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditMailerModalComponent } from './create-or-edit-mailer-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { SelectItem } from 'primeng/api';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateOrEditContactModalComponent } from '../../contacts/contacts/create-or-edit-contact-modal.component';
import { CreateOrEditOfferModalComponent } from '../../offers/offers/create-or-edit-offer-modal.component';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './mailers.component.html',
    encapsulation: ViewEncapsulation.None,    
    animations: [appModuleAnimation()],
    selector: 'Mailer'
})
export class MailersComponent extends AppComponentBase {

    @ViewChild('createOrEditMailerModal', { static: true }) createOrEditMailerModal: CreateOrEditMailerModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    selectedDatabase: any;
    databaseList: SelectItem[] = [];
    advancedFiltersAreShown = false;
    filterText = '';
    contactLastNameFilterText = '';
    contactEmailFilterText = '';
    iIsActiveFilter = -1;
    dto: GetAllSetupInput = new GetAllSetupInput();
    contactType: ContactType = ContactType.Mailer;
    isOpened: any = [];
    expandedRows: {} = {};
    selected: any;
    isSave: boolean = false;
    permissionName: string = 'Pages.MailerContacts.Edit';

    constructor(
        injector: Injector,
        private _mailersServiceProxy: MailersServiceProxy,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.fillDatabaseDropdown();
    }

    fillDatabaseDropdown() {
        this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
            this.databaseList = result;
            this.selectedDatabase = this.databaseList[0].value;
            this.getMailers();
        });
    }
    showOffersHandler(i, id) {      

        if (!this.primengTableHelper.records[i].showContacts && !this.primengTableHelper.records[i].showOffers) {
            this.primengTableHelper.records[i].showOffers = true;
            this.primengTableHelper.records[i].showContacts = false;
            this.isOpened.push({ i, id });        
        }
        else if (this.primengTableHelper.records[i].showOffers) {
            this.primengTableHelper.records[i].showOffers = false;
            this.primengTableHelper.records[i].showContacts = false;
            this.isOpened.pop(i); 
        }       
    }

    showContactsHandler(i, id) {
        
        if (!this.primengTableHelper.records[i].showContacts && !this.primengTableHelper.records[i].showOffers) {
            this.primengTableHelper.records[i].showOffers = false;
            this.primengTableHelper.records[i].showContacts = true;   
            this.isOpened.push({ i, id }); 
        }        
        else if (this.primengTableHelper.records[i].showContacts) {
            this.primengTableHelper.records[i].showOffers = false;
            this.primengTableHelper.records[i].showContacts = false;   
            this.isOpened.pop(i); 
        }           
    }
    resetGrid() {       
       this.isOpened.forEach((i) => {
            this.expandedRows[i.id] = false;
       });       
       this.isOpened = [];       
    }

    getMailers(event?: LazyLoadEvent) {
        this.resetGrid();
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
            this._mailersServiceProxy.getAllMailers(
                this.filterText.trim(),
                this.iIsActiveFilter,
                this.selectedDatabase,
                this.contactLastNameFilterText.trim(),
                this.contactEmailFilterText.trim(),
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event),
            ).subscribe(result => {
                this.expandedRows = {};
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
        }
    }

    createMailersContact(record): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditContactModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.record = record;
        modalRef.componentInstance.contactId = record.id;
        modalRef.componentInstance.contactType = this.contactType;
        modalRef.componentInstance.isEdit = true;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (record.id)
                    this.isSave = true;
                this.getMailers();               
            }
        }
        );        
    }

    createMailer(Id?: any): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditMailerModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.mailerId = Id;
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (Id)
                    this.isSave = true;
                this.getMailers();
            }
        }
        );
    }

    allExportToExcel(): void {
        if (this.selectedDatabase > 0) {
            this.dto.filter = this.filterText.trim();
            this.dto.iIsActiveFilter = this.iIsActiveFilter;
            this.dto.selectedDatabase = this.selectedDatabase;
            this.dto.contactLastNameFilterText = this.contactLastNameFilterText.trim();
            this.dto.contactEmailFilterText = this.contactEmailFilterText.trim();
            this.dto.sorting = null;
            this.dto.skipCount = 0;
            this.dto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;

            this._mailersServiceProxy.exportAllMailerToExcel(this.dto)
                .subscribe(result => {
                    this._fileDownloadService.downloadTempFile(result)
                });
        }
    }

    exportToExcelOffers(): void {
        if (this.selectedDatabase > 0) {
            this.dto.filter = this.filterText.trim();
            this.dto.iIsActiveFilter = this.iIsActiveFilter;
            this.dto.selectedDatabase = this.selectedDatabase;
            this.dto.contactLastNameFilterText = this.contactLastNameFilterText.trim();
            this.dto.contactEmailFilterText = this.contactEmailFilterText.trim();
            this.dto.sorting = null;
            this.dto.skipCount = 0;
            this.dto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;

            this._mailersServiceProxy.exportOffers(this.dto)
                .subscribe(result => {
                    this._fileDownloadService.downloadTempFile(result)
                });
        }
    }

    createOffer(company: string, mailerId: number): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditOfferModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass});
        modalRef.componentInstance.mailerId = mailerId;
        modalRef.componentInstance.company = company;

        modalRef.result.then((result) => {
            if (result.saving) {
                if (mailerId)
                    this.isSave = true;
                this.getMailers();
            }
        }
        );
    }
}
