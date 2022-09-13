import { Component, Injector, OnInit, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SelectItem} from 'primeng/api';
import { AppConsts } from '@shared/AppConsts';
import { BulkSegmentDto, SegmentSelectionsServiceProxy} from '@shared/service-proxies/service-proxies';
import { HttpClient } from '@angular/common/http';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { finalize} from 'rxjs/operators';

@Component({
    selector: 'bulkSegmentModal',
    templateUrl: './bulk-segment-upload-modal.component.html'
})
export class BulkSegmentModalComponent extends AppComponentBase implements OnInit {
    isDefaultFormatHidden: boolean = true;

    @Input() campaignId: number;
    @Input() databaseId: number;
    downloadTooltip: string;
    isGroupByKeycode1: boolean=false;
    isPopulateKeyCode1: boolean=false;
    bulkSegmentData: BulkSegmentDto = new BulkSegmentDto();
    fileToUpload: File = null;
    active: boolean= false;
    isDisabled: boolean = false;
    saving:boolean= false;
    FormatListOption: SelectItem[] = [{ label: 'Default', value: 0 }, { label: 'Radio/TV Station', value: 1 }];

    constructor(
        injector: Injector,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
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
        this._segmentSelectionProxy.downloadTemplate(!this.isDefaultFormatHidden, this.databaseId).subscribe(result => {
            this._fileDownloadService.downloadDocumentAttachment(result);
        });   
    }
    onSave(): void {
        if (!this.ValidateFileExtension(this.fileToUpload.name)) {
            this.saving = false;
            this.message.error(this.l("InvalidFileTypeTooltip"));
        }
        else {
            const formData: FormData = new FormData();
            this.bulkSegmentData.campaignId = this.campaignId;
            this.bulkSegmentData.isDefaultFormat = this.isDefaultFormatHidden;
            this.bulkSegmentData.isGroupByKeyCode1 = this.isGroupByKeycode1;
            this.bulkSegmentData.isPopulateKeycode = this.isPopulateKeyCode1;
            this.bulkSegmentData.databaseId = this.databaseId;
            formData.append('file', this.fileToUpload, this.fileToUpload.name);
            this.saving = true; 
            var uploadUrl = AppConsts.remoteServiceBaseUrl + '/File/UploadBulkSegment?campaignId=' + this.databaseId;
            this._httpClient
                .post<any>(uploadUrl, formData)
                .pipe(finalize(() => { this.saving = true; }))
                .subscribe(response => {
                    this.saving = true;
                    if (response.success && response.result.message == "") {
                        this.bulkSegmentData.path = response.result.path;
                        this._segmentSelectionProxy.saveBulkSegmentFileData(this.bulkSegmentData).pipe(finalize(() => { this.saving = false; })).subscribe(result => {                     
                            if (result) {
                                this.activeModal.close({ isSave: true });
                            }
                            else
                                this.fileToUpload = null;
                        });
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
    clearCheckBoxes() {
        this.isGroupByKeycode1 = false;
        this.isPopulateKeyCode1 = false;
    }                         
}