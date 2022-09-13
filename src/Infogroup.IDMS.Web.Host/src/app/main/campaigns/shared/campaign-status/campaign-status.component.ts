import { Component, OnInit, Input, Injector } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { OrderStatusServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';


@Component({
  selector: 'popover',
  templateUrl: './campaign-status.component.html',
  styleUrls: ['./campaign-status.component.css'],
  animations: [appModuleAnimation()]
})
export class CampaignStatusComponent extends AppComponentBase implements OnInit {
   @Input() campaignId: number;
   @Input() databaseID: number;
   @Input() campaignStatus: number;
 
  constructor(
    private _OrderStatusServiceProxy: OrderStatusServiceProxy,
    injector: Injector,
    private modalService: NgbModal
  ) 
  { 
      super(injector);
  }

  ngOnInit() {
      this.primengTableHelper.showLoadingIndicator();
      this._OrderStatusServiceProxy.getStatusHistory(this.campaignId)
          .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
          .subscribe(result => {
              this.primengTableHelper.records = result;
              
          });      
  }

    getRealTimeStatus(event): void {
        if (this.primengTableHelperActiveStatus.totalRecordsCount > 0 || event.collapsed) {
            return;
        }
        this.primengTableHelperActiveStatus.showLoadingIndicator();
        this._OrderStatusServiceProxy.getLastLogStatement(this.campaignId, this.databaseID)
            .pipe(finalize(() => this.primengTableHelperActiveStatus.hideLoadingIndicator()))
            .subscribe(result => {
                this.primengTableHelperActiveStatus.records = result;
                this.primengTableHelperActiveStatus.totalRecordsCount = result.length;
            });   
    }
}
