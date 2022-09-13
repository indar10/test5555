import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MasterLoLsServiceProxy, CreateOrEditMasterLoLDto, ActionType } from '@shared/service-proxies/service-proxies';
import { combineLatest } from 'rxjs';
import { finalize, tap } from 'rxjs/operators';

@Component({
  selector: 'app-create-or-edit-listoflist',
  templateUrl: './create-or-edit-listoflist.component.html',
  styleUrls: ['./create-or-edit-listoflist.component.css']
})
export class CreateOrEditListoflistComponent extends AppComponentBase {
  
  @Input() databaseId: number;
  @Input() ID: number;
  @Input() isLoading:boolean;
  @Input() checkIfEdit:string;
  
  ListId: number;
  dropdownForOwner: any = [];
  dropdownForManagers: any = [];
  mailers: any = [];
  valuesOfDropdownForProductCode: any = [];
  productCode: any = [];
  listType: any = [];
  decisionGroup: any = [];
  valuesOfDropdownForListType: any = [];
  valuesOfDropdownForDecisionGroup: any = [];
  owners: any;
  managers: any;
  sendOrderTo: any = [];
  DWAPApproval: boolean = true;
  listOfList: CreateOrEditMasterLoLDto = new CreateOrEditMasterLoLDto;
  ListTypes: any;
  ProductCodes: any;
  DecisionGroups: any;
  AvailableMailer: any = [];
  permissionType: boolean = false;
  dwapContacts: any;
  previousDwapContacts:any;
  availableToMailers: any;
  requestedByMailers: any;
  ifDwapEdit: boolean = false;
  ifReqMailerEdit: boolean = false;
  ifAvailableMailer: boolean = false;
  list: any;
  ifedit: any = false;
  isActive:boolean=true;
  saving = false;
  constructor(injector: Injector, private _masterLoLsServiceProxy: MasterLoLsServiceProxy, public activeModal: NgbActiveModal) { super(injector); }

  ngOnInit() {
    this.getAllDropdownData();
    this.getdropdownsFromLookup();
  }
  
