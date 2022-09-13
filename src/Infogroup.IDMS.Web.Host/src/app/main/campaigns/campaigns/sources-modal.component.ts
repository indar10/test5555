import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SegmentListsServiceProxy, SourceDto, ActionType, SaveSourcesInputDto, GetExistingSourceDataForView, GetAllApprovedSourcesInput } from '@shared/service-proxies/service-proxies';
import { Table } from 'primeng/table';
import { finalize } from 'rxjs/operators';
import { CampaignUiHelperService } from '../shared/campaign-ui-helper.service';
import { CampaignAction } from '../shared/campaign-action.enum';

@Component({
    selector: 'sourcesModal',
    styleUrls: ['sources-modal.component.css'],
    templateUrl: './sources-modal.component.html'
})
export class SourcesModalComponent extends AppComponentBase implements OnInit {
    saving: boolean = false;
    @Input() segmentID: number;
    campaignDescription: string = '';
    segmentDescription: string = '';
    filterText: string = '';
    campaignLevel: boolean = false;
    pageTitle: string = '';
    saveDisabled: boolean = false;
    isSelectedListLoading: boolean = false;
    actionType = ActionType;
    approvedSources: SourceDto[] = [];
    addedSources: SourceDto[] = [];
    selectedSources: SourceDto[] = [];
    selectedSourcesLength: number = 0;
    deletedSources: SourceDto[] = [];
    selectedSourceCount: number = 0;
    approvedSourceCount: number = 0;

    @ViewChild('approvedTable', { static: true }) approvedTable: Table;
    @ViewChild('selectedTable', { static: true }) selectedTable: Table;
    constructor(
        injector: Injector,
        private _segmentListsServiceProxy: SegmentListsServiceProxy,
        private _campaignUiHelperService: CampaignUiHelperService,
        private activeModal: NgbActiveModal
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.pageTitle = this.l("SourceTitle");
        this.showSelectedListLoadingIndicator();
        this._segmentListsServiceProxy.getExistingSourceData(
            this.segmentID
        ).pipe(finalize(() => this.hideSelectedListLoadingIndicator())).subscribe((result: GetExistingSourceDataForView) => {
            this.selectedSources = result.selectedSources;
            this.selectedSourcesLength = this.selectedSources.length;
            this.campaignLevel = result.campaignLevel;
            this.pageTitle = this.campaignLevel ? this.l("CampaignSource") : `${this.l("Segment")} ${result.iDedupeOrderSpecified} ${this.l("SourceTitle")}`;
            this.saveDisabled = !(this._campaignUiHelperService.shouldActionBeEnabled(CampaignAction.SaveSelection, result.currentStatus)
                && this.permission.isGranted('Pages.SegmentLists.Edit'));
            this.updateSourceCount();
        });
    }

    getSources(): void {
        let unsavedIDs = this.selectedSources.filter(source => source.action != ActionType.Delete).map(a => a.listID);
        this.primengTableHelper.showLoadingIndicator();
        const input = GetAllApprovedSourcesInput.fromJS(
            {
                filter: this.filterText.trim(),
                segmentID: this.segmentID,
                unsavedListIDs: unsavedIDs
            });
        this._segmentListsServiceProxy.fetchApprovedSources(input)
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe((result: SourceDto[]) => {
            this.approvedSources = result;
            this.updateSourceCount();
        });
    }

    addSelections(): void {
        let existingSource: SourceDto;
        this.addedSources.forEach(
            (source: SourceDto) => {
                existingSource = this.selectedSources.find(a => a.listID == source.listID && a.action == ActionType.Delete);
                if (existingSource)
                    existingSource.action = ActionType.None;
                else
                    this.selectedSources.push(SourceDto.fromJS(source));
            });
        this.filterText = '';
        this.selectedSourcesLength = this.selectedSources.filter(source => source.action != ActionType.Delete).length;
        this.approvedSources = [];
        this.addedSources = [];
        this.deletedSources = [];
        this.updateSourceCount();
    }
    deleteSelections(): void {
        let existingSource: SourceDto;
        this.deletedSources.forEach(
            (source: SourceDto) => {
                existingSource = this.selectedSources.find(a => a.listID == source.listID);
                if (existingSource) {
                    if (existingSource.action == ActionType.Add) this.selectedSources = this.selectedSources.filter(value => value.listID != source.listID);
                    else existingSource.action = ActionType.Delete;
                }
            });
        this.selectedSourcesLength = this.selectedSources.filter(source => source.action != ActionType.Delete).length;
        this.filterText = '';
        this.addedSources = [];
        this.deletedSources = [];
        this.updateSourceCount();
    }
    save(): void {
        this.saving = true;
        let addedSources: SourceDto[] = this.selectedSources.filter(source => source.action == ActionType.Add);
        let deletedSources: SourceDto[] = this.selectedSources.filter(source => source.action == ActionType.Delete && source.id != 0)
        let skip: boolean = (addedSources.length == 0) && (deletedSources.length == 0);
        if (skip) this.close();
        else {
            let input: SaveSourcesInputDto = new SaveSourcesInputDto();
            input.init({ segmentID: this.segmentID, addedSources, deletedSources });
            this._segmentListsServiceProxy.saveSources(input)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.activeModal.close({ isSave: true });
                });
        }
    }
    close(): void {
        this.activeModal.close({ isSave: false });
    }
    showSelectedListLoadingIndicator(): void {
        setTimeout(() => {
            this.isSelectedListLoading = true;
        }, 0);
    }
    hideSelectedListLoadingIndicator(): void {
        setTimeout(() => {
            this.isSelectedListLoading = false;
        }, 0);
    }

    updateSourceCount(): void{
        this.approvedSourceCount = this.approvedSources.filter(source => source.action != this.actionType.Delete).length;
        this.selectedSourceCount = this.selectedSources.filter(source => source.action != this.actionType.Delete).length;
    }
}

