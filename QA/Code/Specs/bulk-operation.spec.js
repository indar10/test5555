// Put x before 'it' to exclude the test case
// Put f before 'it' to focus(include) the test case
describe('IDMS Selection screen test', function () {
    var DashboardTest = new Dashboard();
    var CampaignTest = new Campaign();
    var HomepageTest = new Homepage();
    var newCampaignTest = new newCampaign();
    var CampaignConstantTest = new CampaignConstant();
    var CampaignpageLocatorTest = new CampaignpageLocator();
    var newCampaignConstantTest = new newCampaignConstant();
    var BulkOperationTest = new Bulkoperation();
    var BulkoperationconstantTest = new Bulkoperationconstant();
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

    it('Verify user can able to switch between the screens by using the toggle button in Edit Segment screen.', async function () {

        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(10);
        await BulkOperationTest.clickOnSplitButton();
        expect(await BulkOperationTest.getAllActionsListLabel()).toEqual(BulkoperationconstantTest.bulkOperation_actions);
        await BulkOperationTest.clickOnBulkActions(6);
        expect(await BulkOperationTest.getAllEditSegmentGridLabel()).toEqual(BulkoperationconstantTest.bulkOperation_editSegmentGrid);
        expect(await BulkOperationTest.getAllFieldLabel()).toEqual(BulkoperationconstantTest.bulkOperation_fieldLabels);
        await BulkOperationTest.typeOnSearchField(BulkoperationconstantTest.bulkOperation_segments[0]);
        HomepageTest.wait(10);
        expect(await BulkOperationTest.getTotalCount(2)).toBe(BulkoperationconstantTest.bulkOperation_totalCountValues[0]);
        expect(await BulkOperationTest.getToggleButtonText()).toEqual(BulkoperationconstantTest.bulkOperation_toggleButton);
        await BulkOperationTest.clickOnToggleButton();
        expect(await BulkOperationTest.getAllInlineGridLabel(8)).toEqual(BulkoperationconstantTest.bulkOperation_inlineGrid);
        HomepageTest.wait(10);
        expect(await BulkOperationTest.getTotalCount(1)).toBe(BulkoperationconstantTest.bulkOperation_totalCountValues[0]);
        await HomepageTest.wait(10);
        await BulkOperationTest.clickOnFooterButtons(1);
    });

    it('Verify changes made to the inline edit segment should reflect in the edit segment.', async function () {

        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(10);
        await BulkOperationTest.clickOnSelectionActionButton(1);
        expect(await BulkOperationTest.getAllSelectionActionLabel()).toEqual(BulkoperationconstantTest.selection_actions);
        await BulkOperationTest.clickOnSelectionActions(5);
        expect(await BulkOperationTest.getAllAddSegmentLabel(4)).toEqual(BulkoperationconstantTest.selection_addSegment);
        await BulkOperationTest.createSegment(2, BulkoperationconstantTest.createSegmentValues);
        BulkOperationTest.clickOnSaveAddSegment();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        HomepageTest.wait(10);
        await BulkOperationTest.clickOnSplitButton();
        expect(await BulkOperationTest.getAllActionsListLabel()).toEqual(BulkoperationconstantTest.bulkOperation_actions);
        await BulkOperationTest.clickOnBulkActions(6);
        expect(await BulkOperationTest.getAllEditSegmentGridLabel()).toEqual(BulkoperationconstantTest.bulkOperation_editSegmentGrid);
        expect(await BulkOperationTest.getAllFieldLabel()).toEqual(BulkoperationconstantTest.bulkOperation_fieldLabels);
        await BulkOperationTest.typeOnSearchField(BulkoperationconstantTest.bulkOperation_segments[1]);
        HomepageTest.wait(10);
        expect(await BulkOperationTest.getTotalCount(2)).toBe(BulkoperationconstantTest.bulkOperation_totalCountValues[1]);
        expect(await BulkOperationTest.getToggleButtonText()).toEqual(BulkoperationconstantTest.bulkOperation_toggleButton);
        await BulkOperationTest.clickOnToggleButton();
        expect(await BulkOperationTest.getAllInlineGridLabel(8)).toEqual(BulkoperationconstantTest.bulkOperation_inlineGrid);
        HomepageTest.wait(10);
        expect(await BulkOperationTest.getTotalCount(1)).toBe(BulkoperationconstantTest.bulkOperation_totalCountValues[1]);
        HomepageTest.wait(10);
        expect(await BulkOperationTest.getAllInLineEditSegmentSecondRow()).toEqual(BulkOperationTest.createSegmentValuesArray(BulkoperationconstantTest.createSegmentValues));
        await BulkOperationTest.editInLineEditSegmentSecondRow(BulkoperationconstantTest.editSegmentValues);
        await BulkOperationTest.clickOnFooterButtons(3);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        await BulkOperationTest.clickOnSelectionActionButton(1);
        await BulkOperationTest.clickOnSelectionActions(7);
        HomepageTest.wait(10);
        expect(await BulkOperationTest.getAllEditSegmentValues(4)).toEqual(BulkOperationTest.editSegmentValuesArray(BulkoperationconstantTest.editSegmentValues));
        await BulkOperationTest.clickOnCancelAddSegment();
    });

    it('Verify output quantity column should appears when campaign is at status 40 count completed in Inline Editing Edit segment screen.', async function () {

        expect(await newCampaignTest.getCampaignTitle()).toEqual(CampaignConstantTest.newCampaign_title);
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        await newCampaignTest.createCampaignNDM(newCampaignConstantTest.divsionalValues, true, true);
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);
        await CampaignstatusTest.filterSelectSegement(CampaignstatusconstantTest.selectionScreen_selectSgement[0]);
        HomepageTest.wait(10);
        expect(await CampaignstatusTest.selectionScreen_getSelectionsLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_selectionLabel);
        expect(await CampaignstatusTest.selectionScreen_getCountStatusLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_statusCountLabel);

        //Group
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[1]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirstLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextFirst);
        await CampaignstatusTest.clickOnOptionCheckBoxFirst();
        await selectionTest.clickOnAddRuleButton(0);
        await selectionTest.typeOnRule(selectionConstantTest.selection_fieldName[2]);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecondLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextSecond);
        await CampaignstatusTest.clickOnOptionCheckBoxSecond();
        HomepageTest.wait(20);
        await CampaignstatusTest.clickOnSegmentSaveButton();
        expect(await CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_filterSaveSuccessfullyText);
        HomepageTest.wait(10);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        expect(await newCampaignTest.getAllCampaignListTableLabel()).toEqual(newCampaignConstantTest.campaignList_allTableLabels);
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);
        HomepageTest.wait(10);
        CampaignTest.clickOnCampaignActionDropdown();
        HomepageTest.wait(10);
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        var successfullMsg = await CampaignstatusTest.getSuccessfullMsgWithId();
        expect(successfullMsg[0].trim()).toEqual(CampaignstatusconstantTest.campaignList_campaignText);
        expect(successfullMsg[1].trim()).toEqual(CampaignTest.getCampaignId());
        expect(successfullMsg[2].trim()).toEqual(CampaignstatusconstantTest.campaignList_submittedText);
        expect(successfullMsg[3].trim()).toEqual(CampaignstatusconstantTest.campaignList_successfullyDotText);

        var num = await BulkOperationTest.refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            expect(CampaignpageLocatorTest.campaignList_status_table.getText()).toEqual(CampaignConstantTest.campaignList_status_Count40Text);
            expect(CampaignpageLocatorTest.campaignList_statusDate_table.getText()).toEqual(CampaignTest.generateStatusDate());
            expect(CampaignpageLocatorTest.campaignList_totalCount_table.getText()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
            await BulkOperationTest.clickOnDescription_campaignList();
            await HomepageTest.wait(20);
            await BulkOperationTest.clickOnSplitButton();
            expect(await BulkOperationTest.getAllActionsListLabel()).toEqual(BulkoperationconstantTest.bulkOperation_actions);
            await BulkOperationTest.clickOnBulkActions(6);
            expect(await BulkOperationTest.getAllEditSegmentGridLabel()).toEqual(BulkoperationconstantTest.bulkOperation_editSegmentGrid);
            expect(await BulkOperationTest.getAllFieldLabel()).toEqual(BulkoperationconstantTest.bulkOperation_fieldLabels);
            await BulkOperationTest.typeOnSearchField(BulkoperationconstantTest.bulkOperation_segments[0]);
            HomepageTest.wait(10);
            expect(await BulkOperationTest.getTotalCount(2)).toBe(BulkoperationconstantTest.bulkOperation_totalCountValues[0]);
            expect(await BulkOperationTest.getToggleButtonText()).toEqual(BulkoperationconstantTest.bulkOperation_toggleButton);
            await BulkOperationTest.clickOnToggleButton();
            expect(await BulkOperationTest.getAllInlineGridLabel(9)).toEqual(BulkoperationconstantTest.bulkOperation_inlineGridOutputQty);
            HomepageTest.wait(10);
            expect(await BulkOperationTest.getTotalCount(1)).toBe(BulkoperationconstantTest.bulkOperation_totalCountValues[0]);

            var outputQty = await BulkOperationTest.getInlineEditOutputQty_firstRow();
            outputQty = outputQty.replace(/,/g, "");
            await HomepageTest.wait(10);
            await BulkOperationTest.clickOnFooterButtons(1);
            await HomepageTest.wait(20);
            await browser.executeScript('window.scrollTo(200,0)');
            await CampaignstatusTest.filterSelectSegement(CampaignstatusconstantTest.selectionScreen_selectSgement[1]);
            await BulkOperationTest.clickOnSelectionActionButton(1);
            expect(await BulkOperationTest.getAllSelectionActionLabel()).toEqual(BulkoperationconstantTest.selection_actions);
            await BulkOperationTest.clickOnSelectionActions(7);
            HomepageTest.wait(10);
            expect(await BulkOperationTest.getEditSegmentOutputQty(5)).toEqual(outputQty);
            await BulkOperationTest.clickOnCancelAddSegment();
            HomepageTest.wait(10);
            console.log("status is" + num);
        }
        else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    afterEach(function () {
        HomepageTest.wait(30);
        DashboardTest.clickOnUser();
        DashboardTest.clickOnLogout();
    });
});