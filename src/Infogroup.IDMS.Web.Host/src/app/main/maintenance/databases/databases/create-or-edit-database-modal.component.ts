import { Component, ViewChild, Injector, Output, EventEmitter, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { finalize } from 'rxjs/operators';
import { DatabasesServiceProxy, CreateOrEditDatabaseDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'createOrEditDatabaseModal',
    templateUrl: './create-or-edit-database-modal.component.html'
})
export class CreateOrEditDatabaseModalComponent extends AppComponentBase {
    
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Input() databaseId: number;
    active = false;
    saving = false;
    database: CreateOrEditDatabaseDto = new CreateOrEditDatabaseDto();    
    divisions: any = [];
    divisionCodes: any = [];
    databaseTypes: any = [];

    constructor(
        injector: Injector,
        private _databasesServiceProxy: DatabasesServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }
    ngOnInit() {
        this.show(this.databaseId);
    }

    show(databaseId?: number): void {
        let userId = this.appSession.idmsUser.idmsUserID;
        this._databasesServiceProxy.getDatabaseDropDownsDto(userId, databaseId).subscribe(
            result => {
                this.divisions = result.divisions;
                if (!databaseId) {
                    this.database.divisionId = result.defaultDivision;
                    this.database.lK_DatabaseType = result.defaultDatabaseType;
                }
                this.divisionCodes = result.divisionCodes;
                this.databaseTypes = result.databaseTypes;
            }
        )
        if (!databaseId) {
            this.database = new CreateOrEditDatabaseDto();
            this.active = true;            
        } else {
            this._databasesServiceProxy.getDatabaseForEdit(databaseId).subscribe(result => {
                this.database = result;
                this.active = true;              
            });
        }
    }

    save(databaseForm: NgForm): void {   
        if (databaseForm.dirty) {
            this.saving = true;
            this._databasesServiceProxy.createOrEdit(this.database)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
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
