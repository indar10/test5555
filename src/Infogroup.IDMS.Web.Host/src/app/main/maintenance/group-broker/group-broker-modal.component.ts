import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { GroupBrokersServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'groupbroker',
    templateUrl: './group-broker-modal.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class GroupBrokersComponent extends AppComponentBase {
    @Input() groupId: number;
    @Input() databaseId: number;
    isSave: boolean = false;
    filterText = '';

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    constructor(
        injector: Injector,
        private _groupBrokerServiceProxy: GroupBrokersServiceProxy,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    getGroupBrokers(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;
        this.primengTableHelper.showLoadingIndicator();

        this._groupBrokerServiceProxy.getAllGroupBroker(
            this.filterText.trim(),
            this.groupId,
            this.databaseId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }
}
