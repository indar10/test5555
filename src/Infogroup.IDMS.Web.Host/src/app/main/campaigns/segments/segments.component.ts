import {
  Component,
  Injector,
  ViewChild,
  Input,
  EventEmitter,
  Output
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
  SegmentsServiceProxy,
  GetSegmentListForView,
  GetCampaignsListForView
} from "@shared/service-proxies/service-proxies";
import { LazyLoadEvent } from "primeng/components/common/lazyloadevent";
import { Paginator } from "primeng/components/paginator/paginator";
import { Table } from "primeng/components/table/table";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { CreateOrEditSegmentModalComponent } from "./create-or-edit-segment-modal.component";
import { CampaignUiHelperService } from "../shared/campaign-ui-helper.service";
import { CampaignAction } from "../shared/campaign-action.enum";
import {
  NewStatusInfo,
  MoveSegment,
  CopySegment,
  CreateOrEditResult
} from "../shared/campaign-models";
import { SaveSegment } from "../shared/campaign-models";
import { MoveSegmentComponent } from "./move-segments.component";
import { SegmentDataPreviewComponent } from "./segments-datapreview.compnent";
import { ModalDefaults, ModalSize } from "@shared/costants/modal-contants";
import { CopySegmentComponent } from "./copy-segments.component";
import { CampaignStatus } from "../shared/campaign-status.enum";
import { finalize } from "rxjs/operators";

@Component({
  selector: "segments",
  templateUrl: "./segments.component.html"
})
export class SegmentsComponent extends AppComponentBase {
  filterText = "";
  canEdit: boolean = false;
  showOutputQuantity: boolean = false;
  canEditSelection: boolean = false;
  canDelete: boolean = false;
  canCopy: boolean = false;
  canDataPreview: boolean = false;
  isDisabled: boolean = false;
  canMove: boolean = false;
  @ViewChild("dataTable", { static: true }) dataTable: Table;
  @ViewChild("paginator", { static: true }) paginator: Paginator;
  @Input() orderId: number;
  @Input() splitType: number;
  @Input() campaignDescription: string;
  @Input() campaign: GetCampaignsListForView;
  @Output() openSegmentLevelSelection: EventEmitter<number> = new EventEmitter<
    number
  >();
  @Output() statusUpdated: EventEmitter<NewStatusInfo> = new EventEmitter<
    NewStatusInfo
  >();

  constructor(
    injector: Injector,
    private _segmentServiceProxy: SegmentsServiceProxy,
    private _campaignUiHelperService: CampaignUiHelperService,
    private modalService: NgbModal
  ) {
    super(injector);
  }

