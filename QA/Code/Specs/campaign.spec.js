// Put x before 'it' to exclude the test case
// Put f before 'it' to focus(include) the test case
describe('IDMS "New Campaign" screen test', function () {

    var CampaignTest = new Campaign();
    var HomepageTest = new Homepage();
    var DashboardTest = new Dashboard();
    var CampaignpageLocatorTest = new CampaignpageLocator();
    var CampaignConstantTest = new CampaignConstant();
    var EC = protractor.ExpectedConditions;

    beforeEach(async function () {
        HomepageTest.get();
        HomepageTest.clickOnCookieBtn();
        HomepageTest.setUsername();
        HomepageTest.setPassword();
        HomepageTest.wait(10);
        HomepageTest.clickLoginButton();
        HomepageTest.wait(20);
        CampaignTest.clickOnSideButton();
        HomepageTest.wait(10);
        CampaignTest.clickOnSideNavCampaignButton();
        CampaignTest.clickOnSideButton();
        HomepageTest.wait(20);
    });

    async function createCampaignCustom(database, description, offer, appendTimeStamp) {
        HomepageTest.wait(10);
        CampaignTest.createCampaign(database, description, offer, appendTimeStamp);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(10);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        return id;
    };
    async function createMultipleCampaign(num, appendTimeStamp) {
        let campaignIds = [];
        for (let i = 0; i <= num; i++) {
            campaignIds.push(await createCampaignCustom(CampaignConstantTest.newCampaign_databaseValue[i], CampaignConstantTest.newCampaign_descriptionFieldArray[i], CampaignConstantTest.newCampaign_offerFieldArray[i], appendTimeStamp));
        }
        return campaignIds;
    };

    it('Verify a proper error message is displayed when the Description is left blank on Save', async function () {

        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isDisplayed()).toBe(true);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignpageLocatorTest.newCampaign_descriptionField.isPresent()).toBe(true);
        CampaignTest.clearDescriptionField();
        expect(CampaignpageLocatorTest.newCampaign_descriptionRequiredFieldError.isPresent()).toBe(true);
        expect(CampaignTest.getDescriptionFieldErrorText()).toEqual(CampaignConstantTest.newCampaign_descriptionRequiredError);
        CampaignpageLocatorTest.newCampaign_cancelButton_footer.click();

    });

    //This test case became invalid due to new functionality added.
    /*xit('Verify a proper error message is displayed when the Description is over 50 chars', async function () {
        var EC = protractor.ExpectedConditions;
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignpageLocatorTest.newCampaign_descriptionField.isPresent()).toBe(true);
        CampaignTest.clearDescriptionFieldByClear();
        CampaignpageLocatorTest.newCampaign_descriptionField.sendKeys(CampaignTest.generateRandomText(60));
        CampaignTest.clickOnSaveButtonFooter();
        expect(CampaignpageLocatorTest.newCampaign_description_maxCharAlertWindow.isPresent()).toBe(true);
        expect(CampaignTest.getDescriptionMaxCharErrorText()).toEqual(CampaignConstantTest.newCampaign_descriptionMaxCharError);
        CampaignTest.clickOnErrorPopupOkButton();
        HomepageTest.wait(10);
        CampaignTest.clickOnCancelButtonFooter();
    });*/

    it('Verify New Campaign screen pops up on clicking "New Campaign"', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignpageLocatorTest.newCampaign_newCampaignTitle.isPresent()).toBe(true);
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        expect(CampaignpageLocatorTest.newCampaign_generalLinkTab.isPresent()).toBe(true);
        expect(CampaignTest.getGeneralText()).toEqual(CampaignConstantTest.newCampaign_generalTextLabel);
        expect(CampaignpageLocatorTest.newCampaign_databaseField.isPresent()).toBe(true);
        expect(CampaignTest.getDatabaseLabelText()).toEqual(CampaignConstantTest.newCampaign_databaseLabel);
        expect(CampaignpageLocatorTest.newCampaign_descriptionField.isPresent()).toBe(true);
        expect(CampaignTest.getDescriptionLabelText()).toEqual(CampaignConstantTest.newCampaign_descriptionLabel);
        expect(CampaignpageLocatorTest.newCampaign_offerField.isPresent()).toBe(true);
        expect(CampaignTest.getOfferLabelText()).toEqual(CampaignConstantTest.newCampaign_offerLabel);
        expect(CampaignpageLocatorTest.newCampaign_netRadioButton.isPresent()).toBe(true);
        expect(CampaignTest.getGrossRadioButtonText()).toEqual(CampaignConstantTest.newCampaign_grossRadioButtonText);
        expect(CampaignpageLocatorTest.newCampaign_grossRadioButton.isPresent()).toBe(true);
        expect(CampaignTest.getNetRadioButtonText()).toEqual(CampaignConstantTest.newCampaign_netRadioButtonText);
        expect(CampaignpageLocatorTest.newCampaign_cancelButton_footer.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.newCampaign_closeCrossSymbol.isPresent()).toBe(true);
        CampaignTest.clickOnCancelButtonFooter();
    });

    it('Verify Database Dropdown has only databases user has access to', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignpageLocatorTest.newCampaign_databaseField.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.newCampaign_database_dropdownbutton.isPresent()).toBe(true);
        CampaignTest.clickOnDatabaseFieldDropdown();
        HomepageTest.wait(10);
        CampaignpageLocatorTest.newCampaign_databaseList.then(function (items) {
            expect(items.length).toBe(CampaignConstantTest.newCampaign_databaseCount);
            for (i = 0; i < items.length; i++) {
                expect(items[i].getText()).toContain(CampaignConstantTest.newCampaign_databaseValue[i]);
            }
        });
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the default Campaign Description appear correctly', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.clickOnNewChampaignButton();
        expect(await CampaignpageLocatorTest.newCampaign_descriptionField.isPresent()).toBe(true);
        expect(await CampaignTest.getDescriptionField()).toEqual(CampaignConstantTest.newCampaign_newCampaignText + CampaignTest.getCurrentDate());
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Offer is blank by default', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignpageLocatorTest.newCampaign_offerField.isPresent()).toBe(true);
        expect(CampaignTest.getOfferField()).toEqual('');
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify "Net" is selected by default', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignpageLocatorTest.newCampaign_netRadioButton.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.newCampaign_grossRadioButton.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.newCampaign_netRadioButton.isEnabled()).toBe(true);
        CampaignpageLocatorTest.newCampaign_cancelButton_footer.click();
    });

    it('Verify on Save a New Campaign is created successfully with Net Radio button', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[0], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(10);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        expect(CampaignpageLocatorTest.campaignList_dropdownButton_table.isPresent()).toBe(true);
        CampaignTest.clickOnToShowSegment();
        expect(CampaignTest.getSegmentDidupNo()).toEqual(CampaignConstantTest.campaignList_campaignSegment_didupNoText);
        expect(CampaignTest.getSegmentDescription()).toEqual(CampaignConstantTest.campaignList_campaignSegment_descriptionText);
        expect(CampaignTest.getStatusDate()).toEqual(CampaignTest.generateStatusDate());
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getSegmentTotalCount()).toEqual(CampaignConstantTest.campaignList_campaignSegment_totalCountText);
        expect(CampaignTest.getTotalCount()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
        HomepageTest.wait(7);
        CampaignTest.clickOnToShowSegment();

    });

    it('Verify on Save a New Campaign is created successfully with Gross Radio button', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[0], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        CampaignTest.selectGrossRadioButton();
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(10);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        expect(CampaignpageLocatorTest.campaignList_dropdownButton_table.isPresent()).toBe(true);
        CampaignTest.clickOnToShowSegment();
        expect(CampaignTest.getSegmentDidupNo()).toEqual(CampaignConstantTest.campaignList_campaignSegment_didupNoText);
        expect(CampaignTest.getSegmentDescription()).toEqual(CampaignConstantTest.campaignList_campaignSegment_descriptionText);
        expect(CampaignTest.getStatusDate()).toEqual(CampaignTest.generateStatusDate());
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getSegmentTotalCount()).toEqual(CampaignConstantTest.campaignList_campaignSegment_totalCountText);
        expect(CampaignTest.getTotalCount()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
        HomepageTest.wait(10);
        CampaignTest.clickOnToShowSegment();

    });

    it('Verify Cancel button it should not create new campaign.', async function () {
        var previousId = CampaignTest.getCampaignId();
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[0], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_cancelButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnCancelButtonFooter();
        HomepageTest.wait(7);
        expect(CampaignTest.getCampaignId()).toEqual(previousId);
    });

    it('Verify Database, Description, Offer and Gross radio button should saved correctly', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[1], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        HomepageTest.wait(10);
        CampaignTest.selectGrossRadioButton();
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(10);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        expect(CampaignTest.getDescription()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldText + CampaignTest.generateDescriptionDate());
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDate()).toEqual(CampaignTest.generateStatusDate());
        expect(CampaignTest.getTotalCount()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignTest.clickOnCampaignActionEdit();
        expect(CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
        expect(CampaignpageLocatorTest.editCampaign_closeCrossSymbol.isPresent()).toBe(true);
        expect(CampaignTest.getGeneralTabTextEditCampaign()).toEqual(CampaignConstantTest.newCampaign_generalTextLabel);
        expect(CampaignTest.getReportTabTextEditCampaign()).toEqual(CampaignConstantTest.editCampaign_reports);
        expect(CampaignTest.getOutputTabTextEditCampaign()).toEqual(CampaignConstantTest.editCampaign_output);
        expect(CampaignTest.getMaxPerTabTextEditCampaign()).toEqual(CampaignConstantTest.editCampaign_maxPer);
        expect(CampaignTest.getDatabaseLabelText()).toEqual(CampaignConstantTest.newCampaign_databaseLabel);
        expect(CampaignpageLocatorTest.newCampaign_databaseField.isPresent()).toBe(true);
        HomepageTest.wait(10);
        expect(CampaignTest.getDatabaseValEditCampaign()).toContain(CampaignConstantTest.newCampaign_databaseValue[1]);
        expect(CampaignTest.getDescriptionLabelText()).toEqual(CampaignConstantTest.newCampaign_descriptionLabel);
        expect(CampaignpageLocatorTest.newCampaign_descriptionField.isPresent()).toBe(true);
        expect(CampaignTest.getDescriptionField()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldText + CampaignTest.generateDescriptionDate());
        expect(CampaignTest.getOfferLabelText()).toEqual(CampaignConstantTest.newCampaign_offerLabel);
        expect(CampaignpageLocatorTest.newCampaign_offerField.isPresent()).toBe(true);
        expect(CampaignTest.getOfferField()).toEqual(CampaignConstantTest.newCampaign_offerFieldText);
        expect(CampaignpageLocatorTest.newCampaign_netRadioButton.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.newCampaign_grossRadioButton.isPresent()).toBe(true);
        expect(CampaignTest.getNetRadioButtonText()).toEqual(CampaignConstantTest.newCampaign_netRadioButtonText);
        expect(CampaignTest.getGrossRadioButtonText()).toEqual(CampaignConstantTest.newCampaign_grossRadioButtonText);
        expect(CampaignpageLocatorTest.newCampaign_grossRadioButton.isEnabled()).toBe(true);
        CampaignTest.clickOnCancelButtonFooterEditCampaign();

    });

    it('Verify Database, Description, Offer and Net radio button should saved correctly', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[1], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(10);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        expect(CampaignTest.getDescription()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldText + CampaignTest.generateDescriptionDate());
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDate()).toEqual(CampaignTest.generateStatusDate());
        expect(CampaignTest.getTotalCount()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignTest.clickOnCampaignActionEdit();
        expect(CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
        expect(CampaignpageLocatorTest.editCampaign_closeCrossSymbol.isPresent()).toBe(true);
        expect(CampaignTest.getGeneralTabTextEditCampaign()).toEqual(CampaignConstantTest.newCampaign_generalTextLabel);
        expect(CampaignTest.getReportTabTextEditCampaign()).toEqual(CampaignConstantTest.editCampaign_reports);
        expect(CampaignTest.getOutputTabTextEditCampaign()).toEqual(CampaignConstantTest.editCampaign_output);
        expect(CampaignTest.getDatabaseLabelText()).toEqual(CampaignConstantTest.newCampaign_databaseLabel);
        expect(CampaignpageLocatorTest.newCampaign_databaseField.isPresent()).toBe(true);
        HomepageTest.wait(10);
        expect(CampaignTest.getDatabaseValEditCampaign()).toContain(CampaignConstantTest.newCampaign_databaseValue[1]);
        expect(CampaignTest.getDescriptionLabelText()).toEqual(CampaignConstantTest.newCampaign_descriptionLabel);
        expect(CampaignpageLocatorTest.newCampaign_descriptionField.isPresent()).toBe(true);
        expect(CampaignTest.getDescriptionField()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldText + CampaignTest.generateDescriptionDate());
        expect(CampaignTest.getOfferLabelText()).toEqual(CampaignConstantTest.newCampaign_offerLabel);
        expect(CampaignpageLocatorTest.newCampaign_offerField.isPresent()).toBe(true);
        expect(CampaignTest.getOfferField()).toEqual(CampaignConstantTest.newCampaign_offerFieldText);
        expect(CampaignpageLocatorTest.newCampaign_netRadioButton.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.newCampaign_grossRadioButton.isPresent()).toBe(true);
        expect(CampaignTest.getNetRadioButtonText()).toEqual(CampaignConstantTest.newCampaign_netRadioButtonText);
        expect(CampaignTest.getGrossRadioButtonText()).toEqual(CampaignConstantTest.newCampaign_grossRadioButtonText);
        expect(CampaignpageLocatorTest.newCampaign_netRadioButton.isEnabled()).toBe(true);
        CampaignTest.clickOnCancelButtonFooterEditCampaign();

    });

    //Run this test case from AutoTestUser2
    it('Verify that Campaign List screen shows no data for the databases users have no access to', async function () {
        CampaignTest.typeOnSearchField(CampaignConstantTest.CampaignIdFromInvalidDatabase);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);

    });

    it('Verify that Campaign List screen shows data for the databases users have access to', async function () {
        var campaignIds = await createMultipleCampaign(2, false);
        HomepageTest.wait(20);
        await CampaignTest.typeOnSearchField(campaignIds[0]);
        await CampaignTest.typeOnSearchField(',');
        await CampaignTest.typeOnSearchField(campaignIds[1]);
        await CampaignTest.typeOnSearchField(',');
        await CampaignTest.typeOnSearchField(campaignIds[2]);
        await CampaignTest.clickOnSearchButton();
        await HomepageTest.wait(10);

        expect(CampaignTest.getCampaignId()).toEqual(campaignIds[2]);
        expect(CampaignTest.getCampaignIdNext2nd()).toEqual(campaignIds[1]);
        expect(CampaignTest.getCampaignIdNext3rd()).toEqual(campaignIds[0]);

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_descriptionList_table.getText()), HomepageTest.await(50));
        CampaignpageLocatorTest.campaignList_descriptionList_table.then(function (text) {
            for (i = 0; i < text.length; i++) {
                expect(text[i].getText()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldArray[text.length - (i + 1)]);
            }
        });
        expect(CampaignTest.getStatusList()).toContain(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDateList()).toContain(CampaignTest.generateStatusDate());

    });

    //Run this test case from AutoTestUser2
    it('Verify that Campaign List screen shows no data for the security groups users have no access to', async function () {
        CampaignTest.typeOnSearchField(CampaignConstantTest.CampaignIdFromInvalidSecurityGroup);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);
    });

    it('Verify that Campaign List screen shows data for the security group users have access to', async function () {
        var campaignIds = await createMultipleCampaign(2, false);
        HomepageTest.wait(20);
        CampaignTest.typeOnSearchField(campaignIds[0]);
        CampaignTest.typeOnSearchField(',');
        CampaignTest.typeOnSearchField(campaignIds[1]);
        CampaignTest.typeOnSearchField(',');
        CampaignTest.typeOnSearchField(campaignIds[2]);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);

        expect(CampaignTest.getCampaignId()).toEqual(campaignIds[2]);
        expect(CampaignTest.getCampaignIdNext2nd()).toEqual(campaignIds[1]);
        expect(CampaignTest.getCampaignIdNext3rd()).toEqual(campaignIds[0]);

        expect(CampaignTest.getCampaignIdList().count()).toBe(3);
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_descriptionList_table.getText()), HomepageTest.await(50));
        CampaignpageLocatorTest.campaignList_descriptionList_table.then(function (text) {
            for (i = 0; i < text.length; i++) {
                expect(text[i].getText()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldArray[text.length - (i + 1)]);
            }
        });
        expect(CampaignTest.getStatusList()).toContain(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDateList()).toContain(CampaignTest.generateStatusDate());

    });

    //Run this test case from AutoTestUser2
    it('Verify that in advanced search ID field functionality works', async function () {
        var campaignIds = await createMultipleCampaign(2, false);
        expect(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.isPresent()).toBe(true);
        expect(CampaignTest.getAdvancedSearchButtonText()).toEqual(CampaignConstantTest.advancedFilters_advancedFiltersButtonText);
        CampaignTest.clickOnAdvancedSearchButton();
        expect(CampaignTest.getAdvancedSearchIdLabel()).toEqual(CampaignConstantTest.advancedFilters_idLabel);
        expect(CampaignTest.getAdvancedSearchDescriptionLabel()).toEqual(CampaignConstantTest.advancedFilters_descriptionLabel);
        expect(CampaignTest.getAdvancedSearchStatusLabel()).toEqual(CampaignConstantTest.advancedFilters_statusLabel);
        expect(CampaignTest.getAdvancedSearchStatusDateLabel()).toEqual(CampaignConstantTest.advancedFilters_statusDateLabel);
        expect(CampaignpageLocatorTest.advancedFilters_searchFieldId.isPresent()).toBe(true);
        CampaignTest.typeOnAdvancedSearchFieldId(CampaignConstantTest.CampaignIdFromInvalidDatabase);
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);
        CampaignTest.clearAdvancedSearchFieldId();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        CampaignTest.typeOnAdvancedSearchFieldId(campaignIds[0]);
        CampaignTest.typeOnAdvancedSearchFieldId(',');
        CampaignTest.typeOnAdvancedSearchFieldId(campaignIds[1]);
        CampaignTest.typeOnAdvancedSearchFieldId(',');
        CampaignTest.typeOnAdvancedSearchFieldId(campaignIds[2]);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);
        CampaignTest.clearAdvancedSearchFieldId();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        CampaignTest.typeOnAdvancedSearchFieldId(campaignIds[2]);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignIdList().count()).toBe(1);
        expect(CampaignTest.getDescription()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldArray[2]);
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDate()).toEqual(CampaignTest.generateStatusDate());
        CampaignTest.clearAdvancedSearchFieldId();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        CampaignTest.typeOnAdvancedSearchFieldId(CampaignConstantTest.CampaignIdFromDiffDatabaseAuto1);
        CampaignTest.typeOnAdvancedSearchDatePicker(CampaignConstantTest.advancedFilters_statusDateRange);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);
        expect(CampaignpageLocatorTest.advancedFilters_toggleButton.isPresent()).toBe(true);
        expect(CampaignTest.getAdvancedSearchToggleButtonText()).toEqual(CampaignConstantTest.advancedFilters_toggleButtonOn);
        CampaignTest.clickOnAdvancedSearchToggleButton();
        expect(CampaignTest.getAdvancedSearchToggleButtonText()).toEqual(CampaignConstantTest.advancedFilters_toggleButtonOff);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(5);
        expect(CampaignTest.getCampaignId()).toEqual(CampaignConstantTest.CampaignIdFromDiffDatabaseAuto1);
        expect(CampaignTest.getDescription()).toEqual(CampaignConstantTest.newCampaign_descriptionFieldArray[0]);
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDate()).toEqual(CampaignConstantTest.campaignList_statusDate_tableText);

    });

    it('Verify that in advanced search description field functionality works', async function () {
        await createMultipleCampaign(2, false);
        expect(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.isPresent()).toBe(true);
        expect(CampaignTest.getAdvancedSearchButtonText()).toEqual(CampaignConstantTest.advancedFilters_advancedFiltersButtonText);
        CampaignTest.clickOnAdvancedSearchButton();
        expect(CampaignTest.getAdvancedSearchIdLabel()).toEqual(CampaignConstantTest.advancedFilters_idLabel);
        expect(CampaignTest.getAdvancedSearchDescriptionLabel()).toEqual(CampaignConstantTest.advancedFilters_descriptionLabel);
        expect(CampaignTest.getAdvancedSearchStatusLabel()).toEqual(CampaignConstantTest.advancedFilters_statusLabel);
        expect(CampaignTest.getAdvancedSearchStatusDateLabel()).toEqual(CampaignConstantTest.advancedFilters_statusDateLabel);
        expect(CampaignpageLocatorTest.advancedFilters_searchFieldDescription.isPresent()).toBe(true);
        CampaignTest.typeOnAdvancedSearchFieldDescription(CampaignConstantTest.advancedFilters_invalidDescription);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);
        CampaignTest.clearAdvancedSearchFieldDescription();
        CampaignTest.typeOnAdvancedSearchFieldDescription(CampaignConstantTest.advancedFilters_validDescription);
        CampaignTest.clickOnSearchButton();
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_descriptionList_table), HomepageTest.await(50));
        CampaignpageLocatorTest.campaignList_descriptionList_table.then(function (text) {
            for (i = 0; i < text.length; i++) {
                expect(text[i].getText()).toContain(CampaignConstantTest.advancedFilters_validDescription);
            }
        });
        expect(CampaignTest.getStatus()).toContain(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDateList()).toContain(CampaignTest.generateStatusDate());
        CampaignTest.clearAdvancedSearchFieldDescription();
        CampaignTest.typeOnAdvancedSearchFieldDescription(CampaignConstantTest.newCampaign_databaseValueArray[0]);
        CampaignTest.typeOnAdvancedSearchDatePicker(CampaignConstantTest.advancedFilters_statusDateRange);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(5);
        expect(CampaignTest.getDescription()).toEqual(CampaignConstantTest.newCampaign_databaseValueArray[0]);
        expect(CampaignTest.getStatus()).toContain(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDateList()).toContain(CampaignConstantTest.campaignList_statusDate_tableText);

    });

    it('Verify that in advanced search status field functionality works', async function () {
        expect(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.isPresent()).toBe(true);
        expect(CampaignTest.getAdvancedSearchButtonText()).toEqual(CampaignConstantTest.advancedFilters_advancedFiltersButtonText);
        CampaignTest.clickOnAdvancedSearchButton();
        expect(CampaignTest.getAdvancedSearchIdLabel()).toEqual(CampaignConstantTest.advancedFilters_idLabel);
        expect(CampaignTest.getAdvancedSearchDescriptionLabel()).toEqual(CampaignConstantTest.advancedFilters_descriptionLabel);
        expect(CampaignTest.getAdvancedSearchStatusLabel()).toEqual(CampaignConstantTest.advancedFilters_statusLabel);
        expect(CampaignTest.getAdvancedSearchStatusDateLabel()).toEqual(CampaignConstantTest.advancedFilters_statusDateLabel);
        expect(CampaignpageLocatorTest.advancedFilters_statusField.isPresent()).toBe(true);
        CampaignTest.selectAdvancedSearchStatusDropdown(0);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(5);
        expect(CampaignTest.getStatusList()).toContain(CampaignConstantTest.campaignList_status_tableText);

    });

    it('Verify that in advanced search status date (date picker) functionality works', async function () {
        expect(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.isPresent()).toBe(true);
        expect(CampaignTest.getAdvancedSearchButtonText()).toEqual(CampaignConstantTest.advancedFilters_advancedFiltersButtonText);
        CampaignTest.clickOnAdvancedSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getAdvancedSearchIdLabel()).toEqual(CampaignConstantTest.advancedFilters_idLabel);
        expect(CampaignTest.getAdvancedSearchDescriptionLabel()).toEqual(CampaignConstantTest.advancedFilters_descriptionLabel);
        expect(CampaignTest.getAdvancedSearchStatusLabel()).toEqual(CampaignConstantTest.advancedFilters_statusLabel);
        expect(CampaignTest.getAdvancedSearchStatusDateLabel()).toEqual(CampaignConstantTest.advancedFilters_statusDateLabel);
        expect(CampaignpageLocatorTest.advancedFilters_statusDateField.isPresent()).toBe(true);
        CampaignTest.searchAdvancedSearchDatePicker();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getStatusDate()).toContain(CampaignTest.generateStatusDate(), CampaignTest.generatePreviousStatusDate());

    });

    it('Verify that in advanced search combination of id,description,status and status date functionality works', async function () {
        expect(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.isPresent()).toBe(true);
        expect(CampaignTest.getAdvancedSearchButtonText()).toEqual(CampaignConstantTest.advancedFilters_advancedFiltersButtonText);
        CampaignTest.clickOnAdvancedSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getAdvancedSearchIdLabel()).toEqual(CampaignConstantTest.advancedFilters_idLabel);
        expect(CampaignTest.getAdvancedSearchDescriptionLabel()).toEqual(CampaignConstantTest.advancedFilters_descriptionLabel);
        expect(CampaignTest.getAdvancedSearchStatusLabel()).toEqual(CampaignConstantTest.advancedFilters_statusLabel);
        expect(CampaignTest.getAdvancedSearchStatusDateLabel()).toEqual(CampaignConstantTest.advancedFilters_statusDateLabel);
        expect(CampaignpageLocatorTest.advancedFilters_searchFieldId.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.advancedFilters_searchFieldDescription.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.advancedFilters_statusDateField.isPresent()).toBe(true);
        expect(CampaignpageLocatorTest.advancedFilters_statusField.isPresent()).toBe(true);
        CampaignTest.typeOnAdvancedSearchFieldId(CampaignTest.getCampaignId());
        CampaignTest.typeOnAdvancedSearchFieldDescription(CampaignTest.getDescription());
        CampaignTest.selectAdvancedSearchStatusDropdown(0);
        CampaignTest.searchAdvancedSearchDatePicker();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getStatusList()).toContain(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDate()).toContain(CampaignTest.generateStatusDate(), CampaignTest.generatePreviousStatusDate());
        CampaignTest.searchAdvancedSearchDatePickerNext2Days();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);
    });

    it('Verify that when we put campaign id both on advance search and normal search it should show proper output functionality works', async function () {
        var idNext = CampaignTest.getCampaignIdNext2nd();
        expect(CampaignpageLocatorTest.campaignList_searchField.isPresent()).toBe(true);
        CampaignTest.typeOnSearchField(CampaignTest.getCampaignId());
        expect(CampaignpageLocatorTest.advancedFilters_advancedFiltersButton.isPresent()).toBe(true);
        expect(CampaignTest.getAdvancedSearchButtonText()).toEqual(CampaignConstantTest.advancedFilters_advancedFiltersButtonText);
        CampaignTest.clickOnAdvancedSearchButton();
        CampaignTest.typeOnAdvancedSearchFieldId(CampaignTest.getCampaignId());
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        CampaignTest.clearAdvancedSearchFieldId();
        CampaignTest.clearSearchFieldByClear();
        CampaignTest.typeOnSearchField(CampaignTest.getCampaignId());
        CampaignTest.typeOnAdvancedSearchFieldId(idNext);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);

    });

    it('Verify that search functionality works', async function () {
        expect(CampaignpageLocatorTest.campaignList_searchField.isPresent()).toBe(true);
        CampaignTest.typeOnSearchField(CampaignTest.getCampaignId());
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getTotalCount()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
        CampaignTest.clearSearchField();
        CampaignTest.typeOnSearchField(CampaignConstantTest.CampaignIdFromInvalidDatabase);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getNoDataTableText()).toEqual(CampaignConstantTest.campaignList_noData_table);
        expect(CampaignTest.getCountTableText()).toEqual(CampaignConstantTest.campaignList_totalCount_zero);
        CampaignTest.clearSearchField();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(CampaignTest.getCampaignId());
        CampaignTest.typeOnSearchField(",");
        CampaignTest.typeOnSearchField(CampaignTest.getCampaignIdNext2nd());
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getTotalCount()).toEqual(CampaignConstantTest.campaignList_totalCount_two);
        CampaignTest.clearSearchField();
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        HomepageTest.wait(10);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[0], CampaignConstantTest.newCampaign_descriptionFieldSpecialChar, CampaignConstantTest.newCampaign_offerFieldSpecialChar, true);
        CampaignTest.selectGrossRadioButton();
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(10);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(CampaignConstantTest.newCampaign_descriptionFieldSpecialChar);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getDescription()).toContain(CampaignConstantTest.newCampaign_descriptionFieldSpecialChar);
        CampaignTest.clearSearchField();
        CampaignTest.typeOnSearchField(CampaignConstantTest.campaignList_singleQuote);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getDescription()).toContain(CampaignConstantTest.campaignList_singleQuote);

    });

    it('Verify system doesnt break when single quotes is entered in campaign description or offer', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        var campaignId1st = await createCampaignCustom(CampaignConstantTest.newCampaign_databaseValue[0], CampaignConstantTest.campaignList_singleQuote, CampaignConstantTest.campaignList_singleQuote, false);
        HomepageTest.wait(20);
        CampaignTest.typeOnSearchField(campaignId1st);
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignIdList().count()).toBe(1);
        expect(CampaignTest.getDescription()).toEqual(CampaignConstantTest.campaignList_singleQuote);
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_tableText);
        expect(CampaignTest.getStatusDate()).toEqual(CampaignTest.generateStatusDate());
        expect(CampaignTest.getTotalCount()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);

    });

    it('Verify search doesnt break when single quotes are entered in search values', async function () {
        expect(CampaignpageLocatorTest.campaignList_searchField.isPresent()).toBe(true);
        CampaignTest.typeOnSearchField(CampaignConstantTest.campaignList_singleQuote);
        CampaignTest.clickOnSearchButton();
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
        HomepageTest.wait(10);
        expect(CampaignTest.getDescriptionList()).toContain(CampaignConstantTest.campaignList_singleQuote);
        expect(CampaignTest.getStatusList()).toContain(CampaignConstantTest.campaignList_status_tableText);
    });

    it('Verify that grid is refreshed after saving new campaign', async function () {
        var before = await CampaignTest.getTotalCountNum();
        await createMultipleCampaign(0, false);
        var after = await CampaignTest.getTotalCountNum();
        if (after > before) {
            console.log("Grid refreshed successfully ");
        } else {
            fail('Grid is not refreshed');
        }
    });

    it('Select duration for displaying campaign', function () {
        HomepageTest.wait();
        CampaignTest.campaignDuration(3);
    });

    afterEach(function () {
        HomepageTest.wait(20);
        DashboardTest.clickOnUser();
        DashboardTest.clickOnLogout();
    });

});
