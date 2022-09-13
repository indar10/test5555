import { Component, Injector, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SegmentsServiceProxy, GetSegmentListForView, CreateOrEditSegmentDto, SegmentPrevOrdersesServiceProxy, GetSegmentPrevOrdersForViewDto, ActionType } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { CampaignUiHelperService } from '../shared/campaign-ui-helper.service';
import { CampaignAction } from '../shared/campaign-action.enum';
import { NewStatusInfo } from '../shared/campaign-models';

@Component({
    selector: 'previoussegments',
    styleUrls: ['previous-segments.component.css'],
    templateUrl: './previous-segments.component.html'
})
export class PreviousSegmentsComponent extends AppComponentBase {
    addRows: any[] = [];
    listRows: any[] = [];
    filterText = '';
    segments: any[] = [];
    canEdit: boolean = false;
    canEditSelection: boolean = false;
    canDelete: boolean = false;
    canCopy: boolean = false;
    isDisabled: boolean = false;
    segmentListByPrevOrderId: any[] = [];


    @ViewChild('segmentDataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() orderId: number;
    @Input() prevOrderId: number;
    @Input() campaignDescription: string;
    @Input() prevCampaignDto: GetSegmentPrevOrdersForViewDto = new GetSegmentPrevOrdersForViewDto();
    @Output() segmentInfo: EventEmitter<any> = new EventEmitter<any>();
    @Output() statusUpdated: EventEmitter<NewStatusInfo> = new EventEmitter<NewStatusInfo>();
    @Output() addSegment: EventEmitter<any> = new EventEmitter<any>();
    @Output() removeSegment: EventEmitter<any> = new EventEmitter<any>();


    constructor(
        injector: Injector,
        private _segmentServiceProxy: SegmentsServiceProxy,
        private _campaignUiHelperService: CampaignUiHelperService,
        private _segmentPrevOrdersesServiceProxy: SegmentPrevOrdersesServiceProxy
        
    ) {
        super(injector);
    }   

    onRowSelect(event) {
        this.prevCampaignDto.action = ActionType.Edit;
        this.addSegment.emit({ segment: event.data });
    }
    onRowUnselect(event) {
        this.prevCampaignDto.action = ActionType.Edit;
        this.removeSegment.emit({ segment: event.data });
    }

    getSegment(event?: LazyLoadEvent) {
        var test = this.prevOrderId;
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._segmentServiceProxy.getAllSegmentList(
            this.filterText,
            this.orderId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.pagedSegments.totalCount;
            this.primengTableHelper.records = result.pagedSegments.items;
            this.primengTableHelper.hideLoadingIndicator();
            this.canEditSelection = this._campaignUiHelperService.shouldActionBeEnabled(CampaignAction.SaveSelection, result.currentStatus);
            this.canEdit = this.permission.isGranted('Pages.Segments.Edit');
            this.canCopy = this.canEditSelection && this.permission.isGranted('Pages.Segments.Copy');
            this.canDelete = this.canEditSelection && this.permission.isGranted('Pages.Segments.Delete');
            this.isDisabled = !(this.canEdit || this.canCopy || this.canDelete);
            this.statusUpdated.emit({
                campaignId: this.orderId,
                status: result.currentStatus
            })
            this._segmentPrevOrdersesServiceProxy.getListOfSegmentIDs(this.prevOrderId).subscribe(resultList => {
                this.segmentListByPrevOrderId = resultList;
                for (let i = 0; i < this.segmentListByPrevOrderId.length; i++) {
                    let prevOrderSegment = result.pagedSegments.items.filter(x => x.id == this.segmentListByPrevOrderId[i]);
                    this.addRows = this.addRows.concat(prevOrderSegment[0]);
                    this.addSegment.emit({ segment: prevOrderSegment[0] });

                }
            })
        });
    }
    openEditCampaign(segId: number) {
        this.segmentInfo.emit({ campaignId: this.orderId, segmentId: segId, campaignDescription: this.campaignDescription });
    }

}

