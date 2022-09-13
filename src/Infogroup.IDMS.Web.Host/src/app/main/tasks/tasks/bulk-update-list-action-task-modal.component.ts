import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, BulkUpdateListActionDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";
import { finalize } from "rxjs/operators";

@Component({
    templateUrl: './bulk-update-list-action-task-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class BulkUpdateListActionComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    newBuildList: SelectItem[] = [];
    task: BulkUpdateListActionDto | undefined;

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
            this.task = new BulkUpdateListActionDto();
            this.task.taskGeneral = new TaskGeneralDto();
        }

    }
    save(): void {
        this._taskService.bulkUpdateListAction(this.task)
        .pipe(finalize(() => { this.saving = false; }))
        .subscribe(result => {
            if (result) {
                this.notify.info(this.l('BulkUpdateListActionSuccessed'));
                this.activeModal.close({ saving: this.saving });
            }
            else {
                this.notify.info(this.l('BulkUpdateListActionFailed'));
                this.activeModal.close({ saving: this.saving });
            }
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