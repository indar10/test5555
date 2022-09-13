import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Table } from 'primeng/components/table/table';
import { finalize } from "rxjs/operators";
import * as _ from 'lodash';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LookupsServiceProxy, PageID, ShortSearchServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { CreateOrEditLookupModalComponent } from './create-or-edit-lookup-modal.component';
import { ModalDefaults } from '@shared/costants/modal-contants';
@Component({
  templateUrl: './lookups.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class LookupsComponent extends AppComponentBase {
  @ViewChild('createOrEditLookupModal', { static: true }) createOrEditLookupModal: CreateOrEditLookupModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  isSave: boolean = false;
  filterText: string = '';
  helpData: any = {
    header: "Search Options:",
    examples: []
  };
  isHelpDisabled: boolean = true;
  pageId: PageID = PageID.Lookups
  constructor(injector: Injector, private _shortSearchServiceProxy: ShortSearchServiceProxy,
    private _lookupsServiceProxy: LookupsServiceProxy,
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
  getAllLookups(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
      this.paginator.changePage(0);
      return;
    }
    this.isSave = false;

    this.primengTableHelper.showLoadingIndicator();
    this._lookupsServiceProxy.getAllLookups(
      this.filterText,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).subscribe(result => {
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
    });

  }
  createLookup(id?): void {
    const modalRef: NgbModalRef = this.modalService.open(CreateOrEditLookupModalComponent, { size: ModalDefaults.Size, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
    modalRef.componentInstance.ID = id;
    modalRef.result.then((result) => {
      if (result.saving) {
        if (id)
          this.isSave = true;
        this.getAllLookups();
      }
    }
    );
  }

}




