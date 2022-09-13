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
import { ReportsRoutingComponent } from "./reports-routing.module";
import { AutoCompleteModule } from "primeng/autocomplete";
import { TreeModule } from "primeng/tree";
import { SelectButtonModule } from "primeng/selectbutton";
import { ShippedReportComponent } from "./shipped-reports/shippedreports.component";
import { SelectionfieldcountreportsComponent } from './selection-field-count-reports/selectionfieldcountreports.component';;
import { OrderDetailComponent } from './order-detail/order-detail.component'


@NgModule({
    imports: [
        AutoCompleteModule,
        SelectButtonModule,
        MultiSelectModule,
        ReportsRoutingComponent,
        DropdownModule,
        CheckboxModule,
        FileUploadModule,
        PaginatorModule,
        CommonModule,
        FormsModule,
        TooltipModule,
        AppCommonModule,
        UtilsModule,
        CountoModule,
        TableModule,
        RadioButtonModule,
        NgbModule,
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot(),
        TreeModule
    ],
    declarations: [
        ShippedReportComponent,
        SelectionfieldcountreportsComponent,
        OrderDetailComponent
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
        
    ]
})
export class ReportsModule { }
