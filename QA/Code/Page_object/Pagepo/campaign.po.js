var Campaign = function () {
    var CampaignpageLocatorTest = new CampaignpageLocator();
    var HomepageTest = new Homepage();
    var EC = protractor.ExpectedConditions;

    this.clickOnSideButton = async function () {
        try {
            browser.wait(EC.elementToBeClickable(CampaignpageLocatorTest.sideNavigation_sideButton), HomepageTest.await(50));
            await CampaignpageLocatorTest.sideNavigation_sideButton.click();
        } catch (err) {
            console.log('Error occured' + err);
        }

    };
    this.clickOnSideNavCampaignButton = async function () {
        try {
            browser.wait(EC.elementToBeClickable(CampaignpageLocatorTest.sideNavigation_campaignsButton), HomepageTest.await(50));
            await CampaignpageLocatorTest.sideNavigation_campaignsButton.click();
        } catch (err) {
            console.log('Error occured' + err);
        }
    };
    this.clickOnNewChampaignButton = async function () {
        try {
            browser.wait(EC.elementToBeClickable(CampaignpageLocatorTest.campaignList_newCampaignButton), HomepageTest.await(50));
            await CampaignpageLocatorTest.campaignList_newCampaignButton.click();
        } catch (err) {
            console.log('Error occured' + err);
        }
    };
    this.clearDescriptionFieldByClear = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_descriptionField), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_descriptionField.clear();
    };
    this.getDescriptionFieldErrorText = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_descriptionRequiredFieldError), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_descriptionRequiredFieldError.getText();
    };
    this.getDescriptionMaxCharErrorText = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_descriptionMaxCharError), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_descriptionMaxCharError.getText();
    };
    this.clickOnSaveButtonFooter = async function () {
        return await CampaignpageLocatorTest.newCampaign_saveButton_footer.click();
    };
    this.clickOnCancelButtonFooter = async function () {
        return await CampaignpageLocatorTest.newCampaign_cancelButton_footer.click();
    };
    this.clickOnCrossSymbolHeader = async function () {
        return await CampaignpageLocatorTest.newCampaign_closeCrossSymbol.click();
    }
    this.clickOnFilterCrossSymbol = async function () {
        return await CampaignpageLocatorTest.campaignList_cancelSymbol_filter.click();
    };
    this.clickOnDatabaseFieldDropdown = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_database_dropdownbutton), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_database_dropdownbutton.click();
    };
    this.clickOnErrorPopupOkButton = async function () {
        return await CampaignpageLocatorTest.newCampaign_okButtonOnErrorPopup.click();
    };
    this.clickOnToShowSegment = async function () {
        return await CampaignpageLocatorTest.campaignList_dropdownButton_table.click();
    };
    this.selectGrossRadioButton = async function () {
        return await CampaignpageLocatorTest.newCampaign_grossRadioButton.click();
    };
    this.getNewCampaignTitle = async function () {
        return await CampaignpageLocatorTest.newCampaign_newCampaignTitle.getText();
    };
    this.getDatabaseLabelText = async function () {
        return await CampaignpageLocatorTest.newCampaign_databaseLabel.getText();
    };
    this.getDescriptionLabelText = async function () {
        return await CampaignpageLocatorTest.newCampaign_descriptionLabel.getText();
    };
    this.getOfferLabelText = async function () {
        return await CampaignpageLocatorTest.newCampaign_offerLabel.getText();
    };
    this.getGrossRadioButtonText = async function () {
        return await CampaignpageLocatorTest.newCampaign_grossRadioButtonText.getText();
    }
    this.getNetRadioButtonText = async function () {
        return await CampaignpageLocatorTest.newCampaign_netRadioButton.getText();
    };
    this.selectCampaignType = async function (optionsName) {
        return await CampaignpageLocatorTest.selectDropDown.click();
    };
    this.clearSearchFieldByClear = async function () {
        return CampaignpageLocatorTest.campaignList_searchField.clear();
    };
    this.clickOnSearchButton = async function () {
        return await CampaignpageLocatorTest.campaignList_searchButton.click();
    };
    this.typeOnSearchField = async function (key) {
        browser.waitForAngular();
        return await CampaignpageLocatorTest.campaignList_searchField.sendKeys(key);
    };
    this.getNoDataTableText = async function () {
        return await CampaignpageLocatorTest.campaignList_noData_table.getText();
    };
    this.getCountTableText = async function () {
        return await CampaignpageLocatorTest.campaignList_totalCount_table.getText();
    };
    this.campaignDuration = async function (Dropdown_No) {
        await CampaignpageLocatorTest.campaignList_campaigndurationButton.click();
        await CampaignpageLocatorTest.campaignList_durationDropdownButton(Dropdown_No).click();
        HomepageTest.wait(10);
    };
    this.clickOnCampaignActionDropdown = async function () {
        await CampaignpageLocatorTest.campaignList_actionButton.click();
    };
    this.clickOnCampaignActionEdit = async function () {
        return await CampaignpageLocatorTest.campaignList_actionButton_edit.click();
    };
    this.getEditCampaignTitle = async function () {
        return await CampaignpageLocatorTest.editCamapign_titile.getText();
    };
    this.getGeneralText = async function () {
        return await CampaignpageLocatorTest.newCampaign_generalText.getText();
    };
    this.getDatabaseLabelText = async function () {
        return await CampaignpageLocatorTest.newCampaign_databaseLabel.getText();
    }
    this.getDescriptionLabelText = async function () {
        return await CampaignpageLocatorTest.newCampaign_descriptionLabel.getText();
    }
    this.getOfferLabelText = async function () {
        return await CampaignpageLocatorTest.newCampaign_offerLabel.getText();
    };
    this.getGeneralTabTextEditCampaign = async function () {
        return await CampaignpageLocatorTest.editCamapign_generalTab.getText();
    };
    this.getReportTabTextEditCampaign = async function () {
        return await CampaignpageLocatorTest.editCamapign_reportsTab.getText();
    };
    this.getOutputTabTextEditCampaign = async function () {
        return await CampaignpageLocatorTest.editCamapign_outputTab.getText();
    };
    this.getMaxPerTabTextEditCampaign = async function () {
        return await CampaignpageLocatorTest.editCamapign_maxPerTab.getText();
    };
    this.clickOnCancelButtonFooterEditCampaign = async function () {
        return await CampaignpageLocatorTest.editCampaign_cancelButton_footer.click();
    };
    this.clickOnSaveButtonFooterEditCampaign = async function () {
        return await CampaignpageLocatorTest.editCampaign_saveButton_footer.click();
    };
    this.getDatabaseValEditCampaign = async function () {
        return await CampaignpageLocatorTest.newCampaign_databaseField.getText();
    };
    this.getSegmentDidupNo = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignSegment_didupNo), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignSegment_didupNo.getText();
    };
    this.getSegmentDescription = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignSegment_description), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignSegment_description.getText();
    };
    this.getStatusDate = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_statusDate_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_statusDate_table.getText();
    };
    this.getStatusDateList = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_statusDateList_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_statusDateList_table.getText();
    };
    this.getStatus = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_status_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_status_table.getText();
    };
    this.getStatusList = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_statusList_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_statusList_table.getText();
    };
    this.getTotalCount = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_totalCount_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_totalCount_table.getText();
    };
    this.getTotalCountNum = async function () {

        HomepageTest.wait(10);
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_totalCount_table), HomepageTest.await(50));
        var totalCountBefore = await CampaignpageLocatorTest.campaignList_totalCount_table.getText();
        var num = await totalCountBefore.split(":");
        var totalCount = await num[1].trim();
        return await totalCount;
    };
    this.getSegmentTotalCount = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignSegment_totalCount), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignSegment_totalCount.getText();
    };

    this.getCurrentDate = function () {
        var date = new Date();
        var currentDate = date.getFullYear() + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2);
        return currentDate;
    };
    this.generateStatusDate = function () {
        const moment = require('moment');
        const currentDate = moment().utc().format('MM/DD/YYYY');
        return currentDate;

    };
    this.generatePreviousStatusDate = function () {
        const moment = require('moment');
        const previousDate = moment().utc().subtract(1, "days").format('MM/DD/YYYY');
        return previousDate;
    };
    this.generateRandomText = function (charLength) {
        var description = '';
        var alphabets = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
        for (i = 0; i < charLength; i++) {
            description += alphabets.charAt(Math.floor(Math.random() * alphabets.charLength));
        }
        return description;
    };
    this.generateRandomNumber = function (min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    };
    this.clearDescriptionField = function () {
        CampaignpageLocatorTest.newCampaign_descriptionField.sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        CampaignpageLocatorTest.newCampaign_descriptionField.sendKeys(protractor.Key.BACK_SPACE);
    };
    this.clearSearchField = function () {
        CampaignpageLocatorTest.campaignList_searchField.sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        CampaignpageLocatorTest.campaignList_searchField.sendKeys(protractor.Key.BACK_SPACE);
    }
    this.generateDescriptionDate = function () {
        var date = new Date();
        var desDate = "' " + date.getDate() + "-" + (date.getMonth() + 1) + "-" + date.getFullYear() + " '";
        return desDate;
    };
    this.createCampaign = function (database, description, offer, appendTimeStamp) {
        var date = new Date();
        var timeStamp = appendTimeStamp ? "' " + date.getDate() + "-" + (date.getMonth() + 1) + "-" + date.getFullYear() + " '" : '';
        CampaignpageLocatorTest.campaignList_newCampaignButton.click();
        CampaignpageLocatorTest.newCampaign_database_dropdownbutton.click();
        CampaignpageLocatorTest.newCampaign_databaseSearch.sendKeys(database);
        CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
        CampaignpageLocatorTest.newCampaign_descriptionField.clear();
        CampaignpageLocatorTest.newCampaign_descriptionField.sendKeys(description + timeStamp);
        CampaignpageLocatorTest.newCampaign_offerField.sendKeys(offer);
    };
    this.getCampaignIdLabelTable = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignIdLabel_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignIdLabel_table.getText();
    };
    this.getDescriptionLabelTable = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_descriptionLabel_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_descriptionLabel_table.getText();
    };
    this.getQuantityLabelTabel = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_quantityLabel_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_quantityLabel_table.getText();
    };
    this.getStatusLabelTabel = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_statusLabel_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_statusLabel_table.getText();
    };
    this.getStatusDateLabelTabel = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_statusDateLabel_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_statusDateLabel_table.getText();
    };
    this.getActionsLabelTabel = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_actionsLabel_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_actionsLabel_table.getText();
    };
    this.getCampaignId = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignId_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignId_table.getText();
    };
    this.getCampaignIdFromFilter = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignFilter_campaignId), HomepageTest.await(50));
        var newCampaignId = await CampaignpageLocatorTest.campaignFilter_campaignId.getText();
        var getid = newCampaignId.split("-", 1);
        var id = getid[0].trim();
        return await id;
    };
    this.getCampaignIdList = function () {
        return CampaignpageLocatorTest.campaignList_campaignIdList_table.getText();
    };
    this.getCampaignIdNext2nd = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignIdNext_table_2), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignIdNext_table_2.getText();
    };
    this.getCampaignIdNext3rd = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignIdNext_table_3), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignIdNext_table_3.getText();
    };
    this.getDescriptionField = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_descriptionField), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_descriptionField.getAttribute('value');
    };
    this.getDescription = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_description_table), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_description_table.getText();
    };
    this.getDescriptionList = function () {
        return CampaignpageLocatorTest.campaignList_descriptionList_table.getText();
    };
    this.getOfferField = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_offerField), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_offerField.getAttribute('value');
    };
    this.getSuccessfullMessage = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage.getText();
    };
    this.clickOnAdvancedSearchButton = async function () {
        browser.wait(EC.elementToBeClickable(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton), HomepageTest.await(50));
        return await CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.click();
    };
    this.getAdvancedSearchButtonText = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton), HomepageTest.await(50));
        return await CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.getText();
    };
    this.getAdvancedSearchIdLabel = function () {
        return CampaignpageLocatorTest.advancedFilters_idLabel.getText();
    };
    this.getAdvancedSearchDescriptionLabel = function () {
        return CampaignpageLocatorTest.advancedFilters_descriptionLabel.getText();
    };
    this.getAdvancedSearchStatusLabel = function () {
        return CampaignpageLocatorTest.advancedFilters_statusLabel.getText();
    };
    this.getAdvancedSearchStatusDateLabel = function () {
        return CampaignpageLocatorTest.advancedFilters_statusDateLabel.getText();
    };
    this.typeOnAdvancedSearchFieldId = function (text) {
        CampaignpageLocatorTest.advancedFilters_searchFieldId.sendKeys(text);
    };
    this.clearAdvancedSearchFieldIdByClear = function () {
        return CampaignpageLocatorTest.advancedFilters_searchFieldId.clear();
    }
    this.clearAdvancedSearchFieldId = function () {
        CampaignpageLocatorTest.advancedFilters_searchFieldId.sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        CampaignpageLocatorTest.advancedFilters_searchFieldId.sendKeys(protractor.Key.BACK_SPACE);
    };
    this.typeOnAdvancedSearchFieldDescription = function (text) {
        CampaignpageLocatorTest.advancedFilters_searchFieldDescription.sendKeys(text);
    };
    this.clearAdvancedSearchFieldDescription = function () {
        return CampaignpageLocatorTest.advancedFilters_searchFieldDescription.clear();
    };
    this.getAdvancedSearchToggleButtonText = function () {
        return CampaignpageLocatorTest.advancedFilters_toggleButton.getText();
    };
    this.clickOnAdvancedSearchToggleButton = function () {
        return CampaignpageLocatorTest.advancedFilters_toggleButton.click();
    };
    this.selectAdvancedSearchStatusDropdown = function (val) {
        CampaignpageLocatorTest.advancedFilters_statusDropdownButton.click();
        CampaignpageLocatorTest.advancedFilters_statusDropdown.get(val).click();
    };
    //optional
    this.typeOnAdvancedSearchStatusDropdown = function (text) {
        return CampaignpageLocatorTest.advancedFilters_statusSearch.sendKeys(text);
    };
    //optional
    this.clickOnAdvancedSearchStatusDropdownFirst = function () {
        return CampaignpageLocatorTest.advancedFilters_statusSearch_first.click();
    };
    this.typeOnAdvancedSearchDatePicker = function (searchDate) {
        CampaignpageLocatorTest.advancedFilters_statusDatePicker.clear();
        CampaignpageLocatorTest.advancedFilters_statusDatePicker.sendKeys(searchDate);
    };
    this.searchAdvancedSearchDatePicker = function () {
        CampaignpageLocatorTest.advancedFilters_statusDatePicker.clear();
        const moment = require('moment');
        const yesterdayDate = moment().subtract(1, "days").format('MM/DD/YYYY');
        const currentDate = moment().format('MM/DD/YYYY');
        var searchDate = yesterdayDate + ' - ' + currentDate;
        CampaignpageLocatorTest.advancedFilters_statusDatePicker.sendKeys(searchDate);
    };
    this.searchAdvancedSearchDatePickerNext2Days = function () {
        CampaignpageLocatorTest.advancedFilters_statusDatePicker.clear();
        const moment = require('moment');
        const tomorrowDate = moment().add(1, 'days').format('MM/DD/YYYY');
        const dayAfterTomorrowDate = moment().add(2, 'days').format('MM/DD/YYYY');;
        var searchDate = tomorrowDate + ' - ' + dayAfterTomorrowDate;
        CampaignpageLocatorTest.advancedFilters_statusDatePicker.sendKeys(searchDate);
    };


};
module.exports = Campaign