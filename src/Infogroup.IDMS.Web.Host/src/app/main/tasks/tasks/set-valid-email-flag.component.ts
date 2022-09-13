import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { SetValidEmailFlagDto, TaskGeneralDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { DatePipe } from "@angular/common";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import * as moment from "moment";

@Component({
    selector: 'setValidEmailFlag',
    templateUrl: './set-valid-email-flag.component.html',
    providers: [DatePipe],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }],
    styles: ['.Header{background-color: royalblue; width: 370px; padding: 5px ; color: black; border-radius: 3px;}']
})

export class SetValidEmailFlagComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    scheduledDate: Date;
    currentDate: Date;
    task: SetValidEmailFlagDto | undefined;
    
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

    show(taskId?: number): void  {
        if (taskId) {
            this.active = true;
            this.task = new SetValidEmailFlagDto();
            this.task.taskGeneralFrom = new TaskGeneralDto();
            this.task.taskGeneralTo = new TaskGeneralDto();
            this._taskService.getServerDate().subscribe(result => {
                this.currentDate = new Date(result.date);
                this.scheduledDate = this.currentDate;
                //this.currentDate.setMinutes(this.currentDate.getMinutes() + 1);
                this.task.scheduledTime = result.time //this.currentDate.toLocaleString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit' })
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
                this._taskService.setValidEmailFlag(this.task).subscribe(result => {
                    this.activeModal.close({ saving: this.saving });
                })
            }
        });
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }


    storeTaskGeneralFrom(data: TaskGeneralDto) {
        this.task.taskGeneralFrom.databaseID = data.databaseID;
        this.task.taskGeneralFrom.buildID = data.buildID;
    }

    storeTaskGeneralTo(data: TaskGeneralDto) {
        this.task.taskGeneralTo.databaseID = data.databaseID;
        this.task.taskGeneralTo.buildID = data.buildID;
    }
}