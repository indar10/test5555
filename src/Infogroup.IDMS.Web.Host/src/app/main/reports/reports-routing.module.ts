import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ShippedReportComponent } from './shipped-reports/shippedreports.component';
import { SelectionfieldcountreportsComponent } from './selection-field-count-reports/selectionfieldcountreports.component';
@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'shipped-reports', component: ShippedReportComponent, data: { permission: 'Pages.Report.ShippedReports' } },
                    { path: 'selection-field-count-reports', component: SelectionfieldcountreportsComponent,data: { permission: 'Pages.Report.SelectionFieldCountReport'} }            

                    ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class ReportsRoutingComponent { }