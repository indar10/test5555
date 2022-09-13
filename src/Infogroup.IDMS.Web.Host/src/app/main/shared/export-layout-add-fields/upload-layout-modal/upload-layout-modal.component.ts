import { Component, Injector, OnInit, Input, ElementRef, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppConsts } from '@shared/AppConsts';
import { BulkSegmentDto, ExportLayoutsServiceProxy, ImportLayoutDto} from '@shared/service-proxies/service-proxies';
import { HttpClient } from '@angular/common/http';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { finalize} from 'rxjs/operators';
import { result } from 'lodash';
import { SelectItem } from 'primeng/api';

@Component({
    selector: 'uploadLayoutModal',
    templateUrl: './upload-layout-modal.component.html'
})
export class UploadLayoutModalComponent extends AppComponentBase implements OnInit {
    isDefaultFormatHidden: boolean = true;

    @Input() exportLayoutId: number;
    @Input() databaseId: number;
    @Input() campaignId: number;
    @Input() buildId: number;
    @Input() isCampaign: boolean = false;

    downloadTooltip: string;
    bulkSegmentData: BulkSegmentDto = new BulkSegmentDto();
    fileToUpload: File = null;
    active: boolean= false;
    isDisabled: boolean = false;
    saving:boolean= false;
    @ViewChild('fileInput',{static: false}) fileControl: ElementRef;

    constructor(
        injector: Injector,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
        private _httpClient: HttpClient,
        private _fileDownloadService: FileDownloadService,
        public activeModal: NgbActiveModal,
    ) {
        super(injector);
    }
    ngOnInit() {
        this.active = true;
        this.isDisabled = true;
        this.downloadTooltip = this.l("DownloadTemplateTooltip");
    }
    handleFileInput(files: FileList) {
        this.fileToUpload = files.item(0);
        if (this.fileToUpload === null)
            this.isDisabled = true;
        else
            this.isDisabled = false;
    }
    onDownloadTemplate(): void {
        this._exportLayoutServiceProxy.getTemplatePath(this.databaseId)
            .subscribe(
                result => {
                    this._fileDownloadService.downloadDocumentAttachment(result);
                }
            )
    }

    onSave(): void {
        if (!this.ValidateFileExtension(this.fileToUpload.name)) {
            this.saving = false;
            this.message.error(this.l("InvalidFileTypeTooltip"));
        }
        else {
            const formData: FormData = new FormData();
            const input = new ImportLayoutDto();
            input.layoutId = this.exportLayoutId;
            formData.append('file', this.fileToUpload, this.fileToUpload.name);
            this.saving = true;
            var uploadUrl = AppConsts.remoteServiceBaseUrl + '/File/UploadLayout?databaseId=' + this.databaseId;
            this._httpClient
                .post<any>(uploadUrl, formData)
                .pipe(finalize(() => { this.saving = true; }))
                .subscribe(response => {
                    this.saving = true;
                    if (response.success && response.result.message == "") {
                        input.path = response.result.path;                       
                        this._exportLayoutServiceProxy.importLayout(input,this.buildId,this.campaignId,this.isCampaign,this.databaseId).pipe(finalize(() => { this.saving = false; })).subscribe(() => {
                            this.activeModal.close({ isSave: true });
                        },
                            ()=> {
                                this.fileControl.nativeElement.value = "";
                                this.fileToUpload = null;
                            }
                        );
                    }
                    else if (response.result.message != undefined && response.result.message != "")
                        this.message.error(response.result.message);
                });
        }
    }

    ValidateFileExtension(cFileName: string): boolean {
        var flag = true;
        var validExts = new Array(".xlsx", ".xls");
        var fileExt = cFileName;
        fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
        if (validExts.indexOf(fileExt) < 0)
            flag = false;
        return flag;
    }
    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}