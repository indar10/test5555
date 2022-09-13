import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DivisionalMailerComponent } from './divisional-mailer/divisional-mailer.component';
import { RedisCacheComponent } from './redis-cache/redis-cache.component';
import { DivisionShipTosComponent } from './divisionShipTos/divisionShipTos/divisionShipTos.component';
import { MaintainanceContainerComponent } from './maintainance-tab-container/maintainance-container.component';
import { OwnersComponent } from './owners/owners.component';
import { DatabasesComponent } from './databases/databases/databases.component';
import { MailersComponent } from './mailers/mailers/mailers.component';
import { BrokersComponent } from './brokers/brokers.component';
import { ManagersComponent } from './managers/managers/managers.component';
import { DecoyMailersComponent } from './decoys/decoy-mailers.component';
import { SecurityGroupsComponent } from './securityGroups/security-group-modal.component';
import { ListAutomateComponent } from './list-automate/list-automate/list-automate.component';
import { LookupsComponent } from './lookups/lookups.component';
import {IdmsConfigurationComponent} from './idms-configuration/idms-configuration.component';
import { ExternalDatabaseLinksComponent } from './external-database-links/external-database-links.component';
import { ProcessQueueComponent } from './process-queue/process-queue.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    {path:"external-database-links",component:ExternalDatabaseLinksComponent,data:{permission:'Pages.ExternalBuildTableDatabases'}},
                    { path: 'managers/managers', component: ManagersComponent, data: { permission: 'Pages.Managers' } },
                    { path: 'divisional-mailer', component: DivisionalMailerComponent, data: { permission: 'Pages.DivisionMailers' } },
                    { path: 'divisionShipTos/divisionShipTos', component: DivisionShipTosComponent, data: { permission: 'Pages.DivisionShipTos' } },
                    { path: 'databases/databases', component: DatabasesComponent, data: { permission: 'Pages.Databases' } },
                    { path: 'export-layout', component: MaintainanceContainerComponent, data: { permission: 'Pages.AdminExportLayouts' } },
                    { path: 'redis-cache', component: RedisCacheComponent, data: { permission: 'Pages.RedisCache' } },                   
                    { path: 'owners', component: OwnersComponent, data: { permission: 'Pages.Owners' } },                   
                    { path: 'mailers/mailers', component: MailersComponent, data: { permission: 'Pages.Mailers' } },
                    { path: 'brokers', component: BrokersComponent, data: { permission: 'Pages.Brokers' } },
                    { path: 'decoys', component: DecoyMailersComponent, data: { permission: 'Pages.Decoy' } },
                    { path: 'securityGroups', component: SecurityGroupsComponent, data: { permission: 'Pages.SecurityGroups' } },
                    { path: 'list-automate/list-automate',component: ListAutomateComponent, data: { permission: 'Pages.ListAutomates' } },
                    { path: 'lookups', component: LookupsComponent,data: { permission : 'Pages.Lookups'}},
                    { path: 'idms-configuration', component: IdmsConfigurationComponent,data: { permission : ''}},
                    {path:'process-queue',component:ProcessQueueComponent,data:{permission: 'Pages.ProcessQueues'}}
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MaintenanceRoutingModule { }
