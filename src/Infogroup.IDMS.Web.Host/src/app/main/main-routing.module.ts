import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CustomerDashboardComponent } from './customer-dashboard/customer-dashboard.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                   
                    {
                        path: 'fast-count',
                        loadChildren: () => import('app/main/fast-count/fast-count.module').then(m => m.FastCountModule), //Lazy load main module
                        data: { permission: '' }

                    },
                    {
                        path: 'database-build',
                        loadChildren: () => import('app/main/database-build/database-build.module').then(m => m.DatabaseBuildModule), //Lazy load main module
                        data: { preload: true, permission: '' }
                    },
                    {
                        path: 'fast-count/fast-count-dashboard',
                        loadChildren: () => import('app/main/fast-count/fast-count.module').then(m => m.FastCountModule), //Lazy load main module
                        data: { permission: '' }

                    },
                    {
                        path: 'campaigns',
                        loadChildren: () => import('app/main/campaigns/campaigns.module').then(m => m.CampaignsModule), //Lazy load main module
                        data: { preload: true, permission: 'Pages.Campaigns' }
                    },
                    {
                        path: 'models',
                        loadChildren: () => import('app/main/models/models.module').then(m => m.ModelsModule), //Lazy load main module
                        data: { preload: true, permission: 'Pages.Models' }
                    },
                    {
                        path: 'reports',
                        loadChildren: () => import('app/main/reports/reports.module').then(m => m.ReportsModule), //Lazy load main module
                        data: { preload: true, permission: 'Pages.Report' }
                    },
                    { path: 'dashboard', component: CustomerDashboardComponent, data: { permission: 'Pages.Dashboard' } },

                    {
                        path: 'maintenance',
                        loadChildren: () => import('app/main/maintenance/maintenance.module').then(m => m.MaintenanceModule), //Lazy load main module
                        data: { preload: true, permission: 'Pages.Maintenance' }
                    },
                    {
                        path: 'match-append',
                        loadChildren: () => import('app/main/match-append/match-append.module').then(m => m.MatchAppendModule), //Lazy load main module
                        data: { preload: true, permission: 'Pages.MatchAppends' }
                    },
                    {
                        path: 'tasks',
                        loadChildren: () => import('app/main/tasks/tasks.module').then(m => m.TasksModule), //Lazy load main module
                        data: { preload: true, permission: 'Pages.IDMSTasks' }
                    }

                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }
