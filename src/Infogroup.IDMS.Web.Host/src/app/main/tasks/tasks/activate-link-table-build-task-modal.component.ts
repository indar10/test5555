import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { ActivateLinkTableBuildDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { DatePipe } from "@angular/common";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";
import * as moment from "moment";

@Component({
    templateUrl: './activate-link-table-build-task-modal.component.html',
    providers: [DatePipe],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class ActivateLinkTableBuildComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    currentDate: Date = new Date();
    scheduledDate: Date;
    tableDescription: SelectItem[] = [];
    task: ActivateLinkTableBuildDto | undefined;

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
            this.task = new ActivateLinkTableBuildDto();
            this.tableDescription.push({ label: "Select Table", value: "" });
            this._taskService.getAllTableDescriptionForBuild()
                .subscribe(result => {
                    this.tableDescription = this.tableDescription.concat(result.exportFlagFieldDropdown);
                })
            this._taskService.getServerDate().subscribe(result => {
                this.currentDate = new Date(result.date);//.format("YYYY-MM-DD HH:mm"));
                this.scheduledDate = new Date(result.date);//.format("YYYY-MM-DD HH:mm"));
                this.scheduledDate.setDate(this.scheduledDate.getDate() + 1);
                this.currentDate.setHours(1);
                this.currentDate.setMinutes(0);
                this.task.scheduledTime = this.currentDate.toLocaleTimeString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit' });
            })
        }

    }
    save(): void {
        this.task.scheduledDate = this.scheduledDate.toDateString();
        this._taskService.activateLinkTableBuild(this.task).subscribe(result => {
            if (result) {
                this.notify.info(this.l('ActivateLinkTableBuildSuccessed'));
                this.activeModal.close({ saving: this.saving });
            }
            else {
                this.notify.info(this.l('ActivateLinkTableBuildFailed'));
                this.activeModal.close({ saving: this.saving });
            }
        })
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
    
    onTableNameChange(event): void {
        this.task.tableName = event.value;
    }
}