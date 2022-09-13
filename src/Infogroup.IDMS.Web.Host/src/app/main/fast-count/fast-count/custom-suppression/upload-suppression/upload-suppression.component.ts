import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NgbActiveModal, NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActionType, CreateOrEditMatchAppendDto, GetQueryBuilderDetails, IDMSConfigurationsServiceProxy, MatchAndAppendInputLayoutDto, MatchAndAppendOutputLayoutDto, MatchAppendDto, MatchAppendsServiceProxy, SegmentSelectionsServiceProxy } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent, SelectItem } from 'primeng/api';
import { HttpClient } from '@angular/common/http';
import { finalize } from "rxjs/operators";
import { CustomSuppressionComponent } from '../custom-suppression.component';
import { ModalDefaults, ModalSize } from '@shared/costants/modal-contants';
@Component({
  selector: 'app-upload-suppression',
  templateUrl: './upload-suppression.component.html',
  styleUrls: ['./upload-suppression.component.css']
})
export class UploadSuppressionComponent extends AppComponentBase {
  uploading = false;
  showError: boolean = false;
  isLoading: boolean = false;
  showUpload: boolean = false;
  fileData: SelectItem[] = [{ label: '.csv', value: 'C' }];
  targetFields: any[] = [];

  csvRecordsArray: any = [];
  idmsKeyData: any[] = [];
  csvheadersArray: any[] = [];

  validFileData: string[] = ['csv'];
  uploadUrl: string;
  uploadedFiles: any[] = [];
  files: any[] = [];
  @Input() matchAppendId: number;
  @Input() campaignId: number;
  @Input() databaseId: number;
  @Input() divisionId: number;
  @Input() buildId: number;
  @Input() mailerId: number;
  @Input() isEdit: number;
  @Input() segmentId: number;
  @Input() id: string;
  @Output() openSuppressionevent = new EventEmitter<string>();
  createOrEditMatchAppendDto: CreateOrEditMatchAppendDto = new CreateOrEditMatchAppendDto();
  activeIndex: number = 0;
  fileNameFromFileUpload = '';
  fileNameFromManual = '';
  formData: FormData = new FormData();
  inputLayputs: MatchAndAppendInputLayoutDto[] = [];
  outputLayputs: MatchAndAppendOutputLayoutDto[] = [];
  idmsKeyFields: SelectItem[] = [];
  inputKeyFields: SelectItem[] = [];
  inputStaticKeyFields: any[] = [];
  displayValidation: boolean = false;
  displayValidationMessage: string = '';
  tempDatabaseId = 0;
  tempBuildId = 0;
  EditFileName:string;
  constructor(injector: Injector, public activeModal: NgbActiveModal, private _matchAppendServiceProxy: MatchAppendsServiceProxy, private _httpClient: HttpClient, private _segmentSelectionProxy: SegmentSelectionsServiceProxy, private _idmsConfig: IDMSConfigurationsServiceProxy, private modalService: NgbModal) { super(injector); }

  ngOnInit() {
    this._segmentSelectionProxy
      .getSelectionFieldsNew(
        this.campaignId,
        "1",
        this.databaseId,
        this.buildId,
        this.mailerId
      )
      .subscribe((data: GetQueryBuilderDetails) => {
        this.idmsKeyData = JSON.parse(data.filterDetails);
      });

    this._idmsConfig.getConfigurationValue("MatchAppendInputKeyMapping", this.databaseId).subscribe(result => {
      this.targetFields = JSON.parse(result.mValue);

    });

    this._idmsConfig.getConfigurationValue("MatchAppendMCFields", this.databaseId).subscribe(result => {
      this.inputStaticKeyFields = JSON.parse(result.mValue);


    });

    this.show(this.matchAppendId);
  }
  close(): void {
    this.activeModal.close();
  }
  onFileDropped($event) {
    this.prepareFilesList($event);
  }
  fileBrowseHandler(files) {
    this.prepareFilesList(files);
  }
  uploadFilesSimulator(index: number) {
    setTimeout(() => {
      if (index === this.files.length) {
        return;
      } else {
        const progressInterval = setInterval(() => {
          if (this.files[index].progress === 100) {
            clearInterval(progressInterval);
            this.uploadFilesSimulator(index + 1);
          } else {
            this.files[index].progress += 5;
          }
        }, 200);
      }
    }, 1000);
  }

