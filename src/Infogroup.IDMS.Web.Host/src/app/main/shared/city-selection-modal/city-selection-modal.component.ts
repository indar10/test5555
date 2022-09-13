import { Component, Injector, Input, OnInit, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
    SegmentSelectionsServiceProxy,
    AdvanceSelectionsInputDto,
    StateServiceProxy,
    SegmentSelectionDto,
    AdvanceSelectionScreen,
    Field
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";
import { CitySelectionBuilder } from "./city-selection-builder";
import { Table } from "primeng/table";
import { AdvanceSelection } from '@app/main/campaigns/shared/campaign-models';

@Component({
    selector: "city-selection",
    styleUrls: ["./city-selection-modal.component.css"],
    templateUrl: "./city-selection-modal.component.html"
})
export class CitySelectionModalComponent extends AppComponentBase
    implements OnInit {
    saving: boolean = false;
    @Input() segmentId: number;
    @Input() campaignId: number;
    @Input() databaseId: number;
    @Input() buildId: number;
    @Input() isFastcount: boolean;
    filterText: string = "";
    sortByCode: boolean = false;
    saveDisabled: boolean = false;
    getDisabled: boolean = true;
    isCityLoading: boolean = false;
    isNeighborhoodLoading: boolean = false;
    counties: any[] = [];
    cities: any[] = [];
    neighborhoods: any[] = [];
    targetDatabaseId: number;
    selectionBuilder: CitySelectionBuilder;
    operatorTypes = [
        { label: "Include", value: "IN" },
        { label: "Exclude", value: "NOT IN" }
    ];
    selectedOperator = "IN";
    showCountyFilters: boolean = false;
    showCityFilters: boolean = false;
    showNeighborhoodFilters: boolean = false;
    selection: SegmentSelectionDto;
    @ViewChild("countyTable", { static: true }) countyTable: Table;
    @ViewChild("cityTable", { static: true }) cityTable: Table;
    @ViewChild("neighborhoodTable", { static: true }) neighborhoodTable: Table;
    constructor(
        injector: Injector,
        private _stateServiceProxy: StateServiceProxy,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
        private activeModal: NgbActiveModal
    ) {
        super(injector);
    }
    selected: any;
    ngOnInit(): void {
        this.selectionBuilder = new CitySelectionBuilder();
        let screenType = AdvanceSelectionScreen;
        if (this.campaignId) {
            this._stateServiceProxy
                .getState(this.databaseId, this.buildId, screenType.CountyCity)
                .subscribe(result => {
                    this.getDisabled = false;
                    this.selectionBuilder.states = result.states;
                    if (result.states.length > 0)
                        this.selectionBuilder.selectedState = result.states[0].value;
                    this.selectionBuilder.configuredfields = result.configuredFields;
                    this.targetDatabaseId = result.targetDatabaseId;
                });
        }

        if (this.selection) {
            const advanceSelection = new AdvanceSelection();
            advanceSelection.field = Field.fromJS({
                cQuestionFieldName: this.selection.cQuestionFieldName,
                cTableName: this.selection.cTableName,
                cQuestionDescription: this.selection.cQuestionDescription
            });
            advanceSelection.description = this.selection.cDescriptions;
            advanceSelection.value = this.selection.cValues;
            advanceSelection.data = [{
                root: {
                    code: this.selection.cValues,
                    description: this.selection.cDescriptions,
                }
            }]
            this.selectedOperator = this.selection.cValueOperator;
            this.selectionBuilder.selections.push(advanceSelection);
        }
    }
    onStateChange(): void {
        this.counties = [];
        this.cities = [];
        this.neighborhoods = [];
        this.showCountyFilters = false;
        this.showCityFilters = false;
        this.showNeighborhoodFilters = false;
    }
    getCounty(): void {
        this.counties = [];
        this.cities = [];
        this.neighborhoods = [];
        this.selectionBuilder.selectedNeighborhood = [];        
        this.selectionBuilder.selectedCities = [];
        this.selectionBuilder.selectedCounties = [];
        this.showCountyFilters = false;
        this.showCityFilters = false;
        this.showNeighborhoodFilters = false;
        this.countyTable.reset();
        if (this.selectionBuilder.selectedState !== "") {
            this.primengTableHelper.showLoadingIndicator();
            this._stateServiceProxy
                .getCounty(this.selectionBuilder.selectedState, this.targetDatabaseId)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    this.counties = result;
                    this.showCountyFilters = result.length > 0;
                });
        }
    }
    getCity(cCountyCode?: string): void {
        this.selectionBuilder.selectedCities = [];
        this.showCityFilters = false;
        this.showNeighborhoodFilters = false;
        this.neighborhoods = [];
        this.selectionBuilder.selectedNeighborhood = [];        
        this.cityTable.reset();
        if (!cCountyCode) {
            this.counties = [];
            this.selectionBuilder.selectedCounties = [];
            this.showCountyFilters = false;
        }
        if (this.selectionBuilder.selectedState !== "") {
            this.showCityLoadingIndicator();
            this._stateServiceProxy
                .getCity(
                    this.selectionBuilder.selectedState,
                    cCountyCode,
                    this.targetDatabaseId
                )
                .pipe(finalize(() => this.hideCityLoadingIndicator()))
                .subscribe(result => {
                    this.cities = result;
                    this.showCityFilters = result.length > 0;
                });
        }
    }

    getNeighborhood(cCity?:string):void {
        
        this.selectionBuilder.selectedNeighborhood = [];
        this.showNeighborhoodFilters = false;
        
        this.neighborhoodTable.reset();
        if (this.selectionBuilder.selectedState !== "") {
            this.showNeighborhoodLoadingIndicator();
            this._stateServiceProxy
                .getNeighborhood(this.selectionBuilder.selectedState, this.targetDatabaseId, cCity.toUpperCase())
                .pipe(finalize(() => this.hideNeighborhoodLoadingIndicator()))
                .subscribe(result => {
                    this.neighborhoods = result;
                    this.showNeighborhoodFilters = result.length > 0;
                });
        }
    }

    onCountySelectionChange(): void {
        this.selectionBuilder.selectedCities = [];
        this.neighborhoods = [];
        this.selectionBuilder.selectedNeighborhood = [];
        this.neighborhoodTable.reset();
        this.showNeighborhoodFilters = false;
        this.cities = [];
        if (this.selectionBuilder.selectedCounties.length === 1)
            this.getCity(this.selectionBuilder.selectedCounties[0].value);
        else this.showCityFilters = false;
    }

    onCitySelectionChange(): void {
        this.selectionBuilder.selectedNeighborhood = [];
        this.neighborhoods = [];
        this.selectionBuilder.selectedNeighborhood = [];
        this.neighborhoodTable.reset();
        this.showNeighborhoodFilters = false;
        this.selectionBuilder.selectedCity = "";
        if (this.selectionBuilder.selectedCities.length === 1) {
            this.getNeighborhood(this.selectionBuilder.selectedCities[0].value);
            this.selectionBuilder.selectedCity=this.selectionBuilder.selectedCities[0].value;
        }
        
    }

    clearCities(): void {
        this.cities = [];
    }
    save(): void {
        let cityFields: SegmentSelectionDto[] = this.selectionBuilder.selections.map(
            sic => {
                return SegmentSelectionDto.fromJS({
                    segmentId: this.segmentId,
                    cQuestionFieldName: sic.field.cQuestionFieldName,
                    cQuestionDescription: sic.field.cQuestionDescription,
                    cJoinOperator: "OR",
                    iGroupNumber: 0,
                    iGroupOrder: 1,
                    cGrouping: "Y",
                    cValues: sic.value,
                    cValueMode: "T",
                    cDescriptions: sic.description,
                    cValueOperator: this.selectedOperator,
                    cFileName: "",
                    cSystemFileName: "",
                    cCreatedBy: "",
                    dCreatedDate: null,
                    cModifiedBy: "",
                    dModifiedDate: null,
                    cTableName: sic.field.cTableName,
                    orderID: this.campaignId,
                    id: 0
                });
            }
        );

        if (this.isFastcount) {
            if (this.selection) {
                const isSame = cityFields.every(item => item.cQuestionFieldName == this.selection.cQuestionFieldName);
                if (!isSame) {
                    this.message.error(`Choose only: ${this.selection.cQuestionFieldName}`);
                    return;
                }
            }
            this.activeModal.close({ isSave: true, selections: cityFields });
            return;
        }
        
        cityFields[0].cJoinOperator = "AND";
        let input = new AdvanceSelectionsInputDto();
        input.segmentID = this.segmentId;
        input.sicFields = cityFields;
        input.primarySICField = null;
        this.saving = true;
        this._segmentSelectionProxy
            .saveAdvanceSelection(this.campaignId, input)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe(() => {
                this.notify.info(this.l("SavedSuccessfully"));
                this.activeModal.close({ isSave: true });
            });
    }
    addSelection(): void {
        let result = this.selectionBuilder.add();
        if (!result.success)
            this.message.error(this.l(result.localizationKey, ...result.params));
    }
    delete(index: number): void {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                this.selectionBuilder.selections.splice(index, 1);
            }
        });
    }

    close(): void {
        this.activeModal.close({ isSave: false });
    }

    showCityLoadingIndicator(): void {
        setTimeout(() => {
            this.isCityLoading = true;
        }, 0);
    }
    hideCityLoadingIndicator(): void {
        setTimeout(() => {
            this.isCityLoading = false;
        }, 0);
    }
    showNeighborhoodLoadingIndicator(): void {
        setTimeout(() => {
            this.isNeighborhoodLoading = true;
        }, 0);
    }
    hideNeighborhoodLoadingIndicator(): void {
        setTimeout(() => {
            this.isNeighborhoodLoading = false;
        }, 0);
    }
}
