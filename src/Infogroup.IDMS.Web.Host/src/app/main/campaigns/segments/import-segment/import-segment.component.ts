import { Component, Injector, OnInit, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { SegmentsServiceProxy, ImportSegmentDTO } from '@shared/service-proxies/service-proxies';
import { SaveSegment } from '../../shared/campaign-models';
import { finalize } from 'rxjs/operators';


@Component({
    selector: 'ImportSegmentModal',
    templateUrl: './import-segment.component.html'

})
export class ImportSegmentModalComponent extends AppComponentBase implements OnInit {

    active: boolean = false;
    saving: boolean = false;
    fromcampaignId: any;
    isCampaignIdChanged: boolean = true;

    @Input() campaignId: number;

    importSegment: ImportSegmentDTO = new ImportSegmentDTO();

    constructor(
        injector: Injector,
        public activeModal: NgbActiveModal,
        private _segmentsServiceProxy: SegmentsServiceProxy

    ) {
        super(injector);
    }
    ngOnInit() {
        this.active = true;
    }
     
    save(): void {
        if (this.isCampaignIdChanged) {
            this.message.warn(this.l("SearchAvailableSegments"));
        } else {
            this.saving = true;
            this.importSegment.iCopyFromCampaignID = this.fromcampaignId;
            this.importSegment.iCopyToCampaignID = this.campaignId;
            this._segmentsServiceProxy.importSegment(this.importSegment)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe((result: number) => {
                    let importSegment: SaveSegment = { isSave: true, segmentId: result }
                    this.activeModal.close(importSegment);
                });
        }
        
    }

    validateCampaignId(): void {        
        this._segmentsServiceProxy.validateCampaignIDForImportSegment(this.campaignId, this.fromcampaignId)
             .subscribe((result: ImportSegmentDTO) => {
                 this.importSegment.cSegmentDescription = result.cSegmentDescription;
                 this.importSegment.numberOfSegments = result.numberOfSegments;
                this.isCampaignIdChanged = false;
             });
    }     
               
    onCampaignIdChange(): void {
        this.isCampaignIdChanged = true;       
    }

    close(): void {
        this.active = false;
        let cancelSegment: SaveSegment = { isSave: false, segmentId: 0 }
        this.activeModal.close(cancelSegment);
    }
    
    

}

