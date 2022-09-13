import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FastCountComponent } from './fast-count/fast-count.component';
import { FastCountDashboardComponent } from './fast-count-dashboard/fast-count-dashboard.component';
import { FastCountHistoryComponent } from './fast-count-history/fast-count-history.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            { path: 'fast-count', component: FastCountComponent },
            { path: 'fast-count-dashboard', component: FastCountDashboardComponent },
            { path: 'fast-count-history', component: FastCountHistoryComponent }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class FastCountRoutingModule { }
