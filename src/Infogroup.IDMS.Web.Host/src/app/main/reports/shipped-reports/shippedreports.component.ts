import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ShippedReportServiceProxy, PageID, ShortSearchServiceProxy, GetShippedReportInput} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { finalize } from "rxjs/operators";
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import * as _ from 'lodash';
import { FileDownloadService } from '@shared/utils/file-download.service';



@Component({
    templateUrl: './shippedreports.component.html',
    styleUrls: ['./shippedreports.component.css'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})


export class ShippedReportComponent extends AppComponentBase {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    filterText: string = '';
    isSave: boolean = false;
    pageId: PageID = PageID.ShippedReports;
    helpData: any = {
        header: "Search Options:",
        examples: []
    };
    isHelpDisabled: boolean = true;
    dto: GetShippedReportInput = new GetShippedReportInput();


    constructor(
        injector: Injector,
        private _shippedReportServiceProxy: ShippedReportServiceProxy,
        private _fileDownloadService: FileDownloadService,
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

    getShippedReports(event?: LazyLoadEvent):void {
        if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
            this.paginator.changePage(0);
            return;
        }
        this.isSave = false;        

        
        this.primengTableHelper.showLoadingIndicator();
        this._shippedReportServiceProxy.getAllShippedReports(
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
    downloadShippedReport():void {
        this.dto.filter = this.filterText.trim();
        this.dto.sorting = null;
        this.dto.skipCount = 0;
        this.dto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;
        this._shippedReportServiceProxy.downloadShippedReport(this.dto).subscribe(result => {
            this._fileDownloadService.downloadFile(result);
        });
    }

}