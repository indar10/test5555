import { Component, Injector, Input } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { CreateOrEditSecurityGroupDto, SecurityGroupsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditSecurityGroupModal',
    templateUrl: './create-or-edit-security-group-modal.component.html'

})
export class CreateOrEditSecurityGroupModalComponent extends AppComponentBase {

    @Input() databaseId: number;
    @Input() groupId: number;
    active: boolean = false;
    saving: boolean = false;
    securityGroup: CreateOrEditSecurityGroupDto = new CreateOrEditSecurityGroupDto();

    constructor(
        injector: Injector,
        private _securityGroupServiceProxy: SecurityGroupsServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }
    ngOnInit() {
        this.show(this.groupId);
    }
    show(groupId?: number): void {
        if (!groupId) {
            this.securityGroup = new CreateOrEditSecurityGroupDto();
            this.securityGroup.iIsActive = false;
            this.active = true;
        } else {
            this._securityGroupServiceProxy.getSecurityGroupForEdit(groupId).subscribe(result => {
                this.securityGroup = result;
                this.active = true;
            });
        }
    }

    save(ownerForm: NgForm): void {
        if (ownerForm.dirty) {
            this.saving = true;
            this.securityGroup.databaseId = this.databaseId;
            this._securityGroupServiceProxy.createOrEdit(this.securityGroup)
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
