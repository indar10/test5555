import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { PageID, ShortSearchServiceProxy, MatchAppendsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { finalize } from "rxjs/operators";
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';

import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { MatchAppendStatus } from '../shared/match-append-status.enum';
import { CreateOrEditMatchAppendModalComponent } from './create-or-edit-match-append-modal.component';
import { ModalDefaults, ModalSize } from '@shared/costants/modal-contants';

@Component({
    templateUrl: './match-append.component.html',
    styleUrls: ['./match-append.component.css'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})


export class MatchAppendComponent extends AppComponentBase {
   
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    filterText: string = '';
    isSave: boolean = false;
    pageId: PageID = PageID.MatchAppend;
    helpData: any = {
        header: "Search Options:",
        examples: []
    };
    isHelpDisabled: boolean = true;
    statusType = MatchAppendStatus;
    

    constructor(
        injector: Injector,
        private _matchAppendServiceProxy: MatchAppendsServiceProxy,
        private modalService: NgbModal,
        private _shortSearchServiceProxy: ShortSearchServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
            .subscribe(result => {
                this.helpData = result;
                this.isHelpDisabled = false;
            });
    }

    clearFilters() {
        this.filterText = "";        
        $("#togBtn").prop("checked", true);
        this.getMatchAppendTasks();
    }
    getMatchAppendTasks(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;        
        let userNameFilter = this.appSession.idmsUser.idmsUserName;        
        let myCampaignChecked = $("#togBtn:checkbox:checked").length > 0;
        if (!myCampaignChecked) {            
            userNameFilter = "";
        }
        
        this.primengTableHelper.showLoadingIndicator();
        this._matchAppendServiceProxy.getAllMatchAppendTasks(
            this.filterText.trim(),
            userNameFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
        

    }

    copyMatchAppendTasks(matchAppendId: number) {
        this.message.confirm('',this.l("CopyTaskMessage", matchAppendId.toString()), isConfirmed => {
            if (isConfirmed) {
                this._matchAppendServiceProxy.copyMatchAppendTask(matchAppendId).subscribe(
                    result => {
                        this.notify.info(this.l("MatchAppendCopySuccessful"));
                        this.getMatchAppendTasks();
                    });
            }
        });
    }

    submitOrUnlockTask(matchAppendId: number, statusId: number, isSubmit: boolean) {
        let submitUnlockText = isSubmit ? "submit" : "unlock";
        this.message.confirm('',this.l("SubmitUnlockTaskMessage", submitUnlockText), isConfirmed => {
            if (isConfirmed) {
                this._matchAppendServiceProxy.submitUnlockMatchAppendTask(matchAppendId, isSubmit).subscribe(
                    result => {
                        if (isSubmit) this.notify.info(this.l("MatchAppendSubmitSuccessful"));
                        else this.notify.info(this.l("MatchAppendUnlock"));
                        this.getMatchAppendTasks();
                    });
            }
        });
    }
    createOrEditMatchAppend(matchAppendId?: number,statusId?:number): void {
        let modalRef: NgbModalRef = null;
        modalRef = this.modalService.open(
            CreateOrEditMatchAppendModalComponent,
            {
                size: ModalSize.EXTRA_LARGE,
                backdrop: ModalDefaults.Backdrop,
                windowClass: ModalDefaults.WindowClass
            });

        modalRef.componentInstance.matchAppendId = matchAppendId;
        modalRef.componentInstance.iStatus = statusId;

        modalRef.result.then(params => {
            if (params.saving) {
                if (matchAppendId)
                    this.isSave = true;
                this.getMatchAppendTasks();
            }
        });
    }

   
}