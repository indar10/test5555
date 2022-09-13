import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, BuildsServiceProxy, LoadMailerUsageDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";

@Component({
    selector: 'LoadMailerUsage',
    templateUrl: './load-mailer-usage-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class LoadMailerUsageComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    newBuildList: SelectItem[] = [];
    task: LoadMailerUsageDto | undefined;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _buildServiceProxy: BuildsServiceProxy,
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
            this.task = new LoadMailerUsageDto();
            this.task.taskGeneral = new TaskGeneralDto();
        }

    }
    save(): void {
        this._taskService.loadMailerUsage(this.task).subscribe(result => {
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
        this._buildServiceProxy.getBuildsForDatabase(this.task.taskGeneral.databaseID, this.taskId)
            .subscribe(result => {
                this.newBuildList = result.buildDropDown;
                this.task.newBuildID = result.defaultSelection;
            })
    }
}