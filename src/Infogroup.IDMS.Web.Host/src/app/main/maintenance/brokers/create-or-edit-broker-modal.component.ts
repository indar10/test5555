import { Component, Injector, Output, EventEmitter, Input} from '@angular/core';
import { finalize } from 'rxjs/operators';
import {  BrokersServiceProxy, CreateOrEditBrokerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditBrokerModal',
    templateUrl: './create-or-edit-broker-modal.component.html'    
})
export class CreateOrEditBrokerModalComponent extends AppComponentBase {

    
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Input() databaseId: number;
    @Input() id: number;

    active = false;
    saving = false;
    broker: CreateOrEditBrokerDto = new CreateOrEditBrokerDto();

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _brokersServiceProxy: BrokersServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.id);
    }

    show(ownerId?: number): void {

        if (!ownerId) {
            this.broker = new CreateOrEditBrokerDto();
            this.broker.iIsActive = true;
            this.active = true;
            
        } else {
            this._brokersServiceProxy.getBrokerForEdit(ownerId).subscribe(result => {                
                this.broker = result;
                this.active = true;
                
            });
        }
    }

    save(brokerForm: NgForm): void {
        if (brokerForm.dirty) {
            this.saving = true;
            this.broker.databaseID = this.databaseId;
            this._brokersServiceProxy.createOrEdit(this.broker)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.activeModal.close({ saving: this.saving });
                    this.close();
                    this.modalSave.emit(null);
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
