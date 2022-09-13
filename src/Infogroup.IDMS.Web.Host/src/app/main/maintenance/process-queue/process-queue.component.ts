import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDefaults, ModalSize } from '@shared/costants/modal-contants';
import { PageID, ProcessQueuesServiceProxy, ShortSearchServiceProxy } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { CreateOrEditProcessQueueComponent } from './create-or-edit-process-queue/create-or-edit-process-queue.component';
import { CreateOrEditProcessQueueDatabaseComponent } from './create-or-edit-process-queue-database/create-or-edit-process-queue-database.component';

@Component({
  selector: 'app-process-queue',
  templateUrl: './process-queue.component.html',
  styleUrls: ['./process-queue.component.css']
})
export class ProcessQueueComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  filterText: string = '';
  isSave:boolean=false;
  helpData: any = {
    header: "Search Options:",
    examples: []
  };
  isHelpDisabled: boolean = true;
  pageId: PageID = PageID.ProcessQueues
   
  constructor(injector: Injector, private processQueueServiceProxy: ProcessQueuesServiceProxy, 
    private _shortSearchServiceProxy:ShortSearchServiceProxy,
    private modalService: NgbModal) { super(injector); }

  ngOnInit(event) {
    this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
    .subscribe(result => {
      this.helpData = result;
      this.isHelpDisabled = false;
    });

  }
getProcessQueues(event?:LazyLoadEvent){
  this.processQueueServiceProxy.getAll( this.filterText,
    this.primengTableHelper.getSorting(this.dataTable),
    this.primengTableHelper.getSkipCount(this.paginator, event),
    this.primengTableHelper.getMaxResultCount(this.paginator, event)).subscribe(response=>{
     
    this.primengTableHelper.totalRecordsCount = response.totalCount;
    this.primengTableHelper.records = response.items;
    this.primengTableHelper.hideLoadingIndicator();
  })
}
openModalPopup(id?){
  const modalRef: NgbModalRef = this.modalService.open(CreateOrEditProcessQueueComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
  modalRef.componentInstance.id = id;
  modalRef.result.then((result) => {
      if (result.saving) {
              this.isSave = true;
          this.getProcessQueues();
      }
  }
  );
}
AddDatabase(id,name){
  const modalRef: NgbModalRef = this.modalService.open(CreateOrEditProcessQueueDatabaseComponent,
     { size: ModalSize.DEFAULT, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
  modalRef.componentInstance.id = id;
  modalRef.componentInstance.cDescription=name;
  modalRef.result.then((result) => {
      if (result.saving) {
              this.isSave = true;
          this.getProcessQueues();
      }
  }
  );
}
}
