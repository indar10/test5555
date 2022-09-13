import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BatchProcessQueueComponent } from './batch-process-queue/batch-process-queue.component';
import { TaskComponent } from './tasks/task.component';
@NgModule({
    imports: [
        RouterModule.forChild([

            {
                path: '',
                children:[

                    { path: 'tasks', component: TaskComponent, data: { permission: 'Pages.IDMSTasks' } },

                    {path:'batch-process-queue',component: BatchProcessQueueComponent, data: {permission : 'Pages.BatchQueue'}}
                ]
            }
           
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class TasksRoutingComponent { }