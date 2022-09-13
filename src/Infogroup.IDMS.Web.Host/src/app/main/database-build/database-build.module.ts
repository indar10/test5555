import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListOfListComponent } from './list-of-list/list-of-list.component';
import {DatabaseBuildRoutingModule} from './database-build-routing.module';
import { BuildMaintenanceComponent } from './build-maintenance/build-maintenance.component';

import { FormsModule } from '@angular/forms';
import { FileUploadModule } from 'primeng/fileupload';
import { PaginatorModule } from 'primeng/paginator';
import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';

import {
    ModalModule,
    TabsModule,
    TooltipModule,
    BsDropdownModule
} from 'ngx-bootstrap';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RadioButtonModule } from 'primeng/radiobutton';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import {
    BsDatepickerModule
} from 'ngx-bootstrap/datepicker';
import { PopoverModule } from 'ngx-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { TreeModule } from 'primeng/tree';
import { SelectButtonModule } from 'primeng/selectbutton';

import { StepsModule } from 'primeng/steps';
import { DialogModule } from 'primeng/dialog';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ToastModule } from 'primeng/toast';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CreateOrEditListoflistComponent } from './list-of-list/create-or-edit-listoflist/create-or-edit-listoflist.component';



@NgModule({
  declarations: [ListOfListComponent, BuildMaintenanceComponent,CreateOrEditListoflistComponent],
  imports: [   
    DatabaseBuildRoutingModule,    
    CommonModule,        
        AutoCompleteModule,
        SelectButtonModule,
        MultiSelectModule,
        DropdownModule,
        CheckboxModule,
        FileUploadModule,
        PaginatorModule,
        CommonModule,
        FormsModule,
        ModalModule,
        TooltipModule,
        AppCommonModule,
        UtilsModule,
        CountoModule,
        TableModule,
        RadioButtonModule,
        NgbModule,
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        TabsModule.forRoot(),
        PopoverModule.forRoot(),
        SharedModule,
        TreeModule,
        StepsModule,
        DialogModule,
        OverlayPanelModule,
        ToastModule,
  ],entryComponents:[CreateOrEditListoflistComponent]
})
export class DatabaseBuildModule { }
