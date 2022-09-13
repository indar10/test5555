import { Component, ViewChild, Injector, Input } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { OfferSamplesServiceProxy, CreateOrEditOfferSampleDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
    selector: 'createOrEditOfferSampleModal',
    templateUrl: './create-or-edit-offerSample-modal.component.html'
})
export class CreateOrEditOfferSampleModalComponent extends AppComponentBase {

    @ViewChild('fileInput', { static: false }) fileInput;

    @Input() Id: number;
    @Input() OfferId: number;
    @Input() MailerCompany: string;

    active = false;
    saving = false;
    offerSample: CreateOrEditOfferSampleDto;
    showUploadFile: boolean = false;

    constructor(
        injector: Injector,
        private _offerSamplesServiceProxy: OfferSamplesServiceProxy,
        private _httpClient: HttpClient,
        private _fileDownloadService: FileDownloadService,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.Id);
    }

    show(offerSampleId?: number): void {
        if (!offerSampleId) {
            this.offerSample = new CreateOrEditOfferSampleDto();
            this.offerSample.id = offerSampleId;
            this.offerSample.mailerCompany = this.MailerCompany;
            this.offerSample.offerId = this.OfferId;
            this.active = true;
        } else {
            this._offerSamplesServiceProxy.getOfferSampleForEdit(offerSampleId).subscribe(result => {
                this.offerSample = result;
                this.offerSample.mailerCompany = this.MailerCompany;
                this.active = true;
            });
        }
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }

    save(): void {
        this.saving = true;
        if (this.fileInput != null) {
            var fi = this.fileInput.nativeElement;
            var databaseId = $('#selectedDatabase option:selected').val();
            var formData: FormData = new FormData();
            var file = fi.files[0];
            formData.append('file', file, file.name);
            var uploadUrl = AppConsts.remoteServiceBaseUrl + '/File/UploadAttachedFile?databaseId=' + databaseId;
            this._httpClient
                .post<any>(uploadUrl, formData)
                .subscribe(response => {
                    if (response.success) {
                        this.saveOfferSample(response);
                    }
                    else if (response.error != null) {
                        this.notify.error("Upload Unsuccessful");
                    }
                });
        }
        else {
            this.saveOfferSample();
        }
    }

    saveOfferSample(response?: any): void {
        var sClientFileName = !response ? response : response.result.sClientFileName;
        var path = !response ? response : response.result.path;
        this._offerSamplesServiceProxy.createOrEdit(this.offerSample, sClientFileName, path)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.activeModal.close({ saving: this.saving });
            });
    }

    showUpload(): void {
        this.showUploadFile = true;
    }

    downloadOfferSample(offerSampleId): void {
        event.preventDefault();
        event.stopPropagation();
        var databaseId = $('#selectedDatabase option:selected').val();
        this._offerSamplesServiceProxy.downloadOfferSample(+databaseId, offerSampleId).subscribe(result => {

            this._fileDownloadService.downloadFile(result);
        });
    }
}
