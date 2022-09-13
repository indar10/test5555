import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, ExportEmailHygieneDataDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { DatePipe } from "@angular/common";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";

@Component({
    templateUrl: './export-email-hygien-data-task-modal.component.html',
    providers: [DatePipe],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class ExportEmailHygieneDataComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    exportFlagField: SelectItem[] = [];
    exportField: SelectItem[] = [];
    task: ExportEmailHygieneDataDto | undefined;

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
            this.task = new ExportEmailHygieneDataDto();
            this.task.taskGeneral = new TaskGeneralDto();
            this.task.flagValue = "'M','Y'";
        }

    }
    save(): void {
        this._taskService.exportEmailHygieneData(this.task).subscribe(result => {
            if (result) {
                this.notify.info(this.l('ExportEmailHygieneSuccessed'));
                this.activeModal.close({ saving: this.saving });
            }
            else {
                this.notify.info(this.l('ExportEmailHygieneFailed'));
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
                this.exportFlagField = result.exportFlagFieldDropdown;
                this.task.exportFlagField = result.defaultSelection;
                this.exportField = result.exportFlagFieldDropdown;
                this.task.exportField = result.defaultSelection;
            })
    }

    onExportFlagFieldChange(event): void {
        this.task.exportFlagField = event.value;
    }

    onExportFieldChange(event): void {
        this.task.exportField = event.value;
    }

}