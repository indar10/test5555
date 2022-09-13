import { Component, OnInit, Injector, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { NgbActiveModal, NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { AppComponentBase } from "@shared/common/app-component-base";
import { UserServiceProxy, UserListDto, SegmentSelectionsServiceProxy, GetQueryBuilderDetails, MatchAppendsServiceProxy, GetMatchAppendForViewDto, IDMSConfigurationsServiceProxy } from "@shared/service-proxies/service-proxies";
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Table } from 'primeng/components/table/table';
import { finalize } from "rxjs/operators";
import { Paginator } from 'primeng/components/paginator/paginator';
import { NgForm } from '@angular/forms';
import { result } from 'lodash';
import { AppConsts } from '@shared/AppConsts';
import { PermissionTreeModalComponent } from '../../../../admin/shared/permission-tree-modal.component';
import { CampaignStatus } from '@app/main/campaigns/shared/campaign-status.enum';
import { ModalDirective } from 'ngx-bootstrap';
import { HttpClient } from '@angular/common/http';
import { ModalDefaults, ModalSize } from '@shared/costants/modal-contants';
import { UploadSuppressionComponent } from './upload-suppression/upload-suppression.component';
interface customSuppressionSelectedObjinterface {
  filePath: string,
  idmsMatchFieldName: string
};
@Component({
  selector: 'customSuppressionModal',
  templateUrl: './custom-suppression.component.html',
  styleUrls: ['./custom-suppression.component.css']
})
export class CustomSuppressionComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild("customSuppressionFilters", { static: true }) userFilters: NgForm;
  @ViewChild('userInput', { static: false }) userInput: any;
  @ViewChild('permissionFilterTreeModal', { static: true }) permissionFilterTreeModal: PermissionTreeModalComponent;
  @Output() itemSelected: EventEmitter<string> = new EventEmitter<string>();
  @Input() campaignId: number;
  @Input() segmentId: number;
  @Input() id: string;
  @Input() databaseId: number;
  @Input() buildId: number;
  @Input() mailerId: number;

  searchText: any;
  users: UserListDto[] = [];
  selectedUsers: UserListDto;
  isSave: boolean = false;
  userNameFilter = this.appSession.idmsUser.idmsUserName;
  isLoading: boolean = true;
  selectedMatchAppenedTasks: GetMatchAppendForViewDto[] = [];
  fcMatchAppenedTasks: GetMatchAppendForViewDto[] = [];
  statusType = CampaignStatus;
  active = false;
  customSuppressionChecked = false;
  customSuppressionSelectedObj: any = {}
  selectedSuppressionItem: any[] = []
  MatchAppendFinalOutputPath: string = "";
  saving = false;

  constructor(injector: Injector, private activeModal: NgbActiveModal, private _userServiceProxy: UserServiceProxy,
    private _matchAppendServiceProxy: MatchAppendsServiceProxy, private _httpClient: HttpClient,
    private _segmentSelectionProxy: SegmentSelectionsServiceProxy, private modalService: NgbModal, private _idmsConfig: IDMSConfigurationsServiceProxy) {
    super(injector);
  }

  ngOnInit() {
  }

  show(): void {
    this.active = true;
  }

  getMatchAppendTasks(event?: LazyLoadEvent) {
    this.isSave = false;
    if (this.selectedUsers != undefined)
      var userNameFilter = this.selectedUsers.userName;

    this._matchAppendServiceProxy.getAllMatchAppendTasks(
      this.searchText,
      userNameFilter ? userNameFilter : undefined,
      '',
      0,
      1000
    )
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.fcMatchAppenedTasks = result.items;
        this._idmsConfig.getConfigurationValue("MatchAppendFinalOutputPath", this.databaseId).subscribe(result => {
          this.MatchAppendFinalOutputPath =result.cValue;
          });

      });
  }


  close(): void {
    this.activeModal.close({ isSave: true, items: this.selectedSuppressionItem });
    this.active = false;
  }

  getUsers(event) {
    this._userServiceProxy
      .getUsers(event.query, undefined, undefined, false, undefined, 1000, 0)
      .subscribe((result) => (this.users = result.items
      ));
  }

  clearSuppressionFilters(): void {
    const isDirty = this.userFilters.dirty;
    this.userFilters.resetForm();
    this.userNameFilter = '';
    this.searchText = '';
    if (isDirty) this.getMatchAppendTasks();

  }

  singleChecked(event, record) {
    if (event) {
      this.selectedMatchAppenedTasks.push(record);
    }
    else {
      var deSelectedItem = this.selectedMatchAppenedTasks.find(item => item.id == record.id);
      var indexOfdeSelectedItem = this.selectedMatchAppenedTasks.indexOf(deSelectedItem);
      this.selectedMatchAppenedTasks.splice(indexOfdeSelectedItem, 1);
    }
  }


  save() {
    this.selectedMatchAppenedTasks.forEach(element => {
      var getUrl = AppConsts.remoteServiceBaseUrl + '/File/ValidateSuppressionFile?campaignId=' + this.campaignId + '&fileName=' + element.cClientFileName + '&cFieldName=' + element.idmsMatchFieldName;
      this._httpClient
        .get<any>(getUrl)
        .pipe(finalize(() => { this.saving = true; }))
        .subscribe(response => {
          if (response.success) {
            if (response != undefined || response != null)
              this.customSuppressionSelectedObj= {};
              this.customSuppressionSelectedObj.cQuestionFieldName = element.idmsMatchFieldName,
              this.customSuppressionSelectedObj.cQuestionDescription = "",
              this.customSuppressionSelectedObj.cJoinOperator = "AND",
              this.customSuppressionSelectedObj.iGroupNumber = 1,
              this.customSuppressionSelectedObj.cGrouping = "N",
              this.customSuppressionSelectedObj.cValues = "",
              this.customSuppressionSelectedObj.cValueMode = "F",
              this.customSuppressionSelectedObj.cDescriptions = "",
              this.customSuppressionSelectedObj.cValueOperator = "NOT IN",
              this.customSuppressionSelectedObj.cFileName = this.MatchAppendFinalOutputPath + "MatchAppendOutput_" + element.id + ".txt",
              this.customSuppressionSelectedObj.cSystemFileName = this.MatchAppendFinalOutputPath + "MatchAppendOutput_" + element.id + ".txt",
              this.customSuppressionSelectedObj.cCreatedBy = null,
              this.customSuppressionSelectedObj.cTableName = "",
              this.customSuppressionSelectedObj.isRuleUpdated = 0,
              this.customSuppressionSelectedObj.isDirectFileUpload = 1,
              this.customSuppressionSelectedObj.id = 0
              this.customSuppressionSelectedObj.segmentId = this.segmentId;
              this.selectedSuppressionItem.push(this.customSuppressionSelectedObj);
            if (this.selectedMatchAppenedTasks.length === this.selectedSuppressionItem.length) {
              this.activeModal.close({ isSave: true, items: this.selectedSuppressionItem })
            }
          }
          else {
            this.notify.success(this.l("Error"));
            this.activeModal.close({ isSave: true, items: this.selectedSuppressionItem })
          }
        })

    });

  }
  createOrEditMatchAppend(matchAppendId?: number, statusId?: number): void {
    const modalRef: NgbModalRef = this.modalService.open(UploadSuppressionComponent, {
      size: ModalSize.EXTRA_LARGE,
      windowClass: ModalDefaults.WindowClass,
      backdrop: ModalDefaults.Backdrop,
    });
    modalRef.componentInstance.matchAppendId = matchAppendId;
    modalRef.componentInstance.iStatus = statusId;
    modalRef.componentInstance.campaignId = this.campaignId;
    modalRef.componentInstance.databaseId = this.databaseId;
    modalRef.componentInstance.buildId = this.buildId;
    modalRef.componentInstance.mailerId = this.mailerId;
    modalRef.componentInstance.isEdit = 1;
    modalRef.result.then((params) => {
    });
  }
  copyMatchAppendTasks(matchAppendId: number) {
    this.isLoading = true;
    this.message.confirm('', this.l("CopyTaskMessage", matchAppendId.toString()), isConfirmed => {
      if (isConfirmed) {
        this._matchAppendServiceProxy.copyMatchAppendTask(matchAppendId).pipe(finalize(() => (this.isLoading = false))).subscribe(
          result => {
            this.notify.info(this.l("MatchAppendCopySuccessful"));
            this.getMatchAppendTasks();
          });
      }
      else {
        this.isLoading = false;
      }
    });
  }
}
