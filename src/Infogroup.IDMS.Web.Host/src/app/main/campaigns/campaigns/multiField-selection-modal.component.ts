import { Component, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SegmentSelectionDto, BuildTableLayoutsServiceProxy, GetBuildTableLayoutForViewDto, SegmentSelectionsServiceProxy, SegmentSelectionSaveDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { PrimengTableHelper } from '@shared/helpers/PrimengTableHelper';
import { SelectItem } from 'primeng/api';

@Component({
    selector: 'multiField-selection',
    styleUrls: ['./multiField-selection-modal.component.css'],
    templateUrl: './multiField-selection-modal.component.html'

})
export class MultiFieldSelectionModalComponent extends AppComponentBase {
    savingAND: boolean = false;
    savingOR: boolean = false;
    @Input() segmentId: number;
    @Input() campaignId: number;
    @Input() databaseId: number;
    @Input() mailerId: number;
    @Input() buildId: number;
    filterText: string = '';
    saveDisabled: boolean = false; selection
    getDisabled: boolean = true;
    getSaveDisabled: boolean = true;
    multiFields: any[] = [];
    selectedMultiFields: SegmentSelectionDto[] = [];
    selectedRows: GetBuildTableLayoutForViewDto[] = [];
    operationTypes = [
        { label: 'In', value: 'IN' },
        { label: 'Not In', value: 'NOT IN' },
        { label: 'Greater', value: '>' },
        { label: 'Less', value: '<' },
        { label: 'Greater Equal', value: '>=' },
        { label: 'Less Equal', value: '<=' }
    ];
    relationTypes: SelectItem[] =  [
        { label: 'AND', value: 'AND' },
        { label: 'OR', value: 'OR' }
    ];
    relation: string = 'OR';
    operation: string = this.operationTypes[0].value;
    showCountyFilters: boolean = false;
    showCityFilters: boolean = false;
    primengTableHelper: PrimengTableHelper;

    constructor(
        injector: Injector,
        private _buildTableLayoutsAppServiceProxy: BuildTableLayoutsServiceProxy,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
        private activeModal: NgbActiveModal
    ) {
        super(injector);
    }


    getMultiFields(): void {
        this.selectedRows = [];
        this.multiFields = [];
        this.primengTableHelper.showLoadingIndicator();
        this._buildTableLayoutsAppServiceProxy.getAllMultiFields(
            this.filterText.trim(),
            this.buildId,
            this.databaseId,
            this.mailerId
        ).subscribe(result => {
            this.primengTableHelper.hideLoadingIndicator();
            this.multiFields = result;
            this.getDisabled = false;
        });
    }

    addSelection(): void {
        var selectedOperation = this.operation;
        let isChkSelectChecked = this.selectedRows.length;

        if (isChkSelectChecked == 0) {
            return this.message.error(this.l('MultiFieldNotSelected'));
        }

        if ((isChkSelectChecked > 1) && (selectedOperation == '>' || selectedOperation == '<' || selectedOperation == '<=' || selectedOperation == '>=')) {
            return this.message.error(this.l('MultiFieldValidation'));
        }

        this.selectedRows.forEach(row => {
            var existingRecord = this.selectedMultiFields.find(sel => sel.id === row.id && sel.segmentId === this.segmentId
                && sel.cJoinOperator === this.relation && sel.cValueOperator === this.operation);
            if (existingRecord) {
                var cValues = existingRecord.cValues.split(',').toString();
                if (!cValues.includes(row.cValue)) {
                    existingRecord.cValues = existingRecord.cValues + ',' + row.cValue;
                    existingRecord.cDescriptions = existingRecord.cDescriptions + ',' + row.cDescription;
                }
            }
            else {
                var selectedMultiField = new SegmentSelectionDto();
                selectedMultiField.id = row.id;
                selectedMultiField.orderID = this.campaignId;
                selectedMultiField.segmentId = this.segmentId;
                selectedMultiField.cFieldDescription = row.cFieldDescription;
                selectedMultiField.cQuestionFieldName = row.cFieldName;
                selectedMultiField.cQuestionDescription = "";
                selectedMultiField.cJoinOperator = this.relation;
                selectedMultiField.cValues = row.cValue;
                selectedMultiField.cValueMode = "G";
                selectedMultiField.cDescriptions = row.cDescription;
                selectedMultiField.cValueOperator = this.operation;
                selectedMultiField.cTableName = row.cTableName;
                this.selectedMultiFields.push(selectedMultiField);
            }

        });
        this.getSaveDisabled = false;
        this.selectedRows = [];
    }

    saveWithAND(): void {
        this.selectedMultiFields[0].cJoinOperator = 'AND';
        this.savingAND = true;
        this.save();

    }

    save(): void {
        let input = new SegmentSelectionSaveDto();
        input.campaignId = this.campaignId;
        input.selections = this.selectedMultiFields;

        this._segmentSelectionProxy.saveMultiFieldSelection(input).pipe(finalize(() => {
            this.savingAND = false;
            this.savingOR = false;
        })).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.activeModal.close({ isSave: true });
        });
    }

    delete(index: number) {
        this.message.confirm(
            this.l(''),
            (isConfirmed) => {
                if (isConfirmed) {
                    this.selectedMultiFields.splice(index, 1);
                }
            }
        );
    }

    close(): void {
        this.activeModal.close({ isSave: false });
    }
}

