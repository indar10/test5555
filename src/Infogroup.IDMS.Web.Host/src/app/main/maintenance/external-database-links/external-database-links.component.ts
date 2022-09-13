import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDefaults, ModalSize } from '@shared/costants/modal-contants';
import { ExternalBuildTableDatabasesServiceProxy, GetAllForTableInput, PageID, ShortSearchServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { CreateOrEditExternaDatabaseLinksComponent } from './create-or-edit-externa-database-links/create-or-edit-externa-database-links.component';

@Component({
  selector: 'app-external-database-links',
  templateUrl: './external-database-links.component.html',
  styleUrls: ['./external-database-links.component.css']
})
export class ExternalDatabaseLinksComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  filterText: string = '';
  helpData: any = {
    header: "Search Options:",
    examples: []
  };
  isHelpDisabled: boolean = true;
  pageId: PageID = PageID.ExternalDatabaseLinks
  exportToexcelflag: boolean=false;
  dto: GetAllForTableInput= new GetAllForTableInput();
  writePermission:boolean;
  constructor(injector: Injector,private externaldbProxy:ExternalBuildTableDatabasesServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _shortSearchServiceProxy: ShortSearchServiceProxy,
    private modalService: NgbModal) 
  { 
     super(injector);
    }

  ngOnInit() {
     this.writePermission= this.isGranted("Pages.ExternalBuildTableDatabases.Write");
    this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
    .subscribe(result => {
      this.helpData = result;
      this.isHelpDisabled = false;
    });
  }

  getAll(event? : LazyLoadEvent){
    this.externaldbProxy.getAllLinks(this.filterText.trim(),
    this.primengTableHelper.getSorting(this.dataTable),
    this.primengTableHelper.getSkipCount(this.paginator, event),
    this.primengTableHelper.getMaxResultCount(this.paginator, event),this.exportToexcelflag).subscribe(result=>{
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
    });

  }
  deleteRecord(id?){

    this.message.confirm(
      this.l(""),
      this.l("Are you sure you want to delete?"),
      isConfirmed=>{
        if(isConfirmed){
          this.externaldbProxy.deleteRecord(id).subscribe(()=>{
            this.getAll();
          });
        
        }
      
      
    })
     
  }
  openModalPopup(id?){
    const modalRef: NgbModalRef = this.modalService.open(CreateOrEditExternaDatabaseLinksComponent, 
      { size: ModalSize.DEFAULT, backdrop: ModalDefaults.Backdrop, windowClass: ModalDefaults.WindowClass });
      if(id){
        modalRef.componentInstance.id=id;
      }
      modalRef.result.then((result) => {
      if (result.saving) {
        this.getAll();
      }
    }
    );
  }
  ExportToExcelDbLinks(){
    this.exportToexcelflag= true;
    this.externaldbProxy.exportAllExternalDbLinksToExcel(this.dto,this.exportToexcelflag).subscribe(result=>{
      this._fileDownloadService.downloadTempFile(result);
    })
  }
}
