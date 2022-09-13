import {
    Component,
    Injector,
    Input, ViewChild
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    GetQueryBuilderDetails,
    SegmentSelectionsServiceProxy,
    SegmentsServiceProxy,
    CampaignsServiceProxy,
    SegmentSelectionSaveDto,
    SegmentSelectionDto,
    IDMSConfigurationsServiceProxy,
    ValueList,
    CampaignStatus,
    SegmentPrevOrdersesServiceProxy
} from "@shared/service-proxies/service-proxies";
import * as moment from "moment";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { ModalDefaults, ModalSize } from "../../../../shared/costants/modal-contants";
import { finalize } from "rxjs/operators";
import { GeoMappingComponent } from "@app/main/shared/geo-mapping/geo-mapping.component";
import { SelectItemGroup } from 'primeng/api';
import * as _ from 'lodash';
import { SuppressionComponent } from './suppression/suppression.component';
import { CustomSuppressionComponent } from './custom-suppression/custom-suppression.component'
import { FcSavecountComponent } from './create-or-edit/fc-savecount/fc-savecount.component';
import { FcMaxperComponent } from './create-or-edit/fc-maxper/fc-maxper.component';
import { FcCountreportComponent } from './create-or-edit/fc-countreport/fc-countreport.component';
import { FcPlaceorderComponent } from './create-or-edit/fc-placeorder/fc-placeorder.component';
import { FcGeographyComponent } from './fc-geography/fc-geography.component';
import { UploadSuppressionComponent } from "./custom-suppression/upload-suppression/upload-suppression.component";
import { CitySelectionModalComponent } from '@app/main/shared/city-selection-modal/city-selection-modal.component';
import { SICSelectionModalComponent } from '@app/main/shared/sic-selection-modal/sic-selection-modal.component';
import { Listbox } from 'primeng/listbox';

interface OptionValueList {
    id: number;
    cDescription: string;
    cValue: string;
}

@Component({
    selector: 'fastCount',
    templateUrl: "./fast-count.component.html",
    styleUrls: ["./fast-count.component.css"],
})
export class FastCountComponent extends AppComponentBase {
    @Input() databaseId: number;
    currentUISelection: any = {};
    dto: SegmentSelectionSaveDto = new SegmentSelectionSaveDto();
    selectedOptions: OptionValueList[] = [];
    selectedValuesOfCurrentSelection: OptionValueList[] = [];
    optionValues: OptionValueList[];
    popupData: string;
    currentStatus: number = 0;

    public selectedDBAttribute: {
        label: string;
        field: string;
        description: string;
        type: string;
        input: string;
        multiple: boolean;
        id: string;
        data: {
            cTableName: string;
        };
    };
    groupFilterOptions: SelectItemGroup[] = [];
    excludedSelectionsFilters: SelectItemGroup[] = [];
    isOptionAvailable = false;
    active = 1; // 1 = Checkbox options, 2 = Text input
    buildLolId: number;
    isLoading: boolean = true;
    fastCountSelectionDetails: any = [];
    textData = "";
    currentQuickCount: number = 0;
    previousQuickCount: number = 0;
    isRecalculating: boolean = false;
    isRecalculateFail: boolean = false;
    isSave = false;
    selected_row: number;
    Edit: number;
    addOmitChange = false;
    fastCountConfig: any;
    rangeData: RangeData[];
    selectedFastCountAttribute: any = {};
    permissionForSaveCount: boolean;
    permissionForPlaceOrder: boolean;
    maxperCount: number;
    isMaxPerLoading: boolean = true;
    isCustomSuppression: boolean = false;
    isSelectionField = false;
    databaseName: string = '';
    excludeOmitOption = false;
    excludeAddAndOmitOption = false;
    queryBuilderfilterDetails: any[];
    isOrderSuppression: boolean = false;
    orderSuppressionCount: number;
    @ViewChild('myListbox', { static: false }) myListbox: Listbox;

    constructor(
        injector: Injector,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
        private modalService: NgbModal,
        private _segmentServiceProxy: SegmentsServiceProxy,
        public router: Router,
        private _campaignServiceProxy: CampaignsServiceProxy,
        public activatedRoute: ActivatedRoute,
        private _idmsConfig: IDMSConfigurationsServiceProxy,
        private _segmentPrevOrdersesServiceProxy: SegmentPrevOrdersesServiceProxy,
    ) {
        super(injector);
        if (window.location.href.includes("?")) {
            this.activatedRoute.queryParams.subscribe((params: Params) => {
                this.fastCountSelectionDetails.campaignId = params["campaignId"];
                this.fastCountSelectionDetails.databaseId = params["databaseId"];
                this.fastCountSelectionDetails.segmentId = params["segmentId"];
                this.fastCountSelectionDetails.divisionId = params["divisionId"];
                this.fastCountSelectionDetails.mailerId = params["mailerId"];
                this.fastCountSelectionDetails.buildId = params["buildId"];
                this.fastCountSelectionDetails.databaseName = params["databaseName"];
                this.Edit = params["Edit"] == "1" ? 1 : 0;
            });
        }
        this.dto.selections = [];
        this.dto.campaignId = this.fastCountSelectionDetails.campaignId;
        this.dto.databaseId = this.fastCountSelectionDetails.databaseId;
        this.dto.deletedSelections = [];
        this.databaseName = this.fastCountSelectionDetails.databaseName;
    }

    // tslint:disable-next-line:use-lifecycle-interface
    ngOnInit() {
        this.getPermission();
        this.getAvailableQuantity();
        this.getCurrentCampaignStatus(this.getSelectionFilterData());
        this.setDivisionalDatabase();
    }

    getPermission() {
        this.permissionForSaveCount = this.isGranted("Pages.FastCount.Create");
        this.permissionForPlaceOrder = this.isGranted("Pages.PlaceOrder");
    }

    getAvailableQuantity() {
        this._segmentServiceProxy
            .getAllSegmentList(
                "",
                this.fastCountSelectionDetails.campaignId,
                "",
                0,
                10
            )
            .subscribe((result) =>
                this.currentQuickCount = Number(
                    result.pagedSegments.items[0].iAvailableQty
                )
            );
    }

    getCurrentCampaignStatus(callback) {
        this._campaignServiceProxy.getCampaignStatus(this.dto.campaignId)
            .subscribe(result => {
                this.currentStatus = result.value;
                if (this.Edit) {
                    this.Edit = result.value == CampaignStatus.OrderCreated || result.value == CampaignStatus.OrderFailed || result.value == CampaignStatus.OutputCompleted || result.value == CampaignStatus.OutputFailed ? 1 : 0;
                }
                if (callback) callback();
            });
    }

