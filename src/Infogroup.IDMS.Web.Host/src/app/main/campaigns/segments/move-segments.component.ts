import { Component, Injector, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SegmentsServiceProxy, GetSegmentListForView, CreateOrEditSegmentDto, DropdownOutputDto } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CampaignUiHelperService } from '../shared/campaign-ui-helper.service';
import { CampaignAction } from '../shared/campaign-action.enum';
import { NewStatusInfo } from '../shared/campaign-models';
import { SaveSegment } from '../shared/campaign-models';
import { SelectItem } from 'primeng/api';
import { finalize } from 'rxjs/operators';


@Component({
    selector: 'move-segments',
    templateUrl: './move-segments.component.html'
})
export class MoveSegmentComponent extends AppComponentBase {
    active = false;
    moving = false;
    @Input() segmentDescription: any;
    @Input() segmentId: any;
    @Input() campaignId: any;
    @Input() iDedupeOrderSpecified: any;
    segmentMoved: boolean = false;
    
    segmentRangeDropdown: SelectItem[] = [];
    selectedSegmentFrom: any;
    selectedSegmentTo: any;
    selectedToLocation: any;
    @Output() statusUpdated: EventEmitter<NewStatusInfo> = new EventEmitter<NewStatusInfo>();

    constructor(
        injector: Injector,
        private _segmentServiceProxy: SegmentsServiceProxy,
        private _campaignUiHelperService: CampaignUiHelperService,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.active = true;
        this._segmentServiceProxy.getMaximumIDedupeNumber(this.campaignId).subscribe(result => {
            for (let i = 1; i <= result; i++) {
                this.segmentRangeDropdown.push({ label: i.toString(), value: i });
                this.selectedSegmentFrom = this.iDedupeOrderSpecified;
                this.selectedSegmentTo = this.iDedupeOrderSpecified;
                this.selectedToLocation = this.segmentRangeDropdown[0].value;
            }
        })
    }
    moveSegment() { 
        this.moving = true;
        this._segmentServiceProxy.moveSegment(this.segmentId, this.selectedSegmentFrom, this.selectedSegmentTo, this.selectedToLocation, this.campaignId).pipe(finalize(() =>{ this.moving = false })).subscribe(result => {
            this.moving = true;
            this.segmentMoved = true;
            this.active = false;
            
            this.activeModal.close({ isMove: this.segmentMoved });
            this.moving = false;
           
        });
       
    }

    close(): void {
        this.active = false;
        this.segmentMoved = false;
        this.moving = false;
        this.activeModal.close({ isMove: this.segmentMoved });
    }

}

