import { Injectable } from '@angular/core';
import { CampaignAction } from './campaign-action.enum';
import { CampaignStatus } from './campaign-status.enum';

@Injectable({
    providedIn: 'root'
})
export class CampaignUiHelperService {

    private mapActionToStatus: any = {
        [CampaignAction.SubmitOrder]: [
            CampaignStatus.CampaignCreated,
            CampaignStatus.CampaignSubmitted,
            CampaignStatus.CampaignFailed,
            CampaignStatus.CampaignCompleted
        ],
        [CampaignAction.SubmitOutput]: [
            CampaignStatus.CampaignCompleted,
            CampaignStatus.OutputSubmitted],
        [CampaignAction.Ship]: [
            CampaignStatus.OutputCompleted],
        [CampaignAction.Reship]: [
            CampaignStatus.Shipped],
        [CampaignAction.CancelCampaign]: [
            CampaignStatus.Shipped,
            CampaignStatus.ShippingFailed],
        [CampaignAction.Delete]: [
            CampaignStatus.CampaignCreated,
            CampaignStatus.CampaignCompleted,
            CampaignStatus.ReadytoOutput],
        [CampaignAction.SaveSelection]: [
            CampaignStatus.CampaignCreated,
            CampaignStatus.CampaignCompleted,
            CampaignStatus.CampaignFailed,
            CampaignStatus.OutputCompleted,
            CampaignStatus.OutputFailed
        ],
        [CampaignAction.ScheduleCampaign]: [
            CampaignStatus.CampaignCreated,
            CampaignStatus.CampaignCompleted],
        [CampaignAction.OutputLayout]: [
            CampaignStatus.CampaignCreated,
            CampaignStatus.CampaignCompleted,
            CampaignStatus.OutputCompleted,
            CampaignStatus.OutputFailed,
            CampaignStatus.CampaignFailed],
        [CampaignAction.ResetShip]: [
            CampaignStatus.ShippingFailed
        ]

    };
    private statusTemp = [
        { statusDescription: '10: Count Created', id: 10 },
        { statusDescription: '20: Count Submitted', id: 20 },
        { statusDescription: '30: Count Running', id: 30 },
        { statusDescription: '40: Count Completed', id: 40 },
        { statusDescription: '50: Count Failed', id: 50 },
        { statusDescription: '60: Ready to Output', id: 60 },
        { statusDescription: '70: Output Submitted', id: 70 },
        { statusDescription: '80: Output Running', id: 80 },
        { statusDescription: '90: Output Completed', id: 90 },
        { statusDescription: '100: Output Failed', id: 100 },
        { statusDescription: '110: Approved for Shipping', id: 110 },
        { statusDescription: '120: Waiting to Ship', id: 120 },
        { statusDescription: '130: Shipped', id: 130 },
        { statusDescription: '140: Shipment Failed', id: 140 },
        { statusDescription: '150: Cancelled', id: 150 }
    ];

    shouldActionBeEnabled(action: CampaignAction, status: CampaignStatus) {
        return this.mapActionToStatus[action].includes(status);
    }

    getStatusValue() {
        return this.statusTemp;
    }
    getStatusDescription(id: number): string {
        return this.statusTemp.find(status => status.id == id).statusDescription;
    }
}