    getSelectionFilterData() {
        this._segmentSelectionProxy
            .getSelectionFieldsNew(
                this.fastCountSelectionDetails.segmentId,
                "1",
                this.fastCountSelectionDetails.databaseId,
                this.fastCountSelectionDetails.buildId,
                this.fastCountSelectionDetails.mailerId
            )
            .subscribe((data: GetQueryBuilderDetails) => {
                this.addGroupFilters(data, true);
                this.buildLolId = data.buildLolId;
                this.dto.deletedSelections = [];
            });
    }

    addGroupFilters(data: any, isBindSelecteddata: boolean = false) {
        this.queryBuilderfilterDetails = JSON.parse(data.filterDetails);
        const builderData: any[] = JSON.parse(data.filterDetails);
        builderData.forEach((element) => {
            element.isDatafilter = false;
        });
        this._idmsConfig.getConfigurationValue("FastCount", this.fastCountSelectionDetails.databaseId).subscribe(result => {
            this.fastCountConfig = JSON.parse(result.mValue);
            for (const index in this.fastCountConfig.groups) {
                this.groupFilterOptions.push({
                    label: this.fastCountConfig.groups[index],
                    items: []
                });
                this.excludedSelectionsFilters.push({
                    label: this.fastCountConfig.groups[index],
                    items: []
                });
            }
            if (this.fastCountConfig["selection-fields"] == null) {
                this.fastCountConfig["selection-fields"] = {};
                builderData.forEach(item => {
                    this.fastCountConfig["selection-fields"][item.field] = {
                        "field": item.field,
                        "fc-description": item.label,
                        "group": "Demographics",
                        "default-input": item.input,
                        "input-format": ''
                    }
                });
            }
            for (const fieldName in this.fastCountConfig["selection-fields"]) {
                const value = this.fastCountConfig["selection-fields"][fieldName];
                if (builderData.some(item => item.field === value.field)) {
                    if (value["exclude-selection"] != true) {
                        const groupFilterItem = this.groupFilterOptions.find(item => item.label === value.group);
                        const option = builderData.find(item => item.field === value.field);
                        groupFilterItem.items.push({
                            value: option,
                            label: value["fc-description"]
                        });
                    }
                    const excludedSelectionItem = this.excludedSelectionsFilters.find(item => item.label === value.group);
                    const option = builderData.find(item => item.field === value.field);
                    excludedSelectionItem.items.push({
                        value: option,
                        label: value["fc-description"]
                    });
                }
            }
            for (const fieldName in this.fastCountConfig["additional-fields"]) {
                const value = this.fastCountConfig["additional-fields"][fieldName];
                const groupFilterItem = this.groupFilterOptions.find(item => item.label === value.group);
                groupFilterItem.items.push({
                    value: value,
                    label: value["fc-description"]
                });

                const excludedSelectionItem = this.excludedSelectionsFilters.find(item => item.label === value.group);
                excludedSelectionItem.items.push({
                    value: value,
                    label: value["fc-description"]
                });
            }
            this.groupFilterOptions.forEach(item => item.items.sort(
                (a, b) => 0 - (a.label > b.label ? -1 : 1)
            ));

            if (isBindSelecteddata) {
                let filterQuery = JSON.parse(data.filterQuery);
                let getSavedRules =
                    filterQuery.rules && filterQuery.rules.length > 0
                        ? filterQuery.rules[0].rules
                        : [];
                getSavedRules.forEach((ele) => {
                    this.bindSelectedData(ele);
                });
            }
            this.getMaxperData();
            this.isLoading = false;
        });
    }

    setDivisionalDatabase() {
        this._campaignServiceProxy.getDatabaseWithLatestBuild(this.databaseId)
            .subscribe((result) => this.fastCountSelectionDetails.divisionalDatabase = result.divisionalDatabase);
    }
    //For the selected filter get sub options from api
    createComponent(event) {
        this.selectedOptions = [];
        if (event.option.value.field === "Placed_Order" || event.option.value.field === "Saved_Order") {
            this.openSupressionModal(event.option.value.field === "Placed_Order" ? true : false);
            return;
        }
        if (event.option.value.field === 'Custom_Suppression' || event.option.value.field.toUpperCase() === "INDUSTRY_POPUP") {
            this.selectedDBAttribute = this.fastCountConfig["additional-fields"][event.option.value.field];
        }

        if (event.option.value.field === 'Maxper') {
            this.openModalPopup(event.option.value.field);
            return;
        }

        if (event.option.value.field != 'Custom_Suppression' && event.option.value.field.toUpperCase() != "INDUSTRY_POPUP") {
            this.isSelectionField = true;
            this.isCustomSuppression = false;
            this.selectedDBAttribute = event.option.value;
            this.selectedFastCountAttribute = this.fastCountConfig["selection-fields"][this.selectedDBAttribute.field];
            this.excludeAddAndOmitOption = this.selectedFastCountAttribute["exclude-add-omit-option"];
            this.excludeOmitOption = this.selectedFastCountAttribute["exclude-omit-option"];
            if (this.selectedFastCountAttribute['default-input'] == 'range') {
                this.rangeData = [{ from: '', to: '' }];
            }
        }
        switch (this.selectedDBAttribute.field.toUpperCase()) {
            case "GEOMAPPING":
                if (!event.selection) {
                    this.textData = "";
                    this.popupData = "";
                }
                this.openGeoMapping();
                break;

            case "CUSTOM_SUPPRESSION":
                this.isCustomSuppression = true;
                this.isSelectionField = false;
                break;

            case "CITY":
                if (!event.selection) {
                    this.textData = "";
                    this.popupData = "";
                }
                this.openGeographyPopup();
                break;
            case "FIPS_CODE":
            case "STATECITY":
            case "STATECOUNTYCODE":
            case "STATECOUNTYNAME":
            case "CITYBYSTATE":
            case "STATECITYNEIGHBORHOOD":
                if (!event.selection) {
                    this.textData = "";
                    this.popupData = "";
                } else {
                    const popupField = this.dto.selections.find(item => item.cQuestionFieldName.toUpperCase() === this.selectedDBAttribute.field.toUpperCase() && item.cValueOperator === event.selection.cValueOperator);
                    this.textData = popupField.cValues;
                    this.popupData = popupField.cValues;
                }
                this.openCensusPopup(event.selection);
                break;
            case "FRANCHISEBYSIC":
            case "INDUSTRYSPECIFICBYSIC":
            case "MAJORINDUSTRYGROUP":
            case "MINORINDUSTRYGROUP":
            case "PRIMARYSICFLAG":
            case "SICCODE":
            case "INDUSTRY_POPUP":
                if (!event.selection) {
                    this.textData = "";
                    this.popupData = "";
                } else {
                    const popupField = this.dto.selections.find(item => item.cQuestionFieldName.toUpperCase() === this.selectedDBAttribute.field.toUpperCase() && item.cValueOperator === event.selection.cValueOperator);
                    this.textData = popupField.cValues;
                    this.popupData = popupField.cValues;
                }
                this.openSICSearch(event.selection);
                break;
            default:
                this.textData = "";
                this.getFieldsValues(this.selectedDBAttribute, event.selection);
        }
        this.isOptionAvailable = true;
        if (event.selection == null) {
            this.selected_row = -1;
        }
        if (this.myListbox) {
            this.myListbox._filterValue = '';
        }
    }

