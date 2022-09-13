import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, ApogeeExportPointInTimeTaskDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";

@Component({
    selector: 'ApogeeExportPointInTimeTask',
    templateUrl: './apogee-export-point-in-time-task-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class ApogeeExportPointInTimeTaskComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    task: ApogeeExportPointInTimeTaskDto | undefined;

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
            this.task = new ApogeeExportPointInTimeTaskDto();
            this.task.taskGeneral = new TaskGeneralDto();
            this.task.taskGeneral.databaseID = 82;
            this._taskService.getFilePathOfApogeePIT().subscribe(result => {
                this.task.fileLocation = result;
            })
        }

    }
    save(): void {
        this._taskService.apogeeExportPointInTimeTask(this.task).subscribe(result => {
            this.activeModal.close({ saving: this.saving });
        })
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