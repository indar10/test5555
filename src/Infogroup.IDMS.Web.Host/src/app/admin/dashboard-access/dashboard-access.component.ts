import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {IDMSUsersServiceProxy,GetMatchAppendDatabaseUserForViewDto,DatabasesServiceProxy,CreateOrEditMatchAppendDatabaseUserDto,MatchAppendDatabaseUsersServiceProxy,GetUserReportForViewDto,ReportsServiceProxy,UserReportsServiceProxy, ReportDto, CreateOrEditUserReportDto, GetIDMSUserForViewDto, DropdownOutputDto} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { NgForm } from '@angular/forms';
import { includes, result } from 'lodash';
import { elementEventFullName } from '@angular/compiler/src/view_compiler/view_compiler';

@Component({
  selector: 'app-dashboard-access',
  templateUrl: './dashboard-access.component.html',
  styleUrls: ['./dashboard-access.component.css']
})
export class DashboardAccessComponent extends AppComponentBase implements OnInit {
  isLoading: boolean = true;
  users:any = [];
  usersDropdownResult : any = [];
  selectedUsers: any = {};
  selectedMatchAppendUsers: any = {};
  report:ReportDto[] = [];
  selectedReports : ReportDto[] = [];
  obje: CreateOrEditUserReportDto = new CreateOrEditUserReportDto();
  userReports : GetUserReportForViewDto[];
  selectedUserReports: any = {}
  cFirstName = '';
  databaseList : DropdownOutputDto[] = [];
  selectedDatabase:  DropdownOutputDto[] = [];
  existingDatabaseList: GetMatchAppendDatabaseUserForViewDto[] = [];
  constructor(injector:Injector,private _userServiceProxy: IDMSUsersServiceProxy,
    private _reportService: ReportsServiceProxy,private _userReportService : UserReportsServiceProxy,
    private _databaseServiceProxy: DatabasesServiceProxy,private _matchAppendDatabaseUsersServiceProxy: MatchAppendDatabaseUsersServiceProxy) { 
    super(injector);
  }

