import { Component,OnInit,Injector,ViewChild,ViewEncapsulation} from '@angular/core';
import { IDMSConfigurationsServiceProxy,PageID,ShortSearchServiceProxy} from '@shared/service-proxies/service-proxies';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';
import { CreateOrEditIdmsconfigurationComponent } from './create-or-edit-idmsconfiguration.component';

@Component({
  selector: 'app-idms-configuration',
  templateUrl: './idms-configuration.component.html',
  styleUrls: ['./idms-configuration.component.css'],
  encapsulation: ViewEncapsulation.None,    
  animations: [appModuleAnimation()]
})
export class IdmsConfigurationComponent extends AppComponentBase {

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('createOrEditIdmsconfigurationModal', { static: true }) createOrEditIdmsconfigurationModal: CreateOrEditIdmsconfigurationComponent;
  isSave: boolean = false;
  helpData: any = {
    header: "Search Options:",
    examples: []
  };
  isHelpDisabled: boolean = true;
  pageId: PageID = PageID.IDMSConfiguration;
  filterText: string = '';

  constructor(private _idmsConfigurationsServiceProxy:IDMSConfigurationsServiceProxy,injector: Injector,
    private _shortSearchServiceProxy: ShortSearchServiceProxy,
    private modalService: NgbModal) { 
    super(injector);
  }

  ngOnInit() {
    this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
      .subscribe(result => {
        this.helpData = result;
        this.isHelpDisabled = false;
      });
  }

  getAllConfigurationsList(event?: LazyLoadEvent): void
  {
    if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
      this.paginator.changePage(0);
      return;
  }
    this._idmsConfigurationsServiceProxy.getAllConfiguration
      (this.filterText.trim(),this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event))
      .subscribe(result=>{
            this.primengTableHelper.totalRecordsCount = result.totalCount;       
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();  
      }) 
  }

  createIdmsConfiguration(id ?,isCopyConfig?:boolean): void {        
    const modalRef: NgbModalRef = this.modalService.open(CreateOrEditIdmsconfigurationComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.isCopyConfig = isCopyConfig;
    modalRef.result.then((result) => {
        if (result.saving) {
            if (id) 
                this.isSave = true;
            this.getAllConfigurationsList();
        }
    }
    );
}
}
