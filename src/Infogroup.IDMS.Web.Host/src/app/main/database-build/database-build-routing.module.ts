import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {ListOfListComponent} from './list-of-list/list-of-list.component';
import {BuildMaintenanceComponent} from './build-maintenance/build-maintenance.component';



@NgModule({
   imports: [
    RouterModule.forChild([
        { path: 'list-of-list', component: ListOfListComponent }, 
        { path: 'build-maintenance', component: BuildMaintenanceComponent }       
    ])
],
exports: [
    RouterModule
]
})
export class DatabaseBuildRoutingModule { }
