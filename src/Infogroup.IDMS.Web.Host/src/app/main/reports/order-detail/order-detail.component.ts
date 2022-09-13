import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { ContactsServiceProxy, ContactType } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';
import {PageID, ShortSearchServiceProxy,CampaignsServiceProxy,SelectionFieldCountReportsServiceProxy} from '@shared/service-proxies/service-proxies';
import { finalize } from "rxjs/operators";
import { Console } from 'console';

@Component({
  selector: 'order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css']
})
export class OrderDetailComponent extends AppComponentBase  {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Input() cQuestionFieldName: string;
  @Input() iStatus: number;  
  @Input() selectedDatabase:number; 
  @Input() filterText: string = '';
  @Input() isEdit: boolean = true;
  @Input() permissionName: string;
  isSave: boolean = false;

  constructor(injector: Injector,
    private modalService: NgbModal,
    private _selectionFieldCountReportsServiceProxy:SelectionFieldCountReportsServiceProxy) {super(injector); }

getOrderDetails(event?: LazyLoadEvent):void
{    if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
  this.paginator.changePage(0);
  return;
}
    this.isSave = false;        
    this.primengTableHelper.showLoadingIndicator();
    this._selectionFieldCountReportsServiceProxy.getOrderDetails(
    this.filterText.trim(),
    this.cQuestionFieldName,
    this.iStatus,  
    this.selectedDatabase, 
    this.primengTableHelper.getSorting(this.dataTable),
    this.primengTableHelper.getSkipCount(this.paginator, event),
    this.primengTableHelper.getMaxResultCount(this.paginator, event)
)
    .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
    .subscribe(result => {   
    this.primengTableHelper.totalRecordsCount = result.totalCount;
    this.primengTableHelper.records = result.items;
    this.primengTableHelper.hideLoadingIndicator();   
});
}
}