  prepareFilesList(files: Array<any>) {
    if (this.files.length > 0)
      this.files.splice(0);
    for (const item of files) {
      item.progress = 0;
      var extensionFile = item.name.split('.')[1];
      if (this.validFileData.indexOf(extensionFile) > -1) {
        this.createOrEditMatchAppendDto.matchAppendDto.cClientFileName = item.name;
        this.files.push(item);
        this.showError = false;
        this.handleFileInput(this.files);
      }
      else {
        this.showError = true;
        this.uploading = false;
      }
    }
    this.uploadFilesSimulator(0);
  }

  formatBytes(bytes, decimals) {
    if (bytes === 0) {
      return '0 Bytes';
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }


  processDatabaseStep(matchAppendDto: MatchAppendDto) {
    if (this.files != undefined && this.files[0] != null) {
      this.isLoading = true
      let reader = new FileReader();
      reader.readAsText(this.files[0]);
      reader.onload = () => {
        let csvData = reader.result;
        let csvRecordsArray = (<string>csvData).split(/\r\n|\n/);
        this.csvheadersArray = this.getHeaderArray(csvRecordsArray);
        this.csvRecordsArray = this.getDataRecordsArrayFromCSVFile(csvRecordsArray, this.csvheadersArray.length);
      };
      reader.onerror = function () {
        console.log('error is occured while reading file!');
      };

      this._matchAppendServiceProxy.processDatabaseSetupStep(matchAppendDto, this.fileNameFromFileUpload, this.fileNameFromManual).pipe(finalize(() => (this.isLoading = false))).subscribe(result => {
        this.createOrEditMatchAppendDto.matchAppendDto = result;
        this.activeIndex = 1;
        this.showUpload = true;
        this.uploading = false;

      });
      this._matchAppendServiceProxy.getBuildTableLayoutFieldByBuildID(this.createOrEditMatchAppendDto.matchAppendDto.buildID.toString()).subscribe(result => {
        this.idmsKeyFields = result;
        this.fillInputKeyFieldDropDown();
      });
    }
    else if (this.matchAppendId && this.isEdit == 1) {
      this.activeIndex = 1;
      this._matchAppendServiceProxy.getBuildTableLayoutFieldByBuildID(this.buildId.toString()).subscribe(result => {
        this.idmsKeyFields = result;
        this.fillInputKeyFieldDropDown();
      });
    }
  }
  handleFileInput(files: any) {
    if (files[0] != null) {
      this.isLoading = true;
      this.formData = new FormData();
      this.formData.append('file', files[0], files[0].name);
      var uploadUrl = AppConsts.remoteServiceBaseUrl + '/File/UploadAttachmentMatchAppend?databaseId=' + this.createOrEditMatchAppendDto.matchAppendDto.databaseID;
      this._httpClient
        .post<any>(uploadUrl, this.formData)
        .subscribe(response => {
          if (response.success) {

            if (response.result.isCreated) {
              this.notify.success(this.l("UploadSuccessFull"));
              this.fileNameFromFileUpload = response.result.sClientFileName;
              this.createOrEditMatchAppendDto.matchAppendDto.cClientFileName = response.result.sClientFileName;
              this.createOrEditMatchAppendDto.matchAppendDto.uploadFilePath = "@#MOVE#@" + response.result.path;
            }
            else {
              this.fileNameFromFileUpload = response.result.fileName;
              this.createOrEditMatchAppendDto.matchAppendDto.cClientFileName = "";

              this.createOrEditMatchAppendDto.matchAppendDto.uploadFilePath = "";
              this.notify.error(response.result.message);
            }


          } else if (response.error != null) {
            this.createOrEditMatchAppendDto.matchAppendDto.cClientFileName = "";
            this.createOrEditMatchAppendDto.matchAppendDto.uploadFilePath = "";
            this.notify.error(this.l("UploadUnSuccessFull"));
          }
          this.uploading = true;
          this.isLoading = false;

        }, () => {
          this.isLoading = false;
        });
    }
  }
  getHeaderArray(csvRecordsArr: any) {
    let headers = (<string>csvRecordsArr[0]).split(',');
    let headerArray = [];
    for (let j = 0; j < headers.length; j++) {
      let headerName = "";
      if (this.createOrEditMatchAppendDto.matchAppendDto.iSkipFirstRow) {
        const searchRegExp = /\s/g;
        const replaceWith = '';
        headerName = headers[j].replace(searchRegExp, replaceWith);
      } else {
        headerName = "Column" + j;
      }
      headerArray.push({
        headerName,
        mappingField: ""
      });
    }
    return headerArray;
  }
  getDataRecordsArrayFromCSVFile(csvRecordsArray: any, headerLength: any) {
    let csvArr = [];
    for (let i = (this.createOrEditMatchAppendDto.matchAppendDto.iSkipFirstRow ? 1 : 0); i < (csvRecordsArray.length > 5 ? 5 : csvRecordsArray.length); i++) {
      let curruntRecord = (<string>csvRecordsArray[i]).split(',');
      if (curruntRecord.length == headerLength) {
        let csvRecord: any = [];
        csvRecord.id = i;
        for (let k = 0; k < headerLength; k++) {
          csvRecord[`col${k}`] = curruntRecord[k].trim();
        }
        csvArr.push(csvRecord);
      }
    }
    return csvArr;
  }

  show(matchAppendId?: number) {

    this.createOrEditMatchAppendDto.matchAppendDto = new MatchAppendDto();
    if (!matchAppendId) {
      this.createOrEditMatchAppendDto.matchAppendDto = new MatchAppendDto();
      this.createOrEditMatchAppendDto.matchAppendDto.databaseID = this.databaseId;
      this.createOrEditMatchAppendDto.matchAppendDto.buildID = this.buildId;
      this.createOrEditMatchAppendDto.matchAppendDto.iSkipFirstRow = false;//CheckBox Change
      this.createOrEditMatchAppendDto.matchAppendDto.lK_FileType = 'CQ';
      this.createOrEditMatchAppendDto.matchAppendDto.cOrderType = 'N';
      this.createOrEditMatchAppendDto.matchAppendDto.lK_ExportFileFormatID = 'FF';
      this.createOrEditMatchAppendDto.matchAppendDto.iExportType = 2;
      this.createOrEditMatchAppendDto.matchAppendDto.cKeyCode = "";
      this.createOrEditMatchAppendDto.matchAppendDto.cSourceFilter = "";
      this.createOrEditMatchAppendDto.matchAppendDto.cInputFilter = "";
      this.primengTableHelperMatchAppendOutputLayout.records = [];

    }
    else if (this.isEdit == 1) {
      this.getMatchAppendForEdit(matchAppendId);

    }
  }

  getMatchAppendInputLayout() {
    if (this.isEdit == 0) {
      this.inputLayputs = [];
      for (let i = 0; i < this.csvheadersArray.length; i++) {
        let matchAppendInputLayout = new MatchAndAppendInputLayoutDto();
        matchAppendInputLayout.cFieldName = this.csvheadersArray[i].headerName;
        matchAppendInputLayout.startIndex = '';
        matchAppendInputLayout.endIndex = '';
        matchAppendInputLayout.dataLength = '';
        matchAppendInputLayout.importLayoutOrder = '';
        if (this.csvheadersArray[i].mappingField) {
          matchAppendInputLayout.cMCMapping = this.csvheadersArray[i].mappingField;
        }
        matchAppendInputLayout.mappingDescription = '';
        this.inputLayputs.push(matchAppendInputLayout);
      }
      this.createOrEditMatchAppendDto.matchAppendInputLayoutList = [];
      this.createOrEditMatchAppendDto.matchAppendInputLayoutList = this.inputLayputs;
    } else {
      this.createOrEditMatchAppendDto.matchAppendInputLayoutList.forEach(item => {

        item.cMCMapping = this.csvheadersArray.find(inputItem => inputItem.headerName == item.cFieldName).mappingField;

      });
    }
  }

  uploadList() {
    let isValidData = false;
    this.isLoading = true;
    this.getMatchAppendInputLayout();
    this.outputLayputs = [];
    let matchAppendOutLayout = new MatchAndAppendOutputLayoutDto();

    if (this.inputStaticKeyFields.some(item => item.value === this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName)) {
      this.displayValidation = false;
      const option = this.inputStaticKeyFields.find(d => d.value == this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName);
      this.createOrEditMatchAppendDto.matchAppendDto.cIDMSMatchFieldName = option.idmsValue;
    }
    else if (this.createOrEditMatchAppendDto.matchAppendInputLayoutList.some(item => item.cFieldName == this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName)) {
      const selectedMappingField = this.createOrEditMatchAppendDto.matchAppendInputLayoutList.find(item => item.cFieldName == this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName);
      const selectedTargetField = this.targetFields.filter(item => item.value).find(item => item.value == selectedMappingField.cMCMapping);
      if (selectedMappingField && selectedTargetField) {
        const idmsKeyFiled = this.idmsKeyFields.find(item => item.value == selectedTargetField.idmsValue);
        this.createOrEditMatchAppendDto.matchAppendDto.cIDMSMatchFieldName = idmsKeyFiled.value;
        this.displayValidation = false;
      }
      else {
        this.displayValidation = true;
      }
    }
    else {
      this.displayValidation = true;
    }
    this.outputLayputs = [];
    matchAppendOutLayout.cTableName = 'Input';
    matchAppendOutLayout.iOutputLayoutOrder = 1;

    if (this.idmsKeyData.some(item => item.data.cFieldName === this.createOrEditMatchAppendDto.matchAppendDto.cIDMSMatchFieldName)) {
      const option = this.idmsKeyData.find(item => item.data.cFieldName === this.createOrEditMatchAppendDto.matchAppendDto.cIDMSMatchFieldName);
      matchAppendOutLayout.cFieldName = this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName;
      matchAppendOutLayout.cOutputFieldName = option.data.cFieldName;
      matchAppendOutLayout.iOutputLength = option.data.iDataLength;
      matchAppendOutLayout.outputLength = option.data.iDataLength;
    }
    else if (this.inputStaticKeyFields.some(item => item.value === this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName)) {
      const option = this.inputStaticKeyFields.find(d => d.value == this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName);
      const optionidmsKeyData = this.idmsKeyData.find(item => item.data.cFieldName === option.output);
      matchAppendOutLayout.cFieldName = this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName;
      matchAppendOutLayout.cOutputFieldName = option.idmsValue;
      matchAppendOutLayout.iOutputLength = optionidmsKeyData.data.iDataLength;
      matchAppendOutLayout.outputLength = optionidmsKeyData.data.iDataLength;
    }
    this.outputLayputs.push(matchAppendOutLayout);
    if (this.isEdit == 1) {
      this.createOrEditMatchAppendDto.matchAppendOutputLayoutList.forEach(item => {
        item.cFieldName = matchAppendOutLayout.cFieldName;
        item.cOutputFieldName = matchAppendOutLayout.cOutputFieldName;
      });
    }
    else {
      this.createOrEditMatchAppendDto.matchAppendOutputLayoutList = [];
      this.createOrEditMatchAppendDto.matchAppendOutputLayoutList = this.outputLayputs;
    }
    if (this.displayValidation == false) {
      this._matchAppendServiceProxy.createEditAndSubmit(this.createOrEditMatchAppendDto).pipe(finalize(() => (this.isLoading = false)))
        .subscribe(result => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.message.info("", "Custom List is submitted and can be selected from 'View Previous Uploads' once completed.");
          this.activeModal.close();
          if (this.isEdit == 0) {
            this.openSuppressionevent.emit("");
          }
        });
    }
    else {
      this.displayValidationMessage = "Please Map " + this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName + " Field"
      this.isLoading = false
    }

  }
  fillInputKeyFieldDropDown() {
    this.inputKeyFields = [];
    this.inputKeyFields.push({ label: "Select...", value: "" });
    for (let i = 0; i < this.csvheadersArray.length; i++) {
      this.inputKeyFields = this.inputKeyFields.concat({ label: this.csvheadersArray[i].headerName, value: this.csvheadersArray[i].headerName });
    }
    for (let i = 0; i < this.inputStaticKeyFields.length; i++) {
      this.inputKeyFields = this.inputKeyFields.concat({ label: this.inputStaticKeyFields[i].label, value: this.inputStaticKeyFields[i].value });
    }
  }
  back() {
    this.activeIndex = 0;
    this.uploading = true;
  }
  getMatchAppendForEdit(matchAppendId: number) {

    this._matchAppendServiceProxy.getMatchAppendForEdit(matchAppendId).subscribe(result => {
      this.tempDatabaseId = 0;
      this.tempBuildId = 0;
      this.createOrEditMatchAppendDto.matchAppendDto = result.matchAppend.matchAppendDto;
      this.tempBuildId = this.createOrEditMatchAppendDto.matchAppendDto.buildID;
      this.tempDatabaseId = this.createOrEditMatchAppendDto.matchAppendDto.databaseID;
      this.createOrEditMatchAppendDto.matchAppendInputLayoutList = result.matchAppend.matchAppendInputLayoutList;
      this.createOrEditMatchAppendDto.matchAppendOutputLayoutList = result.matchAppend.matchAppendOutputLayoutList;
      this.EditFileName=result.matchAppend.matchAppendDto.cClientFileName;
      this.csvheadersArray = [];
      for (let i = 0; i < this.createOrEditMatchAppendDto.matchAppendInputLayoutList.length; i++) {
        this.csvheadersArray.push({
          headerName: this.createOrEditMatchAppendDto.matchAppendInputLayoutList[i].cFieldName,
          mappingField: this.createOrEditMatchAppendDto.matchAppendInputLayoutList[i].cMCMapping ? this.createOrEditMatchAppendDto.matchAppendInputLayoutList[i].cMCMapping : ''
        });
      }
    })
  }
}
