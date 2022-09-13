import { FastCountRoutingModule } from './fast-count-routing.module';
import { FastCountComponent } from './fast-count/fast-count.component';

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
import { OrderListModule } from 'primeng/orderlist';
import { ScrollPanelModule } from 'primeng/scrollpanel';
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
import { AccordionModule } from 'primeng/accordion';
import { ToastModule } from 'primeng/toast';
import { ListboxModule } from 'primeng/listbox';
import { SelectValuesComponent } from './selectvalues/selectvalues.component';
import { FastCountDashboardComponent } from './fast-count-dashboard/fast-count-dashboard.component';
import { CreateOrEditSaveCountComponent } from './fast-count/create-or-edit-save-count.component';
import { FastCountHistoryComponent } from './fast-count-history/fast-count-history.component';
import { SuppressionComponent } from './fast-count/suppression/suppression.component';
import { CustomSuppressionComponent } from './fast-count/custom-suppression/custom-suppression.component';
import { FcSavecountComponent } from './fast-count/create-or-edit/fc-savecount/fc-savecount.component';
import { FcMaxperComponent } from './fast-count/create-or-edit/fc-maxper/fc-maxper.component';
import { FcCountreportComponent } from './fast-count/create-or-edit/fc-countreport/fc-countreport.component';
import { FcPlaceorderComponent } from './fast-count/create-or-edit/fc-placeorder/fc-placeorder.component';
import { FcGeographyComponent } from './fast-count/fc-geography/fc-geography.component';
import { UploadSuppressionComponent } from './fast-count/custom-suppression/upload-suppression/upload-suppression.component';
import { DragFileDirective } from '@shared/utils/drag-file.directive';
import { FcCategoryComponent } from './fast-count-dashboard/fc-category/fc-category.component';

@NgModule({
    declarations: [FastCountComponent, FastCountDashboardComponent, SelectValuesComponent, CreateOrEditSaveCountComponent, FastCountHistoryComponent, SuppressionComponent, CustomSuppressionComponent, FcSavecountComponent, FcMaxperComponent, FcCountreportComponent, FcPlaceorderComponent,UploadSuppressionComponent,DragFileDirective, FcGeographyComponent, FcCategoryComponent],
    imports: [
        CommonModule,
        FastCountRoutingModule,
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
        OrderListModule,
        ListboxModule,
        ScrollPanelModule,
        AccordionModule
    ],
    entryComponents: [SelectValuesComponent, CreateOrEditSaveCountComponent, SuppressionComponent, CustomSuppressionComponent, FcSavecountComponent, FcMaxperComponent, FcCountreportComponent, FcPlaceorderComponent, UploadSuppressionComponent, FcGeographyComponent, FcCategoryComponent]
})
export class FastCountModule {
}
