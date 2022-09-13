import { Component, Injector, Input, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { SelectItem } from 'primeng/api';
import {
    ModelsServiceProxy,
    CreateOrEditModelDto,
    GetModelScoringDropdownDto,
    ModelSummaryDto,
    ModelDetailDto,
    GetModelTypeAndWeightDto
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { Paginator } from 'primeng/components/paginator/paginator';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { ModelStatus } from "../shared/model-status.enum";

@Component({
    selector: 'createOrEditModelModal',
    templateUrl: './create-or-edit-models-modal.component.html',
    styleUrls: ['./create-or-edit-models-modal.component.css']
})
export class CreateOrEditModelModalComponent extends AppComponentBase {
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('myEditor', { static: false }) Editor: any;
    databases: SelectItem[] = [];
    builds: SelectItem[] = [];
    @Input() modelScoringId: number;
    @Input() isCopy: boolean;
    @Input() iStatus: number;
    formData: FormData = new FormData();
    fileToUpload: any = null;
    active = false;
    saving = false;
    isDisabled: boolean = false;  
    statusType = ModelStatus;
    dropdownData: GetModelTypeAndWeightDto = GetModelTypeAndWeightDto.fromJS({
        modelType: [],
        modelGiftWeight: []
    });
    modelScoring: CreateOrEditModelDto = new CreateOrEditModelDto();

    constructor(
        injector: Injector,
        private _modelsServiceProxy: ModelsServiceProxy,
        public activeModal: NgbActiveModal,
        private _httpClient: HttpClient,
    ) {
        super(injector);
    }
    ngOnInit() {
        this.show(this.modelScoringId);
        this.loadData();
    }

    loadData() {
        this._modelsServiceProxy.getModelTypeAndWeight().subscribe(result => this.dropdownData = result);
    }
    
    handleFileInput(files: FileList) {
        this.fileToUpload = files.item(0);
    }
    
   
    show(modelScoringId?: number): void {
        if (!modelScoringId) {
            this.modelScoring = new CreateOrEditModelDto();
            this.modelScoring.modelSummaryData = new ModelSummaryDto();
            this.modelScoring.modelDetailData = new ModelDetailDto();
            this.modelScoring.modelSummaryData.iIsActive = true;
            this.active = true;
            let currentDatabaseId = this.appSession.idmsUser.currentCampaignDatabaseId;
            this._modelsServiceProxy.getDatabaseWithLatestBuild(currentDatabaseId).subscribe(
                (result) => {
                    this.setControlOptions(result);
                }
            ); 
        }
        else {
            if (this.iStatus == this.statusType.SampleSubmitted || this.iStatus == this.statusType.SampleRunning || this.iStatus == this.statusType.DatabaseSubmitted || this.iStatus == this.statusType.DatabaseRunning)
                this.isDisabled = true;
            if (this.isCopy) {
                this.modelScoring.isCopyModel = this.isCopy;
            }
            this._modelsServiceProxy.getModelForEdit(this.modelScoringId, this.isCopy).subscribe(result => { 
                this.active = true;
                this.modelScoring.id = modelScoringId;
                this.modelScoring.modelSummaryData = result.modelSummaryData;
                this.modelScoring.modelDetailData = result.modelDetailData;
                this._modelsServiceProxy.getDatabaseWithLatestBuild(this.modelScoring.modelSummaryData.databaseId).subscribe(
                    (result) => {
                        this.setControlOptions(result);                       
                    }
                ); 
                
            })
        }
    }

    setControlOptions(result: GetModelScoringDropdownDto): void {
        if (result.databases)
            this.databases = result.databases.databases;

        this.builds = result.builds.buildDropDown;
        if (!this.modelScoringId) {
            if (result.databases)
                this.modelScoring.modelSummaryData.databaseId = result.databases.defaultDatabase;
            this.modelScoring.modelDetailData.buildID = result.builds.defaultSelection;
        }
        else if (this.isCopy)
            this.modelScoring.modelDetailData.buildID = result.builds.defaultSelection;
    }

    onDatabaseChange(event): void {
        this._modelsServiceProxy.getFieldsOnDatabaseChange(
            event.value
        ).subscribe(result => {
            this.setControlOptions(result);
        });
    }

    save(modelScoringForm: NgForm): void {
        if (modelScoringForm.dirty || this.isCopy || this.fileToUpload != null) {
            this.saving = true;
            if (this.fileToUpload != null) {
                this.formData.append('file', this.fileToUpload, this.fileToUpload.name);
                var uploadUrl = AppConsts.remoteServiceBaseUrl + '/File/UploadAttachmentScoreFile?databaseId=' + this.modelScoring.modelSummaryData.databaseId;
                this._httpClient
                    .post<any>(uploadUrl, this.formData)
                    .subscribe(response => {
                        if (response.success) {
                            this.notify.success(this.l("UploadSuccessFull"));
                            this.modelScoring.modelDetailData.cSAS_ScoreRealFileName = response.result.cFileName;
                            this.modelScoring.modelDetailData.cSAS_ScoreFileName = response.result.fileName;
                            this._modelsServiceProxy.create(this.modelScoring)
                                .pipe(finalize(() => { this.saving = false; }))
                                .subscribe(() => {
                                    this.notify.info(this.l('SavedSuccessfully'));
                                    this.activeModal.close({ saving: this.saving });
                                });
                        } else if (response.error != null) {
                            this.notify.error(this.l("UploadUnSuccessFull"));
                        }
                    });
            }
            else {
                this._modelsServiceProxy.create(this.modelScoring)
                    .pipe(finalize(() => { this.saving = false; }))
                    .subscribe(() => {
                        this.notify.info(this.l('SavedSuccessfully'));
                        this.activeModal.close({ saving: this.saving });
                    });
            }
            
            
        }
        else {
            this.notify.info(this.l('SavedSuccessfully'));
            this.activeModal.close({ saving: this.saving });
        }
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}
