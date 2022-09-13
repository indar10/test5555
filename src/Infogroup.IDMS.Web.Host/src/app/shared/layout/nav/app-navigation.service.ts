import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { AppSessionService } from '@shared/common/session/app-session.service';

import { Injectable } from '@angular/core';
import { AppMenu } from './app-menu';
import { AppMenuItem } from './app-menu-item';

@Injectable()
export class AppNavigationService {

    constructor(
        private _permissionCheckerService: PermissionCheckerService,
        private _appSessionService: AppSessionService
    ) {

    }

    getMenu(): AppMenu {
        return new AppMenu('MainMenu', 'MainMenu', [
            new AppMenuItem('Dashboard', 'Pages.Dashboard', 'flaticon-line-graph', '/app/main/dashboard'),

            new AppMenuItem('Campaigns', 'Pages.Campaigns', 'fad fa-abacus', '/app/main/campaigns/campaigns'),
            new AppMenuItem('Fast Count', 'Pages.FastCount', 'flaticon-suitcase', '', [
                new AppMenuItem('New Search', 'Pages.FastCount.NewSearch', 'far fa-search', '/app/main/fast-count/fast-count-dashboard'),
                new AppMenuItem('History', 'Pages.FastCount.History', 'far fa-history', '/app/main/fast-count/fast-count-history'),
                // new AppMenuItem('Suppressions', '', '', ''),
            ]),
            new AppMenuItem('Database Build', 'Pages.DatabaseBuild', 'fa fa-database', '', [
                 new AppMenuItem('List Of Lists', 'Pages.DatabaseBuild.ListOfList', 'fa fa-list', '/app/main/database-build/list-of-list'),
                //  new AppMenuItem('Build Maintainance', 'Pages.DatabaseBuild.BuildMaintenance', 'flaticon-lock', '/app/main/database-build/build-maintenance')
            ]),
            new AppMenuItem('Models', 'Pages.Models', 'fas fa-diagnoses', '/app/main/models/models'),
            new AppMenuItem('Reports', 'Pages.Report', 'fas fa-analytics', '', [
                new AppMenuItem('Shipped Campaigns', 'Pages.Report.ShippedReports', 'flaticon-map', '/app/main/reports/shipped-reports'),
                new AppMenuItem('Selection Field Count', 'Pages.Report.SelectionFieldCountReport', 'fal fa-calculator', '/app/main/reports/selection-field-count-reports')
            ]),
            new AppMenuItem('Match & Append', 'Pages.MatchAppends', 'fas fa-file-signature', '/app/main/match-append/match-append'),
            new AppMenuItem('Tasks', 'Pages.IDMSTasks', 'far fa-tasks', '/app/main/tasks/tasks'),

            new AppMenuItem('Setup', 'Pages.Maintenance', 'flaticon-settings-1', '', [
                new AppMenuItem('DivisionMailers', 'Pages.DivisionMailers', 'fas fa-address-card', '/app/main/maintenance/divisional-mailer'),
                new AppMenuItem('DivisionShipTos', 'Pages.DivisionShipTos', 'far fa-person-dolly', '/app/main/maintenance/divisionShipTos/divisionShipTos'),
                new AppMenuItem('Databases', 'Pages.Databases', 'fas fa-database', '/app/main/maintenance/databases/databases'),
                new AppMenuItem('Output Layout', 'Pages.AdminExportLayouts', 'fad fa-inbox-out', '/app/main/maintenance/export-layout'),
                new AppMenuItem('RedisCache', 'Pages.RedisCache', 'flaticon-lock', '/app/main/maintenance/redis-cache'),
                new AppMenuItem('Owners', 'Pages.Owners', 'fas fa-user-tie', '/app/main/maintenance/owners'),
                new AppMenuItem('Mailers', 'Pages.Mailers', 'fas fa-address-card', '/app/main/maintenance/mailers/mailers'),
                new AppMenuItem('Brokers', 'Pages.Brokers', 'fas fa-handshake-alt-slash', '/app/main/maintenance/brokers'),
                new AppMenuItem('Managers', 'Pages.Managers', 'fas fa-user-tie', '/app/main/maintenance/managers/managers'),
                new AppMenuItem('Seeds', 'Pages.Decoy', 'fas fa-users', '/app/main/maintenance/decoys'),
                new AppMenuItem('Security Groups', 'Pages.SecurityGroups', 'fas fa-key', '/app/main/maintenance/securityGroups'),
                new AppMenuItem('ListAutomates', 'Pages.ListAutomates', 'fa fa-list', '/app/main/maintenance/list-automate/list-automate'),
                new AppMenuItem('Lookups', 'Pages.Lookups', 'fas fa-clipboard', '/app/main/maintenance/lookups'),
                new AppMenuItem('IDMSConfiguration', 'Pages.IDMSConfiguration', 'fa fa-cogs', '/app/main/maintenance/idms-configuration'),
                new AppMenuItem('External Database Links', 'Pages.ExternalBuildTableDatabases', 'fa fa-link', '/app/main/maintenance/external-database-links'),
                new AppMenuItem('Process Queue', 'Pages.ProcessQueues', 'fas fa-tasks-alt', '/app/main/maintenance/process-queue')
            
            ]),
            new AppMenuItem('Tenants', 'Pages.Tenants', 'flaticon-list-3', '/app/admin/tenants'),
            new AppMenuItem('Editions', 'Pages.Editions', 'flaticon-app', '/app/admin/editions'),
            
             new AppMenuItem('Administration', '', 'flaticon-interface-8', '', [
                new AppMenuItem('OrganizationUnits', 'Pages.Administration.OrganizationUnits', 'flaticon-map', '/app/admin/organization-units'),
                new AppMenuItem('Roles', 'Pages.Administration.Roles', 'flaticon-suitcase', '/app/admin/roles'),
                new AppMenuItem('Users', 'Pages.Administration.Users', 'fas fa-user-secret', '/app/admin/users'),
                new AppMenuItem('Languages', 'Pages.Administration.Languages', 'flaticon-tabs', '/app/admin/languages'),
                new AppMenuItem('AuditLogs', 'Pages.Administration.AuditLogs', 'flaticon-folder-1', '/app/admin/auditLogs'),
                new AppMenuItem('Maintenance', 'Pages.Administration.Host.Maintenance', 'flaticon-lock', '/app/admin/maintenance'),
                new AppMenuItem('Subscription', 'Pages.Administration.Tenant.SubscriptionManagement', 'flaticon-refresh', '/app/admin/subscription-management'),
                new AppMenuItem('VisualSettings', 'Pages.Administration.UiCustomization', 'flaticon-medical', '/app/admin/ui-customization'),
                new AppMenuItem('Settings', 'Pages.Administration.Host.Settings', 'flaticon-settings', '/app/admin/hostSettings'),
                new AppMenuItem('Settings', 'Pages.Administration.Tenant.Settings', 'flaticon-settings', '/app/admin/tenantSettings'),          
                 new AppMenuItem('Other Access', 'Pages.Administration.otherAccess','flaticon-user-ok', '/app/admin/dashboard-access')
            ])
        ]);
    }

