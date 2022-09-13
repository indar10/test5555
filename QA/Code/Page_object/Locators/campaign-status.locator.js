var CampaignstatuspageLocator = function () {

    //Campaign list screen   
    //this.campaignList_actionDropdown_table = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(7) button'));
    this.campaignList_campaignActionDropdownList = element(by.css('[class="dropdown open show"]'));
    this.campaignList_campaignActionDropdownExecute = element(by.css('[class="dropdown open show"]>ul li:nth-child(6)'));
    this.campaignList_campaignActionDropdownOutput = element(by.css('[class="dropdown open show"]>ul li:nth-child(5)'));
    this.campaignList_campaignActionDropdownShip = element(by.css('[class="dropdown open show"]>ul li:nth-child(6)'));
    this.campaignList_campaignStatusAsPerId = element(by.xpath("//td[contains(text(),'645425')]//following-sibling::td[3]//span[@id='statusHover']"));

    //Campaign selection screen
    this.campaignFilter_selectSegmentLabel = element(by.css('[class="horizontal-form ng-valid ng-touched ng-dirty"] [class="segment-label"]>label'));
    this.campaignFilter_selectSegmentDropdown = element(by.css('[id="segmentDropdown"]>div>label'));
    this.campaignFilter_selectSegmentSearchField = element(by.css('[class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.campaignFilter_selectionsLabel = element(by.css('[class="horizontal-form ng-valid ng-touched ng-dirty"] [class="modal-body"] [class="form-group"]>div>div:nth-child(1)>div:nth-child(1)'));
    this.campaignFilter_statusCountLabel = element(by.css('[class="horizontal-form ng-valid ng-touched ng-dirty"] [class="modal-body"] [class="form-group"]>div>div:nth-child(1)>div:nth-child(2)'));
    this.campaignFilter_sectionsFilterDropdownButtonFirst = element(by.xpath("//div[@class='rules-list']//div[@class='rules-list']//div[1]//div[contains(@class,'chosen-container')]"));
    this.campaignFilter_sectionsFilterDropdownButtonSecond = element(by.xpath("//div[@class='rules-list']//div[@class='rules-list']//div[2]//div[contains(@class,'chosen-container')]"));
    this.campaignFilter_sectionsFilterSearchFieldFirst = element(by.xpath("//div[@class='rules-list']//div[@class='rules-list']//div[1]//div[contains(@class,'rule-filter-container')]//input[@class='chosen-search-input']"));
    this.campaignFilter_sectionsFilterSearchFieldSecond = element(by.xpath("//div[@class='rules-list']//div[@class='rules-list']//div[2]//div[contains(@class,'rule-filter-container')]//input[@class='chosen-search-input']"));
    this.campaignFilter_sectionsFilterDropdownFirstElement = element(by.xpath("//div[@class='rules-list']//div[@class='rules-list']//div[1]//div[contains(@class,'chosen-container')]//ul/li[2]"));
    this.campaignFilter_sectionsFilterDropdownSecondElement = element(by.xpath("//div[@class='rules-list']//div[@class='rules-list']//div[2]//div[contains(@class,'chosen-container')]//ul//li[2]"));
    this.campaignFilter_sectionsOptionsCheckBoxFirstLabel = element(by.css('[class="rules-list"] [class="rules-list"]>div:nth-child(1)>[class="rule-value-container col align-self-center"] label:nth-child(1)'));
    this.campaignFilter_sectionsOptionsCheckBoxSecondLabel = element(by.css('[class="rules-list"] [class="rules-list"]>div:nth-child(2)>[class="rule-value-container col align-self-center"] label:nth-child(1)'));
    this.campaignFilter_sectionsOptionsCheckBoxFirst = element(by.xpath("//input[contains(@value,'V|||VERIFIED RECORD')]"));
    this.campaignFilter_sectionsOptionsCheckBoxSecond = element(by.xpath("//input[contains(@value,'Y|||Infogroup Contact & when it is not available the contact fields will be blank')]"));
    this.campaignFilter_sectionsAddRuleButton = element(by.css('[class="btn btn-xs btn-success-addrule"]'));
    this.campaignFilter_segmentSaveButton = element(by.css('[class="btn btn-primary blue parse-sql"]'));

    //Edit campaign screen
    //Output Tab : Format 
    this.editCampaign_formatLabelOutputTab = element(by.css('[header="Format"] span:nth-child(2)'));
    this.editCampaign_typeFieldDropdownOutputTab = element(by.css('[name="outputType"] [class="ui-dropdown-trigger ui-state-default ui-corner-right"] span'));
    this.editCampaign_mediaLabelOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(1)>div>div:nth-child(1)>label'));
    this.editCampaign_mediaDefaultValueOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"] [name="outputMedia"] label'));
    this.editCampaign_mediaFieldDropdownOutputTab = element(by.css('[header="Format"] [name="outputMedia"] [class="ui-dropdown-trigger-icon ui-clickable pi pi-chevron-down"]'));
    this.editCampaign_mediaSearchFieldOutputTab = element(by.css('[header="Format"] [name="outputMedia"] [class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.editCampaign_typeLabelOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(1)>div>div:nth-child(2)>label'));
    this.editCampaign_typeDefaultValueOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"] [name="outputType"] label'));
    this.editCampaign_headerRowLabelOutputTab = element(by.css('[label="Header Row"]>label'));
    this.editCampaign_dateFileOnlyLabelOutputTab = element(by.css('[label="Data File Only"]>label'));
    this.editCampaign_unzippedLabelOutputTab = element(by.css('[inputid="unzipped"] label'));
    this.editCampaign_typeSearchFieldOutputTab = element(by.css('[header="Format"] [name="outputType"] [class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.editCampaign_SearchFieldFirstElementOutputTab = element(by.css('[role="option"] span'));

    this.editCampaign_fileNameFieldLabelOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(4)>div>div:nth-child(1) label'));
    this.editCampaign_PGPKeyFieldLabelOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(4)>div>div:nth-child(2) label'));
    this.editCampaign_PGPKeyFieldDefaultValueOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"] [name="outputPgpKey"] label'));
    this.editCampaign_notesLabelOutputTab = element(by.css('[header="Notes"] span:nth-child(2)'));
    this.editCampaign_splitLabelOutputTab = element(by.css('[header="Split"] a>span:nth-child(2)'));
    this.editCampaign_splitDefaultValueOutputTab = element(by.css('[header="Split"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"] [name="outputSplit"] label'));

    //Layout
    this.editCampaign_layoutFieldLabelOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(3)>div>div:nth-child(1) label'));
    this.editCampaign_layoutFieldDefaultValueOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"] [name="outputLayout"] label'));
    this.editCampaign_layoutFieldDropdownOutputTab = element(by.css('[name="outputLayout"] [class="ui-dropdown-trigger ui-state-default ui-corner-right"] span'));
    this.editCampaign_layoutSearchFieldOutputTab = element(by.css('[class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));

    //Sort
    this.editCampaign_sortFieldLabelOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(3)>div>div:nth-child(2) label'));
    this.editCampaign_sortFieldDefaultValueOutputTab = element(by.css('[heading="Output"] [header="Format"] [role="tabpanel"] [class="ui-accordion-content ui-widget-content"] [name="outputSort"] label'));
    this.editCampaign_sortFieldDropdownOutputTab = element(by.css('[header="Format"] [name="outputSort"] [class="ui-dropdown-trigger-icon ui-clickable pi pi-chevron-down"]'));
    this.editCampaign_sortSearchFieldLabelOutputTab = element(by.css('[header="Format"] [name="outputSort"] [class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));

    //Shipping
    this.editCampaign_shippingOutputTab = element(by.css('[header="Shipping"]>div>a'));
    this.editCampaign_shippingLabelOutputTab = element(by.css('[header="Shipping"] span:nth-child(2)'));
    this.editCampaign_shippingShipToPrimarySearchFieldOutputTab = element(by.css('[editable="true"]'));
    this.editCampaign_shippingShipToFieldErrorMessageOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(1)>[class="form-control-feedback ng-star-inserted"]'));
    this.editCampaign_shippingCCFieldOutputTab = element(by.css('[name="shipCC"]'));
    this.editCampaign_shippingCCFieldErrorMessageOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(2)>[class="form-control-feedback ng-star-inserted"]'));
    this.editCampaign_shippingSubjectFieldOutputTab = element(by.css('[name="shipSubject"]'));
    this.editCampaign_shippingNotesFieldOutputTab = element(by.css('[name="shipNotes"]'));

    this.editCampaign_shippingShipToLabelOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(1) label'));
    this.editCampaign_shippingCCLabelOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(2) label'));
    this.editCampaign_shippingSubjectLabelOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(3)>div>div:nth-child(1)>label'));
    this.editCampaign_shippingNotesLabelOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(3)>div>div:nth-child(2)>label'));
    this.editCampaign_shippingFTPsiteLabelOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(4)>div>div:nth-child(1)>label'));
    this.editCampaign_shippingUsernameLabelOutputTab = element(by.css('[header="Shipping"] [class="ui-accordion-content ui-widget-content"]>div:nth-child(4)>div>div:nth-child(2)>label'));

    this.editCampaign_shippingShipToDropdownOutputTab = element(by.css('[name="outputShipTo"] [class="ui-dropdown-trigger ui-state-default ui-corner-right"] span'));
    this.editCampaign_shippingShipToSearchFieldOutputTab = element(by.css('[name="outputShipTo"] [class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.editCampaign_POLabelBillingTab = element(by.css('[heading="Billing"]>fieldset>div:nth-child(1)>label'));
    this.editCampaign_POSearchFieldBillingTab = element(by.css('[heading="Billing"]>fieldset>div:nth-child(1) input'));

    //ErrorMessage
    this.popUpErrorMessage = element(by.css('[id="swal2-title"]'));
    this.popUpErrorMessageOkButton = element(by.css('[class="swal2-confirm swal2-styled"]'));

    //MaxPer: Segment Level
    this.editCampaign_segmentLevelSectionArrowMaxPerTab = element(by.css('[header="Segment Level"] span:nth-child(1)'));
    this.editCampaign_segmentLevelSectionTextMaxPerTab = element(by.css('[header="Segment Level"] span:nth-child(2)'));
    this.editCampaign_segmentLevelGroupLabelMaxPerTab = element(by.css('[header="Segment Level"] [role="tabpanel"] table tr>th:nth-child(1)'));
    this.editCampaign_segmentLevelQuantityLabelMaxPerTab = element(by.css('[header="Segment Level"] [role="tabpanel"] table tr>th:nth-child(2)'));
    this.editCampaign_segmentLevelFieldLabelMaxPerTab = element(by.css('[header="Segment Level"] [role="tabpanel"] table tr>th:nth-child(3)'));
    this.editCampaign_segmentLevelGroupFirstRowMaxPerTab = element(by.css('[header="Segment Level"] [role="tabpanel"] table[class="ui-table-scrollable-body-table"] tr>td:nth-child(1)'));
    this.editCampaign_segmentLevelQuantityFirstRowMaxPerTab = element(by.css('[header="Segment Level"] [role="tabpanel"] table[class="ui-table-scrollable-body-table"] tr>td:nth-child(2)'));
    this.editCampaign_segmentLevelQuantityFirstRowFieldMaxPerTab = element(by.css('[name="maxPerQuantity"]'));
    this.editCampaign_segmentLevelFieldFirstRowMaxPerTab = element(by.css('[header="Segment Level"] [role="tabpanel"] table[class="ui-table-scrollable-body-table"] tr>td:nth-child(3)'));
    this.editCampaign_segmentLevelFieldFirstRowDropdownMaxPerTab = element(by.css('[name="segmentLevelDrop"] [class="ui-dropdown-trigger ui-state-default ui-corner-right"]>span'));
    this.editCampaign_segmentLevelFieldFirstRowDropdownSearchMaxPerTab = element(by.css('[name="segmentLevelDrop"] [class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.editCampaign_campaignLevelSectionArrowMaxPerTab = element(by.css('[header="Campaign Level"] span:nth-child(1)'));
    this.editCampaign_campaignLevelSectionTextMaxPerTab = element(by.css('[header="Campaign Level"] span:nth-child(2)'));

};
module.exports = CampaignstatuspageLocator