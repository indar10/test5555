import { Component, Injector, Input, EventEmitter, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SegmentsServiceProxy, CopySegmentDto } from '@shared/service-proxies/service-proxies';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NewStatusInfo } from '../shared/campaign-models';
import { SelectItem } from 'primeng/api';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'copy-segments',
    templateUrl: './copy-segments.component.html'
})
export class CopySegmentComponent extends AppComponentBase {
    active = false;
    copying = false;
    segmentDescription: any;
    @Input() segmentId: any;
    @Input() campaignId: any;
    @Input() iDedupeOrderSpecified: any;
    @Input() maxPer: any;
    @Input() segment: any;
    segmentCopied: boolean = false;
    maxPerDrop: SelectItem[] = [];
    selectedMaxPer: any;
    selectedCopyMode: any;
    copiesOption: SelectItem[] = [
        { label: "1", value: 1 },
        { label: "2", value: 2 },
        { label: "3", value: 3 },
        { label: "4", value: 4 },
        { label: "5", value: 5 },
        { label: "10", value: 10 },
        { label: "15", value: 15 },
        { label: "20", value: 20 },
    ];
    selectedCopy: any;
    selectedSegmentFrom: any;
    selectedSegmentTo: any;
    selectedToLocation: any;
    @Output() statusUpdated: EventEmitter<NewStatusInfo> = new EventEmitter<NewStatusInfo>();
    maxDedupeId: number;
    keyCode1: string = '';
    keyCode2: string = '';    
    copySegmentDto: CopySegmentDto = new CopySegmentDto();
    irequiredQuantityValue: string;    
    allAvailableText: string = "All Available";

    constructor(
        injector: Injector,
        private _segmentServiceProxy: SegmentsServiceProxy,
        public activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit() {
        this.active = true;
        this._segmentServiceProxy.getMaximumIDedupeNumber(this.campaignId).subscribe(result => {
            this.copySegmentDto.iSegmentFrom = this.segment.iDedupeOrderSpecified;
            this.copySegmentDto.iSegmentTo = this.segment.iDedupeOrderSpecified;
            this.maxDedupeId = result;
        })
        this._segmentServiceProxy.getMaxPerGroups(this.campaignId, this.segmentId).subscribe(
            (result: any) => {
                for (let entry of result.segmentDropDown) {
                    this.maxPerDrop.push({ label: entry.label, value: entry.value });                    
                }
            })
        this.selectedCopyMode = 'from';
        this.copySegmentDto.iNumberOfCopies = 1;
    }
    copySegment() {
        this.copying = true;
        this.copySegmentDto.iSegmentId = this.segmentId;
        this.copySegmentDto.iCampaignId = this.campaignId;
        this.copySegmentDto.iCopyFromOrderID = this.campaignId;
        this.copySegmentDto.iCopyToOrderID = this.campaignId;
        this.copySegmentDto.cCopyMode = this.selectedCopyMode ? "from" : "all";
        this.copySegmentDto.iMaxDedupeId = this.maxDedupeId;
        this._segmentServiceProxy.copy(
            this.copySegmentDto
        ).pipe(finalize(() => { this.copying = false }))
            .subscribe(result => {
                this.copying = true;
                this.segmentCopied = true;
                this.active = false;
                this.activeModal.close({ isCopy: this.segmentCopied, segmentId: result });
                this.copying = false;
            });

    }

    close(): void {
        this.active = false;
        this.segmentCopied = false;
        this.copying = false;
        this.activeModal.close({ isCopy: this.segmentCopied });
    }

    onRequiredQuantityKeyDown(event) {
        if (event.key == '.')
            return false;
        else
            return true;

    }

}

