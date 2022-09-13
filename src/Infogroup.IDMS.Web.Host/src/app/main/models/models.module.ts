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
import { ModelsComponent } from "./models/models.component";
import { ModelsRoutingComponent } from "./models-routing.module";
import { SharedModule } from "../shared/shared.module";
import { AutoCompleteModule } from "primeng/autocomplete";
import { TreeModule } from "primeng/tree";
import { SelectButtonModule } from "primeng/selectbutton";
import { CreateOrEditModelModalComponent } from "./models/create-or-edit-models-modal.component";
import { ModelStatusComponent } from "./shared/model-status/model-status.component";


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
        ModelsRoutingComponent,
        RadioButtonModule,
        NgbModule,
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        TabsModule.forRoot(),
        PopoverModule.forRoot(),
        SharedModule,
        TreeModule
    ],
    declarations: [
        ModelsComponent,
        CreateOrEditModelModalComponent,
        ModelStatusComponent
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
        CreateOrEditModelModalComponent
    ]
})
export class ModelsModule { }
