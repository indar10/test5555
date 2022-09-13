var newCampaignpageLocator = function () {
    //New Campaign pop up screen
    this.newCampaign_NDMGeneralTabLabel = element(by.css('[class="modal-body typeName  level xfield yfield"] ul>li span'));
    this.newCampaign_databaseToBuildLabels = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>label';
        return newCampaign_databaseToBuildLabels = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_channelLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>div:nth-child(1)>div>label';
        return newCampaign_channelLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_typeLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>div:nth-child(2)>div>label';
        return newCampaign_typeLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_postalLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>div:nth-child(1) p-radiobutton:nth-child(1)>label';
        return newCampaign_postalLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_telemarketingLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>div:nth-child(1) p-radiobutton:nth-child(2)>label';
        return newCampaign_telemarketingLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_emailLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>div:nth-child(1) p-radiobutton:nth-child(3)>label';
        return newCampaign_telemarketingLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_netLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>div:nth-child(2) p-radiobutton:nth-child(1)>label';
        return newCampaign_netLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_grossLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ')>div:nth-child(2) p-radiobutton:nth-child(2)>label';
        return newCampaign_grossLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_maxPerFieldLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ') label';
        return newCampaign_maxPerFieldLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_maxPerQuantityLabel = function (number) {
        var partialCSS1 = '[heading="General"] fieldset>div:nth-child(';
        var partialCSS2 = ') div>div:nth-child(2)>label';
        return newCampaign_maxPerQuantityLabel = element(by.css(partialCSS1 + number + partialCSS2));
    };
    this.newCampaign_customerSearch = element(by.css('[name="mailer"] [role="combobox"]'));
    this.newCampaign_customerField = element(by.css('[name="mailer"] input'));
    this.newCampaign_customerDropdown = element(by.css('[name="mailer"] [class="ui-button-icon-left ui-clickable pi pi-caret-down"]'));
    this.newCampaign_brokerSearch = element(by.css('[name="broker"] [role="combobox"]'));
    this.newCampaign_brokerField = element(by.css('[name="broker"] input'));
    this.newCampaign_offerField = element(by.css('[name="offerDD"] label'));
    this.newCampaign_offerDropdownButton = element(by.css('[name="offerDD"] [class="ui-dropdown-trigger-icon ui-clickable pi pi-chevron-down"]'));
    this.newCampaign_dropdownList = element.all(by.css('p-dropdownitem [role="option"] span'));
    this.newCampaign_buildDropdown = element(by.css('[name="buildID"] [class="ui-dropdown-trigger-icon ui-clickable pi pi-chevron-down"]'));
    this.newCampaign_buildDropdownSearch = element(by.css('[class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.newCampaign_channel_postalButton = element(by.css('[heading="General"] fieldset>div:nth-child(7)>div:nth-child(1) p-radiobutton:nth-child(1) span'));
    this.newCampaign_type_netButton = element(by.css('[label=" Net"] [class="ui-radiobutton-icon ui-clickable pi pi-circle-on"]'));

    this.newCampaign_offerDropdown_nonDivisional = element(by.css('[name="offerDD"] [class="ui-dropdown-trigger-icon ui-clickable pi pi-chevron-down"]'));
    this.newCampaign_SearchField_nonDivisional = element(by.css('[class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.newCampaign_buildDropdown_nonDivisional = element(by.css('[name="buildID"] [class="ui-dropdown-trigger-icon ui-clickable pi pi-chevron-down"]'));
    this.newCampaign_channel_postalButton_nonDivisional = element(by.css('[label="Postal"] [class="ui-radiobutton-icon ui-clickable pi pi-circle-on"]'));
    this.newCampaign_type_netButton_nonDivisional = element(by.css('[label=" Net"] [class="ui-radiobutton-icon ui-clickable pi pi-circle-on"]'));

    //Campaign list screen
    this.campaignList_allTableLabels = function (number) {
        var partialCSS1 = '[class="ui-table-scrollable-header-table"] tr>th:nth-child(';
        var partialCSS2 = ')';
        return campaignList_allTableLabels = element(by.css(partialCSS1 + number + partialCSS2));
    };


};
module.exports = newCampaignpageLocator;