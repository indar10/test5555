import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component } from "@angular/core";
import { TaskGeneralDto, BuildsServiceProxy, ExportListConversionDataDto } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import { SelectItem } from "primeng/api";

@Component({
    selector: 'ExportListConversion',
    templateUrl: './export-list-conversion-data-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class ExportListConversionComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    outputTypeList: SelectItem[] = [];
    newBuildList: SelectItem[] = [];
    task: ExportListConversionDataDto | undefined;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _taskService: IDMSTasksServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show(this.taskId);
        this.outputTypeList = [
            { label: "Comma-Delimited", value: "CD" },
            { label: "Pipe Delimited", value: "PD" },
            { label: "Tab-Delimited", value: "TD" },
            { label: "Microsoft Excel", value: "EE" }
        ];
    }

    show(taskId?: number): void {
        if (taskId) {
            this.active = true;
            this.task = new ExportListConversionDataDto();
            this.task.taskGeneral = new TaskGeneralDto();
            this.task.outputQuantity = 0;
            this.task.tableType = 1;
            this.task.outputType = "CD";
        }

    }
    save(): void {
        this._taskService.exportListConversionData(this.task).subscribe(result => {
            if (result != 0) {
                this.notify.success(this.l("SuccessConversion"));
                this.activeModal.close({ saving: this.saving });
            }
            else {
                this.notify.success(this.l("SuccessConversion"));
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

    onOutputTypeChange(event): void {
        this.task.outputType = event.value;
    }
}