    checkChildMenuItemPermission(menuItem): boolean {

        for (let i = 0; i < menuItem.items.length; i++) {
            let subMenuItem = menuItem.items[i];

            if (subMenuItem.permissionName === '' || subMenuItem.permissionName === null || subMenuItem.permissionName && this._permissionCheckerService.isGranted(subMenuItem.permissionName)) {
                return true;
            } else if (subMenuItem.items && subMenuItem.items.length) {
                return this.checkChildMenuItemPermission(subMenuItem);
            }
        }

        return false;
    }

    showMenuItem(menuItem: AppMenuItem): boolean {
        if (menuItem.permissionName === 'Pages.Administration.Tenant.SubscriptionManagement' && this._appSessionService.tenant && !this._appSessionService.tenant.edition) {
            return false;
        }

        let hideMenuItem = false;

        if (menuItem.requiresAuthentication && !this._appSessionService.user) {
            hideMenuItem = true;
        }

        if (menuItem.permissionName && !this._permissionCheckerService.isGranted(menuItem.permissionName)) {
            hideMenuItem = true;
        }

        if (this._appSessionService.tenant || !abp.multiTenancy.ignoreFeatureCheckForHostUsers) {
            if (menuItem.hasFeatureDependency() && !menuItem.featureDependencySatisfied()) {
                hideMenuItem = true;
            }
        }

        if (!hideMenuItem && menuItem.items && menuItem.items.length) {
            return this.checkChildMenuItemPermission(menuItem);
        }

        return !hideMenuItem;
    }
}
