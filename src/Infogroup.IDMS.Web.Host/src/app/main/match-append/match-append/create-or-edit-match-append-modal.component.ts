import { Component, Injector, Input, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { SelectItem, LazyLoadEvent } from 'primeng/api';
import {
    CreateOrEditMatchAppendDto,
    MatchAppendDto,
    MatchAndAppendInputLayoutDto,
    MatchAndAppendOutputLayoutDto,
    MatchAppendsServiceProxy,
    ActionType
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { Paginator } from 'primeng/components/paginator/paginator';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { MenuItem } from 'primeng/api';
import { Table } from 'primeng/table';


@Component({
    selector: 'createOrEditMatchAppendModal',
    templateUrl: './create-or-edit-match-append-modal.component.html',
    styleUrls: ['./create-or-edit-match-append-modal.component.css']
})
export class CreateOrEditMatchAppendModalComponent extends AppComponentBase {
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('myEditor', { static: false }) Editor: any;
    databases: SelectItem[] = [];
    builds: SelectItem[] = [];
    @Input() matchAppendId: number;
    @Input() isCopy: boolean;
    @Input() iStatus: number;
    formData: FormData = new FormData();
    fileToUpload: any = null;
    active = false;
    saving = false;
    isDisabled: boolean = false;
    fileNameFromFileUpload = '';
    fileNameFromManual = ''
    fileUploadStyle = '';
    fileFreeTextStyle = '';
    importFieldsText = '';
    selectedAvailableFields: string[] = [];

    createOrEditMatchAppendDto: CreateOrEditMatchAppendDto = new CreateOrEditMatchAppendDto();
    items: MenuItem[];
    activeIndex: number = 0;
    fileTypes: SelectItem[] = [];
    outputTypes: SelectItem[] = [];
    idmsKeyFields: SelectItem[] = [];
    inputKeyFields: SelectItem[] = [];
    sourceList: SelectItem[] = [];
    availabelFields: SelectItem[] = [];
    selectedTable: any;
    isOrderDisplay: boolean;
    tempDatabaseId = 0;
    tempBuildId = 0;
    mappings: SelectItem[] = [{ label: 'Select...', value: '' },
    { label: 'First Name', value: 'F' },
    { label: 'Last Name', value: 'L' },
    { label: 'Company', value: 'C' },
    { label: 'Address Line 1', value: 'A' },
    { label: 'Zip', value: 'Z' }
    ];
    exportTypes: SelectItem[] = [
        { label: 'Matches Only', value: 0 },
        { label: 'Non-Matches Only', value: 1 },
        { label: 'Matches and Non-Matches', value: 2 }

    ];
    isPopOverOpen: boolean = false;
    inputLayputs: MatchAndAppendInputLayoutDto[] = [];


    constructor(
        injector: Injector,
        private _matchAppendServiceProxy: MatchAppendsServiceProxy,
        public activeModal: NgbActiveModal,

        private _httpClient: HttpClient,
    ) {
        super(injector);
    }
    ngOnInit() {
        this.active = true;
        this.items = [{
            label: this.l("MatchAppendDatabaseSetupStep"),
            command: (event: any) => {
                this.activeIndex = 0;
            }
        },
        {
            label: this.l("MatchAppendInputLayoutStep"),
            command: (event: any) => {
                this.activeIndex = 1;

            }
        },
        {
            label: this.l("MatchAppendOutputLayoutOptionStep"),
            command: (event: any) => {
                this.activeIndex = 2;
            }
        },
        {
            label: this.l("MatchAppendOutputLayoutStep"),
            command: (event: any) => {
                this.activeIndex = 3;
            }
        }

        ];
        this.show(this.matchAppendId);
        this._matchAppendServiceProxy.getMatchAppendDatabasesBasedOnUserId(this.appSession.idmsUser.idmsUserID).subscribe(result => {
            this.databases = result;
        });
        this._matchAppendServiceProxy.getMatchAppendFileTypes().subscribe(result => {
            this.fileTypes = result;
        })
        this._matchAppendServiceProxy.getMatchAppendOutputTypes().subscribe(result => {
            this.outputTypes = result;

        })


    }

    show(matchAppendId?: number) {
        this.createOrEditMatchAppendDto.matchAppendDto = new MatchAppendDto();
        this.createOrEditMatchAppendDto.matchAppendInputLayout = new MatchAndAppendInputLayoutDto();
        this.createOrEditMatchAppendDto.matchAppendOutputLayout = new MatchAndAppendOutputLayoutDto();

        if (!matchAppendId) {

            this.createOrEditMatchAppendDto.matchAppendDto.databaseID = 0;
            this.createOrEditMatchAppendDto.matchAppendDto.lK_FileType = 'C';
            this.primengTableHelperMatchAppendOutputLayout.records = [];
            this.createOrEditMatchAppendDto.matchAppendDto.iSkipFirstRow = false;
            this.createOrEditMatchAppendDto.matchAppendDto.cOrderType = 'N';
            this.createOrEditMatchAppendDto.matchAppendDto.lK_ExportFileFormatID = 'FF';
            this.createOrEditMatchAppendDto.matchAppendDto.iExportType = 0;
            this.createOrEditMatchAppendDto.matchAppendDto.cKeyCode = "";
            this.createOrEditMatchAppendDto.matchAppendDto.cSourceFilter = "";
            this.createOrEditMatchAppendDto.matchAppendDto.cInputFilter = "";
            if (this.createOrEditMatchAppendDto.matchAppendDto.lK_FileType == 'A') {
                this.isOrderDisplay = false;
            }
            else {
                this.isOrderDisplay = true;
            }
            this.getLatestBuild(this.createOrEditMatchAppendDto.matchAppendDto.databaseID,false);
        }
        else {

            this.getMatchAppendForEdit();

        }
    }

    checkForOutputlayoutlistWhenDatabaseChange(flag, databaseId) {
        

            this.message.confirm('', this.l("MatchAppendOutputListClearOnDatabaseChange"), isConfirmed => {
                if (isConfirmed) {
                    this.createOrEditMatchAppendDto.matchAppendDto.databaseID = databaseId;
                    for (var i = 0; i < this.primengTableHelperMatchAppendOutputLayout.records.length; i++) {

                        this.primengTableHelperMatchAppendOutputLayout.records[i].actionType = ActionType.Delete;
                    }
                    this._matchAppendServiceProxy.getLatestBuildFromDatabaseId(this.createOrEditMatchAppendDto.matchAppendDto.databaseID).subscribe(result => {
                        if (result.length <= 0) {
                            this.builds = [];
                            this.createOrEditMatchAppendDto.matchAppendDto.buildID = 0;
                        }
                        else {
                            this.builds = result;
                            this.createOrEditMatchAppendDto.matchAppendDto.buildID = this.builds[0].value;
                        }
                    });
                }
                else {
                    this.createOrEditMatchAppendDto.matchAppendDto.databaseID = this.tempDatabaseId;
                    this.createOrEditMatchAppendDto.matchAppendDto.buildID = this.tempBuildId;
                    this._matchAppendServiceProxy.getLatestBuildFromDatabaseId(this.createOrEditMatchAppendDto.matchAppendDto.databaseID).subscribe(result => {
                        if (result.length <= 0) {
                            this.builds = [];
                            this.createOrEditMatchAppendDto.matchAppendDto.buildID = 0;
                        }
                        else {
                            this.builds = result;
                        }
                    });
                    return;
                }
            });

        }
    

    getLatestBuild(databaseId, flag) {
        if (flag) {
            this.createOrEditMatchAppendDto.matchAppendDto.cIDMSMatchFieldName = '';
    }
    if (flag && this.matchAppendId != 0) {
        this.checkForOutputlayoutlistWhenDatabaseChange(flag, databaseId);
    }
        this._matchAppendServiceProxy.getLatestBuildFromDatabaseId(databaseId).subscribe(result => {
            if (result.length <= 0) {
                this.builds = [];
                this.createOrEditMatchAppendDto.matchAppendDto.buildID = 0;
            }
            else {
                this.builds = result;
                if (this.matchAppendId == 0) {
                    
                    this.createOrEditMatchAppendDto.matchAppendDto.buildID = this.builds[0].value;
                }
            }
        });
        this._matchAppendServiceProxy.getReadyToLoadPathFromConfiguration(databaseId).subscribe(result => {
            this.fileNameFromManual = result;
        })


    }

    getSelectedBuild(event) {
        this.createOrEditMatchAppendDto.matchAppendDto.buildID = event;
    }

    processDatabaseStep(matchAppendDto: MatchAppendDto) {
        this._matchAppendServiceProxy.processDatabaseSetupStep(matchAppendDto, this.fileNameFromFileUpload, this.fileNameFromManual).subscribe(result => {
            this.createOrEditMatchAppendDto.matchAppendDto = result;
            this.primengTableHelper.records = [];
            if (this.createOrEditMatchAppendDto.matchAppendInputLayoutList) {
                this.primengTableHelper.records = this.createOrEditMatchAppendDto.matchAppendInputLayoutList;
            }
            this.activeIndex = 1;

            //this.addRows();
        });
    }

    processInputLayoutStep(isNext: boolean) {

        this._matchAppendServiceProxy.processInputLayoutStep(this.primengTableHelper.records, this.createOrEditMatchAppendDto.matchAppendDto.lK_FileType, this.createOrEditMatchAppendDto.matchAppendDto.databaseID).subscribe(result => {
            if (isNext) {
                this.activeIndex = 2;
                this._matchAppendServiceProxy.getBuildTableLayoutFieldByBuildID(this.createOrEditMatchAppendDto.matchAppendDto.buildID.toString()).subscribe(result => {
                    this.idmsKeyFields = result;

                })
                this.createOrEditMatchAppendDto.matchAppendInputLayoutList = this.primengTableHelper.records;
                this.primengTableHelper.records = this.createOrEditMatchAppendDto.matchAppendInputLayoutList;
                this.fillInputKeyFieldDropDown(this.primengTableHelper.records);
            }
            else {
                this.createOrEditMatchAppendDto.matchAppendInputLayoutList = this.primengTableHelper.records;
                this.primengTableHelper.records = this.createOrEditMatchAppendDto.matchAppendInputLayoutList;
                this.activeIndex = 0;
            }
        });
    }

    scroll(table: Table) {
        let body = table.containerViewChild.nativeElement.getElementsByClassName("ui-table-scrollable-body")[0];
        body.scrollTop = body.scrollHeight;
    }

    fillAvailableFields(tableId: string) {
        this.selectedAvailableFields = [];
        this.availabelFields = [];
        var inputList = [];
        for (let i = 0; i < this.primengTableHelper.records.length; i++) {
            if (this.primengTableHelper.records[i].cFieldName != '' && this.primengTableHelper.records[i].actionType != ActionType.Delete) {
                inputList = inputList.concat(this.primengTableHelper.records[i].cFieldName);
            }
        }
        this.selectedTable = tableId;
        this.createOrEditMatchAppendDto.matchAppendInputLayoutList = this.primengTableHelper.records;
        this._matchAppendServiceProxy.getMatchAppendAvailabelFields(tableId, inputList).subscribe(result => {
            this.availabelFields = result;
        })
    }


    fillInputKeyFieldDropDown(records: any) {
        this.inputKeyFields = [];
        this.inputKeyFields.push({ label: "Select...", value: "" });
        for (let i = 0; i < records.length; i++) {
            if (records[i].cFieldName != '' && records[i].actionType != ActionType.Delete) {
                this.inputKeyFields = this.inputKeyFields.concat({ label: records[i].cFieldName, value: records[i].cFieldName });
            }
        }
        this.inputKeyFields = this.inputKeyFields.concat({ label: 'Individual Matchcode', value: 'Individual_MC' });
        this.inputKeyFields = this.inputKeyFields.concat({ label: 'Household Matchcode', value: 'Household_MC' });
        this.inputKeyFields = this.inputKeyFields.concat({ label: 'Company Matchcode', value: 'Company_MC' });
        this.inputKeyFields = this.inputKeyFields.concat({ label: 'Address Matchcode', value: 'Address_MC' });
    }

    clear() {
        if (this.matchAppendId != 0) {
            for (let i = 0; i < this.primengTableHelper.records.length; i++) {
                this.primengTableHelper.records[i].actionType = ActionType.Delete;

            }
        }
        else {
            this.primengTableHelper.records = [];
        }
        this.addRows();
        this.createOrEditMatchAppendDto.matchAppendDto.lK_FileType = 'C';
        this.createOrEditMatchAppendDto.matchAppendDto.iSkipFirstRow = false;
        this.createOrEditMatchAppendDto.matchAppendDto.cInputMatchFieldName = '';
        this.importFieldsText = '';
    }

    handleFileInput(files: FileList) {
        if (files.item(0) != null) {
            this.formData = new FormData();
            this.formData.append('file', files.item(0), files.item(0).name);
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
                });
        }
    }

    getMatchAppendInputLayout(event?: LazyLoadEvent) {
        if (!this.matchAppendId) {

            if (this.primengTableHelper.records.length == 0) {
                this.primengTableHelper.records = [];
                for (let i = 1; i <= 15; i++) {
                    let matchAppendInputLayout = new MatchAndAppendInputLayoutDto();
                    matchAppendInputLayout.cFieldName = '';
                    matchAppendInputLayout.startIndex = '';
                    matchAppendInputLayout.endIndex = '';
                    matchAppendInputLayout.dataLength = '';
                    
                    matchAppendInputLayout.importLayoutOrder = '';
                    matchAppendInputLayout.cMCMapping = '';
                    matchAppendInputLayout.mappingDescription = '';
                    this.inputLayputs.push(matchAppendInputLayout);
                }
                this.primengTableHelper.records = this.inputLayputs;
                this.primengTableHelper.totalRecordsCount = this.primengTableHelper.records.length;
            }
        }
        else {
            this.primengTableHelper.records = this.createOrEditMatchAppendDto.matchAppendInputLayoutList;
            this.primengTableHelper.totalRecordsCount = this.primengTableHelper.records.length;
            this.addRows();

        }
    }
    addRows() {
        var j = this.primengTableHelper.records.length;
        for (let i = j; i <= j + 15; i++) {
            let matchAppendInputLayout = new MatchAndAppendInputLayoutDto();
            matchAppendInputLayout.cFieldName = '';
            matchAppendInputLayout.startIndex = '';
            matchAppendInputLayout.endIndex = '';
            matchAppendInputLayout.dataLength = '';            
            matchAppendInputLayout.importLayoutOrder = '';
            matchAppendInputLayout.cMCMapping = '';
            matchAppendInputLayout.matchAppendId = this.matchAppendId;
            matchAppendInputLayout.mappingDescription = ''

            this.primengTableHelper.records = this.primengTableHelper.records.concat(matchAppendInputLayout);
        }
        this.primengTableHelper.totalRecordsCount = this.primengTableHelper.records.length;

    }

    calculateEndPos(currentRowNo: number, flag: boolean) {

        if (flag) {
            let record = this.primengTableHelper.records[currentRowNo];
            let S = record.startIndex == '' ? 0 : parseInt(record.startIndex);
            S = S * 1;
            let W = record.dataLength == '' ? 0 : parseInt(record.dataLength);
            W = W * 1
            let E = W + S - 1;
            if (W <= 0) {
                alert('Width should be greater than 0.');
                record.dataLength = '';
                return;
            }
            if (E < 0) {
                alert('Width should be greater than Start Position.');
                record.endIndex = '';
                return;
            }

            else {
                record.endIndex = E.toString();
            }
            currentRowNo = currentRowNo * 1;
            let nextRowPos = currentRowNo + 1;
            if (this.primengTableHelper.records.length > (nextRowPos)) {
                if ((S == 0) && (currentRowNo > 0)) {
                    let nextRecord = this.primengTableHelper.records[currentRowNo - 1];
                    let txt2 = nextRecord.startIndex == '' ? 0 : parseInt(nextRecord.startIndex);
                    nextRecord.startIndex = ((txt2 * 1) + 1).toString();
                    S = parseInt(nextRecord.startIndex);
                    S = S * 1;
                    W = parseInt(nextRecord.datalength);
                    W = W * 1
                    E = W + S - 1;
                    nextRecord.endIndex = E.toString();
                }
                else if ((S == 0) && (currentRowNo == 0)) {
                    record.startIndex = '1';
                    S = record.startIndex == '' ? 0 : parseInt(record.startIndex);
                    S = S * 1;
                    W = record.dataLength == '' ? 0 : parseInt(record.dataLength);
                    W = W * 1
                    E = W + S - 1;
                    record.endIndex = E.toString();
                }
                let txt1 = this.primengTableHelper.records[nextRowPos];
                txt1.startIndex = (E + 1).toString();
                this.primengTableHelper.records = this.primengTableHelper.records;
                this.calculateAll(currentRowNo);

            }
        }
    }


    calculateWidth(currentRowNo: number, flag: boolean) {

        if (flag) {
            let record = this.primengTableHelper.records[currentRowNo];

            let S = record.startIndex == '' ? 0 : parseInt(record.startIndex);
            S = S * 1;
            let E = record.endIndex == '' ? 0 : parseInt(record.endIndex);
            E = E * 1;
            let W = E - S + 1;
            if (S == 0 && E == 0) {
                record.dataLength = '';
                record.iStartIdex = '';
                record.endIndex = '';
                return;
            }
            if (W <= 0) {
                alert('End Position should be greater than Start Position.');
                record.endIndex = E.toString();
                record.dataLength = '';
                return;
            }
            else {
                record.dataLength = W;
            }
            currentRowNo = currentRowNo * 1;
            let nextRowPos = currentRowNo + 1;
            if (this.primengTableHelper.records.length > (nextRowPos)) {
                let record1 = this.primengTableHelper.records[nextRowPos];
                record1.startIndex = (E + 1).toString();
            }
            this.calculateAll(currentRowNo);
        }
    }

    calculateAll(currentRow: number) {
        currentRow = currentRow * 1;
        if (this.primengTableHelper.records.length > (currentRow)) {
            var nextRowPos = currentRow + 1;
            var record = this.primengTableHelper.records[nextRowPos];
            var W;
            var S;
            var E;
            var PE;
            if ((record.startIndex != '') && (record.endIndex != '') && (record.dataLength != '')) {
                if (confirm('Do you want to recalculate all the field values?')) {
                    for (var i = currentRow + 1; i <= this.primengTableHelper.records.length - 1; i++) {
                        var record1 = this.primengTableHelper.records[i];
                        var record2 = this.primengTableHelper.records[i - 1];

                        if ((record1.startIndex != '') && (record1.endIndex != '') && (record1.dataLength != '')) {
                            W = parseInt(record1.dataLength) * 1;
                            PE = parseInt(record2.endIndex) * 1;
                            record1.startIndex = (PE + 1).toString();
                            S = parseInt(record1.startIndex) * 1;
                            record1.endIndex = (W + S - 1).toString();
                            E = parseInt(record1.startIndex);
                        }
                        else if ((record1.startIndex != '') && (record1.endIndex == '') && (record1.dataLength == '')) {
                            record1.startIndex = (E * 1 + 1).toString();
                        }
                    }
                }
            }
        }
    }

    setInputLayoutOrder(currentRowNo: number, flag: boolean) {
        if (flag) {
            var record = this.primengTableHelper.records[currentRowNo];
            var record1 = this.primengTableHelper.records[currentRowNo - 1];
            if (record.cFieldName == '') {
                record.actionType = ActionType.Delete;
                this.primengTableHelper.records.unshift(record);
                this.primengTableHelper.records.splice(currentRowNo+1, 1);
               
            }

            if (this.matchAppendId != 0) {
                if (this.primengTableHelper.records[currentRowNo - 1].actionType == ActionType.Delete && this.primengTableHelper.records[currentRowNo - 1].cFieldName != '') {
                    record1.importLayoutOrder = '';
                }
                
            }


            currentRowNo = currentRowNo * 1;
            if (currentRowNo > 0) {
                var inmportLayoutorder = record1.importLayoutOrder == '' ? 0 : parseInt(record1.importLayoutOrder);
                var Order = inmportLayoutorder;
                Order = Order * 1;
                var Value = Order + 1;
                record.importLayoutOrder = Value;
            }
            else if (currentRowNo == 0) {
                record.importLayoutOrder = 1;
            }
            if (record.cFieldName == '') {
                record.importLayoutOrder = '';
            }
        }
    }

    DisplayOrderColumn() {
        this.isOrderDisplay = true;
    }

    importFields() {
        if (this.createOrEditMatchAppendDto.matchAppendDto.lK_FileType == 'A') {
            this.isOrderDisplay = false;
        }
        else {
            this.isOrderDisplay = true;
        }

        this.primengTableHelper.records = this.primengTableHelper.records.filter(x => x.cFieldName != '');
        var deleteRecordsLength = this.primengTableHelper.records.filter(x => x.actionType == ActionType.Delete).length;
        this._matchAppendServiceProxy.importFielList(this.importFieldsText, this.matchAppendId, this.createOrEditMatchAppendDto).subscribe(result => {            
            if (this.matchAppendId > 0) {
                if (deleteRecordsLength ==0)
                    this.primengTableHelper.records = result;
                else
                this.primengTableHelper.records = this.primengTableHelper.records.concat(result);
            }
            else {
                this.primengTableHelper.records = result;
            }
            this.createOrEditMatchAppendDto.matchAppendInputLayoutList = this.primengTableHelper.records;
            this.importFieldsText = '';

        });
    }


    getDropDownDescription(event, i) {
        let record = this.primengTableHelper.records[i];
        let fieldDescription = this.mappings.find(x => x.value == event.value).label;
        record.mappingDescription = fieldDescription;
        record.cMCMapping = event.value;
    }

    getMatchAppendOutputLayouts(event?: LazyLoadEvent) {
        if (this.matchAppendId != 0) {


            this.primengTableHelperMatchAppendOutputLayout.records = this.createOrEditMatchAppendDto.matchAppendOutputLayoutList;
            this.primengTableHelperMatchAppendOutputLayout.totalRecordsCount = this.primengTableHelperMatchAppendOutputLayout.records.length;

        }
        else {
            if (this.createOrEditMatchAppendDto.matchAppendOutputLayoutList) {
                this.primengTableHelperMatchAppendOutputLayout.records = this.createOrEditMatchAppendDto.matchAppendOutputLayoutList;
            }
            else {
                this.primengTableHelperMatchAppendOutputLayout.records = [];
            }
        }
    }

    addSelectedOutputFields(table: any) {
        this.createOrEditMatchAppendDto.matchAppendInputLayoutList = this.primengTableHelper.records;
        this.createOrEditMatchAppendDto.matchAppendOutputLayoutList = this.primengTableHelperMatchAppendOutputLayout.records;
        this.createOrEditMatchAppendDto.selectedFields = this.selectedAvailableFields;
        this.createOrEditMatchAppendDto.selectedTable = this.selectedTable;
        this._matchAppendServiceProxy.addMatchAppendSelectedFields(this.createOrEditMatchAppendDto, this.matchAppendId).subscribe(result => {
            this.primengTableHelperMatchAppendOutputLayout.records = this.primengTableHelperMatchAppendOutputLayout.records.concat(result);
            this.primengTableHelperMatchAppendOutputLayout.totalRecordsCount = 1;
            this.createOrEditMatchAppendDto.matchAppendOutputLayoutList = this.primengTableHelperMatchAppendOutputLayout.records;
            this.scroll(table);
            this.selectedAvailableFields = [];

        })
    }

    finish(matchAppendForm: NgForm) {
        this.saving = true;

        this._matchAppendServiceProxy.createOrEdit(this.createOrEditMatchAppendDto)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(result => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.activeModal.close({ saving: this.saving });
            });

    }

    processStep3(flag: boolean) {
        if (flag) {
            this.createOrEditMatchAppendDto.selectedTable = this.selectedTable;
            this.fillInputKeyFieldDropDown(this.primengTableHelper.records);

            this._matchAppendServiceProxy.getSourceDropdownList(this.createOrEditMatchAppendDto.matchAppendDto.buildID, this.createOrEditMatchAppendDto.matchAppendDto.databaseID).subscribe(result => {
                this.sourceList = result;
                this.fillAvailableFields(this.sourceList[0].value);
                this.primengTableHelperMatchAppendOutputLayout.totalRecordsCount = this.primengTableHelperMatchAppendOutputLayout.records.filter(x => x.actionType != ActionType.Delete).length;

            })
            this.activeIndex = 3;
        }
        else {
            this.primengTableHelper.records = this.createOrEditMatchAppendDto.matchAppendInputLayoutList;
            this.activeIndex = 1;
        }

    }
    processStep4() {
        if (this.createOrEditMatchAppendDto.matchAppendOutputLayoutList) {
            this.primengTableHelperMatchAppendOutputLayout.records = this.createOrEditMatchAppendDto.matchAppendOutputLayoutList;
            this.primengTableHelperMatchAppendOutputLayout.totalRecordsCount = this.primengTableHelperMatchAppendOutputLayout.records.filter(x => x.actionType != ActionType.Delete).length;
        }
        else {
            this.primengTableHelperMatchAppendOutputLayout.records = [];
        }

        this.activeIndex = 2;

    }

    getMatchAppendForEdit() {
        this._matchAppendServiceProxy.getMatchAppendForEdit(this.matchAppendId).subscribe(result => {
            this.tempDatabaseId = 0;
            this.tempBuildId = 0;
            this.createOrEditMatchAppendDto.matchAppendDto = result.matchAppend.matchAppendDto;
            this.tempBuildId = this.createOrEditMatchAppendDto.matchAppendDto.buildID;
            this.tempDatabaseId = this.createOrEditMatchAppendDto.matchAppendDto.databaseID;
            this.createOrEditMatchAppendDto.matchAppendInputLayoutList = result.matchAppend.matchAppendInputLayoutList;
            this.createOrEditMatchAppendDto.matchAppendOutputLayoutList = result.matchAppend.matchAppendOutputLayoutList;
            this.primengTableHelper.records = this.createOrEditMatchAppendDto.matchAppendInputLayoutList;
            this.primengTableHelperMatchAppendOutputLayout.records = this.createOrEditMatchAppendDto.matchAppendOutputLayoutList;
            this.getLatestBuild(this.createOrEditMatchAppendDto.matchAppendDto.databaseID,false);           
            if (this.createOrEditMatchAppendDto.matchAppendDto.lK_FileType == 'A') {
                this.isOrderDisplay = false;
            }
            else {
                this.isOrderDisplay = true;
            }
            this.getMatchAppendInputLayout();
            this.getMatchAppendOutputLayouts();


        })

    }

    deleteOutputlayout(record, i) {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                record.actionType = ActionType.Delete;
                var recordCounts = 0;
                var counter = 1;
                for (let j = 0; j < this.primengTableHelperMatchAppendOutputLayout.records.length; j++) {
                    if (this.primengTableHelperMatchAppendOutputLayout.records[j].actionType != ActionType.Delete) {
                        recordCounts = recordCounts + 1;
                        this.primengTableHelperMatchAppendOutputLayout.records[j].outputLayoutOrder = counter++;
                        this.primengTableHelperMatchAppendOutputLayout.records[j].iOutputLayoutOrder = parseInt(this.primengTableHelperMatchAppendOutputLayout.records[j].outputLayoutOrder);
                    }
                }
                this.primengTableHelperMatchAppendOutputLayout.totalRecordsCount = recordCounts;
            }
        });

    }

    avoidSpace(event) {
        if (event.keyCode == 32) return false;
    }


    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}
