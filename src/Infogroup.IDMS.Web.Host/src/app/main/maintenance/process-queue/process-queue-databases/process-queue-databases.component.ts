import { Component, Inject, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProcessQueueDatabasesServiceProxy, ProcessQueuesServiceProxy } from '@shared/service-proxies/service-proxies';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-process-queue-databases',
  templateUrl: './process-queue-databases.component.html',
  styleUrls: ['./process-queue-databases.component.css']
})
export class ProcessQueueDatabasesComponent extends AppComponentBase {
@Input() processQueueID:number;
@ViewChild('dataTable', { static: true }) dataTable: Table;
@ViewChild('paginator', { static: true }) paginator: Paginator;
filterText: string = '';
  constructor(injector:Injector,private processQueueServiceProxy: ProcessQueuesServiceProxy,
    private processQueueDatabaseServiceProxy:ProcessQueueDatabasesServiceProxy ) {super(injector) }

  ngOnInit() {
  }
  getProcessQueueDatabase(event?){
    this.processQueueServiceProxy.getAllDbSet(this.filterText,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event),this.processQueueID).subscribe(res=>{
        this.primengTableHelper.totalRecordsCount = res.totalCount;
      this.primengTableHelper.records = res.items;
      this.primengTableHelper.hideLoadingIndicator();
      })
  }
  deleteRecord(record){
    this.message.confirm(
      this.l(""),
      this.l("Are you sure you want to delete?"),
      isConfirmed=>{
        if(isConfirmed){
          this.processQueueDatabaseServiceProxy.delete(record).subscribe(()=>{
            this.getProcessQueueDatabase();
          });
        
        }
      
      
    })
    
  }
}