  ngOnInit(): void {
  this.getUsersDropDown();
  this.searchReports(null);
    this.getDatabaseList();
    
}

getUsersDropDown() {
  this.isLoading = true;
  this._userServiceProxy
    .getAll('','',0, 1000)
    .pipe(finalize(() => this.isLoading = false))
    .subscribe((result) => (
      this.usersDropdownResult = result.items,
      result.items.forEach(ele => {
        this.users.push({'label':ele.cFirstName.concat(" ",ele.cLastName),'value':ele.id});
      })
    )); 
}
searchReports(event){
  this.isLoading = true;
  this._reportService.getAllForReportsDropdown()
  .pipe(finalize(() => this.isLoading = false))
  .subscribe(result => {
    this.report = result;
  })
 
}
getAllUserReport(tblUsercFirstNameFilter)
{
  this.isLoading = true;
  this._userReportService.getAll('',undefined,undefined,undefined,undefined,undefined,tblUsercFirstNameFilter,'','',0,1000).
  subscribe(result => {
    this.userReports = result.items;
    this.userReports.forEach(ele =>{
      this.report.forEach(reportItem => {
        if(reportItem.id === ele.userReport.reportID)
        {
          this.selectedReports.push(reportItem);
         
        }

      })
    })
    this.selectedUserReports = null;
  })
  this.isLoading = false;
}
getSelectedReports(event)
{
  this.selectedReports = [];
  this.selectedUserReports = []
 this.usersDropdownResult.forEach(element => {
   if(element.id === this.selectedUsers)
   {
     this.getAllUserReport(element.cFirstName)
     this.cFirstName = element.cFirstName;
      return;
   }
   
 });
  
}
removeReport(id)
{
  var existingReport = this.selectedReports.filter(ele => ele.id === id)
  var indexOfReport = this.selectedReports.indexOf(existingReport[0]);
  this.selectedReports.splice(indexOfReport,1);
}
getDatabaseList(DBID?: number) {
  this._databaseServiceProxy.getDatabasesForUser().subscribe(result => {
     this.databaseList = result;
  });
}
save(userReportForm:NgForm)
{
  if(this.userReports.length > 0)
  {
    this.isLoading = true;
        this.userReports.forEach(ele => {
           this._userReportService.delete(ele.userReport.id).
            subscribe(result => {
              var indexOfReport = this.userReports.indexOf(ele);
              if(this.selectedReports === undefined || this.selectedReports == null || this.selectedReports.length === 0)
              {
                this.notify.info(this.l('SavedSuccessfully'));
              }
              if(indexOfReport === 0)
              {
                var obj = new CreateOrEditUserReportDto()
                this.selectedReports.forEach(element => {
                  obj.userID = this.selectedUsers;
                  obj.reportID = element.id;
                  this._userReportService.createOrEdit(obj)
                  .subscribe((response) => {
                    var indOfReport = this.selectedReports.indexOf(element);
                  if(indOfReport === 0)
                  {
                     this.notify.info(this.l('SavedSuccessfully'));
                     this.getSelectedReports(null);
                  }
                })
                });
                
              }
              
            })
          
    })
    this.isLoading = false;
  }
  else{
    var obj = new CreateOrEditUserReportDto()
    this.isLoading = true;
    this.selectedReports.forEach(element => {
      obj.userID = this.selectedUsers;
      obj.reportID = element.id;
      this._userReportService.createOrEdit(obj)
      .subscribe((response) => {
        var indOfReport = this.selectedReports.indexOf(element);
        if(indOfReport === 0)
        {
          this.notify.info(this.l('SavedSuccessfully'));
        }
    });
    
  });

}
this.isLoading = false;
}
removeDatabase(databaseID)
{
  var existingDatabase = this.selectedDatabase.filter(ele => ele.value === databaseID)
  var indexOfDB = this.selectedDatabase.indexOf(existingDatabase[0]);
  this.selectedDatabase.splice(indexOfDB,1);
}
getExistingDatabaseForUser(userId)
{
  this.selectedDatabase = [];
  this.isLoading = true;
  this._matchAppendDatabaseUsersServiceProxy.getAll(userId,'',0,1000).
    subscribe(result => {
      this.isLoading = false;
      this.existingDatabaseList = result.items;
      result.items.forEach(element => {
        this.databaseList.forEach(item => {
          if(item.value === element.matchAppendDatabaseUser.databaseID)
          {
            this.selectedDatabase.push(item);
          }
        });
      
      });
    })
}
saveMatchAppendAccess(matchAppendForm : NgForm)
{
  var matchAppendObj = new CreateOrEditMatchAppendDatabaseUserDto();
  matchAppendObj.id = null;
  this.isLoading = true;
  if(this.existingDatabaseList.length > 0){
  this.existingDatabaseList.forEach(item =>{
        this._matchAppendDatabaseUsersServiceProxy.delete(item.matchAppendDatabaseUser.id).
        subscribe(result =>{
            if(this.selectedDatabase.length > 0){
            this.selectedDatabase.forEach(element => {
              matchAppendObj.databaseID = element.value;
              matchAppendObj.userID = this.selectedMatchAppendUsers;
              var indexOfExistingDB = this.existingDatabaseList.indexOf(item);
              if(indexOfExistingDB === 0){
              this._matchAppendDatabaseUsersServiceProxy.createOrEdit(matchAppendObj).
              subscribe(result => {
                var indexOfDB = this.selectedDatabase.indexOf(element);
                this.isLoading = false;
                if(indexOfDB === 0){
                  this.getExistingDatabaseForUser(this.selectedMatchAppendUsers); 
                this.notify.info(this.l('SavedSuccessfully'));
               
              }
              })
            }
            })
          }
          else{
            this.getExistingDatabaseForUser(this.selectedMatchAppendUsers);
            this.notify.info(this.l('SavedSuccessfully'));
            this.isLoading = false;
          }
        });
  })
}
else{
        this.selectedDatabase.forEach(element => {
          matchAppendObj.databaseID = element.value;
          matchAppendObj.userID = this.selectedMatchAppendUsers;
          this._matchAppendDatabaseUsersServiceProxy.createOrEdit(matchAppendObj).
              subscribe(result => {
                var indexOfDB = this.selectedDatabase.indexOf(element);
                
                if(indexOfDB === 0){
                this.isLoading = false;
                this.getExistingDatabaseForUser(this.selectedMatchAppendUsers);
                this.notify.info(this.l('SavedSuccessfully'));
                  
              }
              })
        });
}

}

}
