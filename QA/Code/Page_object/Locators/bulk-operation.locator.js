var BulkoperationLocator = function () {

    this.bulkOperation_splitButton = element(by.css('[title="Bulk Operations"]'));
    this.bulkOperation_actions = function (number) {
        var partialCSS1 = '[class="dropdown-menu show"]>button:nth-child(';
        var partialCSS2 = ')';
        return bulkOperation_actions = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.bulkOperation_editSegmentTitle = element(by.css('[role="document"] [class="modal-title"]>span'));
    this.bulkOperation_cancelButtonFooter = element(by.css('[role="document"] [class="btn btn-default"]'));
    this.bulkOperation_editSegmentGrid = function (number) {
        var partialCSS1 = '[id="AdvanceSelectionGrids"] thead>tr>th:nth-child(';
        var partialCSS2 = ')';
        return bulkOperation_editSegmentGrid = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.bulkOperation_fieldLabels = function (number) {
        var partialCSS1 = '[class="row ng-star-inserted"]>div:nth-child(';
        var partialCSS2 = ')>label';
        return bulkOperation_fieldLabels = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.bulkOperation_editSegmentSearchField = element(by.css('[placeholder="Search Segments..."]'));
    this.bulkOperation_editSegmentSearchButton = element(by.css('[class="search-container__input"] [type="submit"]'));
    this.bulkOperation_editSegmentToggleButton = element(by.css('[class="switch ng-star-inserted"]>span'));
    this.bulkOperation_inLineGrid = function (number) {
        var partialCSS1 = '[id="editable-grid"] [class="ui-table-thead"]>tr>th:nth-child(';
        var partialCSS2 = ')';
        return bulkOperation_inLineGrid = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.bulkOperation_editSegmentTotalCount = function (number) {
        var partialCSS1 = '[role="document"] [class="primeng-paging-container"]>span:nth-child(';
        var partialCSS2 = ')';
        return bulkOperation_editSegmentTotalCount = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_actionsButton = function (number) {
        var partialCSS1 = '[class="action-select"]>div:nth-child(';
        var partialCSS2 = ')>button';
        return selection_actionsButton = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_actions = function (number) {
        var partialCSS1 = '[class="dropdown open show"]>ul>li:nth-child(';
        var partialCSS2 = ')';
        return selection_actions = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_actionstitle = element(by.css('[class="ng-untouched ng-pristine ng-star-inserted ng-valid"] h1>span'));
    this.selection_addSegmentLabelsOneToThree = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>label';
        return selection_addSegmentLabelsOneToThree = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentLabelKeyCode1 = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(1)>label';
        return selection_addSegmentLabelsOneToThree = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentLabelKeyCode2 = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(2)>label';
        return selection_addSegmentLabelsOneToThree = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentLabelMaxPer = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(1)>label';
        return selection_addSegmentLabelsOneToThree = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentLabelOutputQty = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(2)>label';
        return selection_addSegmentLabelsOneToThree = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentLabelNetGroup = element(by.css('fieldset>div>div:nth-child(6)>div>label'));
    this.selection_addSegmentLabelRandomRadiusNth = element(by.css('[class="sliderl round"] span:nth-child(2)'));
    this.selection_addSegmentInputTwoToThree = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>input';
        return selection_addSegmentInputTwoToThree = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentInputKeyCode1 = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(1)>input';
        return selection_addSegmentInputKeyCode1 = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentInputKeyCode2 = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(2)>input';
        return selection_addSegmentInputKeyCode1 = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentDropdownMaxPer = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(1)>p-dropdown span';
        return selection_addSegmentDropdownMaxPer = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_editSegmentNetGroup = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ') input';
        return selection_editSegmentNetGroup = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_editSegmentOutputQty = function (number) {
        var partialCSS1 = 'fieldset>div>div:nth-child(';
        var partialCSS2 = ')>div>div:nth-child(2)>input';
        return selection_editSegmentNetGroup = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.selection_addSegmentDropdownMaxPerSearch = element(by.css('[class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.selection_addSegmentDropdownMaxPerElement = element(by.css('[class="ui-dropdown-items-wrapper"]>ul>p-dropdownitem:nth-child(1)>li'));
    this.selection_addSegmentInputNetGroup = element(by.css('fieldset>div>div:nth-child(6)>div>input'));
    this.selection_addSegmentCancelButton = element(by.css('[class="modal fade show d-block"] [class="modal-footer"]>button:nth-child(1)'));
    this.selection_addSegmentSaveButton = element(by.css('[class="modal fade show d-block"] [class="modal-footer"]>button:nth-child(2)'));
    this.bulkOperation_getInLineEditSegmentFirstRow = function (number) {
        var partialCSS1 = '[datakey="id"] [class="ui-table-scrollable-body"]>table tr:nth-child(1)>td:nth-child(';
        var partialCSS2 = ')>p-celleditor';
        return bulkOperation_getInLineEditSegmentFirstRow = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.bulkOperation_getInLineEditSegmentSecondRow = function (number) {
        var partialCSS1 = '[datakey="id"] [class="ui-table-scrollable-body"]>table tr:nth-child(2)>td:nth-child(';
        var partialCSS2 = ')>p-celleditor';
        return bulkOperation_getInLineEditSegmentSecondRow = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.bulkOperation_inputInLineEditSegmentSecondRow = function (number) {
        var partialCSS1 = '[datakey="id"] [class="ui-table-scrollable-body"]>table tr:nth-child(2)>td:nth-child(';
        var partialCSS2 = ')>p-celleditor input';
        return bulkOperation_inputInLineEditSegmentSecondRow = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.bulkOperation_selectMaxPerDropdownEditSegment = element(by.css('[datakey="id"] [class="ui-table-scrollable-body"]>table tr:nth-child(2)>td:nth-child(5)>p-celleditor>div'));
    this.bulkOperation_maxPerInLineEditSegmentSecondRow = element.all(by.css('[datakey="id"] [class="ui-table-scrollable-body"]>table tr:nth-child(2)>td:nth-child(5)>p-celleditor select>option'));
    this.bulkOperation_footerButtons = function (number) {
        var partialCSS1 = '[role="document"] [class="modal-footer"]>button:nth-child(';
        var partialCSS2 = ')';
        return bulkOperation_footerButtons = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.campaignList_clickOnDescription = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(3)>a'));
}
module.exports = BulkoperationLocator