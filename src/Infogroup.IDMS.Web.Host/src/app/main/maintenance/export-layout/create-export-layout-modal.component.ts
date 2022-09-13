import { Component, Injector, Input, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SelectItem } from 'primeng/api';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GetSegmentListForView, CampaignsServiceProxy, CreateOrEditSegmentDto, SegmentsServiceProxy, CreateOrEditBuildDto, ExportLayoutsServiceProxy, CreateOrEditExportLayoutDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';



@Component({
    selector: 'createExportLayoutModal',
    styleUrls: ['create-export-layout-modal.component.css'],
    templateUrl: './create-export-layout-modal.component.html'

})
export class CreateExportLayoutModalComponent extends AppComponentBase implements OnInit {

    active: boolean = true;
    cDescription: string = "";
    bTelemarketing: boolean = false;
    bKeyCode: boolean = false;
    groupValueList: SelectItem[] = [];
    outputCaseList: SelectItem[] = [];
    selectedGroup: any;
    selectedOutputCase: any;
    databaseId: string = "";
    isSave: boolean = false;
    

    // @ViewChild('paginator', { static: true }) paginator: Paginator;

    constructor(
        injector: Injector,
        private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
        public activeModal: NgbActiveModal

    ) {
        super(injector);
    }
    ngOnInit() {
        this._exportLayoutServiceProxy.getOutputCaseDropDownValues().subscribe(result => {
            this.outputCaseList = result;
            this.selectedOutputCase = "U";

        });
        this._exportLayoutServiceProxy.getGroupNames(parseInt(this.databaseId), 1).subscribe(result => {
            this.groupValueList = result;
            this.selectedGroup = this.groupValueList[0].value
        });
        this.bKeyCode = true;
       

    }

    AddExportLayoutRecord() {
        var record = new CreateOrEditExportLayoutDto();
        record.databaseId = parseInt(this.databaseId);
        record.cDescription = this.cDescription;
        record.groupID = this.selectedGroup;
        record.iHasKeyCode = this.bKeyCode;
        record.iHasPhone = this.bTelemarketing;
        record.cOutputCaseCode = this.selectedOutputCase;
        this._exportLayoutServiceProxy.addExportLayoutRecord(record).subscribe(result => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.isSave = true;
            this.close();
        });
        

    }

   

    close(): void {
        this.active = false;
        
        this.activeModal.close({ isSave: this.isSave });
    }

}

