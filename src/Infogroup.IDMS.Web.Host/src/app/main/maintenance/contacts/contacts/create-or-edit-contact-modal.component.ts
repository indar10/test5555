import { Component, Injector, Input } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { ContactsServiceProxy, CreateOrEditContactDto, ContactType } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditContactModal',
    templateUrl: './create-or-edit-contact-modal.component.html'
    
})
export class CreateOrEditContactModalComponent extends AppComponentBase {
    @Input() contactId: number;
    @Input() company: string;
    @Input() record: any;
    @Input() isEdit: boolean;
    @Input() permissionName: string;

    @Input() Id: number;
    @Input() contactType: ContactType;

    

    active = false;
    saving = false;

    contact: CreateOrEditContactDto;

    constructor(
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.Id);
    }

    show(id?: number): void {
        if (!id) {
            this.contact = new CreateOrEditContactDto();
            this.contact.cCompany = this.record.cCompany;
            this.contact.iIsActive = true;
            this.contact.cAddress1 = this.record.cAddress1;
            this.contact.cAddress2 = this.record.cAddress2;
            this.contact.cCity = this.record.cCity;
            this.contact.cState = this.record.cState;
            this.contact.cZIP = this.record.cZip;
            this.active = true;
        } else {
            this._contactsServiceProxy.getContactForEdit(id).subscribe(result => {
                this.contact = result;
                this.contact.cCompany = this.company;
                this.active = true;
            });
        }
    }

    save(contactForm: NgForm): void {
        if (contactForm.dirty) {
            this.saving = true;
            this.contact.contactID = this.contactId;
            this.contact.contactType = this.contactType;
            this._contactsServiceProxy.createOrEdit(this.contact)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.activeModal.close({ saving: this.saving });
                });
        }
        else {
            this.notify.info(this.l('SavedSuccessfully'));
            this.activeModal.close({ saving: this.saving });
        }
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}