    openSICSearch(selection: any): void {
        if (this.Edit == 1) {
            const modalRef: NgbModalRef = this.modalService.open(
                SICSelectionModalComponent,
                {
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: "sicModalClass"
                }
            );

            modalRef.componentInstance.segmentId = this.fastCountSelectionDetails.segmentId;
            modalRef.componentInstance.campaignId = this.fastCountSelectionDetails.campaignId;
            modalRef.componentInstance.buildId = this.fastCountSelectionDetails.buildId;
            modalRef.componentInstance.databaseId = this.fastCountSelectionDetails.databaseId;
            modalRef.componentInstance.isFastcount = true;

            if (selection) {
                const selectedAttribute = this.dto.selections.find(item => item.cQuestionFieldName.toUpperCase() == this.selectedDBAttribute.field.toUpperCase() && selection.cValueOperator == item.cValueOperator)
                if (selectedAttribute) {
                    modalRef.componentInstance.fcSelection = selectedAttribute;
                }
            }

            modalRef.result.then((params) => {
                if (params.isSave) {
                    const selectionsFromModel: SegmentSelectionDto[] = params.selections;
                    this.addSelectionsFromModel(selectionsFromModel, selection);
                }
            });
        }
    }

    openGeographyPopup() {
        if (this.Edit == 1) {
            const modalRef: NgbModalRef = this.modalService.open(FcGeographyComponent, {
                size: ModalSize.LARGE,
                windowClass: ModalDefaults.WindowClass,
                backdrop: ModalDefaults.Backdrop,
                centered: true,
            });

            modalRef.componentInstance.buildId = this.fastCountSelectionDetails.buildId;
            modalRef.componentInstance.databaseId = this.fastCountSelectionDetails.databaseId;

            if (this.popupData) {
                modalRef.componentInstance.popupData = this.popupData.split(',').map(newArr => _.startCase(newArr.toLowerCase())).join(',');
                this.textData = modalRef.componentInstance.popupData;
                this.popupData = modalRef.componentInstance.popupData;
            }

            modalRef.result.then((params) => {
                if (params.isSave) {
                    this.textData = params.popupData;
                    this.popupData = params.popupData;
                    if (params.isAdd) {
                        this.onAddTitleClick();
                    } else {
                        this.onOmitTitleClick()
                    }
                } else if (this.popupData) {
                    this.textData = this.popupData;
                }
            });
        }
    }

    openCensusPopup(selection: any) {
        if (this.Edit == 1) {
            const modalRef: NgbModalRef = this.modalService.open(CitySelectionModalComponent, {
                size: ModalSize.LARGE,
                windowClass: ModalDefaults.WindowClass,
                backdrop: ModalDefaults.Backdrop,
                centered: true,
            });

            modalRef.componentInstance.buildId = this.fastCountSelectionDetails.buildId;
            modalRef.componentInstance.databaseId = this.fastCountSelectionDetails.databaseId;
            modalRef.componentInstance.segmentId = this.fastCountSelectionDetails.segmentId;
            modalRef.componentInstance.campaignId = this.fastCountSelectionDetails.campaignId;
            modalRef.componentInstance.isFastcount = true;

            if (selection) {
                const selectedAttribute = this.dto.selections.find(item => item.cQuestionFieldName.toUpperCase() == this.selectedDBAttribute.field.toUpperCase() && selection.cValueOperator == item.cValueOperator)
                if (selectedAttribute) {
                    modalRef.componentInstance.fcSelection = selectedAttribute;
                }
            }

            modalRef.result.then((params) => {
                if (params.isSave) {
                    const selectionsFromModel: SegmentSelectionDto[] = params.selections;
                    this.addSelectionsFromModel(selectionsFromModel, selection);
                }
            });
        }
    }

    addSelectionsFromModel(selectionsFromModel: SegmentSelectionDto[], selection: any) {
        if (selectionsFromModel && selectionsFromModel.length) {
            const newItems = [];
            selectionsFromModel.forEach(selItem => {
                let dtoItem: SegmentSelectionDto;
                if (!selection) {
                    dtoItem = this.dto.selections.find(item => selItem.cQuestionFieldName.toUpperCase() == item.cQuestionFieldName.toUpperCase() && item.cValueOperator == selItem.cValueOperator);
                } else {
                    dtoItem = this.dto.selections.find(item => selection.cQuestionFieldName.toUpperCase() == item.cQuestionFieldName.toUpperCase() && item.cValueOperator == selection.cValueOperator); // selected dto item
                    if (selection.cValueOperator != selItem.cValueOperator) {
                        const withDifferentOperator = this.dto.selections.find(item => selItem.cQuestionFieldName.toUpperCase() == item.cQuestionFieldName.toUpperCase() && item.cValueOperator == selItem.cValueOperator);
                        if (withDifferentOperator) {
                            selItem.cValues = _.uniq([...selItem.cValues.split(','), ...withDifferentOperator.cValues.split(',')]).join(',');
                            selItem.cDescriptions = _.uniq([...selItem.cDescriptions.split(','), ...withDifferentOperator.cDescriptions.split(',')]).join(',');
                            const index = this.dto.selections.indexOf(dtoItem);
                            this.dto.selections.splice(index, 1);
                            dtoItem = withDifferentOperator;
                        }
                    }
                }
                if (dtoItem) {
                    dtoItem.cValues = selItem.cValues;
                    dtoItem.cValueOperator = selItem.cValueOperator;
                    dtoItem.cDescriptions = selItem.cDescriptions;
                } else {
                    newItems.push(selItem);
                }
            });
            if (newItems.length) {
                this.dto.selections.push(...newItems);
            }
            this.dto.selections.forEach(item => {
                item.cJoinOperator = "AND";
                item.iGroupNumber = 1;
                item.iGroupOrder = 1;
                item.cQuestionFieldName = this.getFilterOption(item.cQuestionFieldName).field;
            });
            const filterOption = this.getFilterOption(this.selectedDBAttribute.field);
            filterOption.isDatafilter = true;
        }
        this.resetData();
    }

