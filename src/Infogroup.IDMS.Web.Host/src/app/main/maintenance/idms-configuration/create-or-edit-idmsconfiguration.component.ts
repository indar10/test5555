import { Component, OnInit, Injector,Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { IDMSConfigurationsServiceProxy,CampaignsServiceProxy,DatabasesServiceProxy,RedisCachingServiceProxy,CreateOrEditConfigurationDto} from '@shared/service-proxies/service-proxies';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { SelectItem } from 'primeng/api';
import { result } from 'lodash';
import { finalize } from 'rxjs/operators';
import * as moment from 'moment';
import {ChangeDetectorRef } from '@angular/core';


@Component({
  selector: 'app-create-or-edit-idmsconfiguration',
  templateUrl: './create-or-edit-idmsconfiguration.component.html',
  styleUrls: ['./create-or-edit-idmsconfiguration.component.css']
})
export class CreateOrEditIdmsconfigurationComponent extends AppComponentBase {
  @Input() id: number;  
  @Input() isCopyConfig:boolean;
  active = false;
  saving = false;
  configuredDate : Date;
  currentDate: any;
  databaseList: any = [];
  selectedDatabase: any;    
  cDescriptionList: any = [];
  selectedDescription: any;   
  popup:boolean=false;
  cItems:any=[];
  idmsConfigurationDto:CreateOrEditConfigurationDto = new CreateOrEditConfigurationDto();
  databaseId : number;
  encryptCheck : boolean = false;
  activeCheck : boolean = true;
  configDescrption: any = [];
  popmValue:string = '';
  caches:any;
  constructor(private _idmsConfigurationsServiceProxy:IDMSConfigurationsServiceProxy,injector: Injector,public activeModal: NgbActiveModal,
    private _databaseServiceProxy: DatabasesServiceProxy,private _orderServiceProxy: CampaignsServiceProxy,
    private _idmsConfig: IDMSConfigurationsServiceProxy, private _redisCacheService: RedisCachingServiceProxy) 
  { 
    super(injector);
    
  }
  ngOnInit() {
    this.show(this.id);
    if(!this.id){
    this.FillcItemDropdown();
    this.FillDatabaseDropdown();
    this.FillcDescriptionDropdown();
  }
    this._redisCacheService.getAll().subscribe(result => {
      this.caches = result;
  })
   
  }

  show(id?: number):void{
    this.idmsConfigurationDto.id = id;
    if(!id){
    this.idmsConfigurationDto = new CreateOrEditConfigurationDto();
    }
  if(id || this.isCopyConfig){
    this._idmsConfigurationsServiceProxy.getConfigurationForEdit(id).subscribe(result => {
      this.FillDatabaseDropdown(result.databaseID)
      this.FillcDescriptionDropdown(result.cDescription);
      this.FillcItemDropdown(result.cItem);
      this.idmsConfigurationDto = result;
      this.activeCheck = this.idmsConfigurationDto.iIsActive;
      this.encryptCheck = this.idmsConfigurationDto.iIsEncrypted;
      this.popmValue = this.idmsConfigurationDto.mValue;
      if(result.dValue != undefined || result.dValue != null)
      {
          this.currentDate = result.dValue.toDate();
      }
      this.active = true;
     
      });
  }
  }
  FillcDescriptionDropdown(description?:string)
  {
    this._idmsConfig.getConfigurationValue("ConfigDescription", 0).subscribe(result => {
     var descriptionArray = result.cValue.split(',')
      for (let index = 0; index < descriptionArray.length; index++) 
      {
        this.cDescriptionList.push({label: descriptionArray[index], value: descriptionArray[index]});
      }
      if(description)
      {
       this.idmsConfigurationDto.cDescription = this.cDescriptionList.filter(ele => ele.value === description)[0].value;
      }
      else
      {
        this.idmsConfigurationDto.cDescription = this.cDescriptionList[0].value;
      }
    })
   
  }
  FillcItemDropdown(item?:string)
  {
    this._idmsConfigurationsServiceProxy.getConfigurationItems().subscribe(result=>{
      this.cItems = result;
      if(item)
      {
       this.idmsConfigurationDto.cItem = this.cItems.filter(ele => ele.value === item)[0].value;
      }
      else
      {
        this.idmsConfigurationDto.cItem = this.cItems[0].value;
      }
    })
    
  }

  FillDatabaseDropdown(DBID?: number) {
    this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
        this.databaseList.push({
          'label': 'NO DATABASE',
          'value': 0
      });
      for (let index = 0; index < result.length; index++) {
        this.databaseList.push({label: result[index].label, value: result[index].value});
    }
    if(DBID)
    {
      this.selectedDatabase = null;
      this.selectedDatabase = this.databaseList.filter(ele => ele.value === DBID)[0].value;
    }
    else{
      this.selectedDatabase = this.databaseList[0].value;
    }
    });
}

onEncryptChecked(event): void {
  if (event) {
      this.encryptCheck = true;
  }
  else {
      this.encryptCheck = false;
  }
}

onActiveChecked(event){
  if (event) {
    this.activeCheck = true;
}
else {
    this.activeCheck = false;
}
}
  save(idmsConfigForm: NgForm): void {
   
    this.idmsConfigurationDto.iIsActive = this.activeCheck;
    this.idmsConfigurationDto.iIsEncrypted = this.encryptCheck;
    if(this.currentDate !=undefined){
    this.idmsConfigurationDto.dValue =   moment(new Date(this.currentDate).toUTCString())
    }
    this.idmsConfigurationDto.databaseID = this.selectedDatabase;
    this.idmsConfigurationDto.mValue = this.popmValue;

    if(this.isCopyConfig)
    this.idmsConfigurationDto.id=null;
    
    if (idmsConfigForm.dirty) {
      this.saving = true;
       this._idmsConfigurationsServiceProxy.createOrEditIDMSConfig(this.idmsConfigurationDto)
         .pipe(finalize(() => { this.saving = false; }))
          .subscribe(() => {
            this._redisCacheService.clearCache('CFG').subscribe(() => {
              var selectedCache = this.caches.find(item => item.value === 'CFG');
              selectedCache.count = "0";
          });
            this.notify.info(this.l('SavedSuccessfully'));
              this.activeModal.close({ saving: this.saving });
       });
  }
  else {
      this.notify.info(this.l('SavedSuccessfully'));
      this.activeModal.close({ saving: this.saving });
  }
    
  }

  memoClick(event)
 {
    this.popup = true;
 }
  close(): void {    
    this.active = false;
    this.activeModal.close({ saving: this.saving });       
  }
  saveMemoValue(popValue)
  {
    this.popup = popValue;
    this.idmsConfigurationDto.mValue = this.popmValue
  }
}
