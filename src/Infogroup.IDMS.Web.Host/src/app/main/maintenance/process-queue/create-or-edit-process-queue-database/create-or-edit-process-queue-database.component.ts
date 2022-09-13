import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActionType, DatabasesServiceProxy, GetAllForLookupTableInput, GetAllForTableInput, ProcessQueueDatabasesServiceProxy, ProcessQueuesServiceProxy } from '@shared/service-proxies/service-proxies';
import { unset } from 'lodash';
import { combineLatest } from 'rxjs';
import { finalize, tap } from 'rxjs/operators';


@Component({
  selector: 'app-create-or-edit-process-queue-database',
  templateUrl: './create-or-edit-process-queue-database.component.html',
  styleUrls: ['./create-or-edit-process-queue-database.component.css']
})
export class CreateOrEditProcessQueueDatabaseComponent extends AppComponentBase {
  saving=false;
  @Input() id:number;
  @Input() cDescription:string;
  databases=[];
  Originaldatabase:any=[];
  selectedDatabses:any=[];
  selectedDatabsesvalues:any=[];
  isLoading:boolean = true;
  dto: GetAllForLookupTableInput= new GetAllForLookupTableInput();
  showSelection:boolean=false;
  checkselectedDBs: any = [];
  constructor(injector:Injector,
    private databaseServiceProxy:DatabasesServiceProxy,
    private processQueueServiceProxy: ProcessQueuesServiceProxy,
    private processQueueDatabaseServiceProxy: ProcessQueueDatabasesServiceProxy,
    public activeModal: NgbActiveModal) {super(injector) }
  
  ngOnInit() {
    this.getDatabase();    
  }
  getDatabase(){
   
    combineLatest([

      this.databaseServiceProxy.getDatabaseWithNoAccessCheck().pipe(tap(result => {}))
    ]).subscribe((resultArray) => {
      this.databases= resultArray[0];
      this.getselectedDatabase();
     
    })
  }
  getselectedDatabase(){
    this.dto.filter='';
    this.dto.maxResultCount=1;
    this.processQueueServiceProxy.getAllDbSet(this.dto.filter,this.dto.sorting,this.dto.skipCount,this.dto.maxResultCount,this.id).
    subscribe((result)=>{
      this.isLoading = false;
      for (let index = 0; index < result.items.length; index++) {
        this.selectedDatabses[index]={label:result.items[index].cDatabaseName,value:result.items[index].databaseId}
      this.Originaldatabase[index]={label:result.items[index].cDatabaseName,value:result.items[index].databaseId,action:ActionType.None}
      }
    
    })
    
  }
  checkSelectedDatabase(value){
    let result = false;
    this.checkselectedDBs.forEach(element => {
      if(element.value === value){
        this.checkselectedDBs=this.checkselectedDBs.filter(db=>db.value !== value)
        result = true;
      }     
    });
    return result;
  }
  CheckingList(){
    this.checkselectedDBs = this.selectedDatabses;
    this.Originaldatabase.forEach(element => {
      let result = this.checkSelectedDatabase(element.value);
      if(result){
        element.action = ActionType.None;
      }else{
        element.action = ActionType.Delete;
      }
    }); 
    this.checkselectedDBs.forEach(ele => {
      ele.action = ActionType.Add;
    })
  }

  save(form?){
    this.saving=true;
    this.CheckingList();
    let finaList = this.Originaldatabase.concat(this.checkselectedDBs);
    this.processQueueDatabaseServiceProxy.createOrEdit(finaList,this.id).pipe(finalize(() => { this.saving = false; })).subscribe(()=>{
     this.notify.info(this.l('SavedSuccessfully'));
    this.activeModal.close({ saving: this.saving });
    });
  }
  close(){
  this.activeModal.close({saving: this.saving});
  }
}
