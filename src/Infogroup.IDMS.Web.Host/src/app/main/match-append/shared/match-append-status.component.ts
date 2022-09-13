import { Component, OnInit, Input, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MatchAppendsServiceProxy } from '@shared/service-proxies/service-proxies';


@Component({
    selector: 'match-append-history',
    templateUrl: './match-append-status.component.html',
    animations: [appModuleAnimation()]
})
export class MatchAppendStatusComponent extends AppComponentBase implements OnInit {
    @Input() matchAppendId: any;

    constructor(
       private _matchAppendServiceProxy: MatchAppendsServiceProxy,
        injector: Injector
        
    ) {
        super(injector);
    }

    ngOnInit() {
        this._matchAppendServiceProxy.getMatchAppendStatusHistory(this.matchAppendId).subscribe(
            result => {
                this.primengTableHelper.records = result;
            }
        );
    }

}
