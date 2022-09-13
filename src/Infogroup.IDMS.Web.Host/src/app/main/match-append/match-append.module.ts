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

import { PopoverModule } from "ngx-bootstrap";
import { SharedModule } from "../shared/shared.module";
import { AutoCompleteModule } from "primeng/autocomplete";
import { TreeModule } from "primeng/tree";
import { SelectButtonModule } from "primeng/selectbutton";
import { MatchAppendComponent } from "./match-append/match-append.component";
import { MatchAppendRoutingComponent } from "./match-append-routing.module";
import { StepsModule } from 'primeng/steps';
import { DialogModule } from 'primeng/dialog';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { CreateOrEditMatchAppendModalComponent } from "./match-append/create-or-edit-match-append-modal.component";
import { ToastModule } from 'primeng/toast';
import { MatchAppendStatusComponent } from "./shared/match-append-status.component";



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
        MatchAppendRoutingComponent,
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
        ToastModule
        
    ],
    declarations: [
        MatchAppendComponent,
        MatchAppendStatusComponent,
        CreateOrEditMatchAppendModalComponent
    ],
    providers: [
        NgbActiveModal,
        {
            provide: BsDatepickerConfig,
            useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig
        },
        {
            provide: BsDaterangepickerConfig,
            useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig
        },
        {
            provide: BsLocaleService,
            useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale
        }
    ],
    entryComponents: [
        CreateOrEditMatchAppendModalComponent
    ]
})
export class MatchAppendModule { }
