import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, AOPFromPreviousBuildDto, BuildsServiceProxy } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { DatePipe } from "@angular/common";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";

@Component({
    selector: 'AOPFrompreviousbuild',
    templateUrl: './aop-from-previous-build-modal.component.html',
    providers: [DatePipe],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class AOPFromPreviousBuildComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    newBuildList: SelectItem[] = [];
    task: AOPFromPreviousBuildDto | undefined;

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
            this.task = new AOPFromPreviousBuildDto();
            this.task.taskGeneral = new TaskGeneralDto();
        }

    }
    save(): void {
        this._taskService.aOPFrompreviousbuild(this.task).subscribe(result => {
            if (!result) {
                this.task.isRegister = true;
                this.message.confirm(
                    this.l(""),
                    this.l("ConfirmAOP", this.task.listId),
                    isConfirmed => {
                        if (isConfirmed) {
                            this.task.userAgree = true;
                            this._taskService.aOPFrompreviousbuild(this.task).subscribe(result => {
                                this.activeModal.close({ saving: this.saving });
                            })
                        }
                        else {
                            this.task.userAgree = false;
                        }
                    });
            }
            else {
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
        this._buildServiceProxy.getBuildsForDatabase(this.task.taskGeneral.databaseID, this.taskId)
            .subscribe(result => {
                this.newBuildList = result.buildDropDown;
                this.task.newBuildID = result.defaultSelection;
            })
    }
}