import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { DivisionShipTosServiceProxy, IdmsUserLoginInfoDto} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDivisionShipToModalComponent } from './create-or-edit-divisionShipTo-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './divisionShipTos.component.html',
    encapsulation: ViewEncapsulation.None,    
    animations: [appModuleAnimation()]
})
export class DivisionShipTosComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    idmsUserData: IdmsUserLoginInfoDto;
    advancedFiltersAreShown : boolean = false;
    filterText : string = '';
    iIsActiveFilter: boolean = true;
    isSave: boolean = false;

    constructor(
        injector: Injector,
        private _divisionShipTosServiceProxy: DivisionShipTosServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    getAllDivisionalShipTo(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;

        this.primengTableHelper.showLoadingIndicator();
        this._divisionShipTosServiceProxy.getAllDivisionalShipTo(
            this.filterText,
            this.iIsActiveFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    createDivisionShipTo(id : number): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditDivisionShipToModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.divisionShipToId = id;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id) 
                    this.isSave = true;
                this.getAllDivisionalShipTo();
            }
        }
        );
    }

}
