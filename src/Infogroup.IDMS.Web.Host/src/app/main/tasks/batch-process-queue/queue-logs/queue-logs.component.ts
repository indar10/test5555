import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BatchQueuesServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-queue-logs',
  templateUrl: './queue-logs.component.html',
  animations: [appModuleAnimation()]
})
export class QueueLogsComponent extends AppComponentBase implements OnInit {
  @Input() queueId: number;
    queues:any = []

  constructor(  injector: Injector,private _batchqueueServiveproxy:BatchQueuesServiceProxy) {  super(injector); }

  ngOnInit() {
    this.getLogs();
  }

  getLogs(){
    this.primengTableHelper.showLoadingIndicator();
  this._batchqueueServiveproxy.getQueuesData(this.queueId)
  .subscribe(result => {
     this.queues = result;
    this.primengTableHelper.records = Array.of(this.queues);
  });    
  }

}
