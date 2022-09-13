import { Component, Injector, ViewEncapsulation, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { DecoysServiceProxy  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateOrEditDecoyModalComponent } from './create-or-edit-decoy-modal.component';
import { ModalDefaults } from '../../../../shared/costants/modal-contants';

@Component({
    selector: 'decoys',
    templateUrl: './decoys.component.html',
    encapsulation: ViewEncapsulation.None,    
    animations: [appModuleAnimation()]
})
export class DecoysComponent extends AppComponentBase {

    @Input() mailerId: number;
    @Input() selectedDatabaseId: number;
    @Input() filterText: string;
    @Input() mailerCompany: string;     
    isSave: boolean = false;
    @Output() decoyUpdatedCount: EventEmitter<boolean> = new EventEmitter<boolean>();

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;    

    constructor(
        injector: Injector,
        private _decoysServiceProxy: DecoysServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }
       
    getDecoys(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;
        this.primengTableHelper.showLoadingIndicator();

        this._decoysServiceProxy.getDecoysByMailer(
            this.filterText.trim(),
            0,
            this.mailerId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    editDecoy(id: number): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditDecoyModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.databaseId = this.selectedDatabaseId;
        modalRef.componentInstance.mailerId = this.mailerId;
        modalRef.componentInstance.decoyId = id;
        modalRef.componentInstance.mailerCompany = this.mailerCompany;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id)
                    this.isSave = true;
                this.getDecoys();
            }
        });
    }

    copyDecoy(id: number): void {
        this._decoysServiceProxy.copyDecoy(id)
            .subscribe(() => {
                this.notify.info(this.l("SeedCopySuccessful"));
                this.getDecoys();
                this.decoyUpdatedCount.emit(true);
            });
    }

    deleteDecoy(id: number): void {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                this._decoysServiceProxy.deleteDecoy(id)
                    .subscribe(() => {
                        this.notify.info(this.l("SuccessfullyDeleted"));
                        this.getDecoys();
                        this.decoyUpdatedCount.emit(true);
                    });
            }
        });        
    }    
}
