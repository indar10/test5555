import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { UtilsModule } from "@shared/utils/utils.module";
import { CountoModule } from "angular2-counto";
import { DropdownModule } from "primeng/dropdown";
import { MultiSelectModule } from "primeng/multiselect";
import { CheckboxModule } from "primeng/checkbox";
import { CalendarModule } from "primeng/calendar";
import { TableModule } from "primeng/table";
import {
    ModalModule,
    TabsModule,
    BsDropdownModule,
    PopoverModule,
} from "ngx-bootstrap";
import { NgbModule, NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { RadioButtonModule } from "primeng/radiobutton";
import { AppCommonModule } from "@app/shared/common/app-common.module";
import {
    BsDatepickerConfig,
    BsDaterangepickerConfig,
    BsLocaleService,
} from "ngx-bootstrap/datepicker";
import { NgxBootstrapDatePickerConfigService } from "assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service";
import { AutoCompleteModule } from "primeng/autocomplete";
import { SelectButtonModule } from "primeng/selectbutton";
import { TasksRoutingComponent } from "./tasks-routing.module";
import { TaskComponent } from "./tasks/task.component";
import { SetValidEmailFlagComponent } from "./tasks/set-valid-email-flag.component";
import { DatabaseBuildComponent } from "./shared/database-build-modal.component";
import { SharedModule } from "primeng/components/common/shared";
import { InputMaskModule } from "primeng/inputmask";
import { AOPFromPreviousBuildComponent } from "./tasks/aop-from-previous-build-modal.component";
import { LoadMailerUsageComponent } from "./tasks/load-mailer-usage-modal.component";
import { EnhanceAuditReportComponent } from "./tasks/enhance-audit-report-modal.component";
import { ExportListConversionComponent } from "./tasks/export-list-conversion-data-modal.component";
import { ApogeeCustomExportTaskComponent } from "./tasks/apogee-custom-export-task-modal.component";
import { ApogeeExportPointInTimeTaskComponent } from "./tasks/apogee-export-point-in-time-task-modal.component";
import { SearchPreviousOrderHistorybykeyComponent } from "./tasks/search-previous-order-history-bykey-task-modal.component";
import { CopyBuildComponent } from "./tasks/copy-build-task-modal.component";
import { OptoutHardboundComponent } from "./tasks/optout-hardbounce-task-modal.component";
import { ExportEmailHygieneDataComponent } from "./tasks/export-email-hygien-data-task-modal.component";
import { ImportEmailHygieneDataComponent } from "./tasks/import-email-hygiene-data-task-modal.component";
import { ActivateLinkTableBuildComponent } from "./tasks/activate-link-table-build-task-modal.component";
import { CloseNotificationAndDeleteBuildComponent } from "./tasks/close-notification-and-delete-build-task-modal.component";
import { BulkUpdateListActionComponent } from "./tasks/bulk-update-list-action-task-modal.component";
import { BatchProcessQueueComponent } from "./batch-process-queue/batch-process-queue.component";
import { PaginatorModule } from "primeng/paginator";
import { QueueLogsComponent } from "./batch-process-queue/queue-logs/queue-logs.component";
import { ModelPivotReportComponent } from "./tasks/model-pivot-report/model-pivot-report.component";

@NgModule({
    imports: [
        InputMaskModule,
        CalendarModule,
        AutoCompleteModule,
        SelectButtonModule,
        MultiSelectModule,
        DropdownModule,
        CheckboxModule,
        CommonModule,
        FormsModule,
        ModalModule,
        AppCommonModule,
        UtilsModule,
        CountoModule,
        TableModule,
        TasksRoutingComponent,
        RadioButtonModule,
        NgbModule,
        BsDropdownModule.forRoot(),
        TabsModule.forRoot(),
        PopoverModule.forRoot(),
        SharedModule,
        PaginatorModule,
    ],
    declarations: [
        TaskComponent,
        SetValidEmailFlagComponent,
        DatabaseBuildComponent,
        AOPFromPreviousBuildComponent,
        LoadMailerUsageComponent,
        EnhanceAuditReportComponent,
        ExportListConversionComponent,
        ApogeeCustomExportTaskComponent,
        ApogeeExportPointInTimeTaskComponent,
        SearchPreviousOrderHistorybykeyComponent,
        CopyBuildComponent,
        OptoutHardboundComponent,
        ExportEmailHygieneDataComponent,
        ImportEmailHygieneDataComponent,
        ActivateLinkTableBuildComponent,
        CloseNotificationAndDeleteBuildComponent,
        BulkUpdateListActionComponent,
        BatchProcessQueueComponent,
        QueueLogsComponent,
        ModelPivotReportComponent,
    ],
    providers: [
        NgbActiveModal,
        {
            provide: BsDatepickerConfig,
            useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig,
        },
        {
            provide: BsDaterangepickerConfig,
            useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig,
        },
        {
            provide: BsLocaleService,
            useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale,
        },
    ],
    entryComponents: [
        SetValidEmailFlagComponent,
        AOPFromPreviousBuildComponent,
        LoadMailerUsageComponent,
        EnhanceAuditReportComponent,
        ExportListConversionComponent,
        ApogeeCustomExportTaskComponent,
        ApogeeExportPointInTimeTaskComponent,
        SearchPreviousOrderHistorybykeyComponent,
        CopyBuildComponent,
        OptoutHardboundComponent,
        ExportEmailHygieneDataComponent,
        ImportEmailHygieneDataComponent,
        ActivateLinkTableBuildComponent,
        CloseNotificationAndDeleteBuildComponent,
        BulkUpdateListActionComponent,
        ModelPivotReportComponent,
    ],
})
export class TasksModule { }
