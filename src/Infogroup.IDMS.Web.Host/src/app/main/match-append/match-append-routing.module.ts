import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { MatchAppendComponent } from './match-append/match-append.component';
@NgModule({
    imports: [
        RouterModule.forChild([
            { path: 'match-append', component: MatchAppendComponent, data: { permission: 'Pages.MatchAppends' } },
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MatchAppendRoutingComponent { }