  getSegment(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._segmentServiceProxy
      .getAllSegmentList(
        this.filterText,
        this.orderId,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe(result => {
        this.showOutputQuantity = false;
        this.primengTableHelper.totalRecordsCount =
          result.pagedSegments.totalCount;
        this.primengTableHelper.records = result.pagedSegments.items;
        this.primengTableHelper.hideLoadingIndicator();
        this.canEditSelection = this._campaignUiHelperService.shouldActionBeEnabled(
          CampaignAction.SaveSelection,
          result.currentStatus
        );
        this.canEdit = this.permission.isGranted("Pages.Segments.Edit");
        this.canCopy =
          this.canEditSelection &&
          this.permission.isGranted("Pages.Segments.Copy");
        this.canDataPreview =
          result.currentStatus != CampaignStatus.CampaignCreated &&
          result.currentStatus != CampaignStatus.CampaignSubmitted &&
          result.currentStatus != CampaignStatus.CampaignRunning &&
          result.currentStatus != CampaignStatus.CampaignFailed &&
          this.permission.isGranted("Pages.Segments.DataPreview");
        this.canDelete =
          this.canEditSelection &&
          this.permission.isGranted("Pages.Segments.Delete");
        this.canMove =
          this.canEditSelection &&
          this.permission.isGranted("Pages.Segments.Move");
        this.isDisabled = !(
          this.canEdit ||
          this.canCopy ||
          this.canDelete ||
          this.canMove
        );
        if (result.currentStatus >= CampaignStatus.OutputCompleted && result.currentStatus != CampaignStatus.OutputFailed) {
          this.showOutputQuantity = true;
        }
        this.statusUpdated.emit({
          campaignId: this.orderId,
          status: result.currentStatus
        });

      });
  }
  openEditCampaign(segmentId: number) {
    this.openSegmentLevelSelection.emit(segmentId);
  }

  deleteSegment(segment: GetSegmentListForView) {
    this.message.confirm(this.l(""), isConfirmed => {
      if (isConfirmed) {
        this._segmentServiceProxy.delete(segment.id).subscribe(() => {
          this.notify.success(this.l("SuccessfullyDeleted"));
          this.getSegment();
          this.statusUpdated.emit({
            campaignId: this.orderId,
            status: 10
          });
        });
      }
    });
  }

  createSegment(segmentId: any): void {
    const modalRef = this.modalService.open(CreateOrEditSegmentModalComponent, {
      backdrop: ModalDefaults.Backdrop,
      size: ModalDefaults.Size
    });
    modalRef.componentInstance.segmentId = segmentId;
    modalRef.componentInstance.OrderId = this.orderId;

    modalRef.componentInstance.databaseId = this.campaign.databaseID;
    modalRef.componentInstance.buildId = this.campaign.buildID;
    modalRef.componentInstance.mailerId = this.campaign.mailerId;

    modalRef.componentInstance.splitType = this.splitType;
    modalRef.result.then((result: CreateOrEditResult) => {
      if (result.isSave) {
        if (result.isEdit) {
          if (result.newStatus > 0)
            this.statusUpdated.emit({ campaignId: this.orderId, status: result.newStatus });
          this.refreshSegmentById(result.id)
        }
        else this.getSegment();
      }
    });
  }

  refreshSegmentById(segmentId: number) {
    let segmentRow: GetSegmentListForView = this.primengTableHelper.records.find(row => row.id == segmentId);
    if (segmentRow) {
      this.primengTableHelper.showLoadingIndicator();
      this._segmentServiceProxy.getSegmentForView(segmentId)
        .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
        .subscribe(result => segmentRow.init(result));
    }
  }

  copySegment(record: any) {
    const modalRef: NgbModalRef = this.modalService.open(CopySegmentComponent, {
      backdrop: ModalDefaults.Backdrop
    });
    modalRef.componentInstance.campaignId = this.orderId;
    //modalRef.componentInstance.iDedupeOrderSpecified = record.iDedupeOrderSpecified;
    modalRef.componentInstance.segmentId = record.id;
    modalRef.componentInstance.segment = record;
    //modalRef.componentInstance.maxPer = record.cMaxPerGroup;
    modalRef.result.then((result: CopySegment) => {
      if (result.isCopy) {
        this.notify.success(this.l("SegmentSuccessfullyCopied"));
        this.getSegment();
        this.statusUpdated.emit({
          campaignId: this.orderId,
          status: 10
        });
      }
    });
  }
  openDataPreview(segmentId: number) {
    if (segmentId) {
      const modalRef = this.modalService.open(SegmentDataPreviewComponent, {
        size: ModalDefaults.Size,
        backdrop: ModalDefaults.Backdrop,
        windowClass: ModalDefaults.WindowClass
      });
      modalRef.componentInstance.segmentId = segmentId;
    }
  }
  moveSegment(record: any) {
    const modalRef: NgbModalRef = this.modalService.open(MoveSegmentComponent, {
      size: ModalSize.SMALL,
      backdrop: ModalDefaults.Backdrop
    });
    modalRef.componentInstance.campaignId = this.orderId;
    modalRef.componentInstance.segmentDescription = record.description;
    modalRef.componentInstance.iDedupeOrderSpecified =
      record.iDedupeOrderSpecified;
    modalRef.componentInstance.segmentId = record.id;

    modalRef.result.then((result: MoveSegment) => {
      if (result.isMove) {
        this.notify.success(this.l("SegmentSuccessfullyMoved"));
        this.getSegment();
        this.statusUpdated.emit({
          campaignId: this.orderId,
          status: 10
        });
      }
    });
  }
}
