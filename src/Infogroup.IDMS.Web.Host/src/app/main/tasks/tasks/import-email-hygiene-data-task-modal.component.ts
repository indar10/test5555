import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, ImportEmailHygieneDataDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { DatePipe } from "@angular/common";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";

@Component({
    templateUrl: './import-email-hygiene-data-task-modal.component.html',
    providers: [DatePipe],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class ImportEmailHygieneDataComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    matchField: SelectItem[] = [];
    flagField: SelectItem[] = [];
    task: ImportEmailHygieneDataDto | undefined;

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
            this.task = new ImportEmailHygieneDataDto();
            this.task.taskGeneral = new TaskGeneralDto();            
        }

    }
    save(): void {
        this._taskService.importEmailHygieneData(this.task).subscribe(result => {
            if (result) {
                this.notify.info(this.l('ImportEmailHygieneSuccessed'));
                this.activeModal.close({ saving: this.saving });
            }
            else {
                this.notify.info(this.l('ImportEmailHygieneFailed'));
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
        this._taskService.getAllExportFlagFieldForBuild(this.task.taskGeneral.buildID)
            .subscribe(result => {
                this.matchField = result.exportFlagFieldDropdown;
                this.task.matchField = result.defaultSelection;
                this.flagField = result.exportFlagFieldDropdown;
                this.task.flagField = result.defaultSelection;
            });
        this._taskService.getImportEmailHygieneLoadPath(data.databaseID).subscribe(result => {
            this.task.filePath = result;
        })
    }

    onImportMatchFieldChange(event): void {
        this.task.matchField = event.value;
    }

    onImportFlagFieldChange(event): void {
        this.task.flagField = event.value;
    }

}