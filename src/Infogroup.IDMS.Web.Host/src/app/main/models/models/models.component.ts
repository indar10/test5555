import { Component, Injector, ViewEncapsulation, ViewChild, EventEmitter,Output } from '@angular/core';
import { ModelsServiceProxy, PageID, ShortSearchServiceProxy, ModelScoringDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditModelModalComponent } from './create-or-edit-models-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { finalize } from "rxjs/operators";
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';
import { ModelStatus } from "../shared/model-status.enum";

@Component({
    templateUrl: './models.component.html',
    styleUrls: ['./models.component.css'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})


export class ModelsComponent extends AppComponentBase {
    @ViewChild('createOrEditModelsModal', { static: true }) createOrEditModelsModal: CreateOrEditModelModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    filterText: string = '';
    isSave: boolean = false;
    pageId: PageID = PageID.Models;
    helpData: any = {
        header: "Search Options:",
        examples: []
    };
    isHelpDisabled: boolean = true;
    statusType = ModelStatus;

    constructor(
        injector: Injector,
        private _modelsServiceProxy: ModelsServiceProxy,
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

    getmodels(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;        

        
        this.primengTableHelper.showLoadingIndicator();
        this._modelsServiceProxy.getAllModels(
            this.filterText.trim(),
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

    createOrEditModelScoring(modelScoringId?: number, isCopy?: boolean, iStatus?: number): void {
        let modalRef: NgbModalRef = null;       
        modalRef = this.modalService.open(
            CreateOrEditModelModalComponent,
            {
                size: ModalDefaults.Size,
                backdrop: ModalDefaults.Backdrop,
                windowClass: ModalDefaults.WindowClass
            });
        
        modalRef.componentInstance.modelScoringId = modelScoringId;
        modalRef.componentInstance.isCopy = isCopy;
        modalRef.componentInstance.iStatus = iStatus;

        modalRef.result.then(params => {
            if (params.saving) {
                if (modelScoringId)
                    this.isSave = true;
                this.getmodels();
            }
        });
    }

    onModelAction(modelScoringId?: number, isSampleScore?: boolean) {
        this._modelsServiceProxy
            .modelsActions(modelScoringId, isSampleScore)
            .subscribe(result => {
                this.notify.info(this.l('SubmitSuccessfully'));
                    this.getmodels();
            });
    }

    onModelActionCancel(modelScoringId?: number) {
        this._modelsServiceProxy
            .modelsActionsCancel(modelScoringId)
            .subscribe(result => {
                this.notify.info(this.l('Cancelled'));
                this.getmodels();
            });
    }
}