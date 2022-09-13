import { Component, Injector, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDivisionMailerDto, DivisionMailersServiceProxy, DivisionsServiceProxy } from '@shared/service-proxies/service-proxies';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'createOrEditDivisionMailerModal',
    templateUrl: './create-or-edit-divisional-mailer.component.html'
    
})
export class CreateorEditDivisionalMailerComponent extends AppComponentBase implements OnInit {
    divisions: any = [];
    saving: boolean = false;
    mailerSaved: boolean = false;
    active: boolean = false;
    @Input() divisionMailerId: number;
    mailer: CreateOrEditDivisionMailerDto = new CreateOrEditDivisionMailerDto();
    constructor(
        injector: Injector,
        private _divisionMailersServiceProxy: DivisionMailersServiceProxy,
        private _divisionServiceProxy: DivisionsServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.divisionMailerId);
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ isSave: this.mailerSaved });
    }

    save(divisionMailerForm: NgForm): void {
        if (divisionMailerForm.dirty) {         
            this.saving = true;
            this._divisionMailersServiceProxy.createOrEditDivisionMailer(this.mailer)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(result => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.mailerSaved = true;
                    this.close();
                });
        }
        else {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close();
        }
    }

    show(divisionMailerId: number): void {
        if (!divisionMailerId) {
            this.mailer.iIsActive = true;
            this.active = true;
        }

        this._divisionServiceProxy.getDivisionDropdownsForUser().subscribe(
            (result) => {
                this.divisions = result;
            }
        );
        if (divisionMailerId) {
            this._divisionMailersServiceProxy.getDivisionMailerForEdit(divisionMailerId)
                .subscribe(result => {
                    this.mailer.init(result);                    this.active = true;

                });
        }
    }

}
