import { Component, Input, Injector, ViewEncapsulation, ViewChild, ElementRef, EventEmitter, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { IdmsUserLoginInfoDto, DivisionMailersServiceProxy, GetAllDivisionMailersInput } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { NgbModal, NgbModalOptions, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { CreateorEditDivisionalMailerComponent } from './create-or-edit-divisional-mailer.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ModalDefaults } from '@shared/costants/modal-contants';


@Component({
    selector: 'app-divisional-mailer',
    templateUrl: './divisional-mailer.component.html',    
    animations: [appModuleAnimation()]
})
export class DivisionalMailerComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    idmsUserData: IdmsUserLoginInfoDto;
    filterText = '';
    excelTextFilter = '';
    excelIsActiveFilter = '';
    isSave: boolean = false;
    inputDto: GetAllDivisionMailersInput = new GetAllDivisionMailersInput();

    constructor(
        injector: Injector,
        private _divisionMailersServiceProxy: DivisionMailersServiceProxy,
        private _fileDownloadService: FileDownloadService,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    ngOnInit() {
    }

    getAllDivisionMailer(event?: LazyLoadEvent) {
        let isActiveChecked = $('#IsActivetogBtn:checkbox:checked').length > 0;
        this.idmsUserData = this.appSession.idmsUser;

        if (this.idmsUserData) {
            this.getFilteredDivisionMailer(isActiveChecked, event)
        }
        else {
            this.primengTableHelper.totalRecordsCount = 0;
            this.primengTableHelper.records = undefined;
        }
    }

    getFilteredDivisionMailer(isActiveChecked: boolean, event?: LazyLoadEvent): void {
        let isActiveFilter = "1";

        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;
        if (isActiveChecked)
            isActiveFilter = "1";
        else
            isActiveFilter = "0";

        // Call the service method
        this.excelTextFilter = this.filterText;
        this.excelIsActiveFilter = isActiveFilter;
        this.primengTableHelper.showLoadingIndicator();
        this._divisionMailersServiceProxy.getAllDivisionMailerList(
            this.filterText,
            isActiveFilter,            
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }



    openCreateorEditDivisionalMailer(divisionMailerId?: number): void {
        let modalRef: NgbModalRef = this.modalService.open(CreateorEditDivisionalMailerComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
        modalRef.componentInstance.divisionMailerId = divisionMailerId;
        modalRef.result.then((params) => {
            if (params.isSave) {
                if (divisionMailerId) 
                    this.isSave = true;                
                this.getAllDivisionMailer();
            }
        }
        );
    }

    excelDivisionMailers(event?: LazyLoadEvent): void {
        this.inputDto.filter = this.excelTextFilter;
        this.inputDto.isActive = this.excelIsActiveFilter;
        this.inputDto.sorting = null;
        this.inputDto.skipCount = 0;
        this.inputDto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;

        this._divisionMailersServiceProxy.divisionMailerExcel(this.inputDto).subscribe(result => {
            this._fileDownloadService.downloadTempFile(result)
        });
    }
}
