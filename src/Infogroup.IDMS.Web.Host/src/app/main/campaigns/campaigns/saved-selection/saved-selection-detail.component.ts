import { Component, Injector, Input, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { SavedSelectionDetailsServiceProxy } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';

@Component({
    selector: 'savedSelectionDetail',
    templateUrl: './saved-selection-detail.component.html'

})
export class SavedSelectionDetailComponent extends AppComponentBase {  
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;  
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Input() segmentID: number;
    @Input() savedSelectionId: number;
    @Input() userDefault: boolean;

    constructor(
        injector: Injector,
        private _savedSelectionDetailsServiceProxy: SavedSelectionDetailsServiceProxy

    ) {
        super(injector);
    }    


    getSavedSelectionsDetails() {
        this.primengTableHelper.showLoadingIndicator();
        this._savedSelectionDetailsServiceProxy.getAllSavedSelectionsDetails(                       
            this.segmentID,  
            this.savedSelectionId,
            this.userDefault
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.length;
            this.primengTableHelper.records = result;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    
   
}