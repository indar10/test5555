import { Component, Injector, Input, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SelectItem } from 'primeng/api';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GetSegmentListForView, CampaignsServiceProxy, CreateOrEditSegmentDto, SegmentsServiceProxy, CreateOrEditBuildDto, SegmentSelectionsServiceProxy, CreateOrEditSegmentOutputDto, GetQueryBuilderDetails, GetCampaignsListForView, IDMSConfigurationsServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { CampaignUiHelperService } from '../shared/campaign-ui-helper.service';
import { CampaignAction } from '../shared/campaign-action.enum';
import { SaveSegment, CreateOrEditResult } from '../shared/campaign-models';
import { CampaignStatus } from "../shared/campaign-status.enum";
import { NgForm } from '@angular/forms';


@Component({
    selector: 'createOrEditSegmentModal',
    styleUrls: ['create-or-edit-segment-modal.component.css'],
    templateUrl: './create-or-edit-segment-modal.component.html'

})
export class CreateOrEditSegmentModalComponent extends AppComponentBase implements OnInit {

    active = false;
    saving = false;
    divisioncDivisionName = '';
    @Input() segmentId: number;
    dbArray: any;
    outputQuantityDisabled: boolean = true;
    maxPerDrop: SelectItem[] = [];
    maxPerGroup: string;
    irequiredQuantityValue: string;
    allAvailableText: string = "All Available";
    NetGroupWidth: any;
    segmentView: GetSegmentListForView = new GetSegmentListForView();
    segment: CreateOrEditSegmentDto = new CreateOrEditSegmentDto();
    dropSegId: string;
    skipcount = 0;
    maxCount = 1000;
    filterText = '';
    dropSegment: SelectItem[] = [];
    @Input() splitType: number;
    OrderId: number;
    iDedupe: number;
    currentCampaignStatus: string = '';
    saveDisabled: boolean = true;
    CampaignStatus: any;
    iIsCalculateDistance: boolean = false;
    iIsRadiusDisabled: boolean = false;
    isEdit: boolean = false;
    databaseFields: any[] = [];
    @Input() databaseId: number;
    @Input() buildId: number;
    @Input() mailerId: number;
    cNthPriorityField: any;
    pFieldOrder: SelectItem[] = [{
        value: "",
        label: ""
    }, {
        value: "A",
        label: "Ascending"
    }, {
        value: "D",
        label: "Descending"
    }];
    disabledPriorityOrderField: boolean = false;
    defaultPriorityFieldOrder: string = "";
    isLoading: boolean = false;

    constructor(
        injector: Injector,
        private _campaignUiHelperService: CampaignUiHelperService,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
        private _segmentsServiceProxy: SegmentsServiceProxy,
        public activeModal: NgbActiveModal,
        private _idmsConfig: IDMSConfigurationsServiceProxy

    ) {
        super(injector);
    }
    ngOnInit() {
        this.active = true;
        if (!this.segmentId) {
            this._segmentsServiceProxy.getLatestIdedupeorderSpecified(this.OrderId).subscribe(result => {
                this.iDedupe = result;
                this.show(this.segment);
            });
        }
        else {
            this.segment.id = this.segmentId
            this.show(this.segment);
        }
        this.getPriorityOrderFieldConfig();
    }

    getPriorityOrderFieldConfig() {
        this._idmsConfig.getConfigurationValue("NthPriorityFieldDefaultOrder", this.databaseId).subscribe(result => {
            const value = result.cValue;
            this.defaultPriorityFieldOrder = value.toUpperCase();
        });
        this._idmsConfig.getConfigurationValue("NthPriorityFieldOrderEditable", this.databaseId).subscribe(result => {
            this.disabledPriorityOrderField = result.cValue == "1" ? false : true;
        });
    }

