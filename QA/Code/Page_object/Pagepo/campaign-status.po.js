var Campaignstatus = function () {
    var CampaignstatuspagelocatorTest = new CampaignstatuspageLocator();
    var CampaignpageLocatorTest = new CampaignpageLocator();
    var HomepageTest = new Homepage();
    var EC = protractor.ExpectedConditions;

    this.clickOnCampaignActionDropdownExecute = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownExecute), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownExecute.click();
    };
    this.clickOnCampaignActionDropdownOutput = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownOutput), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownOutput.click();
    };
    this.clickOnCampaignActionDropdownShip = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownShip), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownShip.click();
    };
    this.selectionScreen_getSelectSegmentLabel = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_selectSegmentLabel), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.campaignFilter_selectSegmentLabel.getText();
    };
    this.filterSelectSegement = async function (segment) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_selectSegmentDropdown), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_selectSegmentDropdown.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_selectSegmentSearchField), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_selectSegmentSearchField.sendKeys(segment);
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };
    this.selectionScreen_getSelectionsLabel = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_selectionsLabel), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.campaignFilter_selectionsLabel.getText();
    };
    this.selectionScreen_getCountStatusLabel = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_statusCountLabel), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.campaignFilter_statusCountLabel.getText();
    };
    this.clickOnFilterDropdownFirst = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownButtonFirst), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownButtonFirst.click();
    };
    this.clickOnFilterDropdownSecond = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownButtonSecond), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownButtonSecond.click();
    };
    this.typeOnFilterSearchFieldFirst = async function (value) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterSearchFieldFirst), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterSearchFieldFirst.sendKeys(value);
    };
    this.typeOnFilterSearchFieldSecond = async function (value) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterSearchFieldSecond), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterSearchFieldSecond.sendKeys(value);
    };
    this.clickOnOptionCheckBoxFirst = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxFirst.click();
    };
    this.clickOnOptionCheckBoxSecond = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsOptionsCheckBoxSecond.click();
    };
    this.clickOnSectionAddRuleButton = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsAddRuleButton), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsAddRuleButton.click();
    };
    this.clickOnFilterFirstSearchFirstElement = async function () {

        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownFirstElement), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownFirstElement.click();
    };
    this.clickOnFilterSecondSearchFirstElement = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownSecondElement), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_sectionsFilterDropdownSecondElement.click();
    };
    this.clickOnSegmentSaveButton = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignFilter_segmentSaveButton), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.campaignFilter_segmentSaveButton.click();
    };
    this.getCampaignActionDropdownList = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownList), HomepageTest.await(50));
        var text = await CampaignstatuspagelocatorTest.campaignList_campaignActionDropdownList.getText();
        var actionDropdown = text.split("\n");
        return actionDropdown;
    };
    this.getSuccessfullMsgWithId = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage), HomepageTest.await(50));
        var text = await CampaignpageLocatorTest.campaignList_saveSuccessfullyMessage.getText();
        var values = text.split(" ");
        return await values;
    };
    this.getStatusNum = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_status_table), HomepageTest.await(50));
        var text = await CampaignpageLocatorTest.campaignList_status_table.getText();
        var data = text.split(":");
        var num = data[0].trim();
        return await num;
    };
    this.clickOnEditCampaignMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.editCamapign_maxPerTab), HomepageTest.await(50));
        await CampaignpageLocatorTest.editCamapign_maxPerTab.click();
    };
    this.clickOnEditCampaignOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.editCamapign_outputTab), HomepageTest.await(50));
        await CampaignpageLocatorTest.editCamapign_outputTab.click();
    };
    this.clickOnEditCampaignBillingTab = async function () {

        browser.wait(EC.presenceOf(CampaignpageLocatorTest.editCamapign_billingTab), HomepageTest.await(50));
        await CampaignpageLocatorTest.editCamapign_billingTab.click();
    };
    //MaxPer Tab
    this.getSegmentLevelLabelMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelSectionArrowMaxPerTab), HomepageTest.await(50));
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelSectionTextMaxPerTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_segmentLevelSectionTextMaxPerTab.getText();
    };
    this.getCampaignLevelLabelMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_campaignLevelSectionArrowMaxPerTab), HomepageTest.await(50));
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_campaignLevelSectionTextMaxPerTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_campaignLevelSectionTextMaxPerTab.getText();
    };
    this.getSegmentLevelGroupLabelMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelGroupLabelMaxPerTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_segmentLevelGroupLabelMaxPerTab.getText();
    };
    this.getSegmentLevelQuantityLabelMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityLabelMaxPerTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityLabelMaxPerTab.getText();
    };
    this.getSegmentLevelFieldLabelMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldLabelMaxPerTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldLabelMaxPerTab.getText();
    };
    this.getSegmentLevelGroupFirstRowMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelGroupFirstRowMaxPerTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_segmentLevelGroupFirstRowMaxPerTab.getText();
    };
    this.selectSegmentLevelQuantityFirstRowMaxperTab = async function (quantity) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowMaxPerTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowMaxPerTab.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowFieldMaxPerTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowFieldMaxPerTab.sendKeys(quantity);
    };
    this.clearSegmentLevelQuantityFirstRowMaxPerTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowMaxPerTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowMaxPerTab.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowFieldMaxPerTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowFieldMaxPerTab.sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelQuantityFirstRowFieldMaxPerTab.sendKeys(protractor.Key.BACK_SPACE);
    };
    this.selectSegmentLevelFieldFirstRowMaxperTab = async function (field) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldFirstRowMaxPerTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldFirstRowMaxPerTab.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldFirstRowDropdownMaxPerTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldFirstRowDropdownMaxPerTab.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldFirstRowDropdownSearchMaxPerTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_segmentLevelFieldFirstRowDropdownSearchMaxPerTab.sendKeys(field);
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };

    //Shipping section
    this.typeOnShippingShipToOutputTab = async function (email) {

        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_shippingShipToPrimarySearchFieldOutputTab), HomepageTest.await(50));
        CampaignstatuspagelocatorTest.editCampaign_shippingShipToPrimarySearchFieldOutputTab.click();
        browser.switchTo().activeElement().sendKeys(email);
    };
    this.getShippingShipToErrorMessageOutputTab = async function () {

        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_shippingShipToFieldErrorMessageOutputTab), HomepageTest.await(50));
        CampaignstatuspagelocatorTest.editCampaign_shippingShipToFieldErrorMessageOutputTab.getText();
    };
    this.typeOnShippingCCoutputTab = async function (email) {

        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_shippingCCFieldOutputTab), HomepageTest.await(50));
        CampaignstatuspagelocatorTest.editCampaign_shippingCCFieldOutputTab.sendKeys(email);
    };
    this.getShippingCCFieldErrorMessageOutputTab = async function () {

        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_shippingCCFieldErrorMessageOutputTab), HomepageTest.await(50));
        CampaignstatuspagelocatorTest.editCampaign_shippingCCFieldErrorMessageOutputTab.getText();
    };
    this.clickOnShippingSectionOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_shippingOutputTab.click();
    };
    this.getShippingLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_shippingLabelOutputTab.getText();
    };
    this.getShippingShipToLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingShipToLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_shippingShipToLabelOutputTab.getText();
    };
    this.getShippingCCLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingCCLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_shippingCCLabelOutputTab.getText();
    };
    this.getShippingSubjectLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingSubjectLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_shippingSubjectLabelOutputTab.getText();
    };
    this.typeOnShippingSubjectFieldOutputTab = async function (subject) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingSubjectLabelOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_shippingSubjectFieldOutputTab.sendKeys(subject);
    };
    this.typeOnShippingNotesFieldOutputTab = async function (notes) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingNotesFieldOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_shippingNotesFieldOutputTab.sendKeys(notes);
    };
    this.getShippingNotesLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingNotesLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_shippingNotesLabelOutputTab.getText();
    };
    this.getShippingFTPsiteLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingFTPsiteLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_shippingFTPsiteLabelOutputTab.getText();
    };
    this.getShippingUsernameLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingUsernameLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_shippingUsernameLabelOutputTab.getText();
    };
    //Format
    this.getFormatLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_formatLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_formatLabelOutputTab.getText();
    };
    this.getFormatMediaLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_mediaLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_mediaLabelOutputTab.getText();
    };
    this.getFormatMediaDefaultValueOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_mediaDefaultValueOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_mediaDefaultValueOutputTab.getText();
    };
    this.getTypeLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_typeLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_typeLabelOutputTab.getText();
    };
    this.getTypeDefaultValueOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_typeDefaultValueOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_typeDefaultValueOutputTab.getText();
    };
    this.getHeaderRowLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_headerRowLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_headerRowLabelOutputTab.getText();
    };
    this.getDataFileOnlyLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_dateFileOnlyLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_dateFileOnlyLabelOutputTab.getText();
    };
    this.getUnzippedLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_unzippedLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_unzippedLabelOutputTab.getText();
    };
    this.getLayoutLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_layoutFieldLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_layoutFieldLabelOutputTab.getText();
    };
    this.getLayoutDefaultValueOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_layoutFieldDefaultValueOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_layoutFieldDefaultValueOutputTab.getText();
    };
    this.getSortFieldLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_sortFieldLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_sortFieldLabelOutputTab.getText();
    };
    this.getSortFieldDefaultValueOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_sortFieldDefaultValueOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_sortFieldDefaultValueOutputTab.getText();
    };
    this.getFormatFilenameLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_fileNameFieldLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_fileNameFieldLabelOutputTab.getText();
    };
    this.getPGPKeyFieldLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_PGPKeyFieldLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_PGPKeyFieldLabelOutputTab.getText();
    };
    this.getPGPKeyFieldDefaultValueOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_PGPKeyFieldDefaultValueOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_PGPKeyFieldDefaultValueOutputTab.getText();
    };
    this.getSplitLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_splitLabelOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_splitLabelOutputTab.getText();
    };
    this.getSplitDefaultValueOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_splitDefaultValueOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_splitDefaultValueOutputTab.getText();
    };
    //Billing
    this.getPOLabelOutputTab = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_POLabelBillingTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_POLabelBillingTab.getText();
    };
    this.typeOnPOSearchFieldBillingTab = async function (PoNumber) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_POSearchFieldBillingTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_POSearchFieldBillingTab.sendKeys(PoNumber);
    };
    //ErrorMessage
    this.getPopUpErrorMessage = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.popUpErrorMessage), HomepageTest.await(50));
        var popUpMessage = await CampaignstatuspagelocatorTest.popUpErrorMessage.getText();
        var message = popUpMessage.split("\n");
        return await message;
    };
    this.clickOnPopUpErrorMessageOkButton = async function () {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.popUpErrorMessageOkButton), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.popUpErrorMessageOkButton.click();
    };

    //Combine steps
    this.selectMediaFieldOutputTab = async function (media) {
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_mediaFieldDropdownOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_mediaFieldDropdownOutputTab.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_mediaSearchFieldOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_mediaSearchFieldOutputTab.sendKeys(media);
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };
    this.selectTypeFieldOutputTab = async function (type) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_typeFieldDropdownOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_typeFieldDropdownOutputTab.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_typeSearchFieldOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_typeSearchFieldOutputTab.sendKeys(type);
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };
    this.selectLayoutFieldOutputTab = async function (layout) {

        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_layoutFieldDropdownOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_layoutFieldDropdownOutputTab.click();
        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_layoutSearchFieldOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_layoutSearchFieldOutputTab.sendKeys(layout);
        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };
    this.selectSortFieldOutputTab = async function (sort) {

        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_sortFieldDropdownOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_sortFieldDropdownOutputTab.click();
        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_sortSearchFieldLabelOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_sortSearchFieldLabelOutputTab.sendKeys(sort);
        browser.wait(EC.elementToBeClickable(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };
    this.selectShipToFieldOutputTab = async function (shipTo) {

        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingShipToDropdownOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_shippingShipToDropdownOutputTab.click();
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_shippingShipToSearchFieldOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_shippingShipToSearchFieldOutputTab.sendKeys(shipTo);
        await HomepageTest.wait(10);
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };
};
module.exports = Campaignstatus