  getAllDropdownData() {


    combineLatest([

      this._masterLoLsServiceProxy.getAllOwners(this.databaseId).pipe(tap(result => {})),
      this._masterLoLsServiceProxy.getAllManagers(this.databaseId).pipe(tap(result => {})),
      this._masterLoLsServiceProxy.getAllReqMailers(this.databaseId).pipe(tap(result => {})),
      this._masterLoLsServiceProxy.getAllAvailableMailersForDropdown(this.databaseId).pipe(tap(result => {})),
      
   ]).subscribe((resultArray) => {
     /* take the final action as all of the observables emitted at least 1 value */
     this.dropdownForOwner = resultArray[0];
     this.dropdownForManagers = resultArray[1];
     this.mailers = resultArray[2];
     this.AvailableMailer= resultArray[3];
     if(this.ID){
      this.Show(this.ID);
     }
   })
  }
  getdropdownsFromLookup() {
    this._masterLoLsServiceProxy.getallDropdownsfromLookup().subscribe(
      (result) => {
        this.productCode = result.items.filter(v => v.cLookupValue === 'PRODUCTCODE')
        this.listType = result.items.filter(v => v.cLookupValue === 'LISTTYPE')
        this.decisionGroup = result.items.filter(v => v.cLookupValue === 'DECISIONGROUP')

        this.valuesOfDropdownForProductCode = this.productCode.map(item => {
          return { label: item.cDescription, value: item.cCode }
        })

        this.valuesOfDropdownForListType = this.listType.map(item => {
          return { label: item.cDescription, value: item.cCode }
        })

        this.valuesOfDropdownForDecisionGroup = this.decisionGroup.map(item => {
          return { label: item.cDescription, value: item.cCode }
        })



      });
  }
  Show(Id?, listform?: NgForm) {
    if (Id) {
      this._masterLoLsServiceProxy.getListById(Id, this.databaseId).subscribe(result => {
        this.list = result;
        if(this.checkIfEdit=='Edit'){
          this.ifedit = true;
          this.listOfList.cListName = this.list.cListName;
          this.listOfList.cAppearDate=this.list.cAppearDate;
          this.listOfList.cRemoveDate=this.list.cRemoveDate;
          this.listOfList.cLastUpdateDate=this.list.cLastUpdateDate;
        }
        this.listOfList.ownerID = this.list.ownerID;  
        this.listOfList.cCode = this.list.cCode;
        this.listOfList.lK_PermissionType = this.list.lK_PermissionType;
        this.listOfList.cMINDatacardCode = this.list.cMINDatacardCode;
        this.listOfList.nBasePrice_Telemarketing = this.list.nBasePrice_Telemarketing;
        this.listOfList.nBasePrice_Postal = this.list.nBasePrice_Postal;
        this.listOfList.cNextMarkID = this.list.cNextMarkID;
        this.listOfList.iIsMultibuyer = this.list.iIsMultibuyer;
        this.listOfList.iDropDuplicates = this.list.iDropDuplicates;
        this.listOfList.iSendCASApproval = this.list.iSendCASApproval;
        this.DWAPApproval=this.listOfList.iSendCASApproval;
        this.listOfList.iIsNCOARequired = this.list.iIsNCOARequired;
        this.listOfList.iIsProfanityCheckRequired = this.list.iIsProfanityCheckRequired;
        this.listOfList.iIsActive = this.list.iIsActive;
        this.isActive=this.listOfList.iIsActive;
        this.listOfList.cCustomText1 = this.list.cCustomText1;
        this.listOfList.cCustomText2 = this.list.cCustomText2;
        this.listOfList.cCustomText3 = this.list.cCustomText3;
        this.listOfList.cCustomText4 = this.list.cCustomText4;
        this.listOfList.cCustomText5 = this.list.cCustomText5;
        this.listOfList.cCustomText6 = this.list.cCustomText6;
        this.listOfList.cCustomText7 = this.list.cCustomText7;
        this.listOfList.cCustomText8 = this.list.cCustomText8;
        this.listOfList.cCustomText9 = this.list.cCustomText9;
        this.listOfList.cCustomText10 = this.list.cCustomText10;
        this.listOfList.cCreatedBy = this.list.cCreatedBy;
        this.listOfList.dCreatedDate = this.list.dCreatedDate;
        this.listOfList.lK_ListType = this.list.lK_ListType;
        
       
        this.ChkPermissionType();
        //owner
        for (let index = 0; index < this.dropdownForOwner.length; index++) {

          if (this.dropdownForOwner[index].value === this.list.ownerID) {
            this.owners = this.dropdownForOwner[index].value
            
          }
        }
        //manager
        for (let index = 0; index < this.dropdownForManagers.length; index++) {

          if (this.dropdownForManagers[index].value === this.list.managerID) {
            this.managers = this.dropdownForManagers[index].value
          }

        }
        //sendOrderTo && DwapContacts
        this._masterLoLsServiceProxy.getAllContacts(this.list.ownerID, this.list.managerID).subscribe(res => {
          for (let index = 0; index < res.items.length; index++) {
            this.sendOrderTo[index] = { label: res.items[index].name, value: res.items[index].id }
          }
          this.previousDwapContacts=this.list.listofContacts.map(item => item.contactID);
          this.dwapContacts = this.list.listofContacts.map(item => item.contactID);
          this.listOfList.iOrderContactID = this.list.iOrderContactID
        })
        //ListType
        for (let index = 0; index < this.valuesOfDropdownForListType.length; index++) {
          if (this.valuesOfDropdownForListType[index].value === this.list.lK_ListType) {
            this.ListTypes = this.valuesOfDropdownForListType[index].value;
          }
        }
        //Product code
        for (let index = 0; index < this.valuesOfDropdownForProductCode.length; index++) {
          if (this.valuesOfDropdownForProductCode[index].value === this.list.lK_ProductCode) {
            this.ProductCodes = this.valuesOfDropdownForProductCode[index].value;
          }
        }
        //DecisionGroups
        for (let index = 0; index < this.valuesOfDropdownForDecisionGroup.length; index++) {
          if (this.valuesOfDropdownForDecisionGroup[index].value === this.list.lK_DecisionGroup) {
            this.DecisionGroups = this.valuesOfDropdownForDecisionGroup[index].value;
          }
        }
        this.requestedByMailers = this.list.listofReqMailer.map(item => item.mailerID);
        this.availableToMailers = this.list.listOfMailers.map(item => item.mailerID);
        this.isLoading=false;
      })
    }
  }
  ChkPermissionType(event?) {
    if (this.listOfList.lK_PermissionType == 'H' || this.listOfList.lK_PermissionType == 'P') {
      this.permissionType = true
      this.DWAPApproval = false;
      if(this.dwapContacts){
        this.dwapContacts=[];
      }
    }
    if (this.listOfList.lK_PermissionType === 'R') {
      this.permissionType = false;
      this.DWAPApproval = true
    }
  }
  onChangeDwapContacts(event?) {
    this.ifDwapEdit = true;
  }
  onChangeReqMailer(event?) {
    this.ifReqMailerEdit = true;
  }
  onChangeAvailableMailer(event?) {
    this.ifAvailableMailer = true;
  }
  OnDwapApproval(event?){
    if(this.checkIfEdit=='Edit' || this.checkIfEdit=='Copy'){
      if(event){
        this.dwapContacts=this.previousDwapContacts;
      }
      else{
        this.dwapContacts=[];
      }
    }
    

  }

  selectContactsByManger(event) {
    this.managers = event.value
    this._masterLoLsServiceProxy.getAllContacts(this.owners, this.managers).subscribe(
      result => {
        if (result.totalCount == 0) {
          this.sendOrderTo = [{ label: "No Records Available", value: "" }]
        }
        else {
          this.dwapContacts=[];
          this.sendOrderTo = result.items.map(item => {
            return { label: item.name, value: item.id }
          })
        }
      }
    )
  }
  selectContactsByOwner(event) {
    this.owners = event.value;
    this._masterLoLsServiceProxy.getAllContacts(this.owners, this.managers).subscribe(result => {
      if (result.totalCount == 0) {
        this.sendOrderTo = [{ label: "No Records Available", value: "" }]
      }
      else {
        this.dwapContacts=[];
        this.sendOrderTo = result.items.map(item => {
          return { label: item.name, value: item.id }
        })
      }

    })
  }
 
  save(listOfListForm: NgForm): void {
    if (this.ID && this.checkIfEdit === 'Edit') {
      this.listOfList.id = this.ID;
      this.ListId = this.ID;
    }
    this.listOfList.lK_ListType = this.ListTypes;
    this.listOfList.lK_ProductCode = this.ProductCodes;
    this.listOfList.lK_DecisionGroup = this.DecisionGroups;
    this.listOfList.ownerID = this.owners || 0;
    this.listOfList.managerID = this.managers;
    this.listOfList.iSendCASApproval = this.DWAPApproval;
    this.listOfList.databaseId = this.databaseId;
    this.listOfList.iIsActive=this.isActive;
    if (!this.ifedit) {
      this.message.confirm(
        this.l(""),
        this.l("Attention!! Base Price can not be changed once you save this List. Price changes can take place through DWAP forms only"),
        isConfirmed => {
          if (isConfirmed) {
            this.saveForm(listOfListForm);
          }
        })
    } else {
      this.saveForm(listOfListForm);
    }
  }
  saveForm(listform?: NgForm) {
    if(listform.dirty || this.checkIfEdit==='Copy'){
      this.saving=true;
      if(this.checkIfEdit==='Copy'){
        if(this.dwapContacts.length>0){
          this.ifDwapEdit=true;
        }
        if(this.requestedByMailers.length>0){
          this.ifReqMailerEdit=true;
        }
        if(this.availableToMailers.length>0){
          this.ifAvailableMailer=true;
        }
      }
    this._masterLoLsServiceProxy.createOrEdit(this.listOfList).pipe(finalize(() => { this.saving = false; })).subscribe(result => {
      this.ListId = result;
      if (this.dwapContacts && this.ifDwapEdit) {
        this._masterLoLsServiceProxy.createOrEditForDwapContacts(
          (this.dwapContacts), this.ListId).subscribe();
      }
      if (this.requestedByMailers && this.ifReqMailerEdit) {
        this._masterLoLsServiceProxy.createOrEditForRequestedMailer(
          (this.requestedByMailers), this.ListId).subscribe();
      }
      if (this.availableToMailers && this.ifAvailableMailer) {
        this._masterLoLsServiceProxy.createOrEditForAvailableMailer(
          (this.availableToMailers), this.ListId).subscribe();

      }
      this.notify.info(this.l('SavedSuccessfully'));
      this.activeModal.close({ saving: this.saving});
    });
  }
  else {
    this.notify.info(this.l('SavedSuccessfully'));
    this.activeModal.close({ saving: this.saving });
  }
}
  close(): void {
    this.activeModal.close({saving: this.saving});
  }
}
