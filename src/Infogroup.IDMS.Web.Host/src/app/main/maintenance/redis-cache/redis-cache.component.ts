import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { RedisCachingServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    selector: 'app-redis-cache',
    templateUrl: './redis-cache.component.html',
    styleUrls: ['./redis-cache.component.css'],
    animations: [appModuleAnimation()]
})
export class RedisCacheComponent extends AppComponentBase implements OnInit {

    caches: any[];
    constructor(
        injector: Injector,
        private _redisCacheService: RedisCachingServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this._redisCacheService.getAll().subscribe(result => {
            this.caches = result;
        })
    }

    clearCache(cacheKey: string): void {
        this._redisCacheService.clearCache(cacheKey).subscribe(() => {
            var selectedCache = this.caches.find(item => item.value === cacheKey);
            selectedCache.count = "0";
            this.notify.success(this.l('CacheSuccessfullyCleared'));
        });
    }

    clearAllCaches(): void {
        this._redisCacheService.clearAllCaches().subscribe(() => {
            this._redisCacheService.getAll().subscribe(result => {
                this.caches = result;
                this.notify.success(this.l('AllCachesSuccessfullyCleared'));
            });
        });
    }
}