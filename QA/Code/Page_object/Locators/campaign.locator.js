var CampaignpageLocator = function () {
    //side navigation    
    this.sideNavigation_sideButton = element(by.css('#kt_aside_toggler'));
    //old
    //this.sideNavigation_campaignsButton = element(by.css('#kt_aside_menu > ul li:nth-child(2)'));
    this.sideNavigation_campaignsButton = element(by.css('#kt_aside_menu >ul>li:nth-child(1)'));

    //NewCampaign popup 
    this.newCampaign_newCampaignTitle = element(by.css('[class="modal-title"] [class="ng-star-inserted"]'));
    this.newCampaign_generalText = element(by.css('[class="ng-untouched ng-pristine ng-star-inserted ng-valid"] ul>li a>span'));
    this.newCampaign_generalLinkTab = element(by.css('[class="nav-item active ng-star-inserted"]'));
    this.newCampaign_databaseLabel = element(by.css('[heading="General"] fieldset>div:nth-child(1)>label'));
    this.newCampaign_databaseField = element(by.css('p-dropdown[name="databaseID"]'));
    this.newCampaign_databaseSearch = element(by.css('[class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.newCampaign_selectedDatabase = element(by.css('[name="databaseID"] label'));
    this.newCampaign_databaseList = element.all(by.css('[role="option"] span'));
    this.newCampaign_databaseSearch_firstElement = element(by.css('[role="option"] span'));
    this.newCampaign_descriptionLabel = element(by.css('[heading="General"] fieldset>div:nth-child(2)>label'));
    this.newCampaign_descriptionField = element(by.css('[id="Description"]'));
    this.newCampaign_description_maxCharAlertWindow = element(by.css('[class="swal2-container swal2-center swal2-fade swal2-shown"]'));
    this.newCampaign_database_dropdownbutton = element(by.css('[name="databaseID"] [class="ui-dropdown-trigger-icon ui-clickable pi pi-chevron-down"]'));
    this.newCampaign_descriptionRequiredFieldError = element(by.css('[class="form-control-feedback ng-star-inserted"]'));
    this.newCampaign_offerLabel = element(by.css('[heading="General"] fieldset>div:nth-child(3)>label'));
    this.newCampaign_offerField = element(by.css('[id="Offer"]'));
    this.newCampaign_netRadioButton = element(by.css('[label=" Net"]'));
    this.newCampaign_grossRadioButtonText = element(by.css('[label=" Gross"]'));
    this.newCampaign_grossRadioButton = element(by.css('[class="ui-radiobutton-icon ui-clickable"]'));
    this.newCampaign_closeCrossSymbol = element(by.xpath('//span[text()="New Campaign"]//ancestor::form[@class]//button[@class="close"]'));
    this.newCampaign_cancelButton_footer = element(by.xpath("//span[text()='New Campaign']//ancestor::form[@class]//button[text()='Cancel']"));
    this.newCampaign_saveButton_footer = element(by.css('[role="document"] [type="submit"]'));
    this.newCampaign_okButtonOnErrorPopup = element(by.css('[class="swal2-confirm swal2-styled"]'));
    this.newCampaign_descriptionMaxCharError = element(by.css('[id="swal2-content"]'));

    //Campaign List screen//
    this.campaignList_campaignTitle = element(by.css('[class="nav-link active"]>span'));
    this.campaignList_newCampaignButton = element(by.css('[class="btn btn-primary blue"]'));
    this.campaignList_dropdownButton_table = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(1)'));
    this.campaignList_campaignIdLabel_table = element(by.css('[class="ui-table-scrollable-header-table"] tr>th:nth-child(2)'));
    this.campaignList_descriptionLabel_table = element(by.css('[class="ui-table-scrollable-header-table"] tr>th:nth-child(3)'));
    this.campaignList_quantityLabel_table = element(by.css('[class="ui-table-scrollable-header-table"] tr>th:nth-child(4)'));
    this.campaignList_statusLabel_table = element(by.css('[class="ui-table-scrollable-header-table"] tr>th:nth-child(5)'));
    this.campaignList_statusDateLabel_table = element(by.css('[class="ui-table-scrollable-header-table"] tr>th:nth-child(6)'));
    this.campaignList_actionsLabel_table = element(by.css('[class="ui-table-scrollable-header-table"] tr>th:nth-child(7)'));
    //Campaign Id
    this.campaignList_campaignId_table = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(2)'));
    this.campaignList_campaignIdNext_table_2 = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(2)>td:nth-child(2)'));
    this.campaignList_campaignIdNext_table_3 = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(3)>td:nth-child(2)'));
    this.campaignList_campaignIdList_table = element.all(by.xpath("//div[@class='ui-table-scrollable-body']//td[2]"));
    //Campaign description
    this.campaignList_description_table = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(3)'));
    this.campaignList_descriptionList_table = element.all(by.xpath("//div[@class='ui-table-scrollable-body']//td[3]"));
    //Campaign status
    this.campaignList_status_table = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(8)'));
    this.campaignList_statusList_table = element.all(by.xpath("//div[@class='ui-table-scrollable-body']//td[5]"));
    //Campaign status date
    this.campaignList_statusDate_table = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(9)'));
    this.campaignList_statusDateList_table = element.all(by.xpath("//div[@class='ui-table-scrollable-body']//td[6]"));
    this.campaignList_noData_table = element(by.xpath("//div[@class='kt-portlet__body']//div[contains(@class,'primeng-no-data')]"));
    this.campaignList_actionButton = element(by.css('table>tbody[class="ui-table-tbody"]>tr:nth-child(1)>td:nth-child(10)>div>button'));
    this.campaignList_actionButton_edit = element(by.css('[class="dropdown open show"]>ul>li:nth-child(3)'));
    this.campaignList_searchField = element(by.css('[class="input-group"] [name="filterText"]'));
    this.campaignList_cancelSymbol_filter = element(by.css('[class="bs-remove-tab ng-star-inserted"]'));
    this.campaignList_searchButton = element(by.css('[class="btn btn-primary"] [class="flaticon-search-1"]'));
    this.campaignList_totalCount_table = element(by.css('[class="row align-items-center"] [class="total-records-count"]'));
    this.campaignList_saveSuccessfullyMessage = element(by.css('[class="kt-shape-font-color-1"]'));
    this.campaignList_cancelSymbol_filter = element(by.css('[class="bs-remove-tab ng-star-inserted"]'));
    this.campaignList_campaigndurationButton = element(by.css('[class="custom-date btn-group dropdown"] [class="dropdown-toggle btn btn-sm btn-primary"]'));
    this.campaignList_durationDropdownButton = function (Dropdown_No) {
        var partialCSS1 = '[class="dropdown open show"]>ul>li:nth-child(';
        var partialCSS2 = ')';
        return campaignList_durationDropdownButton = element(by.css(partialCSS1 + Dropdown_No + partialCSS2));
    };
    this.campaignList_campaignSegment_didupNo = element(by.xpath("//table[@class='ui-table-scrollable-body-table']//table[contains(@class,'body')]//td[1]"));
    this.campaignList_campaignSegment_description = element(by.xpath("//table[@class='ui-table-scrollable-body-table']//table[contains(@class,'body')]//td[2]"));
    this.campaignList_campaignSegment_totalCount = element(by.css('[class="row align-items-center child"] [class="primeng-paging-container"] [class="total-records-count"]'));

    //Campaign filter screen 
    this.campaignFilter_campaignId = element(by.css('[class="nav-link active"]'));
    //Edit Campaign
    this.editCamapign_titile = element(by.css('[class="modal-title"] [class="ng-star-inserted"]'));
    this.editCamapign_generalTab = element(by.css('[class="modal-body typeName  level xfield yfield"] [class="tab-container"]>ul>li:nth-child(1)>a span'));
    this.editCamapign_reportsTab = element(by.css('[class="modal-body typeName  level xfield yfield"] [class="tab-container"]>ul>li:nth-child(2)>a span'));
    this.editCamapign_maxPerTab = element(by.css('[class="modal-body typeName  level xfield yfield"] [class="tab-container"]>ul>li:nth-child(3)>a span'));
    this.editCamapign_outputTab = element(by.css('[class="modal-body typeName  level xfield yfield"] [class="tab-container"]>ul>li:nth-child(4)>a span'));
    this.editCamapign_billingTab = element(by.css('[class="modal-body typeName  level xfield yfield"] [class="tab-container"]>ul>li:nth-child(5)>a span'));

    this.editCampaign_cancelButton_footer = element(by.xpath("//span[text()='Edit Campaign']//ancestor::form[@class]//button[text()='Cancel']"));
    this.editCampaign_saveButton_footer = element(by.xpath("//span[text()='Edit Campaign']//ancestor::form[@class]//button[@type='submit']"));
    this.editCampaign_databaseVal = element(by.css('[class="ng-tns-c17-27 ui-dropdown-label ui-inputtext ui-corner-all ng-star-inserted"]'));
    this.editCampaign_closeCrossSymbol = element(by.xpath('//span[text()="Edit Campaign"]//ancestor::form[@class]//button[@class="close"]'));
    //Advanced filters
    this.advancedFilters_advancedFiltersButton = element(by.css('[class="row margin-bottom-10"] span'));
    this.advancedFilters_idLabel = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(2)>div label'));
    this.advancedFilters_descriptionLabel = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(3)>div label'));
    this.advancedFilters_statusLabel = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(4)>div label'));
    this.advancedFilters_statusDateLabel = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(5)>div label'));
    this.advancedFilters_searchFieldId = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(2)>div input'));
    this.advancedFilters_searchFieldDescription = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(3)>div input'));
    this.advancedFilters_statusField = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(4)>div input'));
    this.advancedFilters_statusDateField = element(by.css('[class="row m--margin-bottom-10"]>div:nth-child(5)>div input'));

    this.advancedFilters_toggleButton = element(by.css('[class="slider round"]'));
    this.advancedFilters_statusDropdownButton = element(by.css('[name="statusFilterValue"] span'));
    this.advancedFilters_statusSearch = element(by.css('[class="ui-dropdown-filter ui-inputtext ui-widget ui-state-default ui-corner-all"]'));
    this.advancedFilters_statusSearch_first = element(by.css('p-dropdownitem>li'));
    this.advancedFilters_statusDatePicker = element(by.css('[name="selectedDateRange"]'));
    this.advancedFilters_statusDropdown = element.all(by.css('[class="ui-dropdown-item ui-corner-all"]'));



};
module.exports = CampaignpageLocator