    show(segment?: any): void {
        if (!this.segmentId) {
            segment.iRequiredQty = this.allAvailableText;
            this.irequiredQuantityValue = segment.iRequiredQty;
            segment.iGroup = 1;
            segment.iUseAutosuppress = true;
            segment.cDescription = "Segment " + this.iDedupe;
            this._segmentsServiceProxy.fetchConfigValueForCalculateDistance(this.OrderId).subscribe(iIsCalculateDistance => {
                if (iIsCalculateDistance) {
                    segment.iIsRandomRadiusNth = true;
                    this.iIsRadiusDisabled = true;
                }
                else {
                    segment.iIsRandomRadiusNth = false;
                    this.iIsRadiusDisabled = false;
                }
            });
            this.active = true;
            this._segmentsServiceProxy.getIsAutoSupress(this.OrderId).subscribe(result => {
                this.segment.applyDefaultRules = result;
                if (this.segment.applyDefaultRules)
                    this.NetGroupWidth = "col-xs-8 col-sm-8 col-md-8";
                else
                    this.NetGroupWidth = "col-xs-12 col-sm-12 col-md-12";
            });
            this.getMaxPerGroups();
        }
        else {
            this.active = true;
            this.isEdit = true;
            this.isLoading = true;
            this._segmentsServiceProxy.getSegmentForEdit(this.segmentId).subscribe(result => {
                this.segment = result;
                if (this.segment.iIsCalculateDistance) {
                    this.iIsRadiusDisabled = true;
                }
                if (this.segment.applyDefaultRules)
                    this.NetGroupWidth = "col-xs-8 col-sm-8 col-md-8";
                else
                    this.NetGroupWidth = "col-xs-12 col-sm-12 col-md-12";

                if (result.iRequiredQty == 0) {
                    this.irequiredQuantityValue = this.allAvailableText;
                }
                else {
                    this.irequiredQuantityValue = result.iRequiredQty.toString();
                }
                if (result.iOutputQty === -1) {
                    this.segment.iOutputQty = null;
                }

                if (this.segment.cNthPriorityFieldDescription) {
                    const values = this.segment.cNthPriorityField.split('.');
                    this.cNthPriorityField = {
                        label: this.segment.cNthPriorityFieldDescription,
                        data: {
                            cTableName: `${values[0]}_`,
                            cFieldName: values[1]
                        }
                    }
                    this.pFieldOrder.shift();
                }
                this.getMaxPerGroups();
                this.isLoading = false;
            });
        }
    }

    getMaxPerGroups(): void {
        this._segmentsServiceProxy.getMaxPerGroups(this.OrderId, this.segmentId).subscribe(
            (result: any) => {

                this.dbArray = result.segmentDropDown;

                for (let entry of this.dbArray) {

                    this.maxPerDrop.push({ label: entry.label, value: entry.value });
                }
                if (this.segmentId == undefined)
                    this.maxPerGroup = result.defaultMaxPerValue;
                else
                    this.maxPerGroup = this.segment.cMaxPerGroup;
                this.CampaignStatus = result.currentStatus;
                this.currentCampaignStatus = this._campaignUiHelperService.getStatusDescription(result.currentStatus);
                this.saveDisabled = !this._campaignUiHelperService.shouldActionBeEnabled(CampaignAction.SaveSelection, result.currentStatus);
                if (result.currentStatus === CampaignStatus.CampaignCompleted && this.splitType !== undefined && this.splitType !== 4) {
                    this.outputQuantityDisabled = false;
                }
                if (result.currentStatus === CampaignStatus.OutputCompleted || result.currentStatus === CampaignStatus.OutputFailed) {
                    this.outputQuantityDisabled = false;
                }
                if (result.currentStatus === CampaignStatus.CampaignCompleted && this.segment.iOutputQty !== undefined && this.segment.iOutputQty === null) {
                    this.segment.iOutputQty = 0;
                }
            });
    }



    save(form: NgForm): void {
        let orderStatus: number = 10;
        let formControls: any;
        let saveSegment: CreateOrEditResult;
        if (this.irequiredQuantityValue.toUpperCase() == this.allAvailableText.toUpperCase()) {
            this.segment.iRequiredQty = 0;
        }
        else {
            this.segment.iRequiredQty = parseInt(this.irequiredQuantityValue);
        }

        this.segment.orderId = this.OrderId;
        this.segment.cMaxPerGroup = this.maxPerGroup;
        let canSubmit: boolean = !this.segmentId ? true : form.controls.mainForm.dirty;
        
        let canSubmitPriorityChange: boolean = false;
        if (this.segment.cNthPriorityField) {
            if (this.cNthPriorityField && this.cNthPriorityField.data && this.cNthPriorityField.data.cTableName && this.cNthPriorityField.data.cFieldName) {
                const pFieldData = `${(this.cNthPriorityField.data.cTableName as string).split('_')[0]}.${this.cNthPriorityField.data.cFieldName}`;
                canSubmitPriorityChange = this.segment.cNthPriorityField != pFieldData;
            } else {
                canSubmitPriorityChange = true;
            }
        }

        if (canSubmit || canSubmitPriorityChange) {
            if (this.segment.id !== undefined && this.CampaignStatus !== 10) {
                formControls = form.controls.mainForm;
                orderStatus = this.getDirtyStatus(formControls.controls);
            }

            if (this.cNthPriorityField && this.cNthPriorityField.data && this.cNthPriorityField.data.cTableName && this.cNthPriorityField.data.cFieldName) {
                this.segment.cNthPriorityField = `${(this.cNthPriorityField.data.cTableName as string).split('_')[0]}.${this.cNthPriorityField.data.cFieldName}`;
            } else {
                this.segment.cNthPriorityField = "";
            }

            this.saving = true;
            this._segmentsServiceProxy.createOrEdit(this.segment, orderStatus)
                .pipe(finalize(() => { if (this.segment.id != undefined) this.saving = false; }))
                .subscribe((result: CreateOrEditSegmentOutputDto) => {
                    saveSegment = { isEdit: this.isEdit, isSave: true, ...result };
                    this.saving = true;
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.activeModal.close(saveSegment);
                });
        }
        else {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close();
        }
    }



    change(): void {
        this.segment.cMaxPerGroup = this.maxPerGroup;
    }


    close(): void {
        this.active = false;
        let cancelSegment: CreateOrEditResult = { isEdit: this.isEdit, isSave: false, id: 0, newStatus: 0 }
        this.activeModal.close(cancelSegment);
    }
    onRequiredQuantityKeyDown(event) {
        if (event.key == '.')
            return false;
        else
            return true;

    }
    getDirtyStatus(formControls): number {
        let listGeneralTab = new Array("iRequiredQty", "cKeyCode1", "cKeyCode2", "maxPerDrop", "iGroup", "iUseAutosuppress");
        let outputQuantity = "outputQuantity";
        var outputQuantityStatus = formControls[outputQuantity]["dirty"];
        for (let x = 0; x < listGeneralTab.length; x++) {
            if (formControls[listGeneralTab[x]]) {
                var status = formControls[listGeneralTab[x]]["dirty"];
                if (status) {
                    return 10;
                }
            }

        }
        if (outputQuantityStatus) {
            if (this.CampaignStatus === 50) {
                return 50;
            }
            else {
                return 40;
            }
        }

        return 0;

    }

    searchFields(event, isDropdownClick: boolean) {
        this._segmentSelectionProxy
            .getSelectionFieldsNew(
                this.OrderId,
                "1",
                this.databaseId,
                this.buildId,
                this.mailerId
            )
            .subscribe((data: GetQueryBuilderDetails) => {
                this.databaseFields = JSON.parse(data.filterDetails);
                if (!isDropdownClick) {
                    this.databaseFields = this.databaseFields.filter(item => item.label.toLowerCase().includes(event.query.toLowerCase()));
                }
                this.databaseFields = this.databaseFields.sort((a, b) => a.label.localeCompare(b.label));
            });
    }

    onFieldSelect(event) {
        if (this.cNthPriorityField) {
            if ((!this.disabledPriorityOrderField && !this.segment.cNthPriorityFieldOrder) || this.disabledPriorityOrderField) {
                this.segment.cNthPriorityFieldOrder = this.defaultPriorityFieldOrder;
            }
            if (this.pFieldOrder.length === 3) {
                this.pFieldOrder.shift();
            }
        } else {
            this.segment.cNthPriorityFieldOrder = "";
            if (this.pFieldOrder.length < 3) {
                this.pFieldOrder.unshift({
                    value: "",
                    label: ""
                });
            }
        }
    }
}