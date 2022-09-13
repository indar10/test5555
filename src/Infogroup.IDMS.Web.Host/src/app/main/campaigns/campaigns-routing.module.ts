import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CampaignContainerComponent } from './campaigns-tab-container/campaign-container.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            { path: 'campaigns' , component: CampaignContainerComponent, data: { permission: 'Pages.Campaigns' } },
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class CampaignsRoutingModule { }
