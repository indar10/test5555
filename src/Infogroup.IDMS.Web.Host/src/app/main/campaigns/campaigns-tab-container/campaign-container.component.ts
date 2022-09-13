import { Component, ViewChild, ViewChildren, QueryList } from "@angular/core";
import { TabsetComponent } from "ngx-bootstrap";
import { CampaignsComponent } from "../campaigns/campaigns.component";
import { SelectionDetails, EditSegmentCompleteResult } from "../shared/campaign-models";
import { SelectionComponent } from "../campaigns/selection.component";
import { SelectionAction } from "../shared/selection-action.enum";

@Component({
  selector: "campaign-container",
  templateUrl: "./campaign-container.component.html",
  styleUrls: ["./campaign-container.component.css"]
})
export class CampaignContainerComponent {
  @ViewChild(CampaignsComponent, { static: true })
  campaigns: CampaignsComponent;
  @ViewChild(TabsetComponent, { static: true }) tabset: TabsetComponent;
  @ViewChildren(SelectionComponent) selectionTabComponents: QueryList<SelectionComponent>;
  selectionActionType = SelectionAction;
  tabs: any[] = [];
  tabsExportLayout: any[] = [];
  removeTabHandler(tab: any): void {
    this.tabs.splice(this.tabs.indexOf(tab), 1);
  }
  removeTabHandlerExportLayout(tab: any): void {
    this.tabsExportLayout.splice(this.tabsExportLayout.indexOf(tab), 1);
  }
  existingTab = (campaignId: number) =>
    this.tabs.find(tab => tab.campaignId == campaignId);
  existingTabExportLayout = (campaignId: number) =>
    this.tabsExportLayout.filter(tab => tab.campaignId == campaignId);

  openEditTab(inputTab) {
    let existingTab = this.existingTab(inputTab.campaignId);
    if (existingTab) {
      existingTab.active = true;
      if (this.selectionTabComponents) {
        let targetComponent = this.selectionTabComponents.find(selectionTab => selectionTab.campaignId == inputTab.campaignId);
        if (targetComponent) {
          targetComponent.previousSegmentId = targetComponent.selectedSegment;
          targetComponent.selectedSegment = inputTab.segmentId;
          targetComponent.segmentId = inputTab.segmentId;
          targetComponent.onSegmentChange(SelectionAction.SegmentChange);
        }
      }
    } else {
      this.tabs.push(inputTab);
    }
  }

  openEditExportLayout(tab) {
    let tabsExportLayout = this.existingTabExportLayout(tab.campaignId);
    if (tabsExportLayout.length > 0) {
      tabsExportLayout[0].active = true;
    } else {
      this.tabsExportLayout.push(tab);
    }
  }

  openSelectionTab(data: SelectionDetails) {
    let description: string = data.campaignDescription;
    this.openEditTab({
      title: `${data.campaignId}` + " - " + `${description.substring(0, 25)}`,
      disabled: false,
      removable: true,
      active: true,
      ...data
    });
  }

  onEditExportLayout(data: any) {
    this.openEditExportLayout({
      title: "Layout - " + data.layoutName,
      data: data,
      disabled: false,
      removable: true,
      active: true,
      campaignId: data.campaignId,
      layoutName: data.layoutName,
      isCampaign: data.isCampaign,
      currentStatus: data.currentStatus
    });
  }

  onReloadCampaign() {
    this.campaigns.reloadPage();
  }
  onCancelSelection() {
    this.closeActiveTab();
  }

  onCancelSelectionExportLayout() {
    const activeTab = this.tabset.tabs.filter(tab => tab.active);
    this.tabsExportLayout.splice(this.tabsExportLayout.indexOf(activeTab), 1);
    this.tabset.tabs[0].active = true;
  }
  
  onExcuteCampaign(): void {
    this.campaigns.reloadPage();
    this.closeActiveTab();
  }
  closeActiveTab() {
    const activeTab = this.tabset.tabs.filter(tab => tab.active);
    this.tabs.splice(this.tabs.indexOf(activeTab), 1);
    this.tabset.tabs[0].active = true;
  }

  onFavouriteChanged(): void {
    this.campaigns.loadFavouriteCampaigns();
  }
  updateCampaign(event: any): void {
    let existingTab = this.existingTab(event.id);
    if (existingTab)
      existingTab.title = `${existingTab.campaignId}` + " - " + `${event.description.substring(0, 25)}`;
    this.campaigns.refreshCampaignById(event.id);
  }
  refreshSegment(segmentData: EditSegmentCompleteResult) {
    this.campaigns.refreshSegment(segmentData)
  }
}
