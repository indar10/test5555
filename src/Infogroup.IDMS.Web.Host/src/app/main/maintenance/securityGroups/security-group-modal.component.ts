import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { SecurityGroupsServiceProxy, DatabasesServiceProxy, GetAllSecurityGroupsInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { SelectItem } from 'primeng/api';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults, ModalSize } from '@shared/costants/modal-contants';
import { CreateOrEditSecurityGroupModalComponent } from './create-or-edit-security-group-modal.component';
import { SelectBrokerModalComponent } from './select-broker.modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { GroupUserModalComponent } from './group-user-modal.component';

@Component({
    templateUrl: './security-group-modal.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class SecurityGroupsComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    selectedDatabase: number;
    databaseList: SelectItem[] = [];
    filterText: string = '';
    isSave: boolean = false;
    securityGroupDto : GetAllSecurityGroupsInput = new GetAllSecurityGroupsInput();

    constructor(
        injector: Injector,
        private _securityGroupsServiceProxy: SecurityGroupsServiceProxy,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.fillDatabaseDropdown();
    }

    fillDatabaseDropdown() {
        this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
            this.databaseList = result;
            this.selectedDatabase = this.databaseList[0].value;
            this.getSecurityGroups();
        });
    }

    getSecurityGroups(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;       

        if (this.selectedDatabase > 0) {
            this.primengTableHelper.showLoadingIndicator();
            this._securityGroupsServiceProxy.getAllSecurityGroups(
                this.filterText.trim(),
                this.selectedDatabase,
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

    createSecurityGroup(id): void {
        const modalRef: NgbModalRef = this.modalService.open(CreateOrEditSecurityGroupModalComponent, { size: ModalSize.DEFAULT, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.groupId = id;
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id)
                    this.isSave = true;
                this.getSecurityGroups();
            }
        }
        );
    }

    selectBroker(id): void {
        const modalRef: NgbModalRef = this.modalService.open(SelectBrokerModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.groupId = id;
        modalRef.componentInstance.databaseId = this.selectedDatabase;
        modalRef.result.then((result) => {
            if (result.saving) {
                if (id)
                    this.isSave = true;
                this.getSecurityGroups();
            }
        }
        );
    }

    allExportToExcel(groupID: number): void {
        if (groupID > 0)
            this.securityGroupDto.filter = groupID.toString();
        else
            this.securityGroupDto.filter = this.filterText.trim();
        
        this.securityGroupDto.selectedDatabase = this.selectedDatabase;
        this.securityGroupDto.sorting = null;
        this.securityGroupDto.skipCount = 0;
        this.securityGroupDto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;

        this._securityGroupsServiceProxy.exportToExcel(this.securityGroupDto)
            .subscribe(result => {
                this._fileDownloadService.downloadFile(result)
            });
    }

    getUserCountData(group): void {
        const modalRef: NgbModalRef = this.modalService.open(GroupUserModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.groupId = group.id;
        modalRef.componentInstance.groupName = group.cGroupName;
    }
}
