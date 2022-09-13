import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FileUploadModule } from 'primeng/fileupload';
import { PaginatorModule } from 'primeng/paginator';
import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';
import { ModalModule, TabsModule, TooltipModule, BsDropdownModule } from 'ngx-bootstrap';
import { NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { RadioButtonModule } from 'primeng/radiobutton';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { AccordionModule } from 'primeng/accordion';
import { PopoverModule } from 'ngx-bootstrap';
import { SharedRoutingModule } from './shared-routing.module';
import { CreateOrEditExportLayoutComponent } from './export-layout-add-fields/create-or-edit-export-layout.component';;
import { UploadLayoutModalComponent } from './export-layout-add-fields/upload-layout-modal/upload-layout-modal.component';
import { CreateOrEditCampaignModalComponent } from '../campaigns/campaigns/create-or-edit-campaign-modal.component';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { GeoMappingComponent } from './geo-mapping/geo-mapping.component';
import { GroupOrderListComponent } from './group-order-list/group-order-list.component';
import { FilterService } from './group-order-list/FilterService';;
import { PageLoaderComponent } from './page-loader/page-loader.component'
import { CitySelectionModalComponent } from './city-selection-modal/city-selection-modal.component';
import { SelectButtonModule } from "primeng/selectbutton";
import { SICSelectionModalComponent } from './sic-selection-modal/sic-selection-modal.component';
import { TreeModule } from "primeng/tree";

@NgModule({
  imports: [
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
    SharedRoutingModule,
    RadioButtonModule,
    NgbModule,
    BsDatepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    AccordionModule,
    PopoverModule.forRoot(),
    AutoCompleteModule,
    SelectButtonModule,
    TreeModule
  ],
  declarations: [
    CreateOrEditExportLayoutComponent,
    UploadLayoutModalComponent,
    CreateOrEditCampaignModalComponent,
    GeoMappingComponent,
    GroupOrderListComponent,
    PageLoaderComponent,
    CitySelectionModalComponent,
    SICSelectionModalComponent
  ],
  entryComponents: [
    UploadLayoutModalComponent,
    CreateOrEditCampaignModalComponent,
    GeoMappingComponent,
    GroupOrderListComponent,
    PageLoaderComponent,
    CitySelectionModalComponent,
    SICSelectionModalComponent
  ],
  exports: [
    CreateOrEditExportLayoutComponent,
    CreateOrEditCampaignModalComponent,
    GeoMappingComponent,
    GroupOrderListComponent,
    PageLoaderComponent,
    CitySelectionModalComponent,
    SICSelectionModalComponent
  ],
  providers: [
    NgbActiveModal,
    {
      provide: BsDatepickerConfig,
      useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig,
    },
    {
      provide: BsDaterangepickerConfig,
      useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig,
    },
    {
      provide: BsLocaleService,
      useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale,
    },
    FilterService
  ],
})
export class SharedModule { }