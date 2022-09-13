import { Field, GetCampaignsListForView } from "@shared/service-proxies/service-proxies";
import * as _ from 'lodash';

export interface NewStatusInfo {
    campaignId: number;
    status: number;
}
export interface SaveResult {
    isSave: boolean;
}
export interface MoveSegment {
    isMove: boolean;
}

export interface CopySegment {
    isCopy: boolean;
    segmentId: number;
}

export interface SaveSegment extends SaveResult {
    segmentId: number;
}
export interface CreateOrEditResult {
    id: number;
    newStatus: number;
    isEdit: boolean;
    isSave: boolean;
}
export interface CreateOrEditCampaignResult extends CreateOrEditResult {
    description: string;
}

export interface EditSegmentCompleteResult {
    segmentId: number;
    campaignId: number;
    newStatus: number;
}

export interface AlertMessage {
    success: boolean;
    localizationKey?: string;
    params?: string[];
}

export interface FieldData {
    code: string;
    description: string;
}
export interface Selection {
    root: FieldData;
    children?: FieldData[];
}
export interface SelectionDetails {
    buildId: number;
    build: number; // tblBuild.cBuild
    campaignId: number;
    databaseId: number;
    divisionId: number;
    mailerId: number;
    segmentId: number;
    campaignDescription: string;
    splitType: number;
    databaseName: string;
}
export class AdvanceSelection {
    data: Selection[] = [];
    field: Field;
    description: string;
    value: string;
    generateValues(): void {
        let finalArray = [];
        switch (this.field.cConfigFieldName) {
            case "FRANCHISEBYSIC":
            case "INDUSTRYSPECIFICBYSIC":
                this.data.forEach(sic => {
                    var SIC_Code = sic.root.code;
                    var tempSICCodes = sic.children.map(child => {
                        return SIC_Code + child.code;
                    });
                    finalArray = finalArray.concat(tempSICCodes);
                });
                break;
            case "SICCODE":
            case "MINORINDUSTRYGROUP":
            case "MAJORINDUSTRYGROUP":
            case "STATESELECT":
            case "STATECOUNTYSELECT":
            case "STATECITYSELECT":
            case "NEIGHBORHOODSELECT":
                finalArray = this.data.map(sic => sic.root.code);
                break;
            default:
                break;
        }
        this.value = _.uniq(finalArray.toString().split(',')).join(',');
    }
    generateDescription(): void {
        let finalDescription = "";
        switch (this.field.cConfigFieldName) {
            case "FRANCHISEBYSIC":
            case "INDUSTRYSPECIFICBYSIC":
                this.data.forEach(sic => {
                    var SIC_Description = sic.root.description;
                    var sicDesArray = sic.children.map(child => child.description);
                    finalDescription +=
                        SIC_Description + ":" + sicDesArray.toString() + ",";
                });
                this.description = finalDescription.slice(0, -1);
                break;
            case "SICCODE":
            case "MINORINDUSTRYGROUP":
            case "MAJORINDUSTRYGROUP":
            case "STATESELECT":
            case "STATECOUNTYSELECT":
            case "STATECITYSELECT":
            case "NEIGHBORHOODSELECT":
                this.description = this.data
                    .map(sic => sic.root.description)
                    .toString();
                this.description = _.uniq(this.description.split(',')).join(',');
                break;
            default:
                break;
        }
    }
}
