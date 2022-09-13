import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { EnhancedAuditReportDto, TaskGeneralDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import * as moment from "moment";

@Component({
    selector: 'EnhanceAuditReport',
    templateUrl: './enhance-audit-report-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class EnhanceAuditReportComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    scheduledDate: Date;
    currentDate: Date;
    task: EnhancedAuditReportDto | undefined;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _taskService: IDMSTasksServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.taskId);
    }

    show(taskId?: number): void {
        if (taskId) {
            this.active = true;
            this.task = new EnhancedAuditReportDto();
            this.task.taskGeneral = new TaskGeneralDto();
            this._taskService.getServerDate().subscribe(result => {
                this.currentDate = new Date(result.date)//.format("YYYY-MM-DD HH:mm"));
                this.scheduledDate = this.currentDate;
                this.currentDate.setHours(18);
                this.currentDate.setMinutes(0);
                this.task.scheduledTime = this.currentDate.toLocaleTimeString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit' });
            })
        }

    }
    save(): void {
        this.message.confirm(
            this.l(""),
            this.l("AreYouSure"),
            isConfirmed => {
                if (isConfirmed) {
                    this.task.scheduledDate = this.scheduledDate.toDateString();
                    this._taskService.enhancedAuditReport(this.task).subscribe(result => {
                        this.activeModal.close({ saving: this.saving });
                    })
                }
            });      
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }

    onChecked(event): void {
        if (event) {
            this.task.isListWiseReport = true;
        }
        else {
            this.task.isListWiseReport = false;
        }
    }

    storeTaskGeneral(data: TaskGeneralDto) {
        this.task.taskGeneral.databaseID = data.databaseID;
        this.task.taskGeneral.buildID = data.buildID;
    }
}