import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
    Input,
} from "@angular/core";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { finalize } from "rxjs/operators";
import { NgbModalRef, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ModalDefaults, ModalSize } from "@shared/costants/modal-contants";
import { SetValidEmailFlagComponent } from "./set-valid-email-flag.component";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { AOPFromPreviousBuildComponent } from "./aop-from-previous-build-modal.component";
import { LoadMailerUsageComponent } from "./load-mailer-usage-modal.component";
import { EnhanceAuditReportComponent } from "./enhance-audit-report-modal.component";
import { ExportListConversionComponent } from "./export-list-conversion-data-modal.component";
import { ApogeeCustomExportTaskComponent } from "./apogee-custom-export-task-modal.component";
import { ApogeeExportPointInTimeTaskComponent } from "./apogee-export-point-in-time-task-modal.component";
import { SearchPreviousOrderHistorybykeyComponent } from "./search-previous-order-history-bykey-task-modal.component";
import { CopyBuildComponent } from "./copy-build-task-modal.component";
import { OptoutHardboundComponent } from "./optout-hardbounce-task-modal.component";
import { ExportEmailHygieneDataComponent } from "./export-email-hygien-data-task-modal.component";
import { ImportEmailHygieneDataComponent } from "./import-email-hygiene-data-task-modal.component";
import { ActivateLinkTableBuildComponent } from "./activate-link-table-build-task-modal.component";
import { CloseNotificationAndDeleteBuildComponent } from "./close-notification-and-delete-build-task-modal.component";
import { BulkUpdateListActionComponent } from "./bulk-update-list-action-task-modal.component";
import { ModelPivotReportComponent } from "./model-pivot-report/model-pivot-report.component";

@Component({
    templateUrl: "./task.component.html",
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class TaskComponent extends AppComponentBase {
    filterText: string = "";
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    @Input() outsideClick: false;
    TaskQueueGridShown = false;
    constructor(
        injector: Injector,
        private modalService: NgbModal,
        private _taskAppService: IDMSTasksServiceProxy
    ) {
        super(injector);
    }
    ngOnInit() {
        this.getTasks();
    }

    getTasks() {
        this.primengTableHelper.showLoadingIndicator();
        this._taskAppService
            .getAllTask(
                this.filterText.trim(),
                this.primengTableHelper.getSorting(this.dataTable)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe((result) => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
            });
    }

    openTaskModal(taskId?: number): void {
        let modalRef: NgbModalRef = null;
        switch (taskId) {
            case 1:
                modalRef = this.modalService.open(SetValidEmailFlagComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 2:
                modalRef = this.modalService.open(AOPFromPreviousBuildComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 3:
                modalRef = this.modalService.open(LoadMailerUsageComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 4:
                modalRef = this.modalService.open(EnhanceAuditReportComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 5:
                modalRef = this.modalService.open(ExportListConversionComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 6:
                modalRef = this.modalService.open(ApogeeCustomExportTaskComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 7:
                modalRef = this.modalService.open(
                    ApogeeExportPointInTimeTaskComponent,
                    {
                        size: ModalDefaults.Size,
                        backdrop: ModalDefaults.Backdrop,
                        windowClass: ModalDefaults.WindowClass,
                    }
                );
                modalRef.componentInstance.taskId = taskId;
                break;
            case 8:
                modalRef = this.modalService.open(
                    SearchPreviousOrderHistorybykeyComponent,
                    {
                        size: ModalDefaults.Size,
                        backdrop: ModalDefaults.Backdrop,
                        windowClass: ModalDefaults.WindowClass,
                    }
                );
                modalRef.componentInstance.taskId = taskId;
                break;
            case 9:
                modalRef = this.modalService.open(CopyBuildComponent, {
                    size: ModalSize.SMALL,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 10:
                modalRef = this.modalService.open(OptoutHardboundComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 11:
                modalRef = this.modalService.open(ExportEmailHygieneDataComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 12:
                modalRef = this.modalService.open(ImportEmailHygieneDataComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 13:
                modalRef = this.modalService.open(ActivateLinkTableBuildComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 14:
                modalRef = this.modalService.open(
                    CloseNotificationAndDeleteBuildComponent,
                    {
                        size: ModalSize.DEFAULT,
                        backdrop: ModalDefaults.Backdrop,
                        windowClass: ModalDefaults.WindowClass,
                    }
                );
                modalRef.componentInstance.taskId = taskId;
                break;
            case 15:
                modalRef = this.modalService.open(
                    CloseNotificationAndDeleteBuildComponent,
                    {
                        size: ModalSize.DEFAULT,
                        backdrop: ModalDefaults.Backdrop,
                        windowClass: ModalDefaults.WindowClass,
                    }
                );
                modalRef.componentInstance.taskId = taskId;
                break;
            case 16:
                break;
            case 17:
                modalRef = this.modalService.open(BulkUpdateListActionComponent, {
                    size: ModalDefaults.Size,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            case 18:
                modalRef = this.modalService.open(ModelPivotReportComponent, {
                    size: ModalSize.DEFAULT,
                    backdrop: ModalDefaults.Backdrop,
                    windowClass: ModalDefaults.WindowClass,
                });
                modalRef.componentInstance.taskId = taskId;
                break;
            default:
                break;
        }
    }

    onTaskInvoked(value: boolean) {
        this.TaskQueueGridShown = value;
        this.getTasks();
    }
}
