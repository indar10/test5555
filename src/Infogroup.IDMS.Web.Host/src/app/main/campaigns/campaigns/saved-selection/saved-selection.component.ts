import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { SavedSelectionsServiceProxy, AddSavedSelection } from '@shared/service-proxies/service-proxies';
import { Table } from 'primeng/components/table/table';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'savedSelection',
    templateUrl: './saved-selection.component.html',
    animations: [appModuleAnimation()]
})
export class SavedSelectionComponent extends AppComponentBase implements OnInit {  

    @ViewChild('dataTable', { static: true }) dataTable: Table;  
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() segmentId: number;
    @Input() campaignId: number;
    saving: boolean = false;
    canAdd: boolean = false;
    filterText: string = '';
    selectedSavedSelection: any;  
    addSavedSelectionDTO: AddSavedSelection = new AddSavedSelection();
    constructor(
        injector: Injector,
        private activeModal: NgbActiveModal,
        private _savedSelectionsServiceProxy: SavedSelectionsServiceProxy

    ) {
        super(injector);
    }   

    ngOnInit() {
    }

    getSavedSelections(event?: LazyLoadEvent) {
        this.primengTableHelper.showLoadingIndicator();
        this._savedSelectionsServiceProxy.getAllSavedSelections(
            this.filterText.trim(),            
            this.campaignId,          
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    close(): void {
        this.activeModal.close({ isSave: false });
    }

    addSavedSelection(): void {
        this.saving = true;
        this.addSavedSelectionDTO.segmentID = this.segmentId;
        this.addSavedSelectionDTO.campaignID = this.campaignId;
        this.addSavedSelectionDTO.savedSelectionList = this.selectedSavedSelection;
        this._savedSelectionsServiceProxy.addSavedSelections(this.addSavedSelectionDTO)
        .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.activeModal.close({ isSave: this.saving });
                });        
    }  

    canSave(): void {
        if (this.selectedSavedSelection.length !== 0)
            this.canAdd = true;
        else
            this.canAdd = false;
    }

    deleteSavedSelection(id: number, userDefault: boolean): void {
        this.message.confirm(this.l(""), isConfirmed => {
            if (isConfirmed) {
                if (userDefault) {
                    this._savedSelectionsServiceProxy.deleteUserSavedSelection(id)
                        .subscribe(() => {
                            this.notify.info(this.l('SuccessfullyDeleted'));
                            this.getSavedSelections();
                        });
                }
                else {
                    this._savedSelectionsServiceProxy.deleteSavedSelection(id)
                        .subscribe(() => {
                            this.notify.info(this.l('SuccessfullyDeleted'));
                            this.getSavedSelections();
                        });
                }
            }
        });
        
    }

    onIsDefaultChange(event, i) {
        let record = this.primengTableHelper.records[i];
        this._savedSelectionsServiceProxy.updateIsDefaultRule(record.id, event)
            .subscribe(() => {
                this.getSavedSelections();
            });
    }    
}