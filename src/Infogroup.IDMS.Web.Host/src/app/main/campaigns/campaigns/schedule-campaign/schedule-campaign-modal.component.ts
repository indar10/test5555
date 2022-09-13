import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component, Input } from "@angular/core";
import { TaskGeneralDto, ApogeeCustomExportTaskDto, CampaignsServiceProxy } from "@shared/service-proxies/service-proxies";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ControlContainer, NgForm } from "@angular/forms";
import { IDMSTasksServiceProxy } from "@shared/service-proxies/service-proxies";
import * as moment from "moment";
import { finalize } from "rxjs/operators";

@Component({
    selector: 'schedule-campaign',
    templateUrl: './schedule-campaign-modal.component.html',
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class ScheduleCampaignComponent extends AppComponentBase {
    active: boolean = false;
    taskId: number = null;
    saving = false;
    currentDate: Date;
    scheduleDate: Date;
    scheduleTime: any;
    forStatusString: string = '';
    @Input() campaign: any;
    task: ApogeeCustomExportTaskDto | undefined;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _taskService: IDMSTasksServiceProxy,
        private _campaignService: CampaignsServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.show();
    }

    show(): void {

        this.active = true;

        this._campaignService.getServerDate().subscribe(result => {
            this.currentDate = new Date(result);
            this.scheduleDate = this.currentDate;
            this.forStatusString = this.campaign.status == 10 ? "20: Count Submitted" : "70: Output Submitted";
            this.currentDate.setMinutes(this.currentDate.getMinutes() + 1);
            this.scheduleTime = "18:00";
            if (this.scheduleTime.substring(0, 2) == "24")
                this.scheduleTime = this.scheduleTime.replace(this.scheduleTime.substring(0, 2), "00");
        })


    }
    save(scheduleCampaignForm: NgForm): void {
        let newStatus = 0;
        if (this.campaign.status == 10)
            newStatus = 20;
        if (this.campaign.status == 40)
            newStatus = 70;
        this.saving = true;
        var scheduleDate = this.scheduleDate.toDateString();
        this._campaignService.scheduleCampaign(this.campaign.campaignId, newStatus, scheduleDate, this.scheduleTime.toString(), this.campaign.buildID, this.campaign.status)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(result => {
                this.notify.info(this.l('ScheduledSuccesfully'));
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