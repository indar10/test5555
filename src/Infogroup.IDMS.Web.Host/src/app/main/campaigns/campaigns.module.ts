import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { FileUploadModule } from "primeng/fileupload";
import { PaginatorModule } from "primeng/paginator";
import { UtilsModule } from "@shared/utils/utils.module";
import { CountoModule } from "angular2-counto";
import { DropdownModule } from "primeng/dropdown";
import { MultiSelectModule } from "primeng/multiselect";
import { CheckboxModule } from "primeng/checkbox";
import { TableModule } from "primeng/table";
import {
  ModalModule,
  TabsModule,
  TooltipModule,
  BsDropdownModule
} from "ngx-bootstrap";
import { NgbModule, NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { RadioButtonModule } from "primeng/radiobutton";
import { AppCommonModule } from "@app/shared/common/app-common.module";
import {
  BsDatepickerModule,
  BsDatepickerConfig,
  BsDaterangepickerConfig,
  BsLocaleService
} from "ngx-bootstrap/datepicker";
import { NgxBootstrapDatePickerConfigService } from "assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service";
import { CampaignsComponent } from "./campaigns/campaigns.component";
import { SegmentsComponent } from "./segments/segments.component";
import { PreviousSegmentsComponent } from "./segments/previous-segments.component";
import { CampaignContainerComponent } from "./campaigns-tab-container/campaign-container.component";
import { CampaignsRoutingModule } from "./campaigns-routing.module";
import { CreateOrEditSegmentModalComponent } from "./segments/create-or-edit-segment-modal.component";
import { CreateOrEditCampaignModalComponent } from "./campaigns/create-or-edit-campaign-modal.component";
import { AccordionModule } from "primeng/accordion";
import { SourcesModalComponent } from "./campaigns/sources-modal.component";
import { PreviousOrdersModalComponent } from "./campaigns/previousOrders-modal.component";
import { CampaignStatusComponent } from "./shared/campaign-status/campaign-status.component";
import { PopoverModule } from "ngx-bootstrap";
import { CampaignsQueueComponent } from "./campaigns-queue/campaigns-queue.component";
import { SharedModule } from "../shared/shared.module";
import { CreateOrEditExportLayoutComponent } from "../shared/export-layout-add-fields/create-or-edit-export-layout.component";
import { AutoCompleteModule } from "primeng/autocomplete";
import { TreeModule } from "primeng/tree";
import { SelectButtonModule } from "primeng/selectbutton";
import { MoveSegmentComponent } from "./segments/move-segments.component";
import { SelectionComponent } from "./campaigns/selection.component";
import { SegmentDataPreviewComponent } from "./segments/segments-datapreview.compnent";
import { CopySegmentComponent } from "./segments/copy-segments.component";
import { SavedSelectionComponent } from "./campaigns/saved-selection/saved-selection.component";
import { SavedSelectionDetailComponent } from "./campaigns/saved-selection/saved-selection-detail.component";
import { BulkSegmentModalComponent } from "./campaigns/bulk-segment-upload-modal.component";
import { MultiFieldSelectionModalComponent } from "./campaigns/multiField-selection-modal.component";
import { GeoSearchModalComponent } from "./campaigns/geo-search/geo-search-modal.component";
import { CopyCampaignComponent } from "./campaigns/copy-campaign.component";
import { GlobalChangesModalComponent } from "./campaigns/global-changes/global-changes-modal.component";
import { AddToFavoritesComponent } from "./campaigns/add-to-favorites/add-to-favorites-modal.component";
import { SubsetsModalComponent } from "./campaigns/subsets/subsets-modal.component";
import { ReshipCampaignModalComponent } from "./campaigns/reship-campaign/reship-campaign-modal.component";
import { ScheduleCampaignComponent } from "./campaigns/schedule-campaign/schedule-campaign-modal.component";
import { CalendarModule } from 'primeng/calendar';
import { InputMaskModule } from 'primeng/inputmask';
import { ImportSegmentModalComponent } from "./segments/import-segment/import-segment.component";
import { OccupationSelectionModalComponent } from "./campaigns/occupation-selection/occupation-selection-modal.component";
import { StepsModule } from "primeng/steps";
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { PanelModule } from 'primeng/panel';
import { FieldsetModule } from 'primeng/fieldset';
import { BatchEditSegmentComponent } from './campaigns/global-changes/batch-edit-segment/batch-edit-segment.component';

@NgModule({
  imports: [
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
    CampaignsRoutingModule,
    RadioButtonModule,
    NgbModule,
    BsDatepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    AccordionModule,
    PopoverModule.forRoot(),
    SharedModule,
    TreeModule,
    CalendarModule,
    InputMaskModule,
    StepsModule,
    BlockUIModule,
    ProgressSpinnerModule,
    PanelModule,
    FieldsetModule,
  ],
  declarations: [
    CampaignsComponent,
    SegmentsComponent,
    PreviousSegmentsComponent,
    CampaignContainerComponent,
    SelectionComponent,
    CreateOrEditSegmentModalComponent,
    SourcesModalComponent,
    PreviousOrdersModalComponent,
    CampaignStatusComponent,
    CampaignsQueueComponent,
    CampaignStatusComponent,
    CampaignsQueueComponent,
    MoveSegmentComponent,
    SegmentDataPreviewComponent,
    SegmentDataPreviewComponent,
    SavedSelectionComponent,
    SavedSelectionDetailComponent,
    CopySegmentComponent,
    MultiFieldSelectionModalComponent,
    GeoSearchModalComponent,
    BulkSegmentModalComponent,
    CopyCampaignComponent,
    GlobalChangesModalComponent,
    AddToFavoritesComponent,
    SubsetsModalComponent,
    ReshipCampaignModalComponent,
    ScheduleCampaignComponent,
    ImportSegmentModalComponent,
    OccupationSelectionModalComponent,
    BatchEditSegmentComponent,
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
  ],
  entryComponents: [
    SelectionComponent,
    CreateOrEditSegmentModalComponent,
    CreateOrEditCampaignModalComponent,
    SourcesModalComponent,
    PreviousOrdersModalComponent,
    CreateOrEditExportLayoutComponent,
    MoveSegmentComponent,
    SegmentDataPreviewComponent,
    SavedSelectionComponent,
    CopySegmentComponent,
    BulkSegmentModalComponent,
    SegmentDataPreviewComponent,
    MultiFieldSelectionModalComponent,
    GeoSearchModalComponent,
    CopyCampaignComponent,
    GlobalChangesModalComponent,
    AddToFavoritesComponent,
    SubsetsModalComponent,
    ReshipCampaignModalComponent,
    ScheduleCampaignComponent,
    ImportSegmentModalComponent,
    OccupationSelectionModalComponent,
  ],
})
export class CampaignsModule {}
