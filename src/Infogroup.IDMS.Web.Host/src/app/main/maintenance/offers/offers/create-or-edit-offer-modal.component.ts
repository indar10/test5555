import { Component, Injector, Input } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { OffersServiceProxy, CreateOrEditOfferDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SelectItem } from 'primeng/api';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'createOrEditOfferModal',
    templateUrl: './create-or-edit-offer-modal.component.html',
    styleUrls: ['./create-or-edit-offer-modal.component.css']
})
export class CreateOrEditOfferModalComponent extends AppComponentBase {

    @Input() Id: number;
    @Input() mailerId: number;
    @Input() company: string;

    active = false;
    saving = false;

    offer: CreateOrEditOfferDto;
    mailercCompany = '';
    offerTypeValues: SelectItem[] = [];

    constructor(
        injector: Injector,
        private _offersServiceProxy: OffersServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.Id);
    }

    show(offerId?: number): void {
        if (!offerId) {
            this.offer = new CreateOrEditOfferDto();
            this.mailercCompany = this.company;
            this.offer.mailerId = this.mailerId;
            this.offer.iIsActive = true;
            this._offersServiceProxy.getDDForOfferType().subscribe(result => {
                this.offerTypeValues = result;
                this.offer.lK_OfferType = result[0].value;
                this.active = true;
            });

        } else {

            this._offersServiceProxy.getOfferForEdit(offerId).subscribe(result => {
                this.offer = result.offer;
                this.mailercCompany = result.mailercCompany;
                this.offerTypeValues = result.offer.offerTypeDescription;
                this.active = true;

            });
        }
    }

    save(): void {
        this.saving = true;

        if (this.offer.isAutoApprove) {
            this.message.confirm(
                this.l('Do you want to continue with auto approve'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._offersServiceProxy.createOrEdit(this.offer)
                            .pipe(finalize(() => { this.saving = false; }))
                            .subscribe(() => {
                                this.notify.info(this.l('SavedSuccessfully'));
                                this.activeModal.close({ saving: this.saving });
                            });
                    }
                    else {
                        this.saving = false;
                    }
                }
            );
        }
        else {
            this._offersServiceProxy.createOrEdit(this.offer)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.activeModal.close({ saving: this.saving });
                });
        }
    }


    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}
