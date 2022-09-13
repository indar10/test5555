import { Component,Injector,ViewChild,ViewEncapsulation} from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults } from '@shared/costants/modal-contants';
import { CreateOrEditListautomateModalComponent } from './create-or-edit-listautomate-modal.component';
import { ListAutomatesServiceProxy} from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';

//ListAutomatesServiceProxy
@Component({
  selector: 'app-list-automate',
  templateUrl: './list-automate.component.html',
  encapsulation: ViewEncapsulation.None,    
  animations: [appModuleAnimation()]
})
export class ListAutomateComponent extends AppComponentBase {
  advancedFiltersAreShown : boolean = false;
  isSave: boolean = false;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('createOrEditListautomateModal', { static: true }) createOrEditListautomateModal: CreateOrEditListautomateModalComponent;
  filterText : string = '';
  iIsActiveFilter: boolean = true;

  constructor(injector: Injector, 
    private modalService: NgbModal,
    private _listAutomatesServiceProxy: ListAutomatesServiceProxy
    ) {super(injector); }

  getAllListAutomateSchedule(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
        this.paginator.changePage(0);
       return;
     }
    this.isSave = false;
    this.primengTableHelper.showLoadingIndicator();
    this._listAutomatesServiceProxy.getAllListAutomate(this.filterText,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)).
      subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;       
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();            
        });
}  
  createListAutomate(id ?): void {        
    const modalRef: NgbModalRef = this.modalService.open(CreateOrEditListautomateModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
    modalRef.componentInstance.id = id;
    modalRef.result.then((result) => {
        if (result.saving) {
            if (id) 
                this.isSave = true;
            this.getAllListAutomateSchedule();
        }
    }
    );
}

}
