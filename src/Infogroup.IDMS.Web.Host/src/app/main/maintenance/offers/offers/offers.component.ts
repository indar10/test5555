import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { OffersServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditOfferModalComponent } from './create-or-edit-offer-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { CreateOrEditOfferSampleModalComponent } from '../../offerSamples/offerSamples/create-or-edit-offerSample-modal.component';
import { NgbModalRef, NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './offers.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
    selector: 'OffersComponent'
})
export class OffersComponent extends AppComponentBase {

    @ViewChild('createOrEditOfferModal', { static: true }) createOrEditOfferModal: CreateOrEditOfferModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() mailerId: number;
    isSave: boolean = false;

    constructor(
        injector: Injector,
        private _offersServiceProxy: OffersServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    getOffers(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;

        this.primengTableHelper.showLoadingIndicator();

        this._offersServiceProxy.getAllOffers(
            this.mailerId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    editOffer(Id: number): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditOfferModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.Id = Id;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (Id)
                    this.isSave = true;
                this.getOffers();
            }
        });
    }

    createOfferSample(Id: number, mailerCompany: string): void {
        let config: NgbModalOptions = { ...{ size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass } };
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditOfferSampleModalComponent, config);
        modalRef.componentInstance.OfferId = Id;
        modalRef.componentInstance.MailerCompany = mailerCompany;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (Id)
                    this.isSave = true;
                this.getOffers();
            }
        });
    }

}
