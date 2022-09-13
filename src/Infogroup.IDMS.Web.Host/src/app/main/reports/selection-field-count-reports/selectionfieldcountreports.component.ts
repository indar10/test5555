import { Component, Injector,OnInit,ViewEncapsulation, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { finalize } from "rxjs/operators";
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import {PageID, ShortSearchServiceProxy,GetSelectionFieldCountReportInput,DatabasesServiceProxy,SelectionFieldCountReportsServiceProxy, GetSelectionFieldCountReportView} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { SelectItem } from 'primeng/api';
import { Console } from 'console';

@Component({
  selector: 'app-selectionfieldcountreports',
  templateUrl: './selectionfieldcountreports.component.html',
  styleUrls: ['./selectionfieldcountreports.component.css'],
  encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class SelectionfieldcountreportsComponent extends AppComponentBase {

  @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    selectedDatabase: any;
    databaseList: SelectItem[] = [];
    filterText: string = '';
    SelectedCQuestionFieldName:string;
    SelectediStatus:string;
    helpData: any = {
      header: "Search Options:",
      examples: []
    };
    isHelpDisabled: boolean = true;
    isSave: boolean = false;
    pageId: PageID = PageID.SelectionFieldCountReports;
    dto: GetSelectionFieldCountReportInput = new GetSelectionFieldCountReportInput();

  constructor( injector: Injector,private _shortSearchServiceProxy: ShortSearchServiceProxy,
    private _selectionFieldCountReportsServiceProxy:SelectionFieldCountReportsServiceProxy,
    private _fileDownloadService: FileDownloadService,private _databaseServiceProxy: DatabasesServiceProxy) {  super(injector);}
  ngOnInit() {        
    this._shortSearchServiceProxy.getSearchHelpText(this.pageId)
      .subscribe(result => {
        this.helpData = result;
        this.isHelpDisabled = false;
      });
      this.fillDatabaseDropdown();      
  }

  fillDatabaseDropdown() {
    this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
        this.databaseList = result;
        this.selectedDatabase = this.databaseList[0].value;      
    });
}
 
  getSelectionFieldCountReports(event?: LazyLoadEvent):void {   
    if (this.primengTableHelper.shouldResetPaging(event) && !this.isSave) {
        this.paginator.changePage(0);
        return;
    }
    this.isSave = false;        

    
    this.primengTableHelper.showLoadingIndicator();
    this._selectionFieldCountReportsServiceProxy.getAllSelectionFieldCountReports(
        this.filterText.trim(),        
        this.selectedDatabase,
        this.SelectedCQuestionFieldName,
        this.SelectediStatus,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
    )
        .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
        .subscribe(result => {              
        result.totalCount === undefined ? this.primengTableHelper.totalRecordsCount=0 : this.primengTableHelper.totalRecordsCount = result.totalCount;        
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();       
    });
}
getFieldNameDescription(record)
{  
  return record.cQuestionFieldName+record.cDescription+record.cDescription
}

ExportToExcelRecord(record ?): void {    
  this.downloadSelectionFieldCountReport(record);
}

downloadSelectionFieldCountReport(record:any):void {  
        this.selectedDatabase;    
        this.dto.filter = this.filterText.trim();       
        this.dto.selectedDatabase = this.selectedDatabase;             
        this.dto.selectedcQuestionFieldName = record.cQuestionFieldName;
        this.dto.selectediStatus=record.iStatus;  
        this.dto.sorting = null;
        this.dto.skipCount = 0;                    
  this.dto.maxResultCount = this.primengTableHelper.totalRecordsCount !== 0 ? this.primengTableHelper.totalRecordsCount : 1;
  this._selectionFieldCountReportsServiceProxy.downloadSelectionFieldCountReport(this.dto
    ).subscribe(result => {
      this._fileDownloadService.downloadTempFile(result);
  });
}

}
