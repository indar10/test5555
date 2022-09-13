import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from "@shared/common/app-component-base";
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { PageID, DatabasesServiceProxy, MasterLoLsServiceProxy, ShortSearchServiceProxy,GetAllForTableInput, ExternalBuildTableDatabasesServiceProxy } from '@shared/service-proxies/service-proxies';
import { SelectItem } from 'primeng/api';
import { NgbActiveModal, NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ModalDefaults, ModalSize } from '@shared/costants/modal-contants';
import { CreateOrEditListoflistComponent } from '../../database-build/list-of-list/create-or-edit-listoflist/create-or-edit-listoflist.component'
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  selector: 'app-list-of-list',
  templateUrl: './list-of-list.component.html',
  styleUrls: ['./list-of-list.component.css']
})
export class ListOfListComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  selectedDatabase: any;
  iIsActiveFilter:number;
  dto: GetAllForTableInput = new GetAllForTableInput();
  databaseList: SelectItem[] = [];
  helpData: any = {
    header: "Search Options:",
    examples: []
  };
  isHelpDisabled: boolean = true;
  pageId: PageID = PageID.ListOfList;
  filterText: string = '';

  isLoading:boolean=false;
  checkIfEdit: string;
  constructor(injector: Injector, private _databaseServiceProxy: DatabasesServiceProxy, 
    private _masterLoLsServiceProxy: MasterLoLsServiceProxy,
    private _shortSearchServiceProxy: ShortSearchServiceProxy,
    private _fileDownloadService: FileDownloadService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private externalDbLinks: ExternalBuildTableDatabasesServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.fillDatabaseDropdown();
    this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
      .subscribe(result => {
        this.helpData = result;
        this.isHelpDisabled = false;
      });
  }

  fillDatabaseDropdown() {
    this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
      this.databaseList = result;
      this.selectedDatabase = this.databaseList[0].value;
      if (this.selectedDatabase) {
        this.getAllListOfList();
      }
    });

  }
  getAllListOfList(event?: LazyLoadEvent) {
    if(this.selectedDatabase){
    this.primengTableHelper.showLoadingIndicator();
    this._masterLoLsServiceProxy.
      getAll(this.filterText.trim(), this.selectedDatabase,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)).subscribe(result => {
          this.primengTableHelper.totalRecordsCount = result.totalCount;
          this.primengTableHelper.records = result.items;
          this.primengTableHelper.hideLoadingIndicator();
        })
      }
  }
  PrintListMailerAccess(){
    this.dto.selectedDatabase=this.selectedDatabase;
    
    this._masterLoLsServiceProxy.exportAllListMailerAccessToExcel(this.dto).subscribe(res=>{

      this._fileDownloadService.downloadTempFile(res)
    })

  }
  PrintList(){

    this.dto.selectedDatabase=this.selectedDatabase;
    this._masterLoLsServiceProxy.exportAllMailerAccessToExcel(this.dto).subscribe(res=>{
      this._fileDownloadService.downloadTempFile(res)
    })
     
   
  }
  createList(id?,checkstring?): void {    
    const modalRef: NgbModalRef = this.modalService.open(CreateOrEditListoflistComponent, 
      { size: ModalSize.LARGE, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
    modalRef.componentInstance.databaseId = this.selectedDatabase;
    if(id){
    modalRef.componentInstance.ID=id;
    modalRef.componentInstance.isLoading=true;
    modalRef.componentInstance.checkIfEdit=checkstring;
    }
    modalRef.result.then((result) => {
      if (result.saving) {
        this.getAllListOfList();
      }
    }
    );
  }
}
