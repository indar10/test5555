import { Component, Injector, Input} from '@angular/core';
import { finalize } from 'rxjs/operators';
import { CreateOrEditDecoyDto, DecoysServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditDecoyModal',
    templateUrl: './create-or-edit-decoy-modal.component.html'    
})

export class CreateOrEditDecoyModalComponent extends AppComponentBase {

    @Input() databaseId: number;
    @Input() decoyId: number;
    @Input() mailerId: number;
    @Input() mailerCompany: string;
    active: boolean = false;
    saving: boolean = false;
    decoy: CreateOrEditDecoyDto = new CreateOrEditDecoyDto();
    groupList: any = [];
    selectedGroup: string = '';

    constructor(
        injector: Injector,
        private _decoysServiceProxy: DecoysServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }
    ngOnInit() {
        this.show(this.decoyId);
    }
    show(decoyId?: number): void {

        this._decoysServiceProxy.fillGroupsForEdit().subscribe(result => {
            this.groupList = result;
            this.selectedGroup = result[0].value;
        });

        if (!decoyId) {
            this.decoy = new CreateOrEditDecoyDto();
            this.active = true;
        } else {
            this._decoysServiceProxy.getDecoyForEdit(decoyId).subscribe(result => {
                this.decoy = result;
                this.selectedGroup = result.cDecoyGroup;
                this.active = true;
            });
        }
    }

    save(decoyForm: NgForm): void {

        var result = false;
        var isFirstName = !this.decoy.cFirstName.trim().includes("<##DK##>");
        var isLastName = !this.decoy.cLastName.trim().includes("<##DK##>");
        var isAddress1 = !this.decoy.cAddress1.trim().includes("<##DK##>");
        var isKeyCode1 = this.decoy.cKeyCode1 != null ? !this.decoy.cKeyCode1.trim().includes("<##DK##>") : true;

        if (isFirstName && isLastName && isAddress1 && isKeyCode1)
            result = true;
        else
            result = false;

        if (result) {
            this.message.confirm(this.l('ValidateDecoyKey', '<##DK##>'), isConfirmed => {
                if (isConfirmed) {
                    this.saveDecoy(decoyForm);
                }                
            });
        }
        else {
            this.saveDecoy(decoyForm);
        }      
    }

    saveDecoy(decoyForm: NgForm): void {
        if (decoyForm.dirty) {
            this.saving = true;
            this.decoy.databaseId = this.databaseId;
            this.decoy.mailerID = this.mailerId;
            this.decoy.cDecoyGroup = this.selectedGroup;
            this._decoysServiceProxy.createOrEditDecoy(this.decoy)
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
