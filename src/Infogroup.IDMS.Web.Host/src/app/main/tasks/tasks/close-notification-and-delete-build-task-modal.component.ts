import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";

@Component({
    templateUrl: './close-notification-and-delete-build-task-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class CloseNotificationAndDeleteBuildComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    task: TaskGeneralDto | undefined;

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
            this.task = new TaskGeneralDto();
        }
    }

    save(): void {
        if (this.taskId == 14) {
            this._taskService.closeNotificationJob(this.task)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(result => {
                    if (result) {
                        this.notify.info(this.l('CloseNotificationJobSuccessed'));
                        this.activeModal.close({ saving: this.saving });
                    }
                    else {
                        this.notify.info(this.l('CloseNotificationJobFailed'));
                        this.activeModal.close({ saving: this.saving });
                    }
                })
        }
        else {
            this._taskService.buildDeleteTask(this.task)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(result => {
                    if (result) {
                        this.notify.info(this.l('DeleteBuildTaskSuccessed'));
                        this.activeModal.close({ saving: this.saving });
                    }
                    else {
                        this.notify.info(this.l('DeleteBuildTaskFailed'));
                        this.activeModal.close({ saving: this.saving });
                    }
                })
        }
        
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }


    storeTaskGeneral(data: TaskGeneralDto) {
        this.task.databaseID = data.databaseID;
        this.task.buildID = data.buildID;
    }


}