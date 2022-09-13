import { Component, Injector, Output, EventEmitter, Input } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { ManagersServiceProxy, CreateOrEditManagerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditManagerModal',
    templateUrl: './create-or-edit-manager-modal.component.html'
   
})
export class CreateOrEditManagerModalComponent extends AppComponentBase {
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Input() databaseId: number;
    @Input() Id: number;
    active = false;
    saving = false;
    manager: CreateOrEditManagerDto = new CreateOrEditManagerDto();

    constructor(
        injector: Injector,
        private _managersServiceProxy: ManagersServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.Id);
    }

    show(managerId?: number): void {

        if (!managerId) {
            this.manager = new CreateOrEditManagerDto();
            this.manager.id = managerId;
            this.manager.iIsActive = true;
            this.active = true;
            
        } else {
            this._managersServiceProxy.getManagerForEdit(managerId).subscribe(result => {
                this.manager = result;
                this.active = true;
                
            });
        }
    }

    save(managerForm: NgForm): void {
        if (managerForm.dirty) {
            this.saving = true;
            this.manager.databaseId = this.databaseId;
            this._managersServiceProxy.createOrEdit(this.manager)
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
