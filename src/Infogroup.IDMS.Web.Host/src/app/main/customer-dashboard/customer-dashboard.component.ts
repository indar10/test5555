import {Component, Injector, OnInit} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/common/app-component-base';
import {
    ReportsServiceProxy,
    GetReportForViewDto,
    DropdownOutputDto,
    ReportDto
} from '@shared/service-proxies/service-proxies';
import * as pbi from 'powerbi-client';

@Component({
    templateUrl: './customer-dashboard.component.html',
    styleUrls: ['./customer-dashboard.component.css'],
    animations: [appModuleAnimation()]
})
export class CustomerDashboardComponent extends AppComponentBase implements OnInit {
    embedData: GetReportForViewDto[];
    currentReport: GetReportForViewDto;
    accessToken: string;
    reports: ReportDto[];
    reportOptions: DropdownOutputDto[];
    selectedReportId: number;
    isLoading: boolean;
    selectedMode = 'view';
    embedReport: any;
    selectedReport: ReportDto;

    ngOnInit(): void {
        this.isLoading = true;
        this._reportService.getReports().subscribe(
            (data: GetReportForViewDto) => {
                this.accessToken = data.accessToken;
                this.reports = data.reports;
                this.reportOptions = data.reportOptions;
                this.selectedReportId = data.selectedReport;
                this.isLoading = false;
                this.showReport();
            }, (data: any) => {
                this.isLoading = false;
            });
    }

    constructor(
        injector: Injector,
        private _reportService: ReportsServiceProxy
    ) {
        super(injector);
    }

    switchReportMode(): void {
        this.embedReport.switchMode(this.selectedMode);
    }

    showReport(): void {
        // tslint:disable-next-line:triple-equals
        this.selectedReport = this.reports.find(report => report.id == this.selectedReportId);
        let embedUrl = `https://app.powerbi.com/reportEmbed?reportId=${this.selectedReport.cReportID}&groupId=${this.selectedReport.cReportWorkSpaceID}&config=${this.selectedReport.cReportConfig}`;
        let reportContainer = <HTMLElement>document.getElementById('embedContainer');
        let models = pbi.models;
        let config: pbi.IEmbedConfiguration = {
            type: 'report',
            tokenType: models.TokenType.Aad,
            accessToken: this.accessToken,
            embedUrl,
            id: this.selectedReport.cReportID,
            viewMode: this.selectedMode === 'edit' ? models.ViewMode.Edit : models.ViewMode.View,
            permissions: models.Permissions.All,
            settings: {
                filterPaneEnabled: true,
                navContentPaneEnabled: true,
                layoutType: models.LayoutType.Custom,
                customLayout: {
                    displayOption: models.DisplayOption.FitToWidth
                }
            }
        };
        let powerbi = new pbi.service.Service(pbi.factories.hpmFactory, pbi.factories.wpmpFactory, pbi.factories.routerFactory);
        // Embed the report and display it within the div container.
        powerbi.reset(reportContainer);
        this.embedReport = powerbi.embed(reportContainer, config);
    }

}




