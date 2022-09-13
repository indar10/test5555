import { AppComponentBase } from "@shared/common/app-component-base";
import { Injector, Component, EventEmitter, Output, Input } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { HttpClient } from "@angular/common/http";
import { DatabasesServiceProxy, BuildsServiceProxy, TaskGeneralDto } from '@shared/service-proxies/service-proxies';
import { SelectItem } from 'primeng/api';
import { DatePipe } from "@angular/common";
import { ControlContainer, NgForm } from "@angular/forms";

@Component({
    selector: 'general-database-build',
    templateUrl: './database-build-modal.component.html',
    providers: [DatePipe],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})

export class DatabaseBuildComponent extends AppComponentBase {
    selectedDatabase: any;
    databaseList: SelectItem[] = [];
    selectedBuild: any;
    buildList: SelectItem[] = [];
    task!: TaskGeneralDto | undefined;
    databaseBuild: boolean = true;
    @Output() taskGeneral = new EventEmitter<any>();
    @Input() IsDatabaseIdDefined: boolean = false;
    @Input() taskID: number = 0;
    @Input() onlyDatabaseBuild: boolean = false;

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _databaseServiceProxy: DatabasesServiceProxy,
        private _buildServiceProxy: BuildsServiceProxy,
        private _httpClient: HttpClient,
    ) {
        super(injector);
    }

    ngOnInit() {
        this.task = new TaskGeneralDto();
        if (this.onlyDatabaseBuild)
            this.databaseBuild = false;
        this.FillDatabaseDropdown();
    }

    FillDatabaseDropdown() {
        let currentDatabaseId = this.appSession.idmsUser.currentCampaignDatabaseId;
        let userId = this.appSession.idmsUser.idmsUserID;
        this._databaseServiceProxy.getAllDatabasesForUser(userId, currentDatabaseId)
            .subscribe(result => {
                this.databaseList = result.databases;
                if (this.IsDatabaseIdDefined)
                    this.task.databaseID = 82;
                else
                    this.task.databaseID = result.defaultDatabase;
                this.setControlOptions(this.task.databaseID);
        });
    }

    setControlOptions(selectedDatabase: number) {
        this._buildServiceProxy.getBuildsForDatabase(selectedDatabase, this.taskID)
            .subscribe(result => {
                if (result.defaultSelection == 0) {
                    this.buildList = [];
                    this.task.buildID = null;
                }
                else {
                    this.buildList = result.buildDropDown;
                    this.task.buildID = result.defaultSelection;
                    if (this.task.databaseID && this.task.buildID) {
                        this.taskGeneral.emit({ ...this.task });
                    }
                }
                
            })
            
    }

    onDatabaseChange(event): void {
        this.task.databaseID = event.value;
        this.setControlOptions(event.value);        
    }

    onBuildChange(event): void {
        this.task.buildID = event.value;
        if (this.task.databaseID && this.task.buildID) {
            this.taskGeneral.emit({ ...this.task });
        }
    }
}