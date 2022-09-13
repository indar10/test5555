import { Component, Injector, Input, OnInit, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
  DropdownOutputDto,
  SegmentSelectionsServiceProxy,
  AdvanceSelectionScreen,
  OccupationsServiceProxy,
  AdvanceSelectionFields,
  SegmentSelectionDto,
  AdvanceSelectionsInputDto
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";
import { Table } from "primeng/table";

@Component({
  selector: "occupation-selection",
  styleUrls: ["./occupation-selection-modal.component.css"],
  templateUrl: "./occupation-selection-modal.component.html"
})
export class OccupationSelectionModalComponent extends AppComponentBase implements OnInit {
  saving: boolean = false;
  @Input() segmentId: number;
  @Input() campaignId: number;
  @Input() databaseId: number;
  @Input() buildId: number;
  sortByCode: boolean = false;
  saveDisabled: boolean = false;
  isIndustryLoading: boolean = false;
  isSpecialityLoading: boolean = false;
  isOccupationLoading: boolean = false;

  industries: DropdownOutputDto[] = [];
  occupations: DropdownOutputDto[] = [];
  specialities: DropdownOutputDto[] = [];

  selectedIndustries: DropdownOutputDto[] = [];
  selectedOccupations: DropdownOutputDto[] = [];
  selectedSpecialities: DropdownOutputDto[] = [];

  showOccupationFilters: boolean = false;
  showIndustryFilters: boolean = false;
  showSpecialityFilters: boolean = false;

  industryOperator: string = 'IN';
  occupationOperator: string = 'IN';
  specialityOperator: string = 'IN';

  configuredfields: AdvanceSelectionFields;

  @ViewChild("industryTable", { static: true }) industryTable: Table;
  @ViewChild("occupationTable", { static: true }) occupationTable: Table;
  @ViewChild("specialityTable", { static: true }) specialityTable: Table;

  operatorTypes = [
    { label: 'Include', value: 'IN' },
    { label: 'Exclude', value: 'NOT IN' }
  ];
  constructor(
    injector: Injector,
    private _occupationsServiceProxy: OccupationsServiceProxy,
    private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
    private activeModal: NgbActiveModal
  ) {
    super(injector);
  }
  ngOnInit(): void {
    const screenType = AdvanceSelectionScreen;
    setTimeout(() => { this.isIndustryLoading = true; }, 0);
    this._occupationsServiceProxy
      .getInitialData(this.databaseId, this.buildId, screenType.Occupation)
      .pipe(finalize(() => setTimeout(() => { this.isIndustryLoading = false; }, 0)))
      .subscribe(result => {
        this.configuredfields = result.configuredFields;
        this.industries = result.industries;
      });

  }
  getOccupation(industry: string): void {
    if (industry) {
      setTimeout(() => { this.isOccupationLoading = true; }, 0);
      this._occupationsServiceProxy
        .getAllOccupationByIndustry(industry
        )
        .pipe(finalize(() => setTimeout(() => { this.isOccupationLoading = false; }, 0)))
        .subscribe(result => {
          this.occupations = result;
        });
    }
  }

  getSpeciality(industry: string, occupation: string): void {
    if (industry && occupation) {
      setTimeout(() => { this.isSpecialityLoading = true; }, 0);
      this._occupationsServiceProxy
        .getAllSpecialtyTitleByIndustryOccupation(industry, occupation)
        .pipe(finalize(() => setTimeout(() => { this.isSpecialityLoading = false; }, 0)))
        .subscribe(result => {
          this.specialities = result;
        });
    }
  }

  onIndustrySelectionChange(): void {
    this.resetOccupationGrid();
    this.resetSpecialityGrid();
    if (this.selectedIndustries.length === 1)
      this.getOccupation(this.selectedIndustries[0].value);
  }

  onOccupationSelectionChange(): void {
    this.resetSpecialityGrid();
    if (this.selectedIndustries.length === 1 && this.selectedOccupations.length === 1)
      this.getSpeciality(this.selectedIndustries[0].value, this.selectedOccupations[0].value);

  }
  resetOccupationGrid(): void {
    this.occupationTable.reset();
    this.showOccupationFilters = false;
    this.occupations = [];
    this.selectedOccupations = [];
  }
  resetSpecialityGrid(): void {
    this.specialityTable.reset();
    this.showSpecialityFilters = false;
    this.specialities = [];
    this.selectedSpecialities = [];
  }
  save(): void {
    const occupationFields: SegmentSelectionDto[] = [];
    if (this.selectedIndustries.length > 0) {
      if (this.configuredfields.industrySelection) {
        occupationFields.push(SegmentSelectionDto.fromJS({
          segmentId: this.segmentId,
          cQuestionFieldName: this.configuredfields.industrySelection.cQuestionFieldName,
          cQuestionDescription: '',
          cJoinOperator: 'AND',
          iGroupNumber: 0,
          iGroupOrder: 1,
          cGrouping: 'Y',
          cValues: this.selectedIndustries.map(industry => industry.value).toString().toUpperCase(),
          cValueMode: this.configuredfields.industrySelection.cValueMode,
          cDescriptions: this.selectedIndustries.map(industry => industry.label).toString().toUpperCase(),
          cValueOperator: this.industryOperator,
          cFileName: "",
          cSystemFileName: "",
          cCreatedBy: "",
          dCreatedDate: null,
          cModifiedBy: "",
          dModifiedDate: null,
          cTableName: this.configuredfields.industrySelection.cTableName,
          orderID: this.campaignId,
          id: 0
        }));
      }
      else {
        this.message.error(this.l('IndustryNotConfigured'), '');
        return;
      }
    }
    if (this.selectedOccupations.length > 0) {
      if (this.configuredfields.occupationSelection) {
        occupationFields.push(SegmentSelectionDto.fromJS({
          segmentId: this.segmentId,
          cQuestionFieldName: this.configuredfields.occupationSelection.cQuestionFieldName,
          cQuestionDescription: '',
          cJoinOperator: 'AND',
          iGroupNumber: 0,
          iGroupOrder: 1,
          cGrouping: 'Y',
          cValues: this.selectedOccupations.map(occupation => occupation.value).toString().toUpperCase(),
          cValueMode: this.configuredfields.occupationSelection.cValueMode,
          cDescriptions: this.selectedOccupations.map(occupation => occupation.label).toString().toUpperCase(),
          cValueOperator: this.occupationOperator,
          cFileName: "",
          cSystemFileName: "",
          cCreatedBy: "",
          dCreatedDate: null,
          cModifiedBy: "",
          dModifiedDate: null,
          cTableName: this.configuredfields.occupationSelection.cTableName,
          orderID: this.campaignId,
          id: 0
        }));
      }
      else {
        this.message.error(this.l('OccupationNotConfigured'), '');
        return;
      }
    }
    if (this.selectedSpecialities.length > 0) {
      if (this.configuredfields.specialtySelection) {
        occupationFields.push(SegmentSelectionDto.fromJS({
          segmentId: this.segmentId,
          cQuestionFieldName: this.configuredfields.specialtySelection.cQuestionFieldName,
          cQuestionDescription: '',
          cJoinOperator: 'AND',
          iGroupNumber: 0,
          iGroupOrder: 1,
          cGrouping: 'Y',
          cValues: this.selectedSpecialities.map(speciality => speciality.value).toString().toUpperCase(),
          cValueMode: this.configuredfields.specialtySelection.cValueMode,
          cDescriptions: this.selectedSpecialities.map(speciality => speciality.label).toString().toUpperCase(),
          cValueOperator: this.specialityOperator,
          cFileName: "",
          cSystemFileName: "",
          cCreatedBy: "",
          dCreatedDate: null,
          cModifiedBy: "",
          dModifiedDate: null,
          cTableName: this.configuredfields.specialtySelection.cTableName,
          orderID: this.campaignId,
          id: 0
        }));
      }
      else {
        this.message.error(this.l('SpecialityNotConfigured'), '');
        return;
      }
    }
    const input = new AdvanceSelectionsInputDto();
    input.segmentID = this.segmentId;
    input.sicFields = occupationFields;
    input.primarySICField = null;
    this.saving = true;
    this._segmentSelectionProxy
      .saveAdvanceSelection(this.campaignId, input)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe(() => {
        this.notify.info(this.l("SavedSuccessfully"));
        this.activeModal.close({ isSave: true });
      });
  }
  close(): void {
    this.activeModal.close({ isSave: false });
  }

}