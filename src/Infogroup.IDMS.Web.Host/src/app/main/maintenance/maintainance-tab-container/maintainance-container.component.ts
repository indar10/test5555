import { Component, ViewChild } from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap';
import { ExportLayoutComponent } from '../export-layout/export-layout.component';


@Component({
    selector: 'maintainance-container',
    templateUrl: './maintainance-container.component.html',
    styleUrls: ['./maintainance-container.component.css']
})
export class MaintainanceContainerComponent {
    @ViewChild(ExportLayoutComponent, { static: true }) exportlayout: ExportLayoutComponent;
    @ViewChild(TabsetComponent, { static: true }) tabset: TabsetComponent;
    tabs: any[] = [];
    tabsExportLayout: any[] = [];
    removeTabHandler(tab: any): void {
        this.tabs.splice(this.tabs.indexOf(tab), 1);
    }
    removeTabHandlerExportLayout(tab: any): void {
        this.tabsExportLayout.splice(this.tabsExportLayout.indexOf(tab), 1);
    }
    existingTab = (campaignID: number) => this.tabs.filter(tab => tab.campaignID == campaignID);
    existingTabExportLayout = (campaignID: number) => this.tabsExportLayout.filter(tab => tab.campaignID == campaignID);

   

    
    openEditExportLayout(tab) {
        
        let tabsExportLayout = this.existingTabExportLayout(tab.exportLayoutId);
        if (tabsExportLayout.length > 0) {
            tabsExportLayout[0].active = true;
        }
        else {
            this.tabsExportLayout.push(tab)
        }
    }
    
    
    closeActiveTab() {
        const activeTab = this.tabset.tabs.filter(tab => tab.active);
        this.tabs.splice(this.tabs.indexOf(activeTab), 1);
        this.tabset.tabs[0].active = true;
    }

    onEditMaintainanceExportLayout(data: any) {
        
        this.openEditExportLayout({
            title: 'Layout - ' + data.layoutName,
            data: data,
            disabled: false,
            removable: true,
            active: true,
            exportLayoutId: data.exportLayoutId,
            maintainanceBuildId: data.maintainanceBuildId,
            isCampaign: data.isCampaign,
            databaseId: data.databaseId,
            layoutName: data.layoutName

        });
    }


}
