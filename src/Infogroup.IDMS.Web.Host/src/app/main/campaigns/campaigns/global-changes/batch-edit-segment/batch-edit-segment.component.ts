import { Component, OnInit, Injector, ViewChild, Input } from '@angular/core';
import { SegmentSelectionsServiceProxy, BatchEditSegmentDto, CreateOrEditSegmentDto, SaveBatchSegmentDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SelectItem } from 'primeng/api';
import { Table } from "primeng/components/table/table";
import { finalize } from 'rxjs/operators';
import { NgModel } from '@angular/forms';
import { models } from 'powerbi-client';

@Component({
  selector: 'batch-edit-segment',
  templateUrl: './batch-edit-segment.component.html',
  styleUrls: ['./batch-edit-segment.component.css']
})
export class BatchEditSegmentComponent extends AppComponentBase implements OnInit {
  @Input() buildId: number;
  @Input() databaseId: number;
  @Input() splitType: number;
  @Input() currentStatus: number;
  isCalculateDistanceSet: boolean;
  allowedStatusesForOutputQty: number[] = [40, 90, 100];
  hasOutputQty: boolean;
  filterText: string = '';
  hasDefaultRules: boolean;
  maxPers: SelectItem[];
  maxPersMap: any;
  previousSegments: BatchEditSegmentDto[] = [];
  @ViewChild("editSegmentTable", { static: false }) dataTable: Table;

  constructor(
    injector: Injector,
    private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.hasOutputQty = this.allowedStatusesForOutputQty.includes(this.currentStatus) &&
      this.splitType !== 4;
    this.primengTableHelper.records = [];
    this._segmentSelectionProxy.getInitialStateForBatchEdit(this.buildId, this.databaseId)
      .subscribe(result => {
        this.isCalculateDistanceSet = result.isCalculateDistanceSet;
        this.hasDefaultRules = result.hasDefaultRules;
        this.maxPers = result.maxPers;
        this.updateMaxPerMap();
      });
  }

  getSegmentsForEdit(filter: string, campaignId: number): void {
    this.primengTableHelper.showLoadingIndicator()
    this._segmentSelectionProxy.getSegmentsForInlineEdit(filter, campaignId, this.primengTableHelper.getSorting(this.dataTable), 1, 1)
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe(result => {
        this.filterText = filter;
        this.primengTableHelper.records = result;
        this.primengTableHelper.totalRecordsCount = result.length;
        this.previousSegments = result.map(row => BatchEditSegmentDto.fromJS(row));
      });
  }

  updateRowStatus(modifiedRow: BatchEditSegmentDto, fieldName: string, control: NgModel): void {
    let previousRow = this.previousSegments[modifiedRow.index];
    if (!control.valid) {
      modifiedRow[fieldName] = previousRow[fieldName];
      return;
    }
    if (modifiedRow[fieldName] && (fieldName === 'iRequiredQty' || fieldName === 'iGroup'))
      modifiedRow[fieldName] = Number(modifiedRow[fieldName]);
    if (fieldName === 'iDisplayOutputQty') {
      if (modifiedRow.iDisplayOutputQty) {
        modifiedRow.iDisplayOutputQty = Number(modifiedRow.iDisplayOutputQty);
        modifiedRow.iOutputQty = modifiedRow.iDisplayOutputQty === modifiedRow.iProvidedQty ? -1 : modifiedRow.iDisplayOutputQty;
      }
      else {
        modifiedRow.iOutputQty = -1;
        modifiedRow.iDisplayOutputQty = previousRow.iDisplayOutputQty;
      }
    }
    let hasDescriptionChanged: boolean = modifiedRow.cDescription !== previousRow.cDescription;
    let hasCountFieldsChanged: boolean = (modifiedRow.iRequiredQty !== previousRow.iRequiredQty)
      || (modifiedRow.cKeyCode1 !== previousRow.cKeyCode1)
      || (modifiedRow.cKeyCode2 !== previousRow.cKeyCode2)
      || (modifiedRow.cMaxPerGroup !== previousRow.cMaxPerGroup)
      || (modifiedRow.iGroup !== previousRow.iGroup)
      || (modifiedRow.iIsRandomRadiusNth !== previousRow.iIsRandomRadiusNth)
      || (modifiedRow.iUseAutosuppress !== previousRow.iUseAutosuppress);
    let hasOutputFieldsChanged: boolean = (modifiedRow.iOutputQty !== previousRow.iOutputQty);
    modifiedRow.dirty = hasDescriptionChanged || hasCountFieldsChanged || hasOutputFieldsChanged;
    modifiedRow.nextStatus = hasCountFieldsChanged ? 10 : hasOutputFieldsChanged ? 40 : 1000;
  }

  getSaveBatchSegmentDto(): SaveBatchSegmentDto {
    let modifiedRows = this.getModifiedRows();
    let modifiedStatuses = modifiedRows.map(record => record.nextStatus);
    let nextStatus = Math.min(...modifiedStatuses);
    let modifiedSegments = modifiedRows.map(record => CreateOrEditSegmentDto.fromJS({
      ...record,
      cFieldDescription: '',
      applyDefaultRules: false,
      iIsCalculateDistance: false
    }));
    return SaveBatchSegmentDto.fromJS({ modifiedSegments, nextStatus });
  }


  getModifiedRows(): BatchEditSegmentDto[] {
    return this.primengTableHelper.records
      .filter(record => record.dirty);

  }
  updateMaxPerMap(): any {
    this.maxPersMap = this.maxPers.reduce((accumulator, maxPer) => {
      accumulator[maxPer.value] = maxPer.label;
      return accumulator;
    }, {});
  }
}