    openSupressionModal(isPlaceOrder: boolean): void {
        if (this.Edit == 1) {
            const modalRef: NgbModalRef = this.modalService.open(SuppressionComponent, {
                size: ModalSize.EXTRA_LARGE,
                windowClass: ModalDefaults.WindowClass,
                backdrop: ModalDefaults.Backdrop,
            });
            modalRef.componentInstance.segmentId =
                this.fastCountSelectionDetails.segmentId;
            modalRef.componentInstance.isPlaceOrder = isPlaceOrder;
            modalRef.componentInstance.campaignId =
                this.fastCountSelectionDetails.campaignId;
            modalRef.componentInstance.buildId = this.fastCountSelectionDetails.buildId;
            modalRef.componentInstance.databaseId =
                this.fastCountSelectionDetails.databaseId;
            modalRef.result.then((params) => {
                this.getSeletctedOrderSuppression();
            });
        }
    }

    openGeoMapping(): void {
        if (this.Edit == 1) {
            const modalRef: NgbModalRef = this.modalService.open(GeoMappingComponent, {
                size: ModalSize.EXTRA_LARGE,
                windowClass: ModalDefaults.WindowClass,
                backdrop: ModalDefaults.Backdrop,
                centered: true,
            });
            modalRef.componentInstance.segmentId =
                this.fastCountSelectionDetails.segmentId;
            modalRef.componentInstance.campaignId =
                this.fastCountSelectionDetails.campaignId;
            modalRef.componentInstance.buildId = this.fastCountSelectionDetails.buildId;
            modalRef.componentInstance.databaseId =
                this.fastCountSelectionDetails.databaseId;
            modalRef.componentInstance.pageType = "FastCount";
            if (this.popupData) {
                modalRef.componentInstance.popupData = this.popupData;
            }
            modalRef.result.then((params) => {
                if (params.isSave) {
                    this.textData = params.popupData;
                    this.popupData = params.popupData;
                    this.saveFilters(false);
                } else if (this.popupData) {
                    this.textData = this.popupData;
                }
            });
        }
    }

    isValid() {
        if (this.selectedDBAttribute.field === "GeoMapping") return false;
        if (this.selectedFastCountAttribute['default-input'] == 'range') {
            if (this.rangeData.length > 0) {
                return !this.rangeData.some(item => (item.from == null || item.from.trim() === '') || ((item.to == null || item.to.trim() === '')));
            }
            return false;
        }
        let IsValid = this.selectedOptions.length !== 0 || this.textData !== "";
        return IsValid;
    }

    //on Add Title btn click - In cValueOperator
    onAddTitleClick() {
        if (this.isValid()) {
            this.saveFilters(false);            
        }
    }

    //on omit Title btn click - Not In cValueOperator
    onOmitTitleClick() {
        if (this.isValid()) {
            this.saveFilters(true);
        }
    }

    getFieldsValues(selectedDBAttribute, selection) {
        const options = this.selectedFastCountAttribute["options"];
        if (options) {
            const field = Object.getOwnPropertyNames(options)[0];
            this.optionValues = [{
                cDescription: field,
                id: 0,
                cValue: options[field]["cValues"]
            }];
            if (this.dto.selections.some(item => item.cQuestionFieldName.toUpperCase() == selectedDBAttribute.field.toUpperCase())) {
                this.selectedOptions = [{
                    ...this.optionValues[0]
                }];
            }
            this.sortOptions();
        } else {
            this._segmentSelectionProxy
                .getFieldValues(selectedDBAttribute.id, this.buildLolId)
                .subscribe((result) => {
                    let valueList = result;
                    const fastCountField = this.fastCountConfig["selection-fields"][selectedDBAttribute.field]
                    if (fastCountField && fastCountField["group-values"]) {
                        const groupedValues = _.groupBy(valueList, (item) => item.cDescription);
                        const groupValueList: ValueList[] = [];
                        for (const key in groupedValues) {
                            const values = groupedValues[key];
                            const firstValue = values[0].cValue;
                            const lastValue = values[values.length - 1].cValue;
                            const cValue = values.length > 1 ? `${firstValue}-${lastValue}` : `${firstValue}`;
                            groupValueList.push(ValueList.fromJS({
                                cValue,
                                cDescription: key,
                            }))
                        }
                        this.optionValues = groupValueList;
                    } else {
                        this.optionValues = valueList;
                    }

                    let cValueArray = [];
                    //show last checked checkbox / values of textarea.
                    if (this.dto.selections != null) {
                        this.dto.selections.forEach((sl) => {
                            if (
                                selection && sl.cQuestionFieldName.toUpperCase() === this.selectedDBAttribute.field.toUpperCase() &&
                                sl.cValueOperator === selection.cValueOperator
                            ) {
                                if (sl.cValueMode === "T") {
                                    if (this.selectedFastCountAttribute['default-input'] == 'range') {
                                        const rangeArray = sl.cValues.split(',');
                                        this.rangeData = [];
                                        rangeArray.forEach(item => {
                                            const values = item.split('-');
                                            this.rangeData.push({
                                                from: values[0],
                                                to: values[1]
                                            })
                                        })
                                    } else {
                                        this.textData = sl.cValues;
                                        if (sl.cValueOperator == 'LIKE' || sl.cValueOperator == 'NOT LIKE') {
                                            this.textData = this.textData.replace(/%/g, '');
                                        }
                                        this.active = 2;
                                    }
                                    return;
                                } else {
                                    cValueArray = sl.cValues.split(",");
                                    return true;
                                }
                            }
                        });
                        //Edit Campagian
                        if (selection != null) {
                            this.optionValues.forEach((element) => {
                                cValueArray.forEach((cval) => {
                                    if (element.cValue === cval && !this.selectedOptions.some(option => option.cValue === cval)) {
                                        this.selectedOptions.push(element);
                                    }
                                });
                            });
                        }
                        if (cValueArray.length > 0) {
                            this.selectedValuesOfCurrentSelection.forEach((element) => {
                                cValueArray.forEach((cval) => {
                                    if (element.cValue === cval && !this.selectedOptions.some(option => option.cValue === cval)) {
                                        this.selectedOptions.push(element);
                                    }
                                });
                            });
                            this.active = 1;
                        }
                    }

                    this.optionValues.forEach(
                        (ov) =>
                            (ov.cDescription = ov.cDescription ? ov.cDescription : ov.cValue)
                    );

                    // removing options given in exclude-options in FC config
                    if (this.selectedFastCountAttribute["exclude-options"]) {
                        const excludeOptions = this.selectedFastCountAttribute["exclude-options"];
                        for (let eo in excludeOptions) {
                            let ind = _.findIndex(this.optionValues, { 'cValue': excludeOptions[eo] });
                            if (ind > -1) {
                                this.optionValues.splice(ind, 1);
                            }
                        }
                    }

                    // masking option description based on mask-options given in FC Config
                    const maskOptions = this.selectedFastCountAttribute["mask-options"];
                    if (maskOptions) {
                        for (let field in maskOptions) {
                            if (this.optionValues.some(item => item.cDescription.toLowerCase() == field.toLowerCase())) {
                                this.optionValues.forEach(item => {
                                    if (item.cDescription.toLowerCase() == field.toLowerCase()) {
                                        item.cDescription = maskOptions[field];
                                    }
                                });
                            }
                        }
                    }
                    this.sortOptions();
                });
        }

    }

