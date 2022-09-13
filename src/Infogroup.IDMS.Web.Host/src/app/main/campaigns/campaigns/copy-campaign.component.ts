import { Component, OnInit, Input, Injector } from '@angular/core';
import { CampaignsServiceProxy, DropdownOutputDto, CampaignCopyDto } from '@shared/service-proxies/service-proxies';
import { DatePipe } from '@angular/common';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ControlContainer, NgForm } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SelectItem } from 'primeng/api';

@Component({
    selector: 'copy-campaign',
    templateUrl: './copy-campaign.component.html',
    providers: [DatePipe],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class CopyCampaignComponent extends AppComponentBase implements OnInit {
    @Input() campaignId: number;
    builds: SelectItem[] = [];
    offers: SelectItem[] = [];
    campaign: CampaignCopyDto = new CampaignCopyDto();
    mailers: SelectItem[] = [];
    brokers: SelectItem[] = [];
    brokerVisible: boolean = true;
    offerDDVisible: boolean = false;
    defaultOfferId: number = -1;
    copying: boolean = false;
    divisionalDatabase: boolean = false;
    isCopyCompleted: boolean = false;
    active: boolean = false;
    saving: boolean = false;
    saveDisabled: boolean = true;
    customerVisible = true;
    brokerVisibleOnCount: boolean = true;
    offerVisibleOnCount: boolean = true;

    constructor(
        injector: Injector,
        private _campaignsServiceProxy: CampaignsServiceProxy,
        private activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.active = true;
        this._campaignsServiceProxy.getCampaignForCopy(this.campaignId).subscribe(result => {
            this.campaign.init(result.campaignCopyData);
            this.changeFormType(this.campaign.divisionalDatabase);
            this.builds = result.builds;
            this.offers = result.offers;
            this.isCopyCompleted = false;
            this.setFormByUserDatabaseMailer(result.userDatabaseMailerRecordCount);
        });
    }

    searchMailers(event): void {
        if (this.campaign.databaseId) {
            this._campaignsServiceProxy.getSearchResultForMailer(
                event.query,
                this.campaign.databaseId,
                this.campaign.divisionalDatabase
            ).subscribe(result => { this.mailers = result });
        }
    }

    searchBrokers(event): void {
        this._campaignsServiceProxy.getDivisionBrokers(
            event.query,
            this.campaign.databaseId,
            this.campaign.divisionalDatabase
        ).subscribe(result => { this.brokers = result });
    }

    changeFormType(divisionalDatabase: boolean): void {
        this.divisionalDatabase = divisionalDatabase;
        this.brokerVisible = divisionalDatabase;
        this.offerDDVisible = !divisionalDatabase;
    }

    onMailerSelect(event): void {
        if (!this.campaign.divisionalDatabase && event.value > 0) {
            this.offers = [];
            this._campaignsServiceProxy.getOffersDDForMailer(
                event.value
            ).subscribe(result => {
                if (!this.campaign.divisionalDatabase) {
                    this.offers = result.slice();
                    this.offers.splice(0, 0, DropdownOutputDto.fromJS({ label: "Select Offer" }));
                    if (this.defaultOfferId > -1) {
                        this.campaign.offerId = this.defaultOfferId;
                    }
                    else {
                        if (this.offers.length > 2) {
                            this.campaign.offerId = this.offers[0].value;
                        }
                        else {
                            this.campaign.offerId = this.offers[1].value;
                        }
                    }
                }
            });
        }
    }
    copyCampaign(): void {
        this.copying = true;
        this._campaignsServiceProxy.createCopyCampaign(this.campaign).subscribe(() => {
            this.notify.info(this.l("SuccessfulCopyCampaign"));
            this.isCopyCompleted = true;
            this.close();
        });
    }
    close(): void {
        this.copying = false;
        this.activeModal.close({ isSave: this.isCopyCompleted });
        this.active = false;
    }

    copyKeyDown = (event) => event.key != 0;

    setFormByUserDatabaseMailer(rowCount: number): void {
        if (rowCount <= 0) {
            this.brokerVisibleOnCount = true;
            this.customerVisible = true;
            this.offerVisibleOnCount = true;
        }
        else {
            this.brokerVisibleOnCount = false;
            this.customerVisible = false;
            if (!this.campaign.divisionalDatabase)
                this.offerVisibleOnCount = false;
            else
                this.offerVisibleOnCount = true;
        }
    }
}
