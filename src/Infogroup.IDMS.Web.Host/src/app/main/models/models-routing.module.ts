import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ModelsComponent } from './models/models.component';
@NgModule({
    imports: [
        RouterModule.forChild([
            { path: 'models', component: ModelsComponent, data: { permission: 'Pages.Models' } },
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class ModelsRoutingComponent { }