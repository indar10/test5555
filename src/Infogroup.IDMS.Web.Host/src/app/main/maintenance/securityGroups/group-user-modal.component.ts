import { Component, Injector, Input, ViewChild } from '@angular/core';
import { SecurityGroupsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';

@Component({
    selector: 'GroupUserModal',
    templateUrl: './group-user-modal.component.html'

})
export class GroupUserModalComponent extends AppComponentBase {
    @Input() groupId: number;
    @Input() groupName: string;
    filterText: string = '';
    active = false;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    isSave: boolean;
    saving: any;

    constructor(
        injector: Injector,
        private _securityGroupServiceProxy: SecurityGroupsServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    getGroupUsers(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }

        this.isSave = false;
        this.primengTableHelper.showLoadingIndicator();

        this._securityGroupServiceProxy.getAllUserCount(
            this.filterText.trim(),
            this.groupId,
            this.primengTableHelper.getSorting(this.dataTable))
            .subscribe(result => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }    

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}
