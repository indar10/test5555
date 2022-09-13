// Put x before 'it' to exclude the test case
// Put f before 'it' to focus(include) the test case
describe('IDMS Selection screen test', function () {
    var DashboardTest = new Dashboard();
    var CampaignTest = new Campaign();
    var HomepageTest = new Homepage();
    var newCampaignTest = new newCampaign();
    var CampaignConstantTest = new CampaignConstant();
    var newCampaignConstantTest = new newCampaignConstant();
    var CampaignstatusTest = new Campaignstatus();
    var CampaignstatusconstantTest = new Campaignstatusconstant();
    var CampaignstatuspagelocatorTest = new CampaignstatuspageLocator();
    var selectionTest = new Selection();
    var selectionConstantTest = new SelectionConstant();

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

    it('Verify selection screen contains 2 groups and total of 7 rules.', async function () {

        //Create campaign and go to selection screen.
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(20);
        await CampaignstatusTest.filterSelectSegement(CampaignstatusconstantTest.selectionScreen_selectSgement[0]);
        HomepageTest.wait(10);
        expect(await CampaignstatusTest.selectionScreen_getSelectionsLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_selectionLabel);
        expect(await CampaignstatusTest.selectionScreen_getCountStatusLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_statusCountLabel);

        //Group 1.
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[1]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirstLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextFirst);
        await CampaignstatusTest.clickOnOptionCheckBoxFirst();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[2]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecondLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextSecond);
        await CampaignstatusTest.clickOnOptionCheckBoxSecond();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[0]);
        await selectionTest.typeOnRuleDropdown(0, selectionConstantTest.selection_addressTypeValues);
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[3]);
        await selectionTest.typeOnRuleTextArea(0, selectionConstantTest.selection_companyNameValues);

        //Group 2.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[4]);
        await selectionTest.typeOnRuleTextArea(1, selectionConstantTest.selection_IGIDValues);
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[5]);
        await selectionTest.checkAdSizeCodeValues();
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[6]);
        await selectionTest.typeOnRuleDropdown(1, selectionConstantTest.selection_callStatusCode);

        //Exit.
        HomepageTest.wait(30);
        await CampaignstatusTest.clickOnSegmentSaveButton();
        expect(await CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_filterSaveSuccessfullyText);
    });

    it('Verify selection screen contains 3 groups and total of 10 rules.', async function () {

        //Create campaign and go to selection screen.
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(20);
        await CampaignstatusTest.filterSelectSegement(CampaignstatusconstantTest.selectionScreen_selectSgement[0]);
        HomepageTest.wait(10);
        expect(await CampaignstatusTest.selectionScreen_getSelectionsLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_selectionLabel);
        expect(await CampaignstatusTest.selectionScreen_getCountStatusLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_statusCountLabel);

        //Group 1.
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[1]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirstLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextFirst);
        await CampaignstatusTest.clickOnOptionCheckBoxFirst();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[2]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecondLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextSecond);
        await CampaignstatusTest.clickOnOptionCheckBoxSecond();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[0]);
        await selectionTest.typeOnRuleDropdown(0, selectionConstantTest.selection_addressTypeValues);
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[3]);
        await selectionTest.typeOnRuleTextArea(0, selectionConstantTest.selection_companyNameValues);

        //Group 2.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[4]);
        await selectionTest.typeOnRuleTextArea(1, selectionConstantTest.selection_IGIDValues);
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[5]);
        await selectionTest.checkAdSizeCodeValues();
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[6]);
        await selectionTest.typeOnRuleDropdown(1, selectionConstantTest.selection_callStatusCode);

        //Group 3.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[7]);
        await selectionTest.typeOnRuleTextArea(2, selectionConstantTest.selection_affiliatedLocations);
        await selectionTest.clickOnAddRuleButton(2);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[8]);
        await selectionTest.clickOnAddRuleButton(2);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[9]);
        await selectionTest.typeOnRuleDropdown(2, selectionConstantTest.selection_locationStates);

        //Exit.
        HomepageTest.wait(30);
        await CampaignstatusTest.clickOnSegmentSaveButton();
        expect(await CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_filterSaveSuccessfullyText);
    });

    it('Verify selection screen contains 4 groups and total of 13 rules.', async function () {

        //Create campaign and go to selection screen.
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(20);
        await CampaignstatusTest.filterSelectSegement(CampaignstatusconstantTest.selectionScreen_selectSgement[0]);
        HomepageTest.wait(10);
        expect(await CampaignstatusTest.selectionScreen_getSelectionsLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_selectionLabel);
        expect(await CampaignstatusTest.selectionScreen_getCountStatusLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_statusCountLabel);

        //Group 1.
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[1]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirstLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextFirst);
        await CampaignstatusTest.clickOnOptionCheckBoxFirst();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[2]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecondLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextSecond);
        await CampaignstatusTest.clickOnOptionCheckBoxSecond();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[0]);
        await selectionTest.typeOnRuleDropdown(0, selectionConstantTest.selection_addressTypeValues);
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[3]);
        await selectionTest.typeOnRuleTextArea(0, selectionConstantTest.selection_companyNameValues);

        //Group 2.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[4]);
        await selectionTest.typeOnRuleTextArea(1, selectionConstantTest.selection_IGIDValues);
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[5]);
        await selectionTest.checkAdSizeCodeValues();
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[6]);
        await selectionTest.typeOnRuleDropdown(1, selectionConstantTest.selection_callStatusCode);

        //Group 3.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[7]);
        await selectionTest.typeOnRuleTextArea(2, selectionConstantTest.selection_affiliatedLocations);
        await selectionTest.clickOnAddRuleButton(2);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[8]);
        await selectionTest.clickOnAddRuleButton(2);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[9]);
        await selectionTest.typeOnRuleDropdown(2, selectionConstantTest.selection_locationStates);

        //Group 4.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[10]);
        await selectionTest.typeOnRuleTextArea(3, selectionConstantTest.selection_locationCity);
        await selectionTest.clickOnAddRuleButton(3);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[11]);
        await selectionTest.checkExpenseAccountingValues();
        await selectionTest.clickOnAddRuleButton(3);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[12]);
        await selectionTest.typeOnRuleDropdown(3, selectionConstantTest.selection_contactTitleRoleCode);

        //Exit.
        HomepageTest.wait(30);
        await CampaignstatusTest.clickOnSegmentSaveButton();
        expect(await CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_filterSaveSuccessfullyText);
    });

    it('Verify selection screen contains 5 groups and total of 16 rules.', async function () {

        //Create campaign and go to selection screen.
        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(20);
        await CampaignstatusTest.filterSelectSegement(CampaignstatusconstantTest.selectionScreen_selectSgement[0]);
        HomepageTest.wait(10);
        expect(await CampaignstatusTest.selectionScreen_getSelectionsLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_selectionLabel);
        expect(await CampaignstatusTest.selectionScreen_getCountStatusLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_statusCountLabel);

        //Group 1.
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[1]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirstLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextFirst);
        await CampaignstatusTest.clickOnOptionCheckBoxFirst();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[2]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecondLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextSecond);
        await CampaignstatusTest.clickOnOptionCheckBoxSecond();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[0]);
        await selectionTest.typeOnRuleDropdown(0, selectionConstantTest.selection_addressTypeValues);
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[3]);
        await selectionTest.typeOnRuleTextArea(0, selectionConstantTest.selection_companyNameValues);

        //Group 2.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[4]);
        await selectionTest.typeOnRuleTextArea(1, selectionConstantTest.selection_IGIDValues);
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[5]);
        await selectionTest.checkAdSizeCodeValues();
        await selectionTest.clickOnAddRuleButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[6]);
        await selectionTest.typeOnRuleDropdown(1, selectionConstantTest.selection_callStatusCode);

        //Group 3.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[7]);
        await selectionTest.typeOnRuleTextArea(2, selectionConstantTest.selection_affiliatedLocations);
        await selectionTest.clickOnAddRuleButton(2);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[8]);
        await selectionTest.clickOnAddRuleButton(2);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[9]);
        await selectionTest.typeOnRuleDropdown(2, selectionConstantTest.selection_locationStates);

        //Group 4.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[10]);
        await selectionTest.typeOnRuleTextArea(3, selectionConstantTest.selection_locationCity);
        await selectionTest.clickOnAddRuleButton(3);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[11]);
        await selectionTest.checkExpenseAccountingValues();
        await selectionTest.clickOnAddRuleButton(3);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[12]);
        await selectionTest.typeOnRuleDropdown(3, selectionConstantTest.selection_contactTitleRoleCode);

        //Group 5.
        await selectionTest.clickOnAddGroupButton(1);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[13]);
        await selectionTest.typeOnRuleTextArea(4, selectionConstantTest.selection_locationCountryCode);
        await selectionTest.clickOnAddRuleButton(4);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[14]);
        await selectionTest.checkBusinessStatusValues();
        await selectionTest.clickOnAddRuleButton(4);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[15]);
        await selectionTest.typeOnRuleDropdown(4, selectionConstantTest.selection_contactProfessionalTitle);

        //Exit.
        HomepageTest.wait(30);
        await CampaignstatusTest.clickOnSegmentSaveButton();
        expect(await CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_filterSaveSuccessfullyText);
    });

    afterEach(function () {
        HomepageTest.wait(30);
        DashboardTest.clickOnUser();
        DashboardTest.clickOnLogout();
    });
});