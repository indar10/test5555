import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActionType, CampaignBillingDto, CampaignGeneralDto, CampaignLevelMaxPerDto, CampaignsServiceProxy, CreateOrEditCampaignDto, DropdownOutputDto, EditCampaignsOutputDto, GetCampaignMaxPerForViewDto, GetCampaignXTabReportsListForView, GetDecoyForViewDto, GetXtabReportsDataDto, ReportsServiceProxy, SegmentSelectionDto } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import { SelectItem, SelectItemGroup } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import {Constants} from "@app/main/fast-count/constants";

@Component({
  selector: 'app-fc-countreport',
  templateUrl: './fc-countreport.component.html'
})

export class FcCountreportComponent extends AppComponentBase implements OnInit {
  isLoading: boolean = true;
  showDownLoad: boolean = false;
  instantBreakdownData: any;
  fastCountConfigXFields: SelectItemGroup[] = [];
  fastCountConfigYFields: SelectItemGroup[] = [];
  reportDataToShowOnUI: any[];  

  @ViewChild("countReportForm", { static: true }) countReportForm: NgForm;

  campaign: CreateOrEditCampaignDto = CreateOrEditCampaignDto.fromJS({
    reportsData: GetXtabReportsDataDto.fromJS({
      xFieldDropdown: [],
      yFieldDropdown: [],
      xtabRecords: [],
    })
  });

  @Input() campaignId: number;
  @Input() databaseId: number;
  @Input() divisionId: number;
  @Input() buildId: number;
  @Input() segmentId: number;
  @Input() fastCountConfig: any;
  @Input() selections: SegmentSelectionDto[];

  constructor(injector: Injector, public activeModal: NgbActiveModal, private _orderServiceProxy: CampaignsServiceProxy, private _reportService: ReportsServiceProxy, private _fileDownloadService: FileDownloadService) {
    super(injector);
  }

  ngOnInit(): void {
    this.getCampaignForEdit();
  }

