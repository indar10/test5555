import { Component, OnInit, ViewChild, Injector, Input, Output, EventEmitter } from '@angular/core';
import { Table } from 'primeng/table';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CampaignsServiceProxy, IdmsUserLoginInfoDto, OrderStatusServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'campaigns-queue',
  templateUrl: './campaigns-queue.component.html',
  styleUrls: ['./campaigns-queue.component.css'],
  animations: [appModuleAnimation()]
})
export class CampaignsQueueComponent extends AppComponentBase implements OnInit {

  @ViewChild('dataTable', { static: true }) dataTable: Table;  
  @Output() switchToCampaign: EventEmitter<any> = new EventEmitter<any>();
  @Input() outsideClick: false;

  idmsUserData: IdmsUserLoginInfoDto;

  constructor(
    injector: Injector,
    private _campaignServiceProxy: CampaignsServiceProxy,
    private _OrderStatusServiceProxy: OrderStatusServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getCampaignsQueue();
  }

  getCampaignsQueue() {
    
    this.idmsUserData = this.appSession.idmsUser;

    let callAppService = this.idmsUserData !== undefined;
    if (callAppService) {
        this.createFilters();
    }
    else {
        this.primengTableHelper.totalRecordsCount = 0;
        this.primengTableHelper.records = undefined;

    }
  }

  createFilters(): void {
    let userIDFilter = this.idmsUserData.idmsUserID;

    this.primengTableHelper.showLoadingIndicator();
        this._campaignServiceProxy.getAllCampaignQueue(userIDFilter)
        .subscribe(result => {            
            this.primengTableHelper.totalRecordsCount = result.length;
            this.primengTableHelper.records = result;
            this.primengTableHelper.hideLoadingIndicator();
        });
  }

  stopCampaignQueue(campaignId?: number): void {
    this.message.confirm(
        this.l(''),
        (isConfirmed) => {
            if (isConfirmed) {
              this._OrderStatusServiceProxy.updateCampaignStatus(campaignId).subscribe(() => {
                this.getCampaignsQueue();
                this.notify.info(this.l('StopSuccessfully'));
              });                
            }
        }
    );
    } 

    getCampaigns() {
        this.switchToCampaign.emit(false);
    }    
    
}


