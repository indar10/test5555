import { Component, Injector, OnInit } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
  BuildsServiceProxy,
  DatabasesServiceProxy,
  DropdownOutputDto,
  ExportLayoutsServiceProxy,
  IDMSTasksServiceProxy,
  ModelPivotReportDto,
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";
import * as _ from "lodash";

@Component({
  selector: "app-model-pivot-report",
  templateUrl: "./model-pivot-report.component.html",
})
export class ModelPivotReportComponent extends AppComponentBase {
  isLoading: boolean = true;
  modelPivotReport: ModelPivotReportDto;
  taskId: number = undefined;
  modelDropdown: DropdownOutputDto[] = [];
  databaseList: DropdownOutputDto[] = [];
  buildList: DropdownOutputDto[] = [];

  constructor(
    injector: Injector,
    public activeModal: NgbActiveModal,
    private _taskService: IDMSTasksServiceProxy,
    private _buildServiceProxy: BuildsServiceProxy,
    private _databaseServiceProxy: DatabasesServiceProxy
  ) {
    super(injector);
    this.modelPivotReport = new ModelPivotReportDto();
  }

  ngOnInit() {
    this.loadDatabases();
  }

  loadDatabases() {
    this.isLoading = true;
    const currentDatabaseId =
      this.appSession.idmsUser.currentCampaignDatabaseId;
    const userId = this.appSession.idmsUser.idmsUserID;
    this._databaseServiceProxy
      .getAllDatabasesForUser(userId, currentDatabaseId)

      .subscribe(
        (result) => {
          this.databaseList = _.sortBy(result.databases, (dto) => dto.label);
          this.modelPivotReport.databaseID = result.defaultDatabase;
          if (this.modelPivotReport.databaseID) {
            this.loadBuilds();
          }
        },
        () => (this.isLoading = false)
      );
  }

  loadBuilds() {
    this._buildServiceProxy
      .getBuildsForDatabase(this.modelPivotReport.databaseID, this.taskId)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((result) => {
        this.buildList = result.buildDropDown;
        if (result.defaultSelection == 0) {
          this.buildList = [];
          this.modelDropdown = [];
          this.modelPivotReport.buildID = undefined;
          this.modelPivotReport.model1 = undefined;
          this.modelPivotReport.model2 = undefined;
        } else {
          this.buildList = _.sortBy(result.buildDropDown, (dto) => dto.label);
          this.modelPivotReport.buildID = result.defaultSelection;
          this.getChildTables();
        }
      });
  }

  getChildTables() {
    if (this.modelPivotReport.databaseID && this.modelPivotReport.buildID) {
      this.isLoading = true;
      this._buildServiceProxy
        .getChildAndExternalTablesByBuild(
          this.modelPivotReport.buildID,
          this.modelPivotReport.databaseID
        )
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe((result) => {
          this.modelDropdown = _.sortBy(result, (dto) => dto.label);
          this.modelPivotReport.model1 = "";
          this.modelPivotReport.model2 = "";
        });
    }
  }

  save() {
    this.isLoading = true;
    this._taskService
      .saveModelPivotReport(this.modelPivotReport)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((result) => {
        if (result) {
          this.notify.info(this.l("Saved Successfully"));
          this.close(true);
        } else {
          this.notify.info(this.l("CopyFailed"));
          this.close(false);
        }
      });
  }

  close(saved: boolean = false) {
    this.activeModal.close({ saved });
  }
}
