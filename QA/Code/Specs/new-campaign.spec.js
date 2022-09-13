// Put x before 'it' to exclude the test case
// Put f before 'it' to focus(include) the test case
describe('IDMS "New Campaign(No default mailer)" screen test', function () {

    var CampaignTest = new Campaign();
    var HomepageTest = new Homepage();
    var DashboardTest = new Dashboard();
    var newCampaignTest = new newCampaign();
    var newCampaignConstantTest = new newCampaignConstant();
    var CampaignConstantTest = new CampaignConstant();
    var CampaignpageLocatorTest = new CampaignpageLocator();
    var newCampaignLocatorTest = new newCampaignpageLocator();
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
        HomepageTest.wait(30);
    });

    it('Verify new campaign screen pops up on clicking "New Campaign" button.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        console.log(await newCampaignTest.getAllCampaignListTableLabel());
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Database drop down is enabled.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        CampaignTest.clickOnDatabaseFieldDropdown();
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify search works in the database drop down.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        CampaignTest.clickOnDatabaseFieldDropdown();
        newCampaignTest.typeOnDatabaseSearchField(newCampaignConstantTest.newCampaign_allDatabaseName[2]);
        newCampaignTest.selectFirstSearchElement();
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedDatabase()).toEqual(newCampaignConstantTest.newCampaign_allDatabases[2]);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify search works with Database ID.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        CampaignTest.clickOnDatabaseFieldDropdown();
        HomepageTest.wait(10);
        newCampaignTest.typeOnDatabaseSearchField(newCampaignConstantTest.newCampaign_allDatabaseID[1]);
        newCampaignTest.selectFirstSearchElement();
        HomepageTest.wait(10);
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedDatabase()).toEqual(newCampaignConstantTest.newCampaign_allDatabases[1]);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify search works with database name.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        CampaignTest.clickOnDatabaseFieldDropdown();
        HomepageTest.wait(10);
        newCampaignTest.typeOnDatabaseSearchField(newCampaignConstantTest.newCampaign_allDatabaseName[0]);
        newCampaignTest.selectFirstSearchElement();
        HomepageTest.wait(10);
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.nonDivisionalIteration, newCampaignConstantTest.nonDivisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalAllLabels);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedDatabase()).toEqual(newCampaignConstantTest.newCampaign_allDatabases[0]);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Database Dropdown shows only databases user has access to.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        CampaignTest.clickOnDatabaseFieldDropdown();
        HomepageTest.wait(10);
        var database = await newCampaignTest.getDatabaseListNDM();
        expect(database.length).toBe(newCampaignConstantTest.newCampaign_databaseCountNDM);
        for (i = 0; i < database.length; i++) {
            expect(database[i]).toEqual(newCampaignConstantTest.newCampaign_allDatabases[i]);
        }
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the database drop down values are sorted alphabetically.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        CampaignTest.clickOnDatabaseFieldDropdown();
        HomepageTest.wait(10);
        var database = await newCampaignTest.getDatabaseListNDM();
        expect(database.length).toBe(newCampaignConstantTest.newCampaign_databaseCountNDM);
        expect(await newCampaignTest.getSortedDatabaseIds()).toEqual(newCampaignConstantTest.newCampaign_allDatabaseID);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the default database appear correctly.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        HomepageTest.wait(10);
        CampaignTest.clickOnDatabaseFieldDropdown();
        newCampaignTest.typeOnDatabaseSearchField(newCampaignConstantTest.newCampaign_allDatabaseName[0]);
        newCampaignTest.selectFirstSearchElement();
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedDatabase()).toEqual(newCampaignConstantTest.newCampaign_allDatabases[0]);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the most recently database appears as default on all new campaigns.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(20);
        CampaignTest.clickOnFilterCrossSymbol();
        CampaignTest.clickOnNewChampaignButton();
        HomepageTest.wait(20);
        expect(await newCampaignTest.getSelectedDatabase()).toContain(newCampaignConstantTest.newCampaign_allDatabases[1]);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the default Campaign Description appear correctly.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        expect(await CampaignTest.getDescriptionField()).toEqual(CampaignConstantTest.newCampaign_newCampaignText + CampaignTest.getCurrentDate());
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Description, Customer and Broker fields are manadatory for Divisional campaigns (Ex: Database ID - 992).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        await newCampaignTest.newCampaignMandatoryFields(newCampaignConstantTest.mandatoryDivisionalFields, true);
        HomepageTest.wait(10);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isEnabled()).toBe(true);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify offer is not mandatory and it is a text field for Divisional campaigns (Ex: Database ID - 992).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        await newCampaignTest.newCampaignMandatoryFields(newCampaignConstantTest.mandatoryDivisionalFields, true);
        expect(CampaignTest.getOfferField()).toEqual('');
        HomepageTest.wait(20);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isEnabled()).toBe(true);
        CampaignTest.clickOnCrossSymbolHeader();
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        CampaignTest.clickOnNewChampaignButton();
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.divisionalIteration, newCampaignConstantTest.divisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_divisionalAllLabels);
        await newCampaignTest.newCampaignMandatoryFields(newCampaignConstantTest.mandatoryDivisionalFields, true);
        await newCampaignTest.typeOnDivisionalOfferField(CampaignConstantTest.newCampaign_offerFieldText);
        HomepageTest.wait(20);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isEnabled()).toBe(true);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Description, Customer and offer fields are manadatory for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.newCampaignMandatoryFields(newCampaignConstantTest.mandatoryNonDivisionalFields, true);
        HomepageTest.wait(10);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isEnabled()).toBe(true);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Broker field doesnt appear for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.nonDivisionalIteration, newCampaignConstantTest.nonDivisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalAllLabels);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify offer field is a drop down for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getAllNewCampaignLabel(newCampaignConstantTest.nonDivisionalIteration, newCampaignConstantTest.nonDivisionalLabelCount)).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalAllLabels);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.nonDivisionalValues.customer);
        HomepageTest.wait(10);
        expect(newCampaignLocatorTest.newCampaign_offerDropdown_nonDivisional.isEnabled()).toBe(true);
        newCampaignLocatorTest.newCampaign_offerDropdown_nonDivisional.click();
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });
    //User AutoTestUserB for this test case.
    xit('Verify the correct default broker appears if the default broker is set up for the user account for Divisional campaigns (Ex: Database ID - 992).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[2]);
        expect(await newCampaignTest.getSelectedBroker()).toEqual(newCampaignConstantTest.newCampaign_defaultBroker);
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the no default broker appears if it is not set up for the user account for Divisional campaigns (Ex: Database ID - 992).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[2]);
        expect(await newCampaignTest.getSelectedBroker()).toEqual('');
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the search works when 2 or more chars and Customer id are entered in the Customer search box for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_twoChar);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalCustomerNameId);
        await newCampaignTest.clearCustomerField(true);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_moreThanTwoChar);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalCustomerNameId);
        await newCampaignTest.clearCustomerField(true);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_nonDivisionalCustomerId);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalCustomerNameId);
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the search works when 2 or more chars and Broker id are entered in the Broker search box for Divisional campaigns (Ex: Database ID - 992).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[2]);
        HomepageTest.wait(10);
        await newCampaignTest.selectBroker(newCampaignConstantTest.newCampaign_twoChar);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedBroker()).toEqual(newCampaignConstantTest.newCampaign_divisionalBrokerNameId);
        await newCampaignTest.clearBrokerField(true);
        await newCampaignTest.selectBroker(newCampaignConstantTest.newCampaign_moreThanTwoChar);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedBroker()).toEqual(newCampaignConstantTest.newCampaign_divisionalBrokerNameId);
        await newCampaignTest.clearBrokerField(true);
        await newCampaignTest.selectBroker(newCampaignConstantTest.newCampaign_divisionalBrokerId);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedBroker()).toEqual(newCampaignConstantTest.newCampaign_divisionalBrokerNameId);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify when a customer has only one active offer, it gets populated by default on the Offer field for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_nonDivisionalCustomerAdmin);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedOffer()).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalOffer);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify when a customer has more than one active offer, no offer is selected by default for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_nonDivisionalCustomerId);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedOffer()).toEqual(newCampaignConstantTest.newCampaign_multipleOfferList[0]);
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the search works when 2 or more chars and Customer id are entered in the Customer search box for Divisional campaigns (Ex: Database ID - 992).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[2]);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_twoChar);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_divisionalCustomerNameIdTest);
        await newCampaignTest.clearCustomerField(true);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_moreThanTwoChar);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_divisionalCustomerNameIdTest);
        await newCampaignTest.clearCustomerField(true);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_divisionalCustomerId);
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_divisionalCustomerNameId);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify when selecting a different customer, the offer drop down gets updated with the offers belonging to the selected customer for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_nonDivisionalCustomerId);
        await newCampaignTest.clickOnOfferDropdown();
        expect(await newCampaignTest.getDropdownList()).toEqual(newCampaignConstantTest.newCampaign_multipleOfferList);
        HomepageTest.wait(10);
        await newCampaignTest.clickOnOfferDropdown();
        await newCampaignTest.clearCustomerField(true);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_nonDivisionalCustomerAdmin);
        await newCampaignTest.clickOnOfferDropdown();
        HomepageTest.wait(10);
        expect(await newCampaignTest.getDropdownList()).toEqual(newCampaignConstantTest.newCampaign_singleOfferList);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the Customer drop down shows all the active customers and Broker drop down shows all the active divisional brokers for Divisional campaigns (Ex: Database ID - 992).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[2]);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_divisionalCustomerId);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_divisionalCustomerNameId);
        HomepageTest.wait(10);
        await newCampaignTest.selectBroker(newCampaignConstantTest.newCampaign_divisionalBrokerId);
        expect(await newCampaignTest.getSelectedBroker()).toEqual(newCampaignConstantTest.newCampaign_divisionalBrokerNameId);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify the Customer drop down shows only the customers the user has access to , search works with offer name, search works with offer ID for Non-Divisional campaigns (Ex: Database ID - 65).', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.newCampaign_nonDivisionalCustomerId);
        expect(await newCampaignTest.getSelectedCustomer()).toEqual(newCampaignConstantTest.newCampaign_nonDivisionalCustomerNameId);
        await newCampaignTest.clickOnOfferDropdown();
        HomepageTest.wait(10);
        expect(await newCampaignTest.getDropdownList()).toEqual(newCampaignConstantTest.newCampaign_multipleOfferList);
        await newCampaignTest.clickOnOfferDropdown();
        await newCampaignTest.clickOnOfferDropdown();
        await newCampaignTest.selectNonDivisionalOffer(newCampaignTest.getOfferValues(1, 0));
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedOffer()).toEqual(newCampaignConstantTest.newCampaign_multipleOfferList[1]);
        await newCampaignTest.clickOnOfferDropdown();
        await newCampaignTest.clearDropdownSearchfield();
        await newCampaignTest.selectNonDivisionalOffer(newCampaignTest.getOfferValues(2, 1));
        HomepageTest.wait(10);
        expect(await newCampaignTest.getSelectedOffer()).toEqual(newCampaignConstantTest.newCampaign_multipleOfferList[2]);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify search doesnt break with single quotes in the Database, Customer, Broker and build drop downs for both Divisional campaigns.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[2]);
        HomepageTest.wait(10);
        await newCampaignTest.typeOnCustomerField(newCampaignConstantTest.newCampaign_singleQuote);
        await newCampaignTest.typeOnBrokerField(newCampaignConstantTest.newCampaign_singleQuote);
        await newCampaignTest.selectBuild(newCampaignConstantTest.newCampaign_singleQuote);
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Save button is not enabled when any of the required fields is left blank or not selected in Divisional campaigns.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[2]);
        await CampaignTest.clearDescriptionField();
        HomepageTest.wait(10);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.typeOnDescriptionField(CampaignConstantTest.newCampaign_descriptionFieldText);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.mandatoryDivisionalFields.customer);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.selectBroker(newCampaignConstantTest.mandatoryDivisionalFields.broker);
        expect(await newCampaignTest.checkSaveButton()).toBe(true);
        await newCampaignTest.clearCustomerField(false);
        HomepageTest.wait(10);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.mandatoryDivisionalFields.customer);
        HomepageTest.wait(10);
        await newCampaignTest.clearBrokerField(false);
        HomepageTest.wait(20);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await CampaignTest.clearDescriptionField();
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    it('Verify Save button is not enabled when any of the required fields is left blank or not selected in Non-Divisional campaigns.', async function () {
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        CampaignTest.clickOnNewChampaignButton();
        expect(CampaignTest.getNewCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_newCampaignTitle);
        await newCampaignTest.selectDatabase(newCampaignConstantTest.newCampaign_allDatabases[5]);
        HomepageTest.wait(10);
        await CampaignTest.clearDescriptionField();
        HomepageTest.wait(10);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.typeOnDescriptionField(CampaignConstantTest.newCampaign_descriptionFieldText);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.mandatoryNonDivisionalFields.customer);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.clickOnOfferDropdown();
        await newCampaignTest.selectNonDivisionalOffer(newCampaignTest.getOfferValues(1, 0));
        expect(await newCampaignTest.checkSaveButton()).toBe(true);
        await newCampaignTest.clearCustomerField(false);
        HomepageTest.wait(10);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await newCampaignTest.selectCustomer(newCampaignConstantTest.mandatoryNonDivisionalFields.customer);
        HomepageTest.wait(10);
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        await CampaignTest.clearDescriptionField();
        expect(await newCampaignTest.checkSaveButton()).toBe(false);
        HomepageTest.wait(10);
        CampaignTest.clickOnCrossSymbolHeader();
    });

    afterEach(function () {
        HomepageTest.wait(20);
        DashboardTest.clickOnUser();
        DashboardTest.clickOnLogout();
    });

});