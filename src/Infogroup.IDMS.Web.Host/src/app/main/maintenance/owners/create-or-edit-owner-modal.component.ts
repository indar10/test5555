import { Component, Injector, Input} from '@angular/core';
import { finalize } from 'rxjs/operators';
import { OwnersServiceProxy, CreateOrEditOwnerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditOwnerModal',
    templateUrl: './create-or-edit-owner-modal.component.html'
    
})
export class CreateOrEditOwnerModalComponent extends AppComponentBase {

    @Input() databaseId: number;
    @Input() ownerId: number;
    active = false;
    saving = false;
    owner: CreateOrEditOwnerDto = new CreateOrEditOwnerDto();

    constructor(
        injector: Injector,
        private _ownersServiceProxy: OwnersServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }
    ngOnInit() {
        this.show(this.ownerId);
    }
    show(ownerId?: number): void {

        if (!ownerId) {
            this.owner = new CreateOrEditOwnerDto();
            this.owner.iIsActive = true;
            this.active = true;
        } else {
            this._ownersServiceProxy.getOwnerForEdit(ownerId).subscribe(result => {
                this.owner = result;
                this.active = true;
            });
        }
    }

    save(ownerForm: NgForm): void {
        if (ownerForm.dirty) {
            this.saving = true;
            this.owner.databaseId = this.databaseId;
            this._ownersServiceProxy.createOrEdit(this.owner)
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
