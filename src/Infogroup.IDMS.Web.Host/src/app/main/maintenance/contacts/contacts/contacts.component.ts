import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { ContactsServiceProxy, ContactType } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditContactModalComponent } from './create-or-edit-contact-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    selector: 'contacts',
    templateUrl: './contacts.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ContactsComponent extends AppComponentBase {
    @Input() contactId: number;
    @Input() company: string;
    @Input() contactType: ContactType;
    @Input() isEdit: boolean = true;
    @Input() permissionName: string;
   

    isSave: boolean = false;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    constructor(
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    getContacts(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;
        this.primengTableHelper.showLoadingIndicator();

        this._contactsServiceProxy.getAllContacts(
            this.contactId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.contactType
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    createContact(id: number): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditContactModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass});
        modalRef.componentInstance.company = this.company;
        modalRef.componentInstance.Id = id;
        modalRef.componentInstance.permissionName = this.permissionName;
        modalRef.componentInstance.isEdit = this.isEdit;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id)
                    this.isSave = true;
                this.getContacts();
            }
        }
        );
    }

}
