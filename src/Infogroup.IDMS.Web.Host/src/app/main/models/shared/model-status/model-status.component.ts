import { Component, OnInit, Input, Injector } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModelsServiceProxy } from '@shared/service-proxies/service-proxies';


@Component({
    selector: 'popover',
    templateUrl: './model-status.component.html',
    animations: [appModuleAnimation()]
})
export class ModelStatusComponent extends AppComponentBase implements OnInit {
    @Input() modelScoringId: any;

    constructor(
        private _modelsServiceProxy: ModelsServiceProxy,
        injector: Injector,
        private modalService: NgbModal
    ) {
        super(injector);
    }

    ngOnInit() {

        this._modelsServiceProxy.getStatusHistory(this.modelScoringId).subscribe(
            result => {
                this.primengTableHelper.records = result;
            }
        );
    }

}
