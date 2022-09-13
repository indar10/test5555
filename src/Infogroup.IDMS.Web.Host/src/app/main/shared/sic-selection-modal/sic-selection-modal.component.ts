import { Component, Injector, Input, OnInit } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
  SICCodesServiceProxy,
  DropdownOutputDto,
  SegmentSelectionDto,
  SegmentSelectionsServiceProxy,
  AdvanceSelectionsInputDto,
  BuildTableLayoutsServiceProxy,
  AdvanceSelectionScreen,
  ValidateSICCodesInputDto,
  Field
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";
import { TreeNode } from "primeng/api";
import { SICSelectionBuilder } from "./sic-selection";
import { AdvanceSelection } from '@app/main/campaigns/shared/campaign-models';

@Component({
  selector: "sic-selection",
  styleUrls: ["./sic-selection-modal.component.css"],
  templateUrl: "./sic-selection-modal.component.html"
})
export class SICSelectionModalComponent extends AppComponentBase
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
  isTreeLoading: boolean = false;
  isFranchiseLoading: boolean = false;
  isIndustryLoading: boolean = false;
  industries: DropdownOutputDto[] = [];
  franchises: DropdownOutputDto[] = [];
  selectedSICs: SICSelectionBuilder;
  sicTree: TreeNode[] = [];
  showTree: boolean = false;
  rootNode: TreeNode = {
    label: "",
    leaf: false,
    expanded: true,
    key: "0",
    data: {
      level: 0
    },
    children: []
  };
  showFranchiseFilters: boolean = false;
  showIndustryFilters: boolean = false;
  isSmartAddLoading: boolean = false;
  currentSICLength: number = 6;
  sicTypes = [
    { label: "Primary", value: "P" },
    { label: "Secondary", value: "S" },
    { label: "Any", value: "A" }
  ];
  sicLengths = [
    { label: "6 Digits", value: 6 },
    { label: "4 Digits", value: 4 },
    { label: "2 Digits", value: 2 }
  ];
  operatorTypes = [
    { label: "Include", value: "IN" },
    { label: "Exclude", value: "NOT IN" }
  ];
  selectedOperator = "IN";
  indicators = [
    { name: "Franchise", color: "red" },
    { name: "Industry", color: "green" },
    { name: "Both", color: "blue" },
    { name: "None", color: "black" }
  ];
  YDescription: string;
  NDescription: string;
  fcSelection: SegmentSelectionDto;

  constructor(
    injector: Injector,
    private _SICCodesServiceProxy: SICCodesServiceProxy,
    private _buildTableLayoutsServiceProxy: BuildTableLayoutsServiceProxy,
    private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
    private activeModal: NgbActiveModal
  ) {
    super(injector);
  }
  selected: any;

  ngOnInit(): void {
    this.selectedSICs = new SICSelectionBuilder();
    let screenType = AdvanceSelectionScreen;
    if (this.campaignId) {
      this._buildTableLayoutsServiceProxy
        .getFieldDetails(this.databaseId, this.buildId, screenType.Industry)
        .subscribe(result => {
          this.selectedSICs.configuredfields = result;
          this.YDescription = result.primarySICFlag.values.find(
            item => item.value === "Y"
          ).label;
          if (this.YDescription == undefined) this.YDescription = "Yes";
          this.NDescription = result.primarySICFlag.values.find(
            item => item.value === "N"
          ).label;
          if (this.NDescription == undefined) this.NDescription = "No";

          if (this.fcSelection && this.isFastcount) {
            const advanceSelection = new AdvanceSelection();
            advanceSelection.field = Field.fromJS({
              cQuestionFieldName: this.fcSelection.cQuestionFieldName,
              cTableName: this.fcSelection.cTableName,
              cQuestionDescription: this.fcSelection.cQuestionDescription
            });
            advanceSelection.description = this.fcSelection.cDescriptions;
            advanceSelection.value = this.fcSelection.cValues;
            advanceSelection.data = [{
              root: {
                code: this.fcSelection.cValues,
                description: this.fcSelection.cDescriptions,
              },
              children: []
            }]
            this.selectedOperator = this.fcSelection.cValueOperator;
            this.selectedSICs.selections.push(advanceSelection);
          }
        });
    }
  }
  
  getSICCodes(): void {
    this.showTreeLoadingIndicator();
    this.sicTree = [];
    this.clearTables();
    this.selectedSICs.clearAll();
    this._SICCodesServiceProxy
      .getSICCode(
        this.filterText,
        this.selectedSICs.selectedLength.toString(),
        this.sortByCode
      )
      .pipe(finalize(() => this.hideTreeLoadingIndicator()))
      .subscribe(result => {
        if (result.length > 0) {
          this.currentSICLength = result[0].data.sicLength;
          this.sicTree.push({ ...this.rootNode });
          this.sicTree[0].children = result;
        }
      });
  }

  sortSIC(): void {
    if (this.sicTree.length > 0) {
      this.sortChildren(this.sicTree[0].children);
    }
  }
  sortChildren(children: TreeNode[]): void {
    if (typeof children !== "undefined" && children.length > 0) {
      if (this.sortByCode)
        children.sort((a, b) => a.data.cSICCode.localeCompare(b.data.cSICCode));
      else
        children.sort((a, b) =>
          a.data.cDescription.localeCompare(b.data.cDescription)
        );
      children.forEach(child => this.sortChildren(child.children));
    }
  }

  getFranchiseAndIndustry(): void {
    let selectedSICCodes = this.selectedSICs.selectedNodes.filter(
      node => node.key !== "0"
    );
    if (
      selectedSICCodes.length === 1 &&
      selectedSICCodes[0].data.cIndicator !== "N"
    ) {
      this.showLoadingIndicator();
      this._SICCodesServiceProxy
        .getFranchiseNIndustryCode(
          selectedSICCodes[0].data.cSICCode,
          selectedSICCodes[0].data.cIndicator
        )
        .pipe(
          finalize(() => {
            this.hideLoadingIndicator();
          })
        )
        .subscribe(result => {
          this.franchises = result.franchises;
          this.industries = result.industries;
        });
    }
  }
  clearTables(): void {
    this.franchises = [];
    this.industries = [];
    this.showFranchiseFilters = this.showIndustryFilters = false;
    this.selectedSICs.selectedfranchises = [];
    this.selectedSICs.selectedindustries = [];
  }
  onSICSelect(event): void {
    this.clearTables();
    if (event.node.data.level === 0) {
      this.selectedSICs.selectedNodes = [...event.node.children];
      this.selectedSICs.selectedNodes.push(event.node);
    } else {
      this.getFranchiseAndIndustry();
    }
  }
  onSICDeselect(event): void {
    this.clearTables();
    if (event.node.data.level === 0) {
      this.selectedSICs.selectedNodes = [];
    } else {
      this.getFranchiseAndIndustry();
    }
  }

  save(): void {
    let primarySICField: SegmentSelectionDto;
    let sicFields: SegmentSelectionDto[] = this.selectedSICs.selections.map(
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
    sicFields[0].cJoinOperator = "AND";
    let primaryFlagDetails =
      this.selectedSICs.selectedSICType === "P"
        ? { value: "Y", description: this.YDescription }
        : this.selectedSICs.selectedSICType === "S"
          ? { value: "N", description: this.NDescription }
          : undefined;
    if (primaryFlagDetails) {
      primarySICField = SegmentSelectionDto.fromJS({
        segmentId: this.segmentId,
        cQuestionFieldName: this.selectedSICs.configuredfields.primarySICFlag
          .cQuestionFieldName,
        cQuestionDescription: "",
        cJoinOperator: "AND",
        iGroupNumber: 0,
        iGroupOrder: 1,
        cGrouping: "N",
        cValues: primaryFlagDetails.value,
        cValueMode:
          this.selectedSICs.configuredfields.primarySICFlag.cValueMode === "G"
            ? "G"
            : "T",
        cDescriptions: primaryFlagDetails.description,
        cValueOperator: "IN",
        cFileName: "",
        cSystemFileName: "",
        cCreatedBy: "",
        dCreatedDate: null,
        cModifiedBy: "",
        dModifiedDate: null,
        cTableName: this.selectedSICs.configuredfields.primarySICFlag
          .cTableName,
        orderID: this.campaignId,
        id: 0
      });
    }
    if (this.isFastcount) {
      if (this.fcSelection) {
        const isSame = sicFields.every(item => item.cQuestionFieldName == this.fcSelection.cQuestionFieldName);
        if (!isSame) {
          this.message.error(`Choose only: ${this.fcSelection.cQuestionFieldName}`);
          return;
        }
      }
      this.activeModal.close({ isSave: true, selections: sicFields });
      return;
    }

    let input = new AdvanceSelectionsInputDto();
    input.segmentID = this.segmentId;
    input.sicFields = sicFields;
    input.primarySICField = primarySICField;
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
    if (this.selectedSICs.selectedLength !== this.currentSICLength)
      this.message.error(this.l("SICDirty"));
    else {
      let result = this.selectedSICs.add();
      if (!result.success)
        this.message.error(this.l(result.localizationKey, ...result.params));
    }
  }
  addSmartSelection(): void {
    if (this.filterText) {
      let input: ValidateSICCodesInputDto = ValidateSICCodesInputDto.fromJS({ searchText: this.filterText });
      setTimeout(() => { this.isSmartAddLoading = true; }, 0);
      this._SICCodesServiceProxy.validateSICCodes(input)
        .pipe(finalize(() => setTimeout(() => { this.isSmartAddLoading = false; }, 0)))
        .subscribe(result => {
          result.sicSelections.forEach((selections, index) => {
            if (selections.length > 0) {
              let result = this.selectedSICs.smartAdd(selections, ((index + 1) * 2).toString())
              if (!result.success) {
                this.message.error(this.l(result.localizationKey, ...result.params));
                return;
              }
            }
          });
          if (result.warningMessage)
            this.message.warn(result.warningMessage, '')
        });
    }
  }
  delete(index: number) {
    this.message.confirm(this.l(""), isConfirmed => {
      if (isConfirmed) {
        this.selectedSICs.selections.splice(index, 1);
      }
    });
  }

  close(): void {
    this.activeModal.close({ isSave: false });
  }
  showTreeLoadingIndicator(): void {
    setTimeout(() => {
      this.isTreeLoading = true;
    }, 0);
  }
  hideTreeLoadingIndicator(): void {
    setTimeout(() => {
      this.isTreeLoading = false;
    }, 0);
  }
  showLoadingIndicator(): void {
    setTimeout(() => {
      this.isFranchiseLoading = true;
      this.isIndustryLoading = true;
    }, 0);
  }
  hideLoadingIndicator(): void {
    setTimeout(() => {
      this.isFranchiseLoading = false;
      this.isIndustryLoading = false;
    }, 0);
  }
}
