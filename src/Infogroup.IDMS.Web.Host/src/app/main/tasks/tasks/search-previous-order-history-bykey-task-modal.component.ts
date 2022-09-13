import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, SearchPreviousOrderHistorybyKeyTaskDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import * as moment from "moment";

@Component({
    templateUrl: './search-previous-order-history-bykey-task-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class SearchPreviousOrderHistorybykeyComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    startDate: Date = new Date();
    endDate: Date = new Date();
    minDate: Date = new Date();
    task: SearchPreviousOrderHistorybyKeyTaskDto | undefined;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _taskService: IDMSTasksServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.minDate.setMonth(this.minDate.getMonth() - 13);
        this.show(this.taskId);
    }

    show(taskId?: number): void {
        if (taskId) {
            this.active = true;
            this.task = new SearchPreviousOrderHistorybyKeyTaskDto();
            this.task.taskGeneral = new TaskGeneralDto();
        }

    }
    save(): void {
        this.task.startDate = moment(this.startDate).startOf('day');
        this.task.endDate = moment(this.endDate).endOf('day');
        this._taskService.searchPreviousOrderHistorybyKeyTask(this.task).subscribe(result => {
            this.activeModal.close({ saving: this.saving });
        })
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }


    storeTaskGeneralFrom(data: TaskGeneralDto) {
        this.task.taskGeneral.databaseID = data.databaseID;
        this.task.taskGeneral.buildID = data.buildID;
    }

   
}