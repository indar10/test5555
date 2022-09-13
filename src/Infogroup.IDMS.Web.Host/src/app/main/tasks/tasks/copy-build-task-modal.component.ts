import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { CopyBuildDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";

@Component({
    templateUrl: './copy-build-task-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class CopyBuildComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    task: CopyBuildDto | undefined;

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
            this.task = new CopyBuildDto();
        }

    }
    save(): void {
        this._taskService.copyBuild(this.task).subscribe(result => {
            if (result) {
                this.notify.info(this.l('CopiedBuild'));
                this.activeModal.close({ saving: this.saving });
            }
            else {
                this.notify.info(this.l('CopyFailed'));
                this.activeModal.close({ saving: this.saving });
            }
        })
    }

    close(): void {
        this.active = false;
        this.activeModal.close({ saving: this.saving });
    }
}