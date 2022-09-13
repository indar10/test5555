import {
  Field,
  DropdownOutputDto,
  AdvanceSelectionFields
} from "@shared/service-proxies/service-proxies";
import { TreeNode } from "primeng/api";
import * as _ from "lodash";
import { AdvanceSelection, FieldData, Selection, AlertMessage } from '@app/main/campaigns/shared/campaign-models';

export class SICSelectionBuilder {
  selectedindustries: DropdownOutputDto[] = [];
  selectedfranchises: DropdownOutputDto[] = [];
  selectedNodes: TreeNode[];
  selections: AdvanceSelection[] = [];
  selectedLength: number = 6;
  selectedSICType = "A";
  selectedField: Field;
  configuredfields: AdvanceSelectionFields;
  sicFields = {
    6: "sicCode",
    4: "minorIndustryGroup",
    2: "majorIndustryGroup",
    5: "franchiseBySIC",
    3: "industrySpecificBySIC",
    1: "primarySICFlag"
  };
  clearAll(): void {
    this.selectedindustries = [];
    this.selectedfranchises = [];
    this.selectedNodes = [];
  }
  getSelectedField(): string {
    return this.selectedfranchises.length > 0
      ? this.sicFields[5]
      : this.selectedindustries.length > 0
        ? this.sicFields[3]
        : this.sicFields[this.selectedLength];
  }
  validateConfiguration(fieldName: string): AlertMessage {
    let success =
      typeof this.configuredfields !== "undefined" &&
      typeof this.configuredfields[fieldName] !== "undefined";
    let params = [];
    let localizationKey = "FieldNotConfigured1";
    params.push(fieldName.toUpperCase());
    if (
      this.selectedSICType !== "A" &&
      typeof this.configuredfields.primarySICFlag === "undefined"
    ) {
      localizationKey = "FieldNotConfigured2";
      params.push("Primary SIC Flag");
    }
    return { success, localizationKey, params };
  }
  add(): AlertMessage {
    let row: AdvanceSelection;
    let selectedSICCodes = this.selectedNodes.filter(node => node.key !== "0");
    if (selectedSICCodes.length === 0) {
      return { success: false, localizationKey: "NoSICSelected" };
    }
    if (
      this.selectedfranchises.length > 0 &&
      this.selectedindustries.length > 0
    ) {
      return { success: false, localizationKey: "BothFNISelected" };
    }
    let fieldName = this.getSelectedField();
    let validateConfig = this.validateConfiguration(fieldName);
    if (!validateConfig.success) return validateConfig;
    this.selectedField = this.configuredfields[fieldName];
    let existingRow: AdvanceSelection = this.selections.find(
      sel =>
        sel.field.cQuestionFieldName === this.selectedField.cQuestionFieldName
    );
    if (existingRow && existingRow.field && !existingRow.field.cConfigFieldName) {
      existingRow.field.cConfigFieldName = this.selectedField.cConfigFieldName;
    }
    switch (this.selectedField.cConfigFieldName) {
      case "FRANCHISEBYSIC":
        let franchises: FieldData[] = this.selectedfranchises.map(fr => {
          return { code: fr.value, description: fr.label };
        });
        row = this.addFranchiseIndustry(existingRow, franchises);
        break;
      case "INDUSTRYSPECIFICBYSIC":
        let industries: FieldData[] = this.selectedindustries.map(fr => {
          return { code: fr.value, description: fr.label };
        });
        row = this.addFranchiseIndustry(existingRow, industries);
        break;
      default:
        row = this.addSICFields(existingRow, false);
        break;
    }
    row.generateValues();
    row.generateDescription();
    return { success: true };
  }
  addSICFields(existingRow: AdvanceSelection, isSmartAdd: boolean, smartSelections?: DropdownOutputDto[]) {
    let selectedSICCodes: Selection[];
    if (isSmartAdd) {
      selectedSICCodes = smartSelections.map(node => {
        return {
          root: {
            code: node.value,
            description: node.label
          },
          children: []
        };
      });
    }
    else {
      selectedSICCodes = this.selectedNodes
        .filter(node => node.key !== "0")
        .map(node => {
          return {
            root: {
              code: node.data.cSICCode,
              description: node.data.cDescription
            },
            children: []
          };
        });
    }
    if (existingRow) {
      let merged = [...existingRow.data, ...selectedSICCodes];
      existingRow.data = _.uniqBy(merged, "root.code");
      return existingRow;
    } else {
      let row = new AdvanceSelection();
      row.field = this.selectedField;
      row.data = _.uniqBy(selectedSICCodes, "root.code");
      this.selections.push(row);
      return row;
    }
  }
  addFranchiseIndustry(
    existingRow: AdvanceSelection,
    newChildren: FieldData[]
  ) {
    let selectedSICCodes = this.selectedNodes.filter(node => node.key !== "0");
    let sicDetail: FieldData = {
      code: selectedSICCodes[0].data.cSICCode,
      description: selectedSICCodes[0].data.cDescription
    };
    let isExisting = typeof existingRow !== "undefined";
    let row: AdvanceSelection;
    if (isExisting) {
      row = existingRow;
      let existingSIC: Selection = isExisting
        ? existingRow.data.find(sic => sic.root.code === sicDetail.code)
        : undefined;
      if (existingSIC) {
        let children = [...existingSIC.children, ...newChildren];
        existingSIC.children = _.uniqBy(children, "code");
      } else {
        let newSIC: Selection = {
          root: sicDetail,
          children: _.uniqBy(newChildren, "code")
        };
        row.data.push(newSIC);
      }
      return row;
    } else {
      row = new AdvanceSelection();
      let newSIC: Selection = {
        root: sicDetail,
        children: _.uniqBy(newChildren, "code")
      };
      row.data.push(newSIC);
      row.field = this.selectedField;
      this.selections.push(row);
      return row;
    }
  }
  smartAdd(smartSelections: DropdownOutputDto[], iType: string): AlertMessage {
    if (smartSelections.length > 0) {
      let row: AdvanceSelection;
      let fieldName = this.sicFields[iType];
      let validateConfig = this.validateConfiguration(fieldName);
      if (!validateConfig.success) return validateConfig;
      this.selectedField = this.configuredfields[fieldName];
      let existingRow: AdvanceSelection = this.selections
        .find(sel => sel.field.cQuestionFieldName === this.selectedField.cQuestionFieldName);
      row = this.addSICFields(existingRow, true, smartSelections);
      row.generateValues();
      row.generateDescription();
      return { success: true };
    }
  }
}
