// Put x before 'it' to exclude the test case
// Put f before 'it' to focus(include) the test case
describe('IDMS "New Campaign status" screen test', function () {

    var CampaignTest = new Campaign();
    var HomepageTest = new Homepage();
    var DashboardTest = new Dashboard();
    var CampaignpageLocatorTest = new CampaignpageLocator();
    var CampaignConstantTest = new CampaignConstant();
    var CampaignstatuspagelocatorTest = new CampaignstatuspageLocator();
    var CampaignstatusTest = new Campaignstatus();
    var CampaignstatusconstantTest = new Campaignstatusconstant();

    beforeEach(async function () {
        HomepageTest.get();
        HomepageTest.setUsername();
        HomepageTest.setPassword();
        HomepageTest.wait(10);
        HomepageTest.clickLoginButton();
        HomepageTest.wait(20);
        CampaignTest.clickOnSideButton();
        HomepageTest.wait(10);
        CampaignTest.clickOnSideNavCampaignButton();
        HomepageTest.wait(30);
    });

    async function campaignList_advancedSearchLabelAssertion() {
        expect(CampaignTest.getAdvancedSearchIdLabel()).toEqual(CampaignConstantTest.advancedFilters_idLabel);
        expect(CampaignTest.getAdvancedSearchDescriptionLabel()).toEqual(CampaignConstantTest.advancedFilters_descriptionLabel);
        expect(CampaignTest.getAdvancedSearchStatusLabel()).toEqual(CampaignConstantTest.advancedFilters_statusLabel);
        expect(CampaignTest.getAdvancedSearchStatusDateLabel()).toEqual(CampaignConstantTest.advancedFilters_statusDateLabel);
    };
    async function campaignList_tableLabelAssertion() {
        expect(CampaignTest.getCampaignIdLabelTable()).toEqual(CampaignConstantTest.campaignList_campaignIdLabel_table);
        expect(CampaignTest.getDescriptionLabelTable()).toEqual(CampaignConstantTest.campaignList_descriptionLabel_table);
        expect(CampaignTest.getQuantityLabelTabel()).toEqual(CampaignConstantTest.campaignList_quantityLabel_table);
        expect(CampaignTest.getStatusLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusLabel_table);
        expect(CampaignTest.getStatusDateLabelTabel()).toEqual(CampaignConstantTest.campaignList_statusDateLabel_table);
        expect(CampaignTest.getActionsLabelTabel()).toEqual(CampaignConstantTest.campaignList_actionsLabel_table);
    };
    async function campaignList_tableSingleRowAssertion(statusCount) {
        expect(CampaignpageLocatorTest.campaignList_status_table.getText()).toEqual(statusCount);
        expect(CampaignpageLocatorTest.campaignList_statusDate_table.getText()).toEqual(CampaignTest.generateStatusDate());
        expect(CampaignpageLocatorTest.campaignList_totalCount_table.getText()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
    };
    async function successfullySubmittedMessageWithIdAssertion() {
        var successfullMsg = await CampaignstatusTest.getSuccessfullMsgWithId();
        expect(successfullMsg[0].trim()).toEqual(CampaignstatusconstantTest.campaignList_campaignText);
        expect(successfullMsg[1].trim()).toEqual(CampaignTest.getCampaignId());
        expect(successfullMsg[2].trim()).toEqual(CampaignstatusconstantTest.campaignList_submittedText);
        expect(successfullMsg[3].trim()).toEqual(CampaignstatusconstantTest.campaignList_successfullyDotText);
    };
    async function successfullySubmiittedMessageForOutputAssertion() {
        var successfullMsg = await CampaignstatusTest.getSuccessfullMsgWithId();
        expect(successfullMsg[0].trim()).toEqual(CampaignstatusconstantTest.campaignList_campaignText);
        expect(successfullMsg[1].trim()).toEqual(CampaignTest.getCampaignId());
        expect(successfullMsg[2].trim()).toEqual(CampaignstatusconstantTest.campaignList_submittedText);
        expect(successfullMsg[3].trim()).toEqual(CampaignstatusconstantTest.campaignList_successfullyText);
        expect(successfullMsg[4].trim()).toEqual(CampaignstatusconstantTest.campaignList_forText);
        expect(successfullMsg[5].trim()).toEqual(CampaignstatusconstantTest.campaignList_outputText);
    };
    async function campaignApprovedForShippingMessageAssertion() {
        var successfullMsg = await CampaignstatusTest.getSuccessfullMsgWithId();
        expect(successfullMsg[0].trim()).toEqual(CampaignstatusconstantTest.campaignList_campaignText);
        expect(successfullMsg[1].trim()).toEqual(CampaignTest.getCampaignId());
        expect(successfullMsg[2].trim()).toEqual(CampaignstatusconstantTest.campaignList_hasText);
        expect(successfullMsg[3].trim()).toEqual(CampaignstatusconstantTest.campaignList_beenText);
        expect(successfullMsg[4].trim()).toEqual(CampaignstatusconstantTest.campaignList_approvedText);
        expect(successfullMsg[5].trim()).toEqual(CampaignstatusconstantTest.campaignList_forText);
        expect(successfullMsg[6].trim()).toEqual(CampaignstatusconstantTest.campaignList_shippingText);
    };
    async function popUpErrorMessageWithCampaignIdAssertion() {
        var message = await CampaignstatusTest.getPopUpErrorMessage();
        expect(message[0].trim()).toEqual(CampaignstatusconstantTest.editCampaign_errorMessageMaxPerGroupNotSelectedPart1st + await CampaignTest.getCampaignId());
        expect(message[1].trim()).toEqual(CampaignstatusconstantTest.editCampaign_errorMessageMaxPerGroupNotSelectedPart2nd);
        expect(message[2].trim()).toEqual(CampaignstatusconstantTest.editCampaign_errorMessageMaxPerGroupNotSelectedPart3rd);
        expect(message[3].trim()).toEqual(CampaignstatusconstantTest.editCampaign_errorMessageMaxPerGroupNotSelectedPart4th);
    };
    async function formatSectionAllLabelsOutputTabAssertion() {
        expect(CampaignstatusTest.getFormatLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_formatSectionLabelOutputTab);
        expect(CampaignstatusTest.getFormatMediaLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_mediaFieldLabelOutputTab);
        expect(CampaignstatusTest.getFormatMediaDefaultValueOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_mediaFieldDefaultValueOutputTab);
        expect(CampaignstatusTest.getTypeLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_typeFieldLabelOutputTab);
        expect(CampaignstatusTest.getTypeDefaultValueOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
        expect(CampaignstatusTest.getHeaderRowLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_headerRowLabelOutputTab);
        expect(CampaignstatusTest.getDataFileOnlyLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_dataFileOnlyLabelOutputTab);
        expect(CampaignstatusTest.getUnzippedLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_unzippedLabelOutputTab);
        expect(CampaignstatusTest.getLayoutLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_layoutFieldLabelOutputTab);
        expect(CampaignstatusTest.getLayoutDefaultValueOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_layoutFieldDefaultValueOutputTab);
        expect(CampaignstatusTest.getSortFieldLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_sortFieldLabelOutputTab);
        expect(CampaignstatusTest.getSortFieldDefaultValueOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_sortFieldDefaultValueOutputTab);
        expect(CampaignstatusTest.getFormatFilenameLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_fileNameFieldLabelOutputTab);
        expect(CampaignstatusTest.getPGPKeyFieldLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_PGPKeyFieldLabelOutputTab);
        expect(CampaignstatusTest.getPGPKeyFieldDefaultValueOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_PGPKeyFieldDefaultValueOutputTab);
    };
    async function shippingSectionAllLabelsOutputTabAssertion() {
        expect(CampaignstatusTest.getShippingLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_shippingSectionLabelOutputTab);
        expect(CampaignstatusTest.getShippingShipToLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_shippingShipToLabelOutputTab);
        expect(CampaignstatusTest.getShippingCCLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_shippingCCLabelOutputTab);
        expect(CampaignstatusTest.getShippingSubjectLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_shippingSubjectLabelOutputTab);
        expect(CampaignstatusTest.getShippingNotesLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_shippingNotesLabelOutputTab);
        expect(CampaignstatusTest.getShippingFTPsiteLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_shippingFTPsiteLabelOutputTab);
        expect(CampaignstatusTest.getShippingUsernameLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_shippingUsernameLabelOutputTab);
    };
    async function maxPerTabLabelAssertions() {
        expect(await CampaignstatusTest.getSegmentLevelLabelMaxPerTab()).toEqual(CampaignstatusconstantTest.editCampaign_segmentLevelLabelMaxPerTab);
        expect(await CampaignstatusTest.getSegmentLevelGroupLabelMaxPerTab()).toEqual(CampaignstatusconstantTest.editCampaign_segmentLevelGroupLabelMaxPerTab);
        expect(await CampaignstatusTest.getSegmentLevelQuantityLabelMaxPerTab()).toEqual(CampaignstatusconstantTest.editCampaign_segmentLevelQuantityLabelMaxPerTab);
        expect(await CampaignstatusTest.getSegmentLevelFieldLabelMaxPerTab()).toEqual(CampaignstatusconstantTest.editCampaign_segmentLevelFieldLabelMaxPerTab);
        expect(await CampaignstatusTest.getSegmentLevelGroupFirstRowMaxPerTab()).toEqual(CampaignstatusconstantTest.editCampaign_segmentLevelGroupFirstRowMaxPerTab);
        expect(await CampaignstatusTest.getCampaignLevelLabelMaxPerTab()).toEqual(CampaignstatusconstantTest.editCampaign_campaignLevelLabelMaxPerTab);
    };
    async function advanceFilterAllFieldLabelAssertion() {
        expect(CampaignTest.getAdvancedSearchIdLabel()).toEqual(CampaignConstantTest.advancedFilters_idLabel);
        expect(CampaignTest.getAdvancedSearchDescriptionLabel()).toEqual(CampaignConstantTest.advancedFilters_descriptionLabel);
        expect(CampaignTest.getAdvancedSearchStatusLabel()).toEqual(CampaignConstantTest.advancedFilters_statusLabel);
        expect(CampaignTest.getAdvancedSearchStatusDateLabel()).toEqual(CampaignConstantTest.advancedFilters_statusDateLabel);
    };
    async function refereshStatus(status) {
        var num;
        for (var i = 0; i < CampaignstatusconstantTest.refreshCount; i++) {
            await HomepageTest.wait(30);
            await HomepageTest.refresh();
            num = await CampaignstatusTest.getStatusNum();
            await HomepageTest.wait(20);
            if (status == num)
                break;
        }
        return num;
    };
    async function addFilterInSelectionScreen() {
        HomepageTest.wait(20);
        await CampaignstatusTest.filterSelectSegement(CampaignstatusconstantTest.selectionScreen_selectSgementCommonRules);
        HomepageTest.wait(10);
        expect(await CampaignstatusTest.selectionScreen_getSelectionsLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_selectionLabel);
        expect(await CampaignstatusTest.selectionScreen_getCountStatusLabel()).toEqual(CampaignstatusconstantTest.selectionScreen_statusCountLabel);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterSearchFieldFirst.isPresent()).toBe(true);
        //await CampaignstatusTest.clickOnFilterDropdownFirst();
        await CampaignstatusTest.typeOnFilterSearchFieldFirst(CampaignstatusconstantTest.campaignList_filterFieldTextFirst);
        await CampaignstatusTest.clickOnFilterFirstSearchFirstElement();
        HomepageTest.wait(10);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirstLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextFirst);
        await CampaignstatusTest.clickOnOptionCheckBoxFirst();
        HomepageTest.wait(20);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsAddRuleButton.isPresent()).toBe(true);
        await CampaignstatusTest.clickOnSectionAddRuleButton();
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownButtonSecond.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterSearchFieldSecond.isPresent()).toBe(true);
        //await CampaignstatusTest.clickOnFilterDropdownSecond();
        await CampaignstatusTest.typeOnFilterSearchFieldSecond(CampaignstatusconstantTest.campaignList_filterFieldTextSecond);
        await CampaignstatusTest.clickOnFilterSecondSearchFirstElement();
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond.isPresent()).toBe(true);
        expect(await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecondLabel.getText()).toEqual(CampaignstatusconstantTest.campaignList_checkBoxLabelTextSecond);
        await CampaignstatusTest.clickOnOptionCheckBoxSecond();
        await CampaignstatusTest.clickOnSegmentSaveButton();
        HomepageTest.wait(20);
        expect(await CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_filterSaveSuccessfullyText);
    };
    async function evaluateActionDropdown(status, actionDropdown) {
        await CampaignTest.clickOnSearchButton();
        await HomepageTest.wait(20);
        campaignList_tableSingleRowAssertion(status);
        await CampaignTest.clickOnCampaignActionDropdown();
        await HomepageTest.wait(20);
        expect(await CampaignstatusTest.getCampaignActionDropdownList()).toEqual(actionDropdown);
        await HomepageTest.wait(10);
    };

    it('Verify that the values "New Segment", "Copy", "Edit" and "Print" appear in the action drop down at "120: Waiting to Ship" status.', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count120Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_standardPostalLayout);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();

            var num = await refereshStatus(90);
            if (num == 90) {
                await HomepageTest.wait(30);
                await CampaignTest.typeOnSearchField(id);
                await CampaignTest.clickOnSearchButton();
                await HomepageTest.wait(20);
                campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count90Text);
                await CampaignTest.clickOnCampaignActionDropdown();
                await CampaignstatusTest.clickOnCampaignActionDropdownShip();
                await HomepageTest.wait(20);
                await campaignApprovedForShippingMessageAssertion();

                var num = await refereshStatus(120);
                if (num == 120) {
                    await HomepageTest.wait(30);
                    await CampaignTest.typeOnSearchField(id);
                    evaluateActionDropdown(CampaignConstantTest.campaignList_status_Count120Text, CampaignstatusconstantTest.campaignList_campaignActionDropdownCount120);
                    await HomepageTest.wait(10);
                    await CampaignTest.clickOnSearchButton();
                    console.log("status is" + num);
                } else {
                    fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
                }
            } else {
                fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
            }
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify that the values "New Segment", "Copy", "Edit", "Execute" and "Print" appear in the action dropdown at "10: Count Created" status', async function () {

        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[0], CampaignConstantTest.campaignList_status_tableText, CampaignConstantTest.newCampaign_offerFieldArray[0], true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        HomepageTest.wait(10);
        expect(CampaignstatusTest.getCampaignActionDropdownList()).toEqual(CampaignstatusconstantTest.campaignList_campaignActionDropdownCount10);

    });

    it('Verify that the values "New Segment", "Copy", "Edit", "Cancel" and "Print" appear in the action dropdown at "20: Count Submitted" status', async function () {

        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[0], CampaignConstantTest.campaignList_status_Count20Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage.getText()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);
        CampaignTest.clickOnCampaignActionDropdown();
        HomepageTest.wait(10);
        expect(CampaignstatusTest.getCampaignActionDropdownList()).toEqual(CampaignstatusconstantTest.campaignList_campaignActionDropdownCount20);

    });

    it('Verify that the values "New Segment", "Copy", "Edit", "Output" and "Print" appear in the action drop down at "40: Count Completed" status', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count40Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage.getText()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await evaluateActionDropdown(CampaignConstantTest.campaignList_status_Count40Text, CampaignstatusconstantTest.campaignList_campaignActionDropdownCount40);
            console.log("status is" + num);
        }
        else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify that the values "New Segment", "Copy", "Edit", "Cancel" and "Print" appear in the action drop down at "70: Output Submitted" status', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count70Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage.getText()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_allFieldType);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();
            await HomepageTest.wait(10);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count70Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await HomepageTest.wait(10);
            expect(CampaignstatusTest.getCampaignActionDropdownList()).toEqual(CampaignstatusconstantTest.campaignList_campaignActionDropdownCount70);
            await HomepageTest.wait(10);
            console.log("status is" + num);
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }

    });

    it('Verify that the values "New Segment", "Copy", "Edit" and "Print" appear in the action drop down at "100: Output Failed" status', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count100Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage.getText()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_allFieldType);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();
            await HomepageTest.wait(10);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count70Text);

            var num = await refereshStatus(100);
            if (num == 100) {
                await HomepageTest.wait(30);
                await CampaignTest.typeOnSearchField(id);
                await evaluateActionDropdown(CampaignConstantTest.campaignList_status_Count100Text, CampaignstatusconstantTest.campaignList_campaignActionDropdownCount100);
                console.log("status is" + num);
            } else {
                fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
            }
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify that the values "New Segment", "Copy", "Edit", "Ship" and "Print" appear in the action drop down at "90: Output Completed" status', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count90Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await HomepageTest.wait(20);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_standardPostalLayout);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();

            var num = await refereshStatus(90);
            if (num == 90) {
                await HomepageTest.wait(30);
                await CampaignTest.typeOnSearchField(id);
                await evaluateActionDropdown(CampaignConstantTest.campaignList_status_Count90Text, CampaignstatusconstantTest.campaignList_campaignActionDropdownCount90);
                console.log("status is" + num);
            } else {
                fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
            }
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify that the values "New Segment", "Copy", "Edit" and "Print" appear in the Action drop down at "110: Approved for Shipping" status', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count110Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_standardPostalLayout);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();

            var num = await refereshStatus(90);
            if (num == 90) {
                await HomepageTest.wait(30);
                await CampaignTest.typeOnSearchField(id);
                await CampaignTest.clickOnSearchButton();
                await HomepageTest.wait(20);
                campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count90Text);
                await CampaignTest.clickOnCampaignActionDropdown();
                await CampaignstatusTest.clickOnCampaignActionDropdownShip();
                await HomepageTest.wait(20);
                await campaignApprovedForShippingMessageAssertion();
                await HomepageTest.wait(10);
                campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count110Text);
                await CampaignTest.clickOnCampaignActionDropdown();
                expect(CampaignstatusTest.getCampaignActionDropdownList()).toEqual(CampaignstatusconstantTest.campaignList_campaignActionDropdownCount110);
                await HomepageTest.wait(10);
                console.log("status is" + num);
            } else {
                fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
            }
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify that the values "New Segment", "Copy", "Edit" and "Print" appear in the Action drop down at "140: Shipment Failed" status.', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count140Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_standardPostalLayout);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();

            var num = await refereshStatus(90);
            if (num == 90) {
                await HomepageTest.wait(30);
                await CampaignTest.typeOnSearchField(id);
                await CampaignTest.clickOnSearchButton();
                await HomepageTest.wait(20);
                campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count90Text);
                await CampaignTest.clickOnCampaignActionDropdown();
                await CampaignstatusTest.clickOnCampaignActionDropdownShip();
                await HomepageTest.wait(20);
                await campaignApprovedForShippingMessageAssertion();

                var num = await refereshStatus(140);
                if (num == 140) {
                    await HomepageTest.wait(30);
                    await CampaignTest.typeOnSearchField(id);
                    await evaluateActionDropdown(CampaignConstantTest.campaignList_status_Count140Text, CampaignstatusconstantTest.campaignList_campaignActionDropdownCount140)
                    console.log("status is" + num);
                } else {
                    fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
                }
            } else {
                fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
            }
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify that the values "New Segment", "Copy", "Edit" and "Print" appear in the Action drop down at "130: Shipped" status', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.campaignList_status_Count130Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_standardPostalLayout);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab_devNotfi);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();

            var num = await refereshStatus(90);
            if (num == 90) {
                await HomepageTest.wait(30);
                await CampaignTest.typeOnSearchField(id);
                await CampaignTest.clickOnSearchButton();
                await HomepageTest.wait(20);
                campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count90Text);
                await CampaignTest.clickOnCampaignActionDropdown();
                await CampaignstatusTest.clickOnCampaignActionDropdownShip();
                await HomepageTest.wait(20);
                await campaignApprovedForShippingMessageAssertion();

                var num = await refereshStatus(130);
                if (num == 130) {
                    await HomepageTest.wait(30);
                    await CampaignTest.typeOnSearchField(id);
                    await evaluateActionDropdown(CampaignConstantTest.campaignList_status_Count130Text, CampaignstatusconstantTest.campaignList_campaignActionDropdownCount130);
                    console.log("status is" + num);
                } else {
                    fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
                }
            } else {
                fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
            }
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify that the values "New Segment", "Copy", "Edit" and "Print" appear in the action drop down at "150: Cancelled" status', async function () {
        expect(CampaignTest.getAdvancedSearchButtonText()).toEqual(CampaignConstantTest.advancedFilters_advancedFiltersButtonText);
        CampaignTest.clickOnAdvancedSearchButton();
        advanceFilterAllFieldLabelAssertion();
        CampaignTest.selectAdvancedSearchStatusDropdown(14);
        CampaignTest.typeOnAdvancedSearchDatePicker(CampaignstatusconstantTest.advancedFilters_statusDateRange_count150Dates);
        expect(CampaignTest.getAdvancedSearchToggleButtonText()).toEqual(CampaignConstantTest.advancedFilters_toggleButtonOn);
        CampaignTest.clickOnAdvancedSearchToggleButton();
        expect(CampaignTest.getAdvancedSearchToggleButtonText()).toEqual(CampaignConstantTest.advancedFilters_toggleButtonOff);
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        campaignList_tableLabelAssertion();
        expect(CampaignTest.getStatusList()).toContain(CampaignConstantTest.campaignList_status_Count150Text);
        CampaignTest.typeOnSearchField(CampaignTest.getCampaignId());
        CampaignTest.clickOnSearchButton();
        HomepageTest.wait(10);
        expect(CampaignTest.getStatus()).toEqual(CampaignConstantTest.campaignList_status_Count150Text);
        expect(CampaignpageLocatorTest.campaignList_totalCount_table.getText()).toEqual(CampaignConstantTest.campaignList_totalCount_tableText);
        await CampaignTest.clickOnCampaignActionDropdown();
        expect(CampaignstatusTest.getCampaignActionDropdownList()).toEqual(CampaignstatusconstantTest.campaignList_campaignActionDropdownCount150);
    });

    it('Verify that the created campaign should reach "10:Count Created" to "130:Shipped’successfully".', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.newCampaign_descriptionFieldCustomText, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(20);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_standardPostalLayout);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab_devNotfi);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(20);
            await successfullySubmiittedMessageForOutputAssertion();

            var num = await refereshStatus(90);
            if (num == 90) {
                await HomepageTest.wait(30);
                await CampaignTest.typeOnSearchField(id);
                await CampaignTest.clickOnSearchButton();
                await HomepageTest.wait(20);
                campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count90Text);
                await CampaignTest.clickOnCampaignActionDropdown();
                await CampaignstatusTest.clickOnCampaignActionDropdownShip();
                await HomepageTest.wait(20);
                await campaignApprovedForShippingMessageAssertion();

                var num = await refereshStatus(130);
                if (num == 130) {
                    await HomepageTest.wait(30);
                    await CampaignTest.typeOnSearchField(id);
                    await CampaignTest.clickOnSearchButton();
                    await HomepageTest.wait(10);
                    campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count130Text);
                    console.log("status is" + num);
                } else {
                    fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
                }
            } else {
                fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
            }
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify a proper error message is displayed when the "Ship to" is not selected in shipping section output tab.', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(10);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            HomepageTest.wait(10);
            CampaignTest.clickOnCampaignActionDropdown();
            CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(10);
            expect(CampaignstatusTest.getPopUpErrorMessage()).toEqual(CampaignstatusconstantTest.editCampaign_shippingShipToPopUpErrorMessageOutputTab);
            await HomepageTest.wait(10);
            CampaignstatusTest.clickOnPopUpErrorMessageOkButton();
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify a proper error message is displayed when the invalid email id is enterd in "Ship to" and "CC" field shipping section output tab.', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(10);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await HomepageTest.wait(10);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            await HomepageTest.wait(10);
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await HomepageTest.wait(10);
            CampaignstatusTest.typeOnShippingShipToOutputTab(CampaignstatusconstantTest.editCampaign_shippingShipToInvalidDataOutputTab);
            CampaignstatusTest.getShippingShipToErrorMessageOutputTab(CampaignstatusconstantTest.editCampaign_shippingShipToFieldErrorMessageOutputTab);
            HomepageTest.wait(10);
            CampaignstatusTest.typeOnShippingCCoutputTab(CampaignstatusconstantTest.editCampaign_shippingCCFieldInvalidDataOutputTab);
            CampaignstatusTest.getShippingCCFieldErrorMessageOutputTab(CampaignstatusconstantTest.editCampaign_shippingCCFieldErrorMessageOutputTab);
            HomepageTest.wait(10);
            CampaignTest.clickOnCancelButtonFooterEditCampaign();
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify a proper error message is displayed when the "Layout" and "PO#" field is not selected in output tab and billing tab respectively.', async function () {

        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(10);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await HomepageTest.wait(10);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            await HomepageTest.wait(10);
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            CampaignstatusTest.typeOnShippingShipToOutputTab(CampaignstatusconstantTest.editCampaign_shippingShipToFieldEmailOutputTab);
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await HomepageTest.wait(10);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await HomepageTest.wait(10);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(10);
            expect(await CampaignstatusTest.getPopUpErrorMessage()).toEqual(CampaignstatusconstantTest.editCampaign_layoutFieldErrorMessageOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.clickOnPopUpErrorMessageOkButton();
            await HomepageTest.wait(10);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            await HomepageTest.wait(10);
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab_standardPostalLayout);
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            expect(await CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            await HomepageTest.wait(10);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(10);
            await CampaignstatusTest.getPopUpErrorMessage(CampaignstatusconstantTest.editCampaign_POFieldErrorMessageBillingTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.clickOnPopUpErrorMessageOkButton();
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }

    });

    it('Verify a proper error message is displayed when "Sort" field is not appropriately selected as per the “layout” field.', async function () {

        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        await addFilterInSelectionScreen();

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(40);
        if (num == 40) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await CampaignTest.clickOnSearchButton();
            await HomepageTest.wait(10);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await HomepageTest.wait(10);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignTest.clickOnCampaignActionEdit();
            await HomepageTest.wait(10);
            expect(await CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
            await CampaignstatusTest.clickOnEditCampaignOutputTab();
            formatSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectMediaFieldOutputTab(CampaignstatusconstantTest.editCampaign_mediaFieldDefaultValueOutputTab);
            await CampaignstatusTest.selectTypeFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectTypeFieldOutputTab);
            await CampaignstatusTest.selectLayoutFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectLayoutFieldOutputTab);
            await CampaignstatusTest.selectSortFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectSortFieldOutputTab);
            await CampaignstatusTest.clickOnShippingSectionOutputTab();
            await HomepageTest.wait(10);
            shippingSectionAllLabelsOutputTabAssertion();
            await CampaignstatusTest.selectShipToFieldOutputTab(CampaignstatusconstantTest.editCampaign_selectShipToFieldOutputTab_devNotfi);
            await CampaignstatusTest.typeOnShippingSubjectFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingSubjectTextOutputTab);
            await CampaignstatusTest.typeOnShippingNotesFieldOutputTab(CampaignstatusconstantTest.editCampaign_shippingNotesTextOutputTab);
            await CampaignstatusTest.clickOnEditCampaignBillingTab();
            expect(CampaignstatusTest.getPOLabelOutputTab()).toEqual(CampaignstatusconstantTest.editCampaign_POFieldLabelBillingTab);
            await CampaignstatusTest.typeOnPOSearchFieldBillingTab(CampaignstatusconstantTest.editCampaign_POFieldPrefixBillingTab + CampaignTest.generateRandomNumber(1000, 2000));
            await CampaignTest.clickOnSaveButtonFooterEditCampaign();
            await HomepageTest.wait(20);
            expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
            campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count40Text);
            await HomepageTest.wait(10);
            await CampaignTest.clickOnCampaignActionDropdown();
            await CampaignstatusTest.clickOnCampaignActionDropdownOutput();
            await HomepageTest.wait(10);
            expect(await CampaignstatusTest.getPopUpErrorMessage()).toEqual(CampaignstatusconstantTest.editCampaign_sortFieldPopUpErrorMessageOutputTab);
            await HomepageTest.wait(10);
            await CampaignstatusTest.clickOnPopUpErrorMessageOkButton();
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    it('Verify a proper error message is displayed when Max per group is not selected in campaign segment.', async function () {

        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[2], CampaignConstantTest.newCampaign_descriptionFieldText, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignTest.clickOnCampaignActionEdit();
        HomepageTest.wait(20);
        expect(CampaignTest.getEditCampaignTitle()).toEqual(CampaignConstantTest.editCampaign_title);
        expect(CampaignTest.getMaxPerTabTextEditCampaign()).toEqual(CampaignConstantTest.editCampaign_maxPer);
        CampaignstatusTest.clickOnEditCampaignMaxPerTab();
        HomepageTest.wait(20);
        maxPerTabLabelAssertions();
        CampaignstatusTest.selectSegmentLevelQuantityFirstRowMaxperTab(CampaignstatusconstantTest.editCampaign_segmentLevelQuantityFirstRowMaxPerTab);
        HomepageTest.wait(10);
        CampaignTest.clickOnSaveButtonFooterEditCampaign();
        expect(CampaignstatusTest.getPopUpErrorMessage()).toEqual(CampaignstatusconstantTest.editCampaign_errorMessageFieldNotSelected);
        HomepageTest.wait(10);
        CampaignstatusTest.clickOnPopUpErrorMessageOkButton();
        HomepageTest.wait(10);
        await CampaignstatusTest.selectSegmentLevelFieldFirstRowMaxperTab(CampaignstatusconstantTest.editCampaign_segmentLevelFieldFirstRowMaxPerTab);
        HomepageTest.wait(10);
        CampaignstatusTest.clearSegmentLevelQuantityFirstRowMaxPerTab();
        HomepageTest.wait(10);
        CampaignTest.clickOnSaveButtonFooterEditCampaign();
        expect(CampaignstatusTest.getPopUpErrorMessage()).toEqual(CampaignstatusconstantTest.editCampaign_errorMessageQuantityNotSelected);
        HomepageTest.wait(10);
        CampaignstatusTest.clickOnPopUpErrorMessageOkButton();
        CampaignstatusTest.selectSegmentLevelQuantityFirstRowMaxperTab(CampaignstatusconstantTest.editCampaign_segmentLevelQuantityFirstRowMaxPerTab);
        HomepageTest.wait(10);
        CampaignTest.clickOnSaveButtonFooterEditCampaign();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignstatusconstantTest.campaignList_saveSuccessfullyText);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(10);
        await popUpErrorMessageWithCampaignIdAssertion()
        HomepageTest.wait(10);
        CampaignstatusTest.clickOnPopUpErrorMessageOkButton();
    });

    //Should execute this test case at the last.
    it('Verify that the values "New Segment", "Copy", "Edit" and "Print" appear in the action drop down at "30: Count Running" status', async function () {
        expect(CampaignpageLocatorTest.campaignList_newCampaignButton.isPresent()).toBe(true);
        CampaignTest.createCampaign(CampaignConstantTest.newCampaign_databaseValue[1], CampaignConstantTest.campaignList_status_Count30Text, CampaignConstantTest.newCampaign_offerFieldText, true);
        expect(CampaignpageLocatorTest.newCampaign_saveButton_footer.isPresent()).toBe(true);
        CampaignTest.clickOnSaveButtonFooter();
        HomepageTest.wait(20);
        expect(CampaignTest.getSuccessfullMessage()).toEqual(CampaignConstantTest.campaignList_saveSuccessfullyMessage);

        var id = await CampaignTest.getCampaignIdFromFilter();
        expect(CampaignpageLocatorTest.campaignList_cancelSymbol_filter.isPresent()).toBe(true);
        CampaignTest.clickOnFilterCrossSymbol();
        HomepageTest.wait(10);
        CampaignTest.typeOnSearchField(id);
        expect(CampaignpageLocatorTest.campaignList_searchButton.isPresent()).toBe(true);
        CampaignTest.clickOnSearchButton();
        campaignList_tableLabelAssertion();
        HomepageTest.wait(10);
        expect(CampaignTest.getCampaignId()).toEqual(id);

        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_tableText);
        CampaignTest.clickOnCampaignActionDropdown();
        CampaignstatusTest.clickOnCampaignActionDropdownExecute();
        HomepageTest.wait(20);
        successfullySubmittedMessageWithIdAssertion();
        HomepageTest.wait(10);
        campaignList_tableSingleRowAssertion(CampaignConstantTest.campaignList_status_Count20Text);

        var num = await refereshStatus(30);
        if (num == 30) {
            await HomepageTest.wait(30);
            await CampaignTest.typeOnSearchField(id);
            await evaluateActionDropdown(CampaignConstantTest.campaignList_status_Count30Text, CampaignstatusconstantTest.campaignList_campaignActionDropdownCount30);
            console.log("status is" + num);
        } else {
            fail(CampaignstatusconstantTest.campaignList_statusText + num + CampaignstatusconstantTest.campaignList_notFoundText);
        }
    });

    afterEach(function () {
        HomepageTest.wait(20);
        DashboardTest.clickOnUser();
        DashboardTest.clickOnLogout();
    });

});
