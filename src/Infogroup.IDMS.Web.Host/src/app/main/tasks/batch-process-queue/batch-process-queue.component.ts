import { Component, Injector, ViewEncapsulation, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BatchQueuesServiceProxy, PageID, ShortSearchServiceProxy } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-batch-process-queue',
  templateUrl: './batch-process-queue.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class BatchProcessQueueComponent extends AppComponentBase {

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  @Output() switchToTaskQueue: EventEmitter<any> = new EventEmitter<any>();
  @Input() outsideClick: false;

  isSave: boolean = false;
  filterText: string = '';
  helpData: any = {
    header: "Search Options:",
    examples: []
  };
  isHelpDisabled: boolean = true;
  pageId: PageID = PageID.BatchProcessQueue
  constructor(injector: Injector, private _batchqueueServiveproxy: BatchQueuesServiceProxy, private _shortSearchServiceProxy: ShortSearchServiceProxy) { super(injector); }

  ngOnInit() {
    this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
      .subscribe(result => {
        this.helpData = result;
        this.isHelpDisabled = false;
      });
  }
  getAll(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
      this.paginator.changePage(0);
      return;
    }
    this.isSave = false;
    this.primengTableHelper.showLoadingIndicator();
    this._batchqueueServiveproxy.getAll(
      this.filterText,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.records.forEach((data) => {
        var CountId = data.parmData;
        var reg = new RegExp('^[0-9]+$');
        if (!reg.test(CountId)) {
          data.parmData = "";
        }

      });
      this.primengTableHelper.hideLoadingIndicator();
    });
  }
  OnCancelClick(id,status) {
      if (status == 'Waiting') {
        this._batchqueueServiveproxy.createOrEdit(id).pipe(finalize(() => { }))
          .subscribe(() => {
            this.notify.info(this.l('CancelledSuccessfully'));
            this.getAll();
          });
      }
  }
  getTasks(){
    this.switchToTaskQueue.emit(false);
  }
}
