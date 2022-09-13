import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import {CreateOrEditExternalBuildTableDatabaseDto, DatabasesServiceProxy, ExternalBuildTableDatabasesServiceProxy } from '@shared/service-proxies/service-proxies'
import { combineLatest } from 'rxjs';
import { finalize, tap } from 'rxjs/operators';

@Component({
  selector: 'app-create-or-edit-externa-database-links',
  templateUrl: './create-or-edit-externa-database-links.component.html',
  styleUrls: ['./create-or-edit-externa-database-links.component.css']
})
export class CreateOrEditExternaDatabaseLinksComponent extends AppComponentBase {
@Input() id:number;
saving:boolean= false;
databaseName=[];
isLoading:boolean=false;
tableDescription=[];
dbLinks:CreateOrEditExternalBuildTableDatabaseDto = new CreateOrEditExternalBuildTableDatabaseDto();
  constructor(injector: Injector,
    public activeModal: NgbActiveModal,private externaldbProxy:ExternalBuildTableDatabasesServiceProxy,
    private _databaseServiceProxy: DatabasesServiceProxy) 
    {super(injector); }

  ngOnInit() {
   this.FillDropDown();
   
    }
    FillDropDown(){
      combineLatest([
        this._databaseServiceProxy.getDatabaseWithNoAccessCheck().pipe(tap(result => {})),
        this.externaldbProxy.getAllBuildTableDescForDropdown().pipe(tap(result => {})),
      ]).subscribe((resultArray)=>{
        this.databaseName = resultArray[0],
        this.tableDescription= resultArray[1]
        if(this.id){
          this.show(this.id);
         }
      });
    }
 show(id){
  this.externaldbProxy.getById(id).subscribe(res=>{
   this.dbLinks.buildTableId=res.buildTableID;
   this.dbLinks.databaseId= res.databaseID;
  })
 }   
save(form?){
  if(form.dirty){
    this.saving= true;
    if(this.id){
      this.dbLinks.id=this.id;
    }
  this.externaldbProxy.createOrEdit(this.dbLinks).pipe(finalize(() => { this.saving = false; })).subscribe(()=>{
  this.notify.info(this.l('SavedSuccessfully'));
  this.activeModal.close({ saving: this.saving});
});
  }
}
close(){
this.activeModal.close({saving: this.saving});
}
}
