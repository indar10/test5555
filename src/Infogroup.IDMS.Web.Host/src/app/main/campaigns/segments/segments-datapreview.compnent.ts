import { Component, Injector, ViewChild, Input} from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SegmentsServiceProxy} from '@shared/service-proxies/service-proxies';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
    selector: 'segments-datapreview',
    templateUrl: './segments-datapreview.component.html'
})
export class SegmentDataPreviewComponent extends AppComponentBase {
   
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() segmentId: any;
    cols: any[];
    active = false;
    saving = false;
    isExportLayout = false;
    dataPreviewDisabled = false;
    segmentDescription: string;
    descriptionTooltip: string;
    segmentDataPreviewData: string;
    downloadSegmentDataPreviewLink: boolean;
    isExportLayoutChecked: boolean;
    downloadDataPreview: string = this.l("DownloadDataPreview");
    
    constructor(
        injector: Injector,
        private _segmentServiceProxy: SegmentsServiceProxy,
        private _fileDownloadService: FileDownloadService,
         public activeModal: NgbActiveModal
    ) {
        super(injector);
    }
    ngOnInit() {
        this.active = true;        
        this.getSegmentDump(null);        
    }
    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
    downloadSegmentDataPreview() {
        this._segmentServiceProxy.downloadDataPreview(this.segmentId, this.isExportLayout).subscribe(result => {
            this._fileDownloadService.downloadFile(result);
       });   
    }
    getOutputLayoutFloatStyle() {
        return 'right';
    }
    getSegmentDump(event): any {
        this.active = true;
        var isExportLayout = this.isExportLayout;
        this.isExportLayout= event!=null && event ? true : false;
        this.primengTableHelper.showLoadingIndicator();

        this._segmentServiceProxy.getRecordDump(this.segmentId,
            this.isExportLayout
        ).pipe(
            catchError(err => {
            return throwError(err);
        })
        ).subscribe(result => {
            this.primengTableHelper.showLoadingIndicator();
            if (result != null && result != undefined) {
                this.dataPreviewDisabled = !result.isExportLayoutCheckBoxVisible;                
                this.segmentDescription = result.description;              
                this.descriptionTooltip = result.tooltipDescription;
                this.cols = [];
                this.cols = result.columns;
                if (result.data != null && result.data != undefined) {
                    var segmentDataPreviewData = JSON.parse(result.data);
                    this.primengTableHelper.records = segmentDataPreviewData;
                    this.primengTableHelper.totalRecordsCount = this.primengTableHelper.records.length;
                }
                this.downloadSegmentDataPreviewLink = this.primengTableHelper.records != null && this.primengTableHelper.records != undefined && this.primengTableHelper.records.length > 0 ? false : true;
                this.primengTableHelper.hideLoadingIndicator();
            }
        }, err => {
            this.primengTableHelper.hideLoadingIndicator();
            this.isExportLayout = isExportLayout;
            this.isExportLayoutChecked = isExportLayout;
        });
    }
}

