import { Component, Injector, Input, OnInit, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
    SegmentListsServiceProxy,
    SourceDto,
    ActionType,
    SubSelectsServiceProxy,
    CreateOrEditSubSelectDto,
    SubSelectSelectionsServiceProxy,
    SubSelectSelectionsDTO,
    SegmentSelectionsServiceProxy,
    SubSelectListsServiceProxy,
    DropdownOutputDto,
    SubSelectForViewDto,
    IntentTopicsServiceProxy,
    GetAllApprovedSourcesInput
} from "@shared/service-proxies/service-proxies";
import { Table } from "primeng/table";
import { finalize } from "rxjs/operators";
import { LazyLoadEvent } from "primeng/components/common/lazyloadevent";
import { Paginator } from "primeng/paginator";
import { SelectItem, MenuItem } from "primeng/api";

@Component({
    selector: "subsetsModal",
    styleUrls: ["../sources-modal.component.css", "subsets-modal.component.css"],
    templateUrl: "./subsets-modal.component.html"
})
export class SubsetsModalComponent extends AppComponentBase implements OnInit {
    saving: boolean = false;
    totalSubsetCount: number = 0;
    activeIndex: number = 0;
    subsets: any[];
    filtersList: any[] = [];
    selectedFilter: any;
    selectedOperator: any;
    operatorsList: any[];
    filterDropdownValue: DropdownOutputDto[] = [];
    selectedFilterId: string;
    cTableName: string = '';
    intentTopic: string = "Intent_Topic";
    iBTID: number;
    enteredValue: any;
    iShowDefault: number;
    showDefault: boolean = false;
    showTextbox: boolean = false;
    showDropdown: boolean = false;
    isAddFilterClicked: boolean = false;
    editSubsetId: number = 0;
    totalSubSelectionCount: number = 0;
    isSubsetUpdated: boolean = false;
    isRuleLoading: boolean = false;
    isSourcesUpdated: boolean = false;
    @Input() segmentId: number;
    @Input() campaignId: number;
    @Input() buildId: number;
    @Input() mailerId: number;
    @Input() segmentNo: string;
    @Input() campaignLevel: boolean;
    @Input() databaseId: number;
    buildLolId: number;
    filterText: string = "";
    pageTitle: string = "";
    isSubSetLoading: boolean = false;
    actionType = ActionType;
    approvedSources: SourceDto[] = [];
    addedSources: SourceDto[] = [];
    selectedSources: SourceDto[] = [];
    previouslySelectedSources: SourceDto[] = [];
    selectedSourcesLength: number = 0;
    deletedSources: SourceDto[] = [];
    deletedSourcesList: number[] = [];
    items: MenuItem[];
    operatorTypes: SelectItem[] = [
        { label: this.l("Sub_Include"), value: 'I' },
        { label: this.l("Sub_Exclude"), value: 'E' },
    ];
    levelTypes: SelectItem[] = [
        { label: this.l("Sub_Individuals"), value: 'I' },
        { label: this.l("Sub_Company"), value: 'C' },
    ];
    @ViewChild("subsetTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @ViewChild("approvedTable", { static: true }) approvedTable: Table;
    @ViewChild("selectedTable", { static: true }) selectedTable: Table;
    newSubset: CreateOrEditSubSelectDto;
    subsetSelection: SubSelectSelectionsDTO[] = [];
    countDefChanged: boolean = false;
    constructor(
        injector: Injector,
        private _segmentListsServiceProxy: SegmentListsServiceProxy,
        private _subSelectsServiceProxy: SubSelectsServiceProxy,
        private _subSelectsSelectionServiceProxy: SubSelectSelectionsServiceProxy,
        private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
        private _subSelectListServiceProxy: SubSelectListsServiceProxy,
        private _IntentTopicsServiceProxy: IntentTopicsServiceProxy,
        private activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit(): void {
        // Steps
        this.items = [{
            label: this.l("View"),
            command: () => {
                this.activeIndex = 0;
            }
        },
            {
                label: this.l("Sources"),
                command: () => {
                    this.activeIndex = 1;

                }
            },
            {
                label: this.l("Rules"),
                command: () => {
                    this.activeIndex = 2;
                }
            }
        ];

        this.newSubset = CreateOrEditSubSelectDto.fromJS({
            segmentId: this.segmentId,
            campaignId: this.campaignId,
            cIncludeExclude: 'I',
            cCompanyIndividual: 'I',
        });
        this.pageTitle = this.campaignLevel
            ? this.l("CampaignSubsets")
            : `${this.l("Segment")} ${this.segmentNo} ${this.l(
                "SubsetsTitle"
            )}`;
        this.getSubSelects();
    }

    getSources(id?: number, subsetData?: SubSelectForViewDto): void {
        let unsavedIDs = this.selectedSources
            .filter(source => source.action != ActionType.Delete)
            .map(a => a.listID);
        this.primengTableHelper.showLoadingIndicator();
        const input = GetAllApprovedSourcesInput.fromJS(
            {
                filter: this.filterText.trim(),
                segmentID: this.segmentId,
                unsavedListIDs: unsavedIDs
            });
        this._segmentListsServiceProxy
            .fetchApprovedSources(input)
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe((result: SourceDto[]) => {
                this.approvedSources = result;
                if (id) {
                    this.newSubset.cIncludeExclude = subsetData.cIncludeExclude === "Sub_Include" ? "I" : "E";
                    this.newSubset.cCompanyIndividual = subsetData.cCompanyIndividual === "Sub_Company" ? "C" : "I";
                    this.editSubset(id);
                }
            });
    }

    getSubSelects(event?: LazyLoadEvent) {
        this.showSubSetLoadingIndicator();
        this._subSelectsServiceProxy.getAllSubSelect(this.segmentId)
            .pipe(finalize(() => this.hideSubSetListLoadingIndicator()))
            .subscribe(result => {
                this.subsets = result;
                this.totalSubsetCount = result.length;
            });
    }

    addSelections(): void {
        if (this.editSubsetId > 0 && this.addedSources.length > 0) {
            this.isSourcesUpdated = true;
        }
        let existingSource: SourceDto;
        this.addedSources.forEach((source: SourceDto) => {
            existingSource = this.selectedSources.find(
                a => a.listID == source.listID && a.action == ActionType.Delete
            );
            if (existingSource) existingSource.action = ActionType.None;
            else this.selectedSources.push(SourceDto.fromJS(source));
        });
        this.filterText = "";
        this.selectedSourcesLength = this.selectedSources.filter(
            source => source.action != ActionType.Delete
        ).length;
        this.approvedSources = [];
        this.addedSources = [];
        this.deletedSources = [];
    }
    deleteSelections(): void {
        if (this.editSubsetId > 0 && this.deletedSources.length > 0) {
            this.isSourcesUpdated = true;
        }
        let existingSource: SourceDto;
        this.deletedSources.forEach((source: SourceDto) => {
            existingSource = this.selectedSources.find(
                a => a.listID == source.listID
            );
            if (existingSource) {
                if (existingSource.action == ActionType.Add) {
                    this.deletedSourcesList.push(existingSource.id);
                    this.selectedSources = this.selectedSources.filter(
                        value => value.listID != source.listID
                    );
                }
                else existingSource.action = ActionType.Delete;
            }
        });
        this.selectedSourcesLength = this.selectedSources.filter(
            source => source.action != ActionType.Delete
        ).length;
        this.filterText = "";
        this.addedSources = [];
        this.deletedSources = [];
    }

    save(): void {
        this.saving = true;
        let sourceIds: number[] = this.selectedSources.filter(
            source => source.action == ActionType.Add
        ).map(source => source.listID);
        if (this.editSubsetId === 0 && sourceIds.length === 0 && this.isAddFilterClicked === false) {
            this.getFiltersList();
            this.saving = false;
            return;
        }
        this.newSubset.sourceIds = sourceIds;
        if (this.editSubsetId !== 0) {
            this.newSubset.id = this.editSubsetId;
            this.newSubset.deletedSourceIds = this.deletedSourcesList;
        }
        if (this.editSubsetId > 0 && this.isSubsetUpdated) {
            this._subSelectsServiceProxy.updateSubSelect(this.newSubset)
                .subscribe(() => {
                    this.isSubsetUpdated = false;
                    this.countDefChanged = true;
                });
        }
        if (this.editSubsetId > 0 && this.isSourcesUpdated === false) {
            this.getFiltersList();
            return;
        }
        this._subSelectsServiceProxy.createSubSelect(this.newSubset)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(result => {
                this.editSubsetId = result;
                if (this.filtersList.length === 0) {
                    this.getFiltersList();
                }
                if (this.isAddFilterClicked) {
                    this.newFilter();
                    this.isAddFilterClicked = false;
                }
                this.notify.success(this.l("SavedSuccessfully"));
                this.countDefChanged = true;
            });
    }

    onSubSetUpdate(): void {
        if (this.editSubsetId > 0)
            this.isSubsetUpdated = true;
    }

    deleteSubset(id: number) {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                this._subSelectsServiceProxy.deleteSubSelect(this.campaignId, id).subscribe(() => {
                    this.notify.success(this.l("SuccessfullyDeleted"));
                    this.countDefChanged = true;
                    this.getSubSelects();
                });
            }
        });
    }
    close(): void {
        this.activeModal.close({ isSave: this.countDefChanged });
    }
    showSubSetLoadingIndicator(): void {
        setTimeout(() => {
            this.isSubSetLoading = true;
        }, 0);
    }
    hideSubSetListLoadingIndicator(): void {
        setTimeout(() => {
            this.isSubSetLoading = false;
        }, 0);
    }

    showRuleLoadingIndicator(): void {
        setTimeout(() => {
            this.isRuleLoading = true;
        }, 0);
    }
    hideRuleLoadingIndicator(): void {
        setTimeout(() => {
            this.isRuleLoading = false;
        }, 0);
    }

    editSubset(id: number): void {
        this.editSubsetId = id;
        this.addNewSources(false);
        this._subSelectListServiceProxy.getSubSelectSourcesForEdit(id).subscribe(result => {
            result.forEach(value => {
                var selectedSourcesList = this.approvedSources.find(x => x.listID == value.masterLOLID);
                if (selectedSourcesList) {
                    selectedSourcesList.action = ActionType.Add;
                    selectedSourcesList.id = value.id;
                    this.selectedSources.push(selectedSourcesList);
                }
            });
            this.filterText = "";
            this.selectedSourcesLength = this.selectedSources.filter(
                source => source.action != ActionType.Delete
            ).length;
            this.approvedSources = [];
            this.addedSources = [];
            this.deletedSources = [];
            this.deletedSourcesList = [];
        });
    }

    newFilter(): void {
        var newFilter = new SubSelectSelectionsDTO();
        newFilter.cQuestionFieldName = this.selectedFilter;
        newFilter.cQuestionDescription = '';
        newFilter.cJoinOperator = "AND";
        newFilter.cValueOperator = this.selectedOperator.toUpperCase();
        newFilter.cValueMode = this.showDefault ? 'G' : 'T';
        newFilter.cDescriptions = '';

        if (this.selectedFilter === this.intentTopic && newFilter.cValueMode === 'G') {
            newFilter.cValues = this.enteredValue.length > 1 ? this.enteredValue.join(',') : this.enteredValue[0];
            newFilter.cDescriptions = '';
            newFilter.cValueMode = 'T';
        }
        else {
            if (newFilter.cValueMode === 'G') {
                if (this.enteredValue.length > 1) {
                    this.enteredValue.forEach(value => {
                        var valueLabel = this.filterDropdownValue.find(x => x.value === value).label;
                        if (newFilter.cDescriptions === '') {
                            newFilter.cDescriptions = valueLabel;
                        }
                        else {
                            newFilter.cDescriptions = newFilter.cDescriptions.concat(",", valueLabel);
                        }
                    });
                    newFilter.cValues = this.enteredValue.join(',');
                } else {
                    var valueLabel = this.filterDropdownValue.find(x => x.value === this.enteredValue[0]).label;
                    newFilter.cDescriptions = valueLabel;
                    newFilter.cValues = this.enteredValue[0];
                }
            }
            else {
                newFilter.cValues = this.enteredValue.trim().toUpperCase();
            }
        }

        newFilter.subSelectId = this.editSubsetId;
        newFilter.cTableName = this.cTableName;
        newFilter.cGrouping = this.subsetSelection !== null && this.subsetSelection.length > 0 ? 'Y' : 'N';
        newFilter.addedFilterId = this.subsetSelection !== null && this.subsetSelection.length === 1 ? this.subsetSelection[0].id : 0;
        newFilter.campaignId = this.campaignId;

        this._subSelectsSelectionServiceProxy.createOrEditSubSelectSelection(newFilter)
            .subscribe(() => {
                this.notify.success(this.l("SavedSuccessfully"));
                this.countDefChanged = true;
                this.getSubSetSelections();
            });
    }

    saveFilter(): void {
        if (this.showDefault === false && this.enteredValue.trim() === "") {
            this.message.warn(this.l("EnterValue"));
            return;
        }
        if (this.editSubsetId === 0) {
            this.isAddFilterClicked = true;
            this.isSourcesUpdated = true;
            this.save();
        }
        else {
            this.newFilter();
        }
    }

    deleteFilter(id: number): void {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                var filterId = this.subsetSelection !== null && this.subsetSelection.length === 2 ? (id === this.subsetSelection[0].id ? this.subsetSelection[1].id : this.subsetSelection[0].id) : 0;
                this._subSelectsSelectionServiceProxy.deleteSubSelectSelection(id, filterId, this.campaignId)
                    .subscribe(() => {
                        this.notify.success(this.l("SuccessfullyDeleted"));
                        this.countDefChanged = true;
                        this.getSubSetSelections();
                    });
            }
        });
    }

    addNewSources(isEdit: boolean = true): void {
        this.activeIndex = 1;
        this.selectedSources = [];
        if (isEdit) {
            this.editSubsetId = 0;
            this.deletedSourcesList = [];
        }
    }

    previousStep(): void {
        if (this.activeIndex > 0)
            this.activeIndex = this.activeIndex - 1;
        this.saving = false;
        if (this.activeIndex == 1) {
            this.selectedSources = this.previouslySelectedSources;
        } else if (this.activeIndex == 0) {
            this.getSubSelects();
        }
    }

    getSubSetSelections(): void {
        this._subSelectsSelectionServiceProxy.getAllSubSelectSelections(this.editSubsetId, this.segmentId,this.buildId, '2', '')
            .subscribe(result => {
                this.buildLolId = result.buildLolId;
                const { fields } = result;
                this.subsetSelection = [];
                if (fields) {
                    var temp = 0;
                    fields.forEach(value => {
                        var filterDescription = this.filtersList.find(x => x.value === value.cQuestionFieldName.trim()).label;
                        fields[temp].cQuestionDescription = filterDescription;
                        ++temp;
                    });
                    this.subsetSelection = fields;
                    this.totalSubSelectionCount = fields.length;
                }
            });
    }

    addFilters(): void {
        this.save();
        this.activeIndex = 2;
        this.previouslySelectedSources = this.selectedSources;
        this.selectedSources = [];
        this.approvedSources = [];
        this.addedSources = [];
        this.deletedSources = [];
        this.getOperators();
    }

    getFiltersList(): void {
        this.showRuleLoadingIndicator();
        var subsetId = this.editSubsetId > 0 ? this.editSubsetId : -1;
        this._segmentSelectionProxy.getSubSelectSelections(subsetId, '2', this.databaseId, this.buildId , this.mailerId)
            .pipe(finalize(() => this.hideRuleLoadingIndicator()))
            .subscribe(result => {
                this.filtersList = result;
                this.selectedFilter = result[0].value;
                this.cTableName = result[0].data.cTableName;
                this.showTextbox = result[0].data.iShowTextBox;
                this.showDropdown = result[0].data.iShowListBox;
                this.showDefault = result[0].data.iShowDefault === 2 ? true : false;
                this.iShowDefault = result[0].data.iShowDefault;
                this.selectedFilterId = result[0].id;
                this.iBTID = result[0].data.iBTID;
                if (this.selectedFilter === this.intentTopic) {
                    if (this.iShowDefault !== 2 && this.showDropdown === true && this.showTextbox === false) {
                        this.showDefault = true;
                        this.iShowDefault = 2;
                    }
                    else {
                        this.iShowDefault = 2;
                        this.showDropdown = true;
                    }
                }
                if (this.iShowDefault === 2) {
                    this.getFilterValues();
                }
                this.getSubSetSelections();
            });
    }

    getOperators(): void {
        this._segmentSelectionProxy.getOperators().subscribe(result => {
            this.operatorsList = result;
            this.selectedOperator = result[0].value;
        });
    }

    switchInputType(toggleType: boolean): void {
        this.enteredValue = '';
        if (toggleType) {
            if (this.showDropdown) {
                this.showDefault = false;
            }
        }
        else {
            if (this.showTextbox) {
                this.showDefault = true;
            }
        }
    }

    getFilterValues(): void {
        if (this.selectedFilter === this.intentTopic) {
            this._IntentTopicsServiceProxy.getAllIntentTopics()
                .subscribe(result => {
                    this.filterDropdownValue = result;
                });
        }
        else {
            // var subsetId = this.editSubsetId > 0 ? this.editSubsetId : -1;
            this._segmentSelectionProxy.getSubSelectFieldValues(this.selectedFilterId.toString(),this.buildLolId)
                .subscribe(result => {
                    this.filterDropdownValue = result;
                });
        }
    }

    onFilterChange(): void {
        var filter = this.filtersList.find(e => e.value === this.selectedFilter);
        this.cTableName = filter.data.cTableName;
        this.showTextbox = filter.data.iShowTextBox;
        this.showDropdown = filter.data.iShowListBox;
        this.showDefault = filter.data.iShowDefault === 2 ? true : false;
        this.iShowDefault = filter.data.iShowDefault;
        this.selectedFilterId = filter.id;
        this.iBTID = filter.data.iBTID;
        if (this.selectedFilter === this.intentTopic) {
            if (this.iShowDefault !== 2 && this.showDropdown === true && this.showTextbox === false) {
                this.showDefault = true;
                this.iShowDefault = 2;
            }
            else {
                this.iShowDefault = 2;
                this.showDropdown = true;
            }
        }
        if (this.iShowDefault === 2) {
            this.getFilterValues();
        }
        this.enteredValue = '';
        this.filterDropdownValue = [];
    }
}
