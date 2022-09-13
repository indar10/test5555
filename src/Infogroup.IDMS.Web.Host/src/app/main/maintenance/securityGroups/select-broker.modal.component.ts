import { Component, Injector, Input, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { GroupBrokerDto, GroupBrokersServiceProxy, AddBrokerForGroupDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';

@Component({
    selector: 'SelectBrokerModal',
    templateUrl: './select-broker.modal.component.html'

})
export class SelectBrokerModalComponent extends AppComponentBase {
    @Input() databaseId: number;
    @Input() groupId: number;
    active: boolean = false;
    saving: boolean = false;
    filterText: string = '';
    canAdd: boolean = false;
    brokers: any;
    selectedBroker: any = [];

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    groupBroker: GroupBrokerDto = new GroupBrokerDto();
    selectedBrokerData: AddBrokerForGroupDto = new AddBrokerForGroupDto();
    isSave: boolean;

    constructor(
        injector: Injector,
        private _groupBrokerServiceProxy: GroupBrokersServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    getGroupBrokers(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }

        this.isSave = false;
        this.primengTableHelper.showLoadingIndicator();

        this._groupBrokerServiceProxy.getAllBroker(
            this.filterText.trim(),
            this.groupId,
            this.databaseId,
            this.primengTableHelper.getSorting(this.dataTable))
            .subscribe(result => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.brokers = result.items.filter(x => x.isSelected == true);
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    save(): void {
        this.saving = true;
        this.selectedBrokerData.groupID = this.groupId;
        this.selectedBrokerData.selectedBroker = this.primengTableHelper.records;
        this._groupBrokerServiceProxy.addSelectedBroker(this.selectedBrokerData)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.activeModal.close({ saving: this.saving });
            });

    }

    brokerSelected(i): void {
        let broker = this.primengTableHelper.records[i];
        this.primengTableHelper.records[i].isSelected = !broker.isSelected;
    }

    canSave(): void {
        if (this.brokers.length == this.primengTableHelper.records.length) {
            this.primengTableHelper.records.forEach(function (item) {
                item.isSelected = true;
            })
        }
        else {
            this.primengTableHelper.records.forEach(function (item) {
                item.isSelected = false;
            })
        }
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}