  getCampaignForEdit() {
    this.isLoading = true;
    this._orderServiceProxy.getCampaignForEdit(this.campaignId, this.databaseId, this.divisionId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        if (!this.fastCountConfigXFields.length && !this.fastCountConfigYFields.length) {
          this.setReportXandYFields(result.reportsData);
        }
        this.campaign = result;
      });
  }

  setReportXandYFields(reportsData: GetXtabReportsDataDto) {
    let xFields: SelectItem[] = [];
    let yFields: SelectItem[] = [];
    for (const key in this.fastCountConfig['selection-fields']) {
      const value = this.fastCountConfig['selection-fields'][key];
      reportsData.xFieldDropdown.filter(element => {
        if (element.value === value['xtab-report']) {
          element.label = value['fc-description'];
          xFields.push({
            ...element,
            title: value["group"]
          });
        }
      });
      reportsData.yFieldDropdown.filter(element => {
        if (element.value === value['xtab-report']) {
          element.label = value['fc-description'];
          yFields.push({
            ...element,
            title: value["group"]
          });
        }
      });
    }
    if (!xFields.length) {
      xFields = reportsData.xFieldDropdown.map(item => {
        return {
          ...item,
          title: "Demographics"
        }
      });
    }
    if (!yFields.length) {
      yFields = reportsData.yFieldDropdown.map(item => {
        return {
          ...item,
          title: "Demographics"
        }
      });
    }
    this.groupFields(xFields.filter(item => item.value), yFields.filter(item => item.value));
  }

  groupFields(xFields: SelectItem[], yFields: SelectItem[]) {
    const selectedXFields: SelectItem[] = [];
    const selectedYFields: SelectItem[] = [];

    this.selections.forEach(selItem => {
      if (xFields.some(item => (item.value.includes('.') ? item.value.split('.')[1] : item.value).toLowerCase() == selItem.cQuestionFieldName.toLowerCase())) {
        selectedXFields.push(xFields.find(item => (item.value.includes('.') ? item.value.split('.')[1] : item.value).toLowerCase() == selItem.cQuestionFieldName.toLowerCase()));
      }
      if (yFields.some(item => (item.value.includes('.') ? item.value.split('.')[1] : item.value).toLowerCase() == selItem.cQuestionFieldName.toLowerCase())) {
        selectedYFields.push(yFields.find(item => (item.value.includes('.') ? item.value.split('.')[1] : item.value).toLowerCase() == selItem.cQuestionFieldName.toLowerCase()));
      }
    });

    this.insertItems(xFields, selectedXFields, this.fastCountConfigXFields);

    this.insertItems(yFields, selectedYFields, this.fastCountConfigYFields);

    this.insertSelectedItem(selectedXFields, this.fastCountConfigXFields);

    this.insertSelectedItem(selectedYFields, this.fastCountConfigYFields);

    this.fastCountConfigXFields.forEach(item => item.items = _.sortBy(item.items, data => data.label));
    this.fastCountConfigYFields.forEach(item => item.items = _.sortBy(item.items, data => data.label));
  }

  private insertSelectedItem(selectedFields: SelectItem[], configFields: SelectItemGroup[]) {
    if (selectedFields.length) {
      configFields.splice(0, 0, {
        items: selectedFields,
        label: "Selected",
        value: "Selected"
      });
    }
  }

  private insertItems(fields: SelectItem[], selectedFields: SelectItem[], configFields: SelectItemGroup[]) {
    const groupXData = _.groupBy(fields, item => item.title);
    for (let key in groupXData) {
      configFields.push({
        label: key,
        value: key,
        items: groupXData[key].filter(item => {
          if (selectedFields.some(selItem => selItem.value == item.value)) {
            return false;
          }
          return true;
        }),
      });
    }
  }

  onXFieldChange(relativeIndex: number, event: any): void {
    let record = this.campaign.reportsData.xtabRecords[relativeIndex];
    record.cXDesc = this.getDescription(event.value, this.fastCountConfigXFields);
    if (record.id !== 0) {
      record.action = ActionType.Edit;
    } else {
      record.action = ActionType.Add;
    }
    this.countReportForm.form.markAsDirty();
  }

  onYFieldChange(relativeIndex: number, event: any): void {
    let record = this.campaign.reportsData.xtabRecords[relativeIndex];
    record.cYDesc = this.getDescription(event.value, this.fastCountConfigYFields);
    if (record.id !== 0) {
      record.action = ActionType.Edit;
    } else if (record.id === 0 && (record.cYDesc !== '' || record.cYDesc != null)) {
      record.action = ActionType.Add;
    }
    this.countReportForm.form.markAsDirty();
  }

  getDescription(value: string, allOptionsData: SelectItemGroup[]): string {
    const allOptions: SelectItem[] = [];
    allOptionsData.forEach(item => allOptions.push(...item.items));
    return allOptions.find(d => d.value === value).label;
  }

  deleteXtab(relativeIndex: number): void {
    this.message.confirm(
      this.l(''),
      (isConfirmed) => {
        if (isConfirmed) {
          const record = this.campaign.reportsData.xtabRecords[relativeIndex];
          record.action = ActionType.Delete;
          this.countReportForm.form.markAsDirty();
        }
      }
    );
  }

  save() {
    this.isLoading = true;
    if (this.countReportForm.valid && this.countReportForm.pristine) {
      this.getReport();
    } else {
      this._orderServiceProxy.createOrEdit(this.getCampaignData(), this.campaign.currentStatus)
        .subscribe(() => {
          this.getReport();
        }, () => this.isLoading = false);
    }
  }

  getCampaignData(): CreateOrEditCampaignDto {
    const listOfXTabRecords = this.campaign.reportsData.xtabRecords ? this.campaign.reportsData.xtabRecords.filter(r => r.action !== ActionType.None) : null;
    if (listOfXTabRecords && listOfXTabRecords.length) {
      listOfXTabRecords.forEach(item => {
        if (!item.cXField && !item.cYField && item.id) {
          item.action = ActionType.Delete;
        }
      });
    }

    const campaignDto = CreateOrEditCampaignDto.fromJS({
      listOfXTabRecords,
      id: this.campaign.id,
      generalData: CampaignGeneralDto.fromJS(this.campaign.generalData),
      listOfMultidimensionalRecords: [],
      campaignOutputDto: {
        ...EditCampaignsOutputDto.fromJS(this.campaign.getOutputData),
        layoutDescription: undefined,
        layout: undefined,
        email: undefined,
        layoutId: undefined,
        shippedDate: undefined,
      },
      billingData: CampaignBillingDto.fromJS(this.campaign.billingData),
      maxPerData: GetCampaignMaxPerForViewDto.fromJS({
        campaignId: this.campaign.id,
        getSegmentLevelMaxPerData: [],
        getCampaignLevelMaxPerData: CampaignLevelMaxPerDto.fromJS({
          cMaximumQuantity: this.campaign.maxPerData.getCampaignLevelMaxPerData.cMaximumQuantity,
          cMinimumQuantity: this.campaign.maxPerData.getCampaignLevelMaxPerData.cMinimumQuantity,
          cMaxPerFieldOrderLevel: this.campaign.maxPerData.getCampaignLevelMaxPerData.cMaxPerFieldOrderLevel
        }),
      }),
      decoyData: GetDecoyForViewDto.fromJS({
        listOfDecoys: [],
        isDecoyKeyMethod: this.campaign.decoyData.isDecoyKeyMethod,
        decoyKey: this.campaign.decoyData.decoyKey,
        decoyKey1: this.campaign.decoyData.decoyKey1
      }),
      isStatusChangeRequired: false
    });
    if (!campaignDto.generalData.broker || !campaignDto.generalData.broker.value) {
      campaignDto.generalData.broker = DropdownOutputDto.fromJS({
        label: this.fastCountConfig["broker"]["label"],
        value: this.fastCountConfig["broker"]["value"]
      });
      campaignDto.generalData.brokerDescription = this.fastCountConfig["broker"]["label"];
    }
    campaignDto.campaignOutputDto.shipNotes = Constants.fastCountShippingNote;
    return campaignDto;
  }

  getReport(): void {
    const records =  this.campaign.reportsData.xtabRecords.filter(item => item.action != ActionType.Delete && !item.cXField && !item.cYField);
    if (records && records.length) {
      this._reportService.getInstantBreakDown('XTAB', this.segmentId, this.campaign.id)
        .pipe(finalize(() => this.isLoading = false))
        .subscribe(result => {
          this.showDownLoad = true;
          this.instantBreakdownData = result;
          const reportData = this.campaign.reportsData.xtabRecords.filter((item: GetCampaignXTabReportsListForView) => item.cXField || item.cYField);
          const singleDimensionReport: GetCampaignXTabReportsListForView[] = reportData.filter((item: GetCampaignXTabReportsListForView) => (item.cXField && !item.cYField) || (!item.cXField && item.cYField));
          const multiDimensionReport: GetCampaignXTabReportsListForView[] = reportData.filter((item: GetCampaignXTabReportsListForView) => (item.cXField && item.cYField));

          let singleDimensionReportCount = 0;
          let multiDimensionReportCount = 0;
          let reportNo = 1;
          this.reportDataToShowOnUI = [];

          for (const key in result) {
            const reportArray: any[] = result[key];
            let rowOrCol;
            let rowOrColHeader;

            if (singleDimensionReport.length && singleDimensionReportCount < singleDimensionReport.length) {
              let singleData = singleDimensionReport[singleDimensionReportCount];
              let eachRowOrCol = (singleData.cXField ? singleData.cXField : singleData.cYField);
              rowOrColHeader = eachRowOrCol.includes('.') ? eachRowOrCol.split('.')[1] : eachRowOrCol;
              rowOrCol = rowOrColHeader.toLowerCase();
            }
            let row;
            let rowHeader;
            let col;
            let columnHeader;
            if (multiDimensionReport.length && multiDimensionReportCount < multiDimensionReport.length) {
              let multiData = multiDimensionReport[multiDimensionReportCount];
              row = multiData.cXField.includes('.') ? multiData.cXField.split('.')[1].toLowerCase() : multiData.cXField.toLowerCase();
              col = multiData.cYField.includes('.') ? multiData.cYField.split('.')[1].toLowerCase() : multiData.cYField.toLowerCase();
              rowHeader = multiData.cXField.includes('.') ? multiData.cXField.split('.')[1] : multiData.cXField;
              columnHeader = multiData.cYField.includes('.') ? multiData.cYField.split('.')[1] : multiData.cYField;
            }
            const propertiesInLowerCase = Object.getOwnPropertyNames(reportArray[0]).map(property => property.toLowerCase());
            if (propertiesInLowerCase.some(item => item == row) && propertiesInLowerCase.some(item => item == col)) {
              const items: any[] = [];
              const groupedValues = _.groupBy(reportArray, (item) => item[Object.keys(item).find(key => key.toLowerCase() == row)]);
              for (const key in groupedValues) {
                const values = groupedValues[key];
                values.forEach(colValues => {
                  items.push({
                    label: key,
                    colLabel: colValues[Object.keys(colValues).find(key => key.toLowerCase() == col)],
                    value: (+colValues["count"]).toLocaleString(),
                  });
                });
              }
              this.reportDataToShowOnUI.push({
                reportName: `Report ${reportNo}`,
                description: this.getFieldDescription(rowHeader) + ' vs ' + this.getFieldDescription(columnHeader),
                labelHeader: this.getFieldDescription(rowHeader),
                columnHeader: this.getFieldDescription(columnHeader),
                isSingleDimension: false,
                rowCount: (+reportArray[0][Object.keys(reportArray[0]).find(key => key.toLowerCase() == "rowcount")]),
                items
              });
              multiDimensionReportCount = multiDimensionReportCount + 1;
            } else if (propertiesInLowerCase.some(item => item == rowOrCol)) {

              this.reportDataToShowOnUI.push({
                reportName: `Report ${reportNo}`,
                description: this.getFieldDescription(rowOrColHeader),
                labelHeader: this.getFieldDescription(rowOrColHeader),
                isSingleDimension: true,
                rowCount: (+reportArray[0][Object.keys(reportArray[0]).find(key => key.toLowerCase() == "rowcount")]),
                items: reportArray.map(reportItem => {
                  return {
                    label: reportItem[Object.keys(reportItem).find(key => key.toLowerCase() == rowOrCol)],
                    value: (+reportItem["count"]).toLocaleString(),
                    rowCount: (+reportItem[Object.keys(reportItem).find(key => key.toLowerCase() == "rowcount")])
                  };
                })
              });
              singleDimensionReportCount = singleDimensionReportCount + 1;
            }
            reportNo = reportNo + 1;
          }
        });
    } else {
      this.isLoading = true;
      this.getCampaignForEdit();
    }
  }

  getFieldDescription(selection: string): string {
    const field = this.fastCountConfig['selection-fields'][selection];
    return field ? field['fc-description'] : selection;
  }

  downloadDataReport() {
    this.isLoading = true;
    this._reportService.generateExcelReport(this.instantBreakdownData)
      .pipe(finalize(() => this.isLoading = false))

      .subscribe(result => {
        this._fileDownloadService.downloadFile(result);
      });
  }

  close(saving: boolean = false): void {
    this.activeModal.close({ saving });
  }
}
