import { Component, Injector, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { finalize } from 'rxjs/operators';
import { MailersServiceProxy, CreateOrEditMailerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditMailerModal',
    templateUrl: './create-or-edit-mailer-modal.component.html',
    styleUrls: ['./create-or-edit-mailer-modal.component.css']
})
export class CreateOrEditMailerModalComponent extends AppComponentBase {

    @Input() databaseId: number;
    @Input() mailerId: number;

    brokers: any = [];
    active = false;
    saving = false;
    activeWidth: any;
    mailer: CreateOrEditMailerDto;

    constructor(
        injector: Injector,
        private _mailersServiceProxy: MailersServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.mailerId);
    }

    show(mailerId?: number): void {
        this._mailersServiceProxy.getAllBrokersbyDatabaseId(this.databaseId).subscribe(
            (result) => {
                this.brokers = result;
            }
        );

        if (!mailerId) {
            this.mailer = new CreateOrEditMailerDto();
            this.mailer.iIsActive = true;            
            this.active = true;
            this.activeWidth = "col-xs-5 col-sm-5 col-md-5";
        }
        else {
            this._mailersServiceProxy.getMailerForEdit(mailerId).subscribe(result => {
                this.mailer = result;
                this.active = true;
                this.activeWidth = "col-xs-12 col-sm-12 col-md-12";
            });
        }
    }

    save(): void {
        this.saving = true;
        this.mailer.databaseId = this.databaseId;
        this._mailersServiceProxy.createOrEdit(this.mailer)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.activeModal.close({ saving: this.saving });
            });
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}
