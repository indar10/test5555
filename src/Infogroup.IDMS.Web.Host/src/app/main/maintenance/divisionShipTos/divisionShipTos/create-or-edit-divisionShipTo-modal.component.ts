import { Component, Injector, Input } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { DivisionShipTosServiceProxy, CreateOrEditDivisionShipToDto, DivisionsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppConsts } from '@shared/AppConsts';
import { ModalWindowName } from '@shared/costants/modal-contants';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditDivisionShipToModal',
    templateUrl: './create-or-edit-divisionShipTo-modal.component.html'
    
})
export class CreateOrEditDivisionShipToModalComponent extends AppComponentBase {

    @Input() divisionShipToId: number;
    divisions: any = [];
    active = false;
    saving = false;
    ftpServerInput : boolean=true;
    userIdInput: boolean = true;
    passwordInput: boolean = true;
    divisionShipTo: CreateOrEditDivisionShipToDto = new CreateOrEditDivisionShipToDto();

    constructor(
        injector: Injector,
        private _divisionShipTosServiceProxy: DivisionShipTosServiceProxy,
        private _divisionServiceProxy: DivisionsServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }
    ngOnInit() {
        this.show(this.divisionShipToId);
    }
    show(divisionShipToId?: number): void {
        this._divisionServiceProxy.getDivisionDropdownsForUser().subscribe(
            (result) => {
                this.divisions = result;
            }
        );

        if (!divisionShipToId) {
            this.divisionShipTo = new CreateOrEditDivisionShipToDto();
            this.divisionShipTo.iIsActive = true;
            this.active = true;
        } else {
            this._divisionShipTosServiceProxy.getDivisionShipToForEdit(divisionShipToId).subscribe(result => {
                this.divisionShipTo.init(result);
                this.active = true;
            });
        }
    }

    save(divisionShipToForm: NgForm): void {
        if (divisionShipToForm.dirty) {
            if ((this.divisionShipTo.cUserID && !this.divisionShipTo.cFTPServer) || (!this.divisionShipTo.cUserID && this.divisionShipTo.cFTPServer)) {
                if (this.divisionShipTo.cUserID) {
                    this.ftpServerInput = false;
                }
                if (this.divisionShipTo.cFTPServer) {
                    this.userIdInput = false;
                }
                return;
            }
            if (this.divisionShipTo.cUserID && this.divisionShipTo.cFTPServer && !this.divisionShipTo.cPassword) {
                if (this.divisionShipTo.cFTPServer.toUpperCase().indexOf("TYPE#SFTP") == -1) {
                    this.passwordInput = false;
                    return;
                }
            }
            this.saving = true;
            this._divisionShipTosServiceProxy.createOrEdit(this.divisionShipTo)
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
        this.ftpServerInput = true;
        this.userIdInput = true;
        this.passwordInput = true;
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }

    openHelpDoc(): void {
        var param = 'height=750,width=800,resizable=1,top=50,left=100';
        window.open(AppConsts.divisionalShipToHelpUrl, ModalWindowName.BLANK, param);
    }

    onFTPServerChange(): void {
        if (this.divisionShipTo.cFTPServer)
            this.ftpServerInput = true;
    }

    onUserIDChange(): void {
        if (this.divisionShipTo.cUserID)
            this.userIdInput = true;
    }

    onPasswordChange(): void {
        if (this.divisionShipTo.cPassword)
            this.passwordInput = true;
    }
}
