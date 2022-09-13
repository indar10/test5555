import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AppSessionService } from '@shared/common/session/app-session.service';

@Injectable()
export class AccountRouteGuard implements CanActivate {

    constructor(
        private _permissionChecker: PermissionCheckerService,
        private _router: Router,
        private _sessionService: AppSessionService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (route.queryParams['ss'] && route.queryParams['ss'] === 'true') {
            return true;
        }

        if (this._sessionService.user) {
            this._router.navigate([this.selectBestRoute()]);
            return false;
        }

        return true;
    }

    selectBestRoute(): string {
        if (this._permissionChecker.isGranted('Pages.Dashboard')) {
            return '/app/main/dashboard';
        }

        if (this._permissionChecker.isGranted('Pages.Campaigns')) {
            return '/app/main/campaigns/campaigns';
        }

        if (this._permissionChecker.isGranted('Pages.FastCount.History')) {
            return '/app/main/fast-count/fast-count-history';
        }

        //if (this._permissionChecker.isGranted('Pages.Administration.Host.Dashboard')) {
        //    return '/app/admin/hostDashboard';
        //}

        //if (this._permissionChecker.isGranted('Pages.Tenant.Dashboard')) {
        //    return '/app/main/dashboard';
        //}

        return '/app/notifications';
    }
}