    sortOptions() {
        if (this.optionValues && this.optionValues.length > 0 && !this.selectedFastCountAttribute["skip-option-sorting"]) {
            this.optionValues = _.sortBy(this.optionValues, item => item.cDescription);
        }
    }

    //save the filters in-memory object
    saveFilters(isOmit: boolean) {
        const options = this.selectedFastCountAttribute["options"];

        if (options && !(this.selectedOptions && this.selectedOptions.length)) {
            return;
        }

        if (this.selectedOptions != null) {
            this.selectedOptions.forEach((element) => {
                if (
                    this.selectedValuesOfCurrentSelection.filter(
                        (ele) => ele.id === element.id
                    ).length === 0
                ) {
                    this.selectedValuesOfCurrentSelection.push(element);
                }
            });
        }
        if (this.dto.selections != null || undefined || this.textData) {
            this.dto.selections.forEach((sl) => {
                if (sl.cQuestionFieldName.toUpperCase() === this.selectedDBAttribute.field.toUpperCase()) {
                    if (isOmit === false && sl.cValueOperator === "IN") {
                        let indexofFilter = this.dto.selections.indexOf(sl);
                        if (sl.id > 0) {
                            this.dto.deletedSelections.push(sl.id);
                        }
                        this.dto.selections.splice(indexofFilter, 1);
                    } else if (isOmit === false && sl.cValueOperator === "LIKE") {
                        let indexofFilter = this.dto.selections.indexOf(sl);
                        if (sl.id > 0) {
                            this.dto.deletedSelections.push(sl.id);
                        }
                        this.dto.selections.splice(indexofFilter, 1);
                    } else if (isOmit && sl.cValueOperator === "NOT IN") {
                        let indexofFilter = this.dto.selections.indexOf(sl);
                        if (sl.id > 0) {
                            this.dto.deletedSelections.push(sl.id);
                        }
                        this.dto.selections.splice(indexofFilter, 1);
                    } else if (isOmit && sl.cValueOperator === "NOT LIKE") {
                        let indexofFilter = this.dto.selections.indexOf(sl);
                        if (sl.id > 0) {
                            this.dto.deletedSelections.push(sl.id);
                        }
                        this.dto.selections.splice(indexofFilter, 1);
                    } else if (sl.cValueOperator === ">") {
                        let indexofFilter = this.dto.selections.indexOf(sl);
                        if (sl.id > 0) {
                            this.dto.deletedSelections.push(sl.id);
                        }
                        this.dto.selections.splice(indexofFilter, 1);
                    }
                }
            });
        }

        if (options) {
            const optionData = options[Object.keys(options)[0]];
            this.currentUISelection.cValueOperator = optionData["cValueOperator"];
            this.currentUISelection.cValueMode = optionData["cValueMode"];
            this.currentUISelection.cValues = optionData["cValues"];
            this.currentUISelection.cDescriptions = optionData["cValues"];

        } else {
            if (isOmit) {
                if (this.selectedFastCountAttribute["omit-override-operator"]) {
                    this.currentUISelection.cValueOperator = this.selectedFastCountAttribute["omit-override-operator"];
                } else {
                    this.currentUISelection.cValueOperator = "NOT IN";
                }
            } else {
                if (this.selectedFastCountAttribute["add-override-operator"]) {
                    this.currentUISelection.cValueOperator = this.selectedFastCountAttribute["add-override-operator"];
                } else {
                    this.currentUISelection.cValueOperator = "IN";

                }
            }
        }

        this.currentUISelection.id = 0;
        this.currentUISelection.segmentId = this.fastCountSelectionDetails.segmentId;
        this.currentUISelection.cQuestionFieldName = this.selectedDBAttribute.field;
        this.currentUISelection.cQuestionDescription = this.selectedDBAttribute.label;
        this.currentUISelection.cJoinOperator = "AND";
        this.currentUISelection.iGroupNumber = 1;
        this.currentUISelection.iGroupOrder = 1;
        this.currentUISelection.cGrouping = "Y";
        this.currentUISelection.cDescriptions = "";
        this.currentUISelection.cFileName = "";
        this.currentUISelection.cSystemFileName = "";
        this.currentUISelection.cCreatedBy = "";
        this.currentUISelection.dCreatedDate = moment();
        this.currentUISelection.cTableName = this.selectedDBAttribute.data.cTableName;
        this.currentUISelection.isRuleUpdated = 0;
        this.currentUISelection.isDirectFileUpload = 0;

        if (!options) {
            if (this.selectedFastCountAttribute['default-input'] == 'range' || this.selectedFastCountAttribute['default-input'] == 'textarea') {
                this.currentUISelection.cValueMode = "T";
            } else {
                if (this.textData !== "" && this.active === 2) {
                    this.currentUISelection.cValueMode = "T";
                } else {
                    this.currentUISelection.cValueMode = "G";
                }
            }
        }

        let selectedValueArrayList = [];
        let selectedValueDescriptionList = [];

        if (!options) {
            if (
                this.selectedOptions.length > 0 &&
                this.currentUISelection.cValueMode === "G"
            ) {
                for (let i = 0; i < this.selectedOptions.length; i++) {
                    selectedValueArrayList.push(
                        this.selectedOptions[i].cValue.toString()
                    );
                    selectedValueDescriptionList.push(
                        this.selectedOptions[i].cDescription.toString()
                    );
                }

                selectedValueArrayList = _.union(selectedValueArrayList);
                this.currentUISelection.cValues = selectedValueArrayList.join(",");
                this.currentUISelection.cDescriptions = selectedValueDescriptionList.join(",");
            } else {
                if (this.selectedFastCountAttribute['default-input'] == 'range') {
                    const rangeWithFromTo = this.rangeData.map(item => `${item.from.trim()
                        }-${item.to.trim()}`);
                    const rangeWithComma = rangeWithFromTo.join(',');
                    this.currentUISelection.cValues = rangeWithComma;
                    this.currentUISelection.cDescriptions = rangeWithComma;
                } else {
                    if (this.textData.trim() && ( this.currentUISelection.cValueOperator == 'LIKE' || this.currentUISelection.cValueOperator == 'NOT LIKE')) {
                        const valSplit = this.textData.split(",");
                        if (valSplit.length > 0) {
                            for (var i = 0; i < valSplit.length; i++) {
                                if (!valSplit[i].endsWith('%'))
                                    valSplit[i] = valSplit[i] + '%';
                            }
                        }
                        this.currentUISelection.cValues = valSplit.join(",");
                    } else {
                        this.currentUISelection.cValues = this.textData;
                    }
                    this.currentUISelection.cDescriptions = this.textData;
                }
            }
        }
        //selected options checkbox - value retains
        let selectionList = SegmentSelectionDto.fromJS(this.currentUISelection);
        if (!options) {
            //clears the values on tab switch
            if (this.currentUISelection.cValueMode === "T") {
                this.selectedOptions = [];
            } else {
                this.textData = "";
            }
        }

        this.currentUISelection = {};
        const filterOption = this.getFilterOption(this.selectedDBAttribute.field)
        filterOption.isDatafilter = true;
        this.selected_row = -1;
        this.dto.selections.push(selectionList);
    }

    //Calling api for save button click if filters not saved and then calling quick count api
    recalculateCount() {
        this.isRecalculating = true;
        this.isRecalculateFail = false;
        this.dto.selections = this.dto.selections.map((selection) =>
            SegmentSelectionDto.fromJS(selection)
        );
        let saveFlag =
            (this.dto.selections.length !== 0 &&
                this.dto.selections.find(function (value) {
                    return value.id === 0;
                })) ||
            this.dto.deletedSelections.length !== 0 ||
            this.addOmitChange;
        if (saveFlag) {
            this.checkForDuplicates();
            this._segmentSelectionProxy
                .createSegmentSelectionDetails(this.dto, true)
                .subscribe(() => {
                    this.GetQuickCount();
                    this._segmentSelectionProxy
                        .getSelectionFieldsNew(
                            this.fastCountSelectionDetails.segmentId,
                            "1",
                            this.fastCountSelectionDetails.databaseId,
                            this.fastCountSelectionDetails.buildId,
                            this.fastCountSelectionDetails.mailerId
                        )
                        .subscribe((data: GetQueryBuilderDetails) => {
                            this.dto.selections = [];
                            let filterQuery = JSON.parse(data.filterQuery);
                            let getSavedRules =
                                filterQuery.rules && filterQuery.rules.length > 0
                                    ? filterQuery.rules[0].rules
                                    : [];
                            getSavedRules.forEach((ele) => {
                                let savedSelectedData = this.dto.selections.filter(
                                    (d) =>
                                        d.cQuestionFieldName.toUpperCase() === ele.field.toUpperCase() &&
                                        ele.operator === d.cValueOperator
                                )[0];
                                if (savedSelectedData) {
                                    savedSelectedData.id = ele.selectionId;
                                    savedSelectedData.isRuleUpdated = 1;
                                }
                                this.bindSelectedData(ele);
                            });
                            this.addOmitChange = false;
                            this.resetData();
                            this.notify.info(this.l("SavedSuccessfully"));
                            this.dto.deletedSelections = [];
                        });
                });
        } else {
            this.GetQuickCount();
        }
    }

    checkForDuplicates() {
        for (let index = 0; index < this.dto.selections.length; index++) {
            const element = this.dto.selections[index];
            const filterSelections = this.dto.selections.filter(item => item.cQuestionFieldName == element.cQuestionFieldName && item.cValueOperator == element.cValueOperator && item.cValues == element.cValues);
            
            if (filterSelections.length > 1) {
                for (let skipIndex = 0; skipIndex < filterSelections.length; skipIndex++) {
                    const item = filterSelections[skipIndex];
                    if (skipIndex == 0) { 
                        item.id = 0;
                        item.isRuleUpdated = 0;
                    } else {
                        if (item.id && !this.dto.deletedSelections.some(dItem => dItem == item.id)) {
                            this.dto.deletedSelections.push(item.id);
                        }
                        this.dto.selections.forEach((selItem, index) => {
                            if (selItem == item) {
                                this.dto.selections.splice(index, 1);
                            }
                        })
                    }
                }
            }
        }
    }

    //Method To Get Quick Count
    GetQuickCount(): void {
        if (this.fastCountSelectionDetails.segmentId) {
            this._segmentServiceProxy
                .getQuickCount(this.fastCountSelectionDetails.segmentId)
                .pipe(finalize(() => (this.isRecalculating = false)))
                .subscribe(
                    (result) => {
                        this.previousQuickCount = this.currentQuickCount;
                        this.currentQuickCount = Number(result.replace(/,/g, ""));
                        this.isRecalculateFail = false;
                    },
                    () => {
                        this.isRecalculateFail = true;
                    }
                );
        }
    }

    //Clear(Delete) functionality
    onClearClick(ruleToBeDeleted?: SegmentSelectionDto) {
        //clicked on cross icon - single clear
        if (this.Edit == 1) {

            if (ruleToBeDeleted) {
                if (this.selectedDBAttribute && ruleToBeDeleted.cQuestionFieldName.toUpperCase() === this.selectedDBAttribute.field.toUpperCase()) {
                    this.resetData();
                }
                this.dto.selections.forEach((element) => {
                    if (
                        ruleToBeDeleted.cQuestionFieldName.toUpperCase() === element.cQuestionFieldName.toUpperCase() &&
                        ruleToBeDeleted.cValueOperator === element.cValueOperator
                    ) {
                        let cancelledFilter = this.dto.selections.indexOf(element);
                        this.dto.selections.splice(cancelledFilter, 1);
                        if (
                            this.checkExistSelections(element) ||
                            this.dto.selections.length === 0
                        ) {
                            const filterOption = this.getFilterOption(element.cQuestionFieldName)
                            filterOption.isDatafilter = false;
                        }
                        if (element.id > 0) {
                            this.dto.deletedSelections.push(element.id);
                        }
                    }
                });

                if (!this.dto.selections.some(item => ["StateCity", "StateCountyCode", "StateCityNeighborhood", "CITYBYSTATE", "STATECOUNTYCODE"].some(stateItem => stateItem == item.cQuestionFieldName))) {
                    let countyFipsSelection = this.getFilterOption("Fips_Code");
                    if (!countyFipsSelection) {
                        countyFipsSelection = this.getFilterOption("StateCountyName");
                    }
                    countyFipsSelection.isDatafilter = false;
                }
            } else {
                //clicked on clear all - cancel the whole selection
                if (this.selectedDBAttribute && this.dto.selections.some(item => item.cQuestionFieldName.toUpperCase() === this.selectedDBAttribute.field.toUpperCase())) {
                    this.resetData();
                }
                this.dto.selections.forEach((element) => {
                    const filterOption = this.getFilterOption(element.cQuestionFieldName);
                    filterOption.isDatafilter = false;
                    if (["StateCity", "StateCountyCode", "StateCityNeighborhood", "CITYBYSTATE", "STATECOUNTYCODE"].some(item => item == element.cQuestionFieldName)) {
                        let countyFipsSelection = this.getFilterOption("Fips_Code");
                        // for B2b StateCountyName is used as fips code
                        if (!countyFipsSelection) {
                            countyFipsSelection = this.getFilterOption("StateCountyName");
                        }
                        countyFipsSelection.isDatafilter = false;
                    }
                    if (element.id > 0) {
                        this.dto.deletedSelections.push(element.id);
                    }
                });
                this.dto.selections = [];
            }
        }
    }

    resetData() {
        this.selected_row = -1;
        this.rangeData = [{ from: '', to: '' }];
        this.textData = "";
        this.selectedOptions = [];
        this.isOptionAvailable = false;
        this.selectedDBAttribute = undefined;
        this.selectedFastCountAttribute = undefined;
    }

    //Method To Change Add Or Omit In Selections

    gridAddOmitClick(selection: any): void {
        var selectedAttribute = this.fastCountConfig["selection-fields"][selection.cQuestionFieldName];
        if (selectedAttribute && selectedAttribute["exclude-add-omit-option"])
            return;
        if (this.Edit == 1) {
            this.selectedDBAttribute = this.getFilterOption(selection.cQuestionFieldName);
            if (selection.cValueOperator === "IN") {
                selection.cValueOperator = "NOT IN";
            } else if (selection.cValueOperator === "NOT IN") {
                selection.cValueOperator = "IN";
            } else if (selection.cValueOperator === "LIKE") {
                selection.cValueOperator = "NOT LIKE";
            } else if (selection.cValueOperator === "NOT LIKE") {
                selection.cValueOperator = "LIKE";
            }
            if (selection.id > 0) {
                selection.isRuleUpdated = 1;
            }
            this.addOmitChange = true;
        }
        // this.saveFilters(selection.cValueOperator == "IN" ? false : true);
    }

    getFilterOption(fieldName: any) {
        const allFilters = [];
        this.excludedSelectionsFilters.forEach(item => allFilters.push(...item.items));
        const result = allFilters.filter(
            (d) => d.value.field.toUpperCase() === fieldName.toUpperCase()
        );
        if (result && result.length > 0) {
            return result[0].value;
        }
        return null;
    }

    getSelections(selection: any, selcted_row: number): void {
        this.selectedDBAttribute = this.getFilterOption(selection.cQuestionFieldName);
        this.selectedFastCountAttribute = this.fastCountConfig["selection-fields"][this.selectedDBAttribute.field];
        this.selected_row = selcted_row;
        if (this.selectedDBAttribute.field === "GeoMapping" || this.selectedDBAttribute.field === "City") {
            const popupField = this.dto.selections.find(item => item.cQuestionFieldName.toUpperCase() === this.selectedDBAttribute.field.toUpperCase() && item.cValueOperator === selection.cValueOperator);
            this.textData = popupField.cValues;
            this.popupData = popupField.cValues;
        }
        this.createComponent({
            event,
            selection,
            option: {
                value: this.selectedDBAttribute
            },
        });
    }

    checkExistSelections(element: any): boolean {
        let isExist = true;
        if (
            this.dto.selections.filter(
                (d) => d.cQuestionDescription === element.cQuestionDescription
            ).length > 0
        ) {
            isExist = false;
        }
        return isExist;
    }

    openModalPopup(activeForm: string): void {
        if (this.Edit == 1 || activeForm == "CountReport" || activeForm == "PlaceOrder") {
            let component;
            switch (activeForm) {
                case "SaveCount":
                    component = FcSavecountComponent;
                    break;
                case "Maxper":
                    component = FcMaxperComponent;
                    break;
                case "CountReport":
                    component = FcCountreportComponent;
                    break;
                case "PlaceOrder":
                    component = FcPlaceorderComponent;
                    break;
            }
            const modalRef: NgbModalRef = this.modalService.open(
                component,
                {
                    size: ModalSize.LARGE,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass
                }
            );
            modalRef.componentInstance.campaignId = this.fastCountSelectionDetails.campaignId;
            modalRef.componentInstance.databaseId = this.fastCountSelectionDetails.databaseId;
            modalRef.componentInstance.divisionId = this.fastCountSelectionDetails.divisionId;
            modalRef.componentInstance.divisionalDatabase = this.fastCountSelectionDetails.divisionalDatabase;
            modalRef.componentInstance.buildId = this.fastCountSelectionDetails.buildId;
            modalRef.componentInstance.segmentId = this.fastCountSelectionDetails.segmentId;
            modalRef.componentInstance.fastCountConfig = this.fastCountConfig;
            modalRef.componentInstance.selections = this.dto.selections;

            modalRef.result.then((result) => {
                if (result.saving) {
                    this.isSave = true;
                    switch (activeForm) {
                        case "Maxper":
                            this.getMaxperData();
                            break;
                        case "PlaceOrder":
                            this.getCurrentCampaignStatus(undefined);
                            break;
                        case "SaveCount":
                            this.makeMailerIdPositive();
                            break;
                    }
                }
            });
        }
    }

    makeMailerIdPositive() {
        this.router.navigate(
            [],
            {
                relativeTo: this.activatedRoute,
                queryParams: { "mailerId": Math.abs(this.fastCountSelectionDetails.mailerId) },
                queryParamsHandling: 'merge',
            });
        this.fastCountSelectionDetails.mailerId = Math.abs(this.fastCountSelectionDetails.mailerId);
    }

    bindSelectedData(ele: any) {
        let selectedFilterData = this.getFilterOption(ele.field);
        this.currentUISelection.id = ele.selectionId;
        this.currentUISelection.segmentId = this.fastCountSelectionDetails.segmentId;
        this.currentUISelection.cQuestionFieldName = ele.field;
        this.currentUISelection.cQuestionDescription = selectedFilterData.label;
        this.currentUISelection.cJoinOperator = ele.cJoinOperator;
        this.currentUISelection.iGroupNumber = ele.iGroupNumber;
        this.currentUISelection.iGroupOrder = 1;
        this.currentUISelection.cGrouping = ele.cGrouping;
        this.currentUISelection.cValueOperator = ele.operator;
        this.currentUISelection.cFileName = ele.cFileName;
        this.currentUISelection.cSystemFileName = ele.cSystemFileName;
        this.currentUISelection.cCreatedBy = ele.cCreatedBy;
        this.currentUISelection.dCreatedDate = moment();
        this.currentUISelection.cTableName = selectedFilterData.data.cTableName;
        this.currentUISelection.isDirectFileUpload = 0;
        this.currentUISelection.cValueMode = ele.cValueMode;
        this.currentUISelection.cValues = ele.value;
        this.currentUISelection.cDescriptions = ele.value;
        this.currentUISelection.isRuleUpdated = ele.selectionId;

        let selectionList = SegmentSelectionDto.fromJS(this.currentUISelection);
        //clears the values on tab switch
        if (this.currentUISelection.cValueMode === "T") {
            this.selectedOptions = [];
        } else {
            this.textData = "";
        }
        //Get Saved Supressed Order
       this.getSeletctedOrderSuppression();

        this.currentUISelection = {};
        if (["StateCity", "StateCountyCode", "StateCityNeighborhood", "CITYBYSTATE", "STATECOUNTYCODE"].some(item => item == ele.field)) {
            let countyFipsSelection = this.getFilterOption("Fips_Code");
            if (!countyFipsSelection) {
                countyFipsSelection = this.getFilterOption("StateCountyName");
            }
            countyFipsSelection.isDatafilter = true;
        }
        selectedFilterData.isDatafilter = true;
        this.dto.selections.push(selectionList);
    }

    getSelectedAttributeLength(selection: SegmentSelectionDto) {
        if (selection.cQuestionFieldName.toUpperCase() === "GEOMAPPING") return 1;
        return selection.cValues.split(",").length;
    }

    getRecordCount(): string {
        if (this.previousQuickCount > this.currentQuickCount) {
            return `${(this.previousQuickCount - this.currentQuickCount).toLocaleString()
                } removed`;
        }
        return `${(this.currentQuickCount - this.previousQuickCount).toLocaleString()
            } added`;
    }

    getFieldDescription(selection: any): string {
        const field = this.fastCountConfig["selection-fields"][selection.cQuestionFieldName];
        return field ? field["fc-description"] : (selection.cQuestionDescription ? selection.cQuestionDescription : selection.cQuestionFieldName);
    }

    addNewRange() {
        this.rangeData.push({ from: '', to: '' });
    }

    removeRangeData(index: number) {
        this.rangeData.splice(index, 1);
    }

    getMaxperData() {
        this.isMaxPerLoading = true;
        this._campaignServiceProxy.fetchOrderLevelMaxPer(this.fastCountSelectionDetails.campaignId).subscribe(result => {
            const filterOption = this.getFilterOption("Maxper");
            if (result.cMaxPerFieldOrderLevel) {
                this.maxperCount = 1;
                filterOption.isDatafilter = true;
            } else {
                this.maxperCount = 0;
                filterOption.isDatafilter = false;
            }
            this.isMaxPerLoading = false;
        });
    }

    openCustomSupressionModal(): void {

        const modalRef: NgbModalRef = this.modalService.open(CustomSuppressionComponent, {
            size: ModalSize.EXTRA_LARGE,
            windowClass: ModalDefaults.WindowClass,
            backdrop: ModalDefaults.Backdrop,
        });
        modalRef.componentInstance.databaseId = this.fastCountSelectionDetails.databaseId;
        modalRef.componentInstance.mailerId = this.fastCountSelectionDetails.mailerId;
        modalRef.componentInstance.buildId = this.fastCountSelectionDetails.buildId;
        modalRef.componentInstance.campaignId = this.fastCountSelectionDetails.campaignId;
        modalRef.componentInstance.segmentId = this.fastCountSelectionDetails.segmentId;
        modalRef.componentInstance.id = this.selectedDBAttribute.id;
        modalRef.result.then((params) => {
            params.items.forEach(item => {
                this.queryBuilderfilterDetails.forEach(element => {
                    if (element.field === item.cQuestionFieldName) {
                        item.cTableName = element.data.cTableName;
                        item.cQuestionDescription = element.data.cFieldDescription; 
                    }
                })
                if (!this.dto.selections.some(d => d.cSystemFileName == item.cSystemFileName)) {
                    this.dto.selections.push(item);
                }
            });
        });

    }

    displayNotIn(selection): boolean {
        if (this.fastCountConfig["selection-fields"] && this.fastCountConfig["selection-fields"][selection.cQuestionFieldName]) {
            const result = this.fastCountConfig["selection-fields"][selection.cQuestionFieldName];
            return !(result["exclude-not-in-operator"] || result["exclude-omit-option"]);
        }
        return true;
    }

    displayBulkEnterChoice(): boolean {
        return !this.selectedFastCountAttribute["exclude-bulk-input"];
    }

    openUploadListModal(): void {
        const modalRef: NgbModalRef = this.modalService.open(UploadSuppressionComponent, {
            size: ModalSize.EXTRA_LARGE,
            windowClass: ModalDefaults.WindowClass,
            backdrop: ModalDefaults.Backdrop,
        });
        modalRef.componentInstance.campaignId =
            this.fastCountSelectionDetails.campaignId;
        modalRef.componentInstance.databaseId =
            this.fastCountSelectionDetails.databaseId;
        modalRef.componentInstance.divisionId =
            this.fastCountSelectionDetails.divisionId;
        modalRef.componentInstance.buildId =
            this.fastCountSelectionDetails.buildId;
        modalRef.componentInstance.mailerId =
            this.fastCountSelectionDetails.mailerId;
        modalRef.componentInstance.isEdit = 0;
        modalRef.componentInstance.segmentId = this.fastCountSelectionDetails.segmentId;
        modalRef.componentInstance.id = this.selectedDBAttribute.id;
        modalRef.componentInstance.openSuppressionevent.subscribe(() => {
            this.openCustomSupressionModal();
        })
        modalRef.result.then((params) => {
        });
    }

    isReadOnly() {
        return this.selectedFastCountAttribute["readonly"];
    }

    getSeletctedSuppression(item) {
        this.queryBuilderfilterDetails.forEach(element => {
            if (element.field === item.cQuestionFieldName) {
                item.cTableName = element.data.cTableName;
                item.cQuestionDescription = element.data.cFieldDescription;
            }
        })
        this.dto.selections.push(item);
    }

    isPlaceOrderDisabled() {
        if (!this.permissionForPlaceOrder) {
            return true;
        }
        let isEnabledByStatus = false;
        if (this.currentStatus == CampaignStatus.OrderCreated || this.currentStatus == CampaignStatus.OrderFailed
            || this.currentStatus == CampaignStatus.OutputFailed || this.currentStatus == CampaignStatus.OutputCompleted
            || this.currentStatus == CampaignStatus.ShippingFailed || this.currentStatus == CampaignStatus.Cancelled) {
            isEnabledByStatus = true;
        }
        return !this.currentQuickCount || +this.fastCountSelectionDetails.mailerId <= 0 || !isEnabledByStatus;
    }

    getSeletctedOrderSuppression() {
        this._segmentPrevOrdersesServiceProxy.getExistingPreviousOrders(
            this.fastCountSelectionDetails.campaignId,
            this.fastCountSelectionDetails.segmentId
        ).pipe(finalize(() => this.isLoading = false))
            .subscribe(result => {
                if (result.listOfSegmentPrevOrders.length)
                 {
                    this.isOrderSuppression=true;
                    this.orderSuppressionCount = result.listOfSegmentPrevOrders.length;
                }else{
                    this.isOrderSuppression=false;
                    this.orderSuppressionCount =0;
                }});
    }
}

interface RangeData {
    from?: string;
    to?: string;
}
