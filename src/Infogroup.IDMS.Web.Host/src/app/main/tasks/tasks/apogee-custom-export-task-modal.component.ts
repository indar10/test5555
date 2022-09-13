import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, ApogeeCustomExportTaskDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import * as moment from "moment";

@Component({
    selector: 'ApogeeCustomExportTask',
    templateUrl: './apogee-custom-export-task-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class ApogeeCustomExportTaskComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    scheduledDate: Date;
    currentDate: Date;
    task: ApogeeCustomExportTaskDto | undefined;

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
            this.task = new ApogeeCustomExportTaskDto();
            this.task.taskGeneral = new TaskGeneralDto();
            this._taskService.getServerDate().subscribe(result => {
                this.currentDate = new Date(result.date);//.format("YYYY-MM-DD HH:mm"));
                this.scheduledDate = this.currentDate;
                //this.currentDate.setMinutes(this.currentDate.getMinutes() + 1);
                this.task.scheduledTime = result.time;//this.currentDate.toLocaleTimeString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit' });
                if (this.task.scheduledTime.substring(0, 2) == "24")
                    this.task.scheduledTime = this.task.scheduledTime.replace(this.task.scheduledTime.substring(0, 2), "00");                
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
                    this._taskService.apogeeCustomExportTask(this.task).subscribe(result => {
                        this.activeModal.close({ saving: this.saving });
                    });
                }
            });
       
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }


    storeTaskGeneral(data: TaskGeneralDto) {
        this.task.taskGeneral.databaseID = data.databaseID;
        this.task.taskGeneral.buildID = data.buildID;
    }
}