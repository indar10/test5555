import { DropdownOutputDto, AdvanceSelectionFields, Field } from "@shared/service-proxies/service-proxies";
import * as _ from 'lodash';
import { AdvanceSelection, Selection, AlertMessage } from "../../campaigns/shared/campaign-models";

export class CitySelectionBuilder {
    selectedState: string;
    selectedCity: string;
    configuredfields: AdvanceSelectionFields;
    selectedCounties: DropdownOutputDto[] = [];
    selectedCities: DropdownOutputDto[] = [];
    selectedNeighborhood: DropdownOutputDto[] = [];
    selections: AdvanceSelection[] = [];
    currentField: Field;
    states: DropdownOutputDto[] = [];
    clearAll(): void {
    }
    getSelectedField(): string {
        return this.selectedNeighborhood.length > 0 ? 'neighborhoodSelect' :
            this.selectedCities.length > 0 ? 'stateCitySelect' :
                this.selectedCounties.length > 0 ? 'stateCountySelect' :
                    'stateSelect';
    }
    validateConfiguration(fieldName: string): AlertMessage {
        let localizationKey = "";
        let success = typeof this.configuredfields !== 'undefined' && typeof this.configuredfields[fieldName] !== 'undefined';
        let params = [];
        if (fieldName.toUpperCase() == "NEIGHBORHOODSELECT") {
            localizationKey = 'NeighborhoodConfigException';
        }
        else {
            localizationKey = 'FieldNotConfigured1';
        }
        params.push(fieldName.toUpperCase());
        return { success, localizationKey, params };
    }

    add(): AlertMessage {
        let newRows: Selection[];
        let row: AdvanceSelection;
        if (this.selectedState === '' || this.selectedState == undefined) {
            return { success: false, localizationKey: 'StateNotSelected' };
        }
        let fieldName = this.getSelectedField();
        let validateConfig = this.validateConfiguration(fieldName);
        if (!validateConfig.success) return validateConfig;
        this.currentField = this.configuredfields[fieldName];
        let existingRow: AdvanceSelection = this.selections.find(sel => sel.field.cQuestionFieldName === this.currentField.cQuestionFieldName);
        if (existingRow && existingRow.field && !existingRow.field.cConfigFieldName) {
            existingRow.field.cConfigFieldName = this.currentField.cConfigFieldName;
        }
        let stateCode = this.selectedState.toUpperCase();
        let stateDescription = this.states.find(state => state.value === stateCode).label.toUpperCase();
        switch (this.currentField.cConfigFieldName) {
            case "STATECITYSELECT":
                newRows = this.selectedCities.map(item => {
                    return {
                        root: { code: `${stateCode}${item.value.toUpperCase()}`, description: `${stateDescription}:${item.label}` },
                        children: []
                    }
                });
                row = this.addFields(existingRow, newRows);
                break;
            case "STATECOUNTYSELECT":
                newRows = this.selectedCounties.map(item => {
                    return {
                        root: { code: `${stateCode}${item.value.toUpperCase()}`, description: `${stateDescription}:${item.label}` },
                        children: []
                    }
                });
                row = this.addFields(existingRow, newRows);
                break;
            case "NEIGHBORHOODSELECT":
                newRows = this.selectedNeighborhood.map(item => {
                    return {
                        root: { code: `${stateCode}${this.selectedCity.toUpperCase()}${item.value.toUpperCase()}`, description: `${stateDescription}${this.selectedCity.toUpperCase()}${item.label}` },
                        children: []
                    }
                });
                row = this.addFields(existingRow, newRows);
                break;
            default:
                newRows = [];
                newRows.push({ children: [], root: { code: stateCode, description: stateDescription } });
                row = this.addFields(existingRow, newRows);
                break;
        }
        row.generateValues();
        row.generateDescription();
        return { success: true };
    }
    addFields(existingRow: AdvanceSelection, selectedCodes: Selection[]) {
        if (existingRow) {
            let merged = [...existingRow.data, ...selectedCodes]
            existingRow.data = _.uniqBy(merged, 'root.code');
            return existingRow;
        }
        else {
            let row = new AdvanceSelection();
            row.field = this.currentField;
            row.data = _.uniqBy(selectedCodes, 'root.code');
            this.selections.push(row);
            return row;
        }
    }
}