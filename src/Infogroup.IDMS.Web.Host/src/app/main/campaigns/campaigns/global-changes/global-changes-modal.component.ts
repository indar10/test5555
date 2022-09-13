import { Component, Injector, Input, OnInit, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
  SegmentSelectionsServiceProxy,
  SaveGlobalChangesInputDto,
  PageID,
  ShortSearchServiceProxy,
  SaveBatchSegmentDto
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";
import { Paginator } from "primeng/components/paginator/paginator";
import { Table } from "primeng/components/table/table";
import {
  SelectionAction,
  GlobalChangesAction
} from "../../shared/selection-action.enum";
import { LazyLoadEvent } from "primeng/components/common/lazyloadevent";
import { SelectItem } from "primeng/api";
import { BatchEditSegmentComponent } from "./batch-edit-segment/batch-edit-segment.component";

@Component({
  selector: "global-changes",
  styleUrls: ["./global-changes-modal.component.css"],
  templateUrl: "./global-changes-modal.component.html"
})
export class GlobalChangesModalComponent extends AppComponentBase
  implements OnInit {
  @Input() sourceSegmentId: number;
  @Input() campaignId: number;
  @Input() buildId: number;
  @Input() mailerId: number;
  @Input() databaseId: number;
  @Input() divisionId: number;
  @Input() selectedAction: any;
  @Input() splitType: number;
  isInlineEdit = false;
  pageId: PageID = PageID.AddFavorites;
  helpData: any = {
    header: "Search Options:",
    examples: []
  };
  fields: SelectItem[] = [];
  @ViewChild("segmentTable", { static: false }) dataTable: Table;
  @ViewChild("paginator", { static: false }) paginator: Paginator;
  saving: boolean = false;
  savingFromSave: boolean = false;
  actionType = SelectionAction;
  currentStatus: number;
  filterText: string = "";
  finalFilterText: string = "";
  globalChangesTitle: string = "";
  showFindReplace: boolean = false;
  selectedField: any;
  findText: string;
  replaceText: string;
  campaignSearchText: string;
  isFieldsLoading: boolean = false;
  isHelpDisabled: boolean = true;
  findRequired: boolean = false;
  historySearchRequired: boolean = false;
  replaceRequired: boolean = false;
  fieldRequired: boolean = false;
  showSaveAndClose: boolean = false;
  isSaved: boolean = false;
  isFieldDisabled: boolean = false;
  compareFieldName: string = "I";
  includeExclude: string = "I";
  segmentFieldsForReload: string[] = ['cDescription', 'iRequiredQty', 'cKeyCode1'];
  segmentFields: SelectItem[] = [
    { value: "cDescription", label: "Segment Description" },
    { value: "iUseAutosuppress", label: "Apply Default Rules" },
    { value: "cKeyCode1", label: "Key Code 1" },
    { value: "cKeyCode2", label: "Key Code 2" },
    { value: "iRequiredQty", label: "Required Quantity" },
    { value: "cMaxPerGroup", label: "Max Per Group" },
    { value: "cTitleSuppression", label: "Title Suppression" },
    { value: "cFixedTitle", label: "Fixed Title" },
    { value: "iDoubleMultiBuyerCount", label: "Double Multi Buyer Count" },
    { value: "iOutputQty", label: "Output Quantity" },
    { value: "iIsRandomRadiusNth", label: "Random Radius Nth" }
  ];
  operatorTypes: SelectItem[] = [
    { label: this.l("Sub_Include"), value: 'I' },
    { label: this.l("Sub_Exclude"), value: 'E' },
  ];
  levelTypes: SelectItem[] = [
    { label: this.l("Sub_Individuals"), value: 'I' },
    { label: this.l("Sub_Company"), value: 'C' },
  ];
  historyActions = [
    { label: this.l("AppendHistory"), value: 2 },
    { label: this.l("ReplaceHistory"), value: 1 },
    { label: this.l("DeleteHistory"), value: 3 },
  ];
  option: number = 2;
  showEditableGrid: boolean = false;
  @ViewChild(BatchEditSegmentComponent, { static: true }) editablaGrid: BatchEditSegmentComponent;
  constructor(
    injector: Injector,
    private activeModal: NgbActiveModal,
    private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
    private _shortSearchServiceProxy: ShortSearchServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.primengTableHelper.records = [];
    this.isHelpDisabled = true;
    this.showSaveAndClose = this.selectedAction == SelectionAction.BulkCampaignHistory || this.selectedAction == SelectionAction.EditSegments || this.selectedAction == SelectionAction.FindReplace;
    this._shortSearchServiceProxy
      .getSearchHelpText(this.pageId)
      .subscribe(result => {
        this.helpData = result;
        this.isHelpDisabled = false;
      });
    switch (this.selectedAction) {
      case SelectionAction.AppendRules:
        this.globalChangesTitle = "AppendRulesTitle";
        break;
      case SelectionAction.BulkCampaignHistory:
        this.globalChangesTitle = "BulkCamapignHistoryTitle";
        this.historySearchRequired = true;
        break;
      case SelectionAction.DeleteRules:
        this.globalChangesTitle = "DeleteRulesTitle";
        break;
      case SelectionAction.DeleteSegments:
        this.globalChangesTitle = "DeleteSegmentsTitle";
        break;
      case SelectionAction.EditSegments:
        this.globalChangesTitle = "EditSegmentsTitle";
        this.fieldRequired = true;
        this.fields = [...this.segmentFields];
        if (this.fields.length > 0)
          this.selectedField = this.segmentFields[0].value;
        break;
      case SelectionAction.FindReplace:
        this.globalChangesTitle = "FindReplaceTitle";
        this.findRequired = true;
        this.replaceRequired = true;
        this.fieldRequired = true;
        this.isFieldsLoading = true;
        this._segmentSelectionProxy
          .getFieldsForFindReplace(this.buildId, this.databaseId , this.mailerId)
          .subscribe(result => {
            this.fields = result;
            if (this.fields.length > 0)
              this.selectedField = this.fields[0].value;
            this.isFieldsLoading = false;
          });
        break;
    }
  }
  getSegments(event?: LazyLoadEvent) {
    if (this.filterText != undefined && this.filterText != "") {
      if (this.showEditableGrid)
        this.editablaGrid.getSegmentsForEdit(this.filterText, this.campaignId);
      else {
        if (this.primengTableHelper.shouldResetPaging(event)) {
          this.paginator.changePage(0);
          return;
        }
        this.showFindReplace = false;
        this.primengTableHelper.showLoadingIndicator();
        this.primengTableHelper.records = [];
        this.primengTableHelper.totalRecordsCount = 0;
        this._segmentSelectionProxy
          .getSegmentsForGlobalChanges(
            this.filterText,
            this.campaignId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
          )
          .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
          .subscribe(result => {
            this.primengTableHelper.totalRecordsCount =
              result.pagedSegments.totalCount;
            this.primengTableHelper.records = result.pagedSegments.items;
            this.finalFilterText = result.finalFilterText;
          });
      }
    }
  }

  save(saveAndclose: boolean): void {
    if (this.showEditableGrid) this.saveSegmentGrid(saveAndclose);
    else {
      if (this.filterText !== this.finalFilterText) {
        this.message.error(this.l("SearchQueryChangedError"));
        return;
      }
      this.message.confirm(this.l(""), isConfirmed => {
        if (isConfirmed) {
          let input = SaveGlobalChangesInputDto.fromJS({
            campaignId: this.campaignId,
            filterText: this.finalFilterText,
            databaseId: this.databaseId,
            divisionId: this.divisionId,
            sourceSegment: 0
          });
          input.campaignStatus = this.currentStatus;
          switch (this.selectedAction) {
            case SelectionAction.AppendRules:
              input.sourceSegment = this.sourceSegmentId;
              input.action = GlobalChangesAction.AppendRules;
              break;
            case SelectionAction.BulkCampaignHistory:
              input.sourceSegment = this.sourceSegmentId;
              input.action = GlobalChangesAction.CampaignHistory;
              input.searchValue = this.campaignSearchText;
              input.option = this.option;
              input.includeExclude = this.includeExclude;
              input.compareFieldName = this.compareFieldName;
              break;
            case SelectionAction.DeleteRules:
              input.sourceSegment = this.sourceSegmentId;
              input.action = GlobalChangesAction.DeleteRules;
              break;
            case SelectionAction.DeleteSegments:
              input.action = GlobalChangesAction.DeleteSegments;
              break;
            case SelectionAction.FindReplace:
              input.action = GlobalChangesAction.FindReplace;
              input.fieldName = this.selectedField.cQuestionFieldName;
              input.fieldDescription = this.selectedField.cQuestionDescription;
              input.fieldId = this.selectedField.id;
              input.searchValue = this.findText;
              input.replaceValue = this.replaceText;
              break;
            case SelectionAction.EditSegments:
              input.action = GlobalChangesAction.EditSegments;
              input.fieldName = this.selectedField;
              input.searchValue = this.findText;
              input.replaceValue =
                this.replaceText == undefined ? "" : this.replaceText;
              break;
            default:
              return;
          }
          this.changeSavingState(saveAndclose, true);
          this._segmentSelectionProxy
            .saveGlobalChanges(input)
            .pipe(
              finalize(() => this.changeSavingState(saveAndclose, false))
            )
            .subscribe(result => {
              if (result.toUpperCase() === "UNDER CONSTRUCTION") {
                this.message.info(result);
              } else {
                this.isSaved = true;
                this.notify.success(result);
                if (saveAndclose)
                  this.activeModal.close({ isSave: this.isSaved });
                else if (this.selectedAction == SelectionAction.EditSegments && this.segmentFieldsForReload.includes(this.selectedField)) {
                  this.filterText = this.finalFilterText
                  this.getSegments();
                }
              }
            });
        }
      });
    }
  }
  saveSegmentGrid(saveAndclose: boolean): void {
    let saveBatchSegmentDto: SaveBatchSegmentDto = this.editablaGrid.getSaveBatchSegmentDto();
    if (saveBatchSegmentDto.nextStatus === this.currentStatus)
      saveBatchSegmentDto.nextStatus = 1000;
    this.changeSavingState(saveAndclose, true);
    if (saveBatchSegmentDto.modifiedSegments.length) {
      this._segmentSelectionProxy.saveBatchSegments(saveBatchSegmentDto)
        .pipe(
          finalize(() => this.changeSavingState(saveAndclose, false))
        )
        .subscribe(() => this.onSavingComplete(saveAndclose, true, saveBatchSegmentDto.nextStatus));
    }
    else {
      this.onSavingComplete(saveAndclose, false, 1000);
      this.changeSavingState(saveAndclose, false);
    }
  }
  onSavingComplete(saveAndclose: boolean, reloadGrid: boolean, nextStatus: number): void {
    this.isSaved = reloadGrid ? reloadGrid : this.isSaved;
    this.notify.info(this.l('SavedSuccessfully'));
    if (saveAndclose)
      this.activeModal.close({ isSave: this.isSaved });
    else if (reloadGrid) {
      if (nextStatus !== 1000)
        this.currentStatus = nextStatus;
      this.editablaGrid.getSegmentsForEdit(this.editablaGrid.filterText, this.campaignId);
    }
  }
  close(): void {
    this.activeModal.close({ isSave: this.isSaved });
  }

  changeSavingState(saveAndclose: boolean, isSaving: boolean): void {
    this.isFieldDisabled = isSaving;
    if (saveAndclose && this.showSaveAndClose)
      this.saving = isSaving;
    else
      this.savingFromSave = isSaving;
  }
  switchMode(): void {
    this.primengTableHelper.records = [];
    this.primengTableHelper.totalRecordsCount = 0;
    this.editablaGrid.primengTableHelper.records = [];
    this.editablaGrid.primengTableHelper.totalRecordsCount = 0;
    if (!this.showEditableGrid) this.paginator.first = 0;
    this.getSegments();
  }
}
