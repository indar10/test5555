import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, OptoutHardbounceTaskDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";

@Component({
    templateUrl: './optout-hardbounce-task-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class OptoutHardboundComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    FileTypeDescription: string = null;
    saving = false;
    fileTypeList: SelectItem[] = [];
    newBuildList: SelectItem[] = [];
    task: OptoutHardbounceTaskDto | undefined;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _taskService: IDMSTasksServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.taskId);
        this.fileTypeList = [
            { label: "Select File Type", value: "" },
            { label: "Optout", value: "O" },
            { label: "Hardbounce", value: "H" }
        ];
    }

    show(taskId?: number): void {
        if (taskId) {
            this.active = true;
            this.task = new OptoutHardbounceTaskDto();
            this.task.taskGeneral = new TaskGeneralDto();
            this._taskService.getFilePathOfOptoutHardbounce().subscribe(result => {
                this.task.fileLocation = result;
                if (this.task.fileLocation.length == 0) {
                    this.message.error(this.l('PathNotConfigured'));
                }
            })
        }

    }
    save(): void {        
        this._taskService.optoutHardbounceTask(this.task).subscribe(result => {
            if (result) {
                this.notify.info(this.l('OptoutHardboundComplete'));
                this.activeModal.close({ saving: this.saving });
            }
            else {
                this.notify.info(this.l('OptoutHardboundFailed'));
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

    onFileTypeChange(event): void {
        this.task.fileType = event.value;
        if (this.task.fileType == 'H')
            this.FileTypeDescription = this.l("FileFormatH");
        else if (this.task.fileType == 'O')
            this.FileTypeDescription = this.l("FileFormatO");
        else
            this.FileTypeDescription = this.l("");
    }
}