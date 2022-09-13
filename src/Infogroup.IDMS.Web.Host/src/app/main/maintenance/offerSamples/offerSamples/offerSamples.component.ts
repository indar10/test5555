import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { OfferSamplesServiceProxy, OfferSampleDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditOfferSampleModalComponent } from './create-or-edit-offerSample-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { NgbModalRef, NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import {  ModalDefaults } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './offerSamples.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
    selector: 'offerSamples'
})
export class OfferSamplesComponent extends AppComponentBase {

    @ViewChild('createOrEditOfferSampleModal', { static: true }) createOrEditOfferSampleModal: CreateOrEditOfferSampleModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() offerId: number;
    @Input() company: any;
    isSave: boolean = false;

    constructor(
        injector: Injector,
        private modalService: NgbModal,
        private _offerSamplesServiceProxy: OfferSamplesServiceProxy
    ) {
        super(injector);
    }

    getOfferSamples(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;

        this.primengTableHelper.showLoadingIndicator();

        this._offerSamplesServiceProxy.getAllOfferSamples(
            this.offerId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    EditOfferSample(Id: number): void {
        let config: NgbModalOptions = { ...{ size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass} };
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditOfferSampleModalComponent, config);
        modalRef.componentInstance.Id = Id;
        modalRef.componentInstance.MailerCompany = this.company;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (Id)
                    this.isSave = true;
                this.getOfferSamples();
            }
        }
        );
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    deleteOfferSample(offerSample: OfferSampleDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._offerSamplesServiceProxy.delete(offerSample.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
}
