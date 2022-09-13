var Bulkoperation = function () {

    var bulkOperationLocatorTest = new BulkoperationLocator();
    var HomepageTest = new Homepage();
    var CampaignstatusTest = new Campaignstatus();

    this.refereshStatus = async function (status) {
        var num;
        for (var i = 0; i < 15; i++) {
            await HomepageTest.wait(30);
            await HomepageTest.refresh();
            num = await CampaignstatusTest.getStatusNum();
            await HomepageTest.wait(20);
            if (status == num)
                break;
        }
        return num;
    };

    this.clickOnSplitButton = async function () {
        await bulkOperationLocatorTest.bulkOperation_splitButton.click();
    };

    this.getAllActionsListLabel = async function () {
        var labels = [];
        for (i = 1; i <= 7; i++) {
            labels.push(await bulkOperationLocatorTest.bulkOperation_actions(i).getText());
        }
        return labels;
    };

    this.getAllEditSegmentGridLabel = async function () {
        var labels = [];
        labels.push(await bulkOperationLocatorTest.bulkOperation_editSegmentTitle.getText());
        for (i = 1; i <= 6; i++) {
            labels.push(await bulkOperationLocatorTest.bulkOperation_editSegmentGrid(i).getText());
        }
        return labels;
    };

    this.getAllFieldLabel = async function () {
        var labels = [];
        for (i = 1; i <= 3; i++) {
            labels.push(await bulkOperationLocatorTest.bulkOperation_fieldLabels(i).getText());
        }
        return labels;
    };

    this.clickOnBulkActions = async function (num) {
        await bulkOperationLocatorTest.bulkOperation_actions(num).click();
    };

    this.typeOnSearchField = async function (text) {
        await bulkOperationLocatorTest.bulkOperation_editSegmentSearchField.click();
        await HomepageTest.wait(10);
        await bulkOperationLocatorTest.bulkOperation_editSegmentSearchField.sendKeys(text);
        await bulkOperationLocatorTest.bulkOperation_editSegmentSearchButton.click();
    };

    this.getTotalCount = async function (num) {
        return await bulkOperationLocatorTest.bulkOperation_editSegmentTotalCount(num).getText();
    };

    this.clickOnToggleButton = async function () {
        await bulkOperationLocatorTest.bulkOperation_editSegmentToggleButton.click();
    };

    this.getToggleButtonText = async function () {
        return await bulkOperationLocatorTest.bulkOperation_editSegmentToggleButton.getText();
    };

    this.getAllInlineGridLabel = async function (num) {
        var labels = [];
        labels.push(await bulkOperationLocatorTest.bulkOperation_editSegmentTitle.getText());
        for (i = 1; i <= num; i++) {
            labels.push(await bulkOperationLocatorTest.bulkOperation_inLineGrid(i).getText());
        }
        return labels;
    };

    this.clickOnSelectionActionButton = async function (num) {
        await bulkOperationLocatorTest.selection_actionsButton(num).click();
    }

    this.getAllSelectionActionLabel = async function () {
        var labels = [];
        for (i = 1; i <= 10; i++) {
            labels.push(await bulkOperationLocatorTest.selection_actions(i).getText());
        }
        return labels;
    };

    this.clickOnSelectionActions = async function (num) {
        await bulkOperationLocatorTest.selection_actions(num).click();
    };

    this.getAddSegmentTitle = async function () {
        return await bulkOperationLocatorTest.selection_addSegmentTitle.getText();
    };

    this.getAllAddSegmentLabel = async function (num) {
        var labels = [];
        labels.push(await bulkOperationLocatorTest.selection_actionstitle.getText());
        for (i = 1; i <= 3; i++) {
            labels.push(await bulkOperationLocatorTest.selection_addSegmentLabelsOneToThree(i).getText());
        }
        labels.push(await bulkOperationLocatorTest.selection_addSegmentLabelKeyCode1(num).getText());
        labels.push(await bulkOperationLocatorTest.selection_addSegmentLabelKeyCode2(num).getText());
        labels.push(await bulkOperationLocatorTest.selection_addSegmentLabelMaxPer(num + 1).getText());
        labels.push(await bulkOperationLocatorTest.selection_addSegmentLabelOutputQty(num + 1).getText());
        labels.push(await bulkOperationLocatorTest.selection_addSegmentLabelNetGroup.getText());
        labels.push(await bulkOperationLocatorTest.selection_addSegmentLabelRandomRadiusNth.getText());
        return labels;
    };

    this.getAllEditSegmentValues = async function (num) {
        var labels = [];
        labels.push(await bulkOperationLocatorTest.selection_actionstitle.getText());
        for (i = 1; i <= 3; i++) {
            labels.push(await bulkOperationLocatorTest.selection_addSegmentInputTwoToThree(i).getAttribute('value'));
        }
        labels.push(await bulkOperationLocatorTest.selection_addSegmentInputKeyCode1(num).getAttribute('value'));
        labels.push(await bulkOperationLocatorTest.selection_addSegmentInputKeyCode2(num).getAttribute('value'));
        labels.push(await bulkOperationLocatorTest.selection_editSegmentNetGroup(num + 2).getAttribute('value'));
        return labels;
    };

    this.getEditSegmentOutputQty = async function (num) {
        return await bulkOperationLocatorTest.selection_editSegmentOutputQty(num).getAttribute('value');
    };

    this.createSegment = async function (num, values) {
        await bulkOperationLocatorTest.selection_addSegmentInputTwoToThree(num).clear();
        await bulkOperationLocatorTest.selection_addSegmentInputTwoToThree(num).sendKeys(values.description);
        await bulkOperationLocatorTest.selection_addSegmentInputTwoToThree(num + 1).clear();
        await bulkOperationLocatorTest.selection_addSegmentInputTwoToThree(num + 1).sendKeys(values.requiredQty);
        await bulkOperationLocatorTest.selection_addSegmentInputKeyCode1(num + 2).sendKeys(values.keyCode1);
        await bulkOperationLocatorTest.selection_addSegmentInputKeyCode2(num + 2).sendKeys(values.keyCode2);
        await bulkOperationLocatorTest.selection_addSegmentDropdownMaxPer(num + 3).click();
        await bulkOperationLocatorTest.selection_addSegmentDropdownMaxPerSearch.sendKeys(values.maxPer);
        await bulkOperationLocatorTest.selection_addSegmentDropdownMaxPerElement.click();
        await bulkOperationLocatorTest.selection_addSegmentInputNetGroup.clear();
        HomepageTest.wait(10);
        await bulkOperationLocatorTest.selection_addSegmentInputNetGroup.sendKeys(values.netGroup);
    }

    this.getAllInLineEditSegmentSecondRow = async function () {
        var labels = [];
        for (i = 2; i <= 4; i++) {
            labels.push(await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentSecondRow(i).getText());
        }
        return labels;
    };

    this.createSegmentValuesArray = async function (values) {
        var labels = [];
        labels.push(values.description);
        var qty = values.requiredQty;
        qty = qty.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        labels.push(qty);
        labels.push(values.keyCode1);
        return labels;
    }

    this.editSegmentValuesArray = async function (values) {
        var labels = [];
        labels.push(values.title);
        labels.push(values.segmentNo);
        labels.push(values.description);
        labels.push(values.requiredQty);
        labels.push(values.keyCode1);
        labels.push(values.keyCode2);
        labels.push(values.netGroup);
        return labels;
    }

    this.editInLineEditSegmentSecondRow = async function (values) {
        await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentSecondRow(2).click();
        HomepageTest.wait(10);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(2).sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(2).sendKeys(protractor.Key.BACK_SPACE);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(2).sendKeys(values.description);
        await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentSecondRow(3).click();
        HomepageTest.wait(10);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(3).sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(3).sendKeys(protractor.Key.BACK_SPACE);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(3).sendKeys(values.requiredQty);
        await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentSecondRow(4).click();
        HomepageTest.wait(10);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(4).sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(4).sendKeys(protractor.Key.BACK_SPACE);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(4).sendKeys(values.keyCode1);
        await bulkOperationLocatorTest.bulkOperation_selectMaxPerDropdownEditSegment.click();
        HomepageTest.wait(10);
        await bulkOperationLocatorTest.bulkOperation_maxPerInLineEditSegmentSecondRow.get(2).click();
        await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentSecondRow(6).click();
        HomepageTest.wait(10);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(6).sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(6).sendKeys(protractor.Key.BACK_SPACE);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(6).sendKeys(values.netGroup);
        await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentSecondRow(7).click();
        HomepageTest.wait(10);
        await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentSecondRow(8).click();
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(8).sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(8).sendKeys(protractor.Key.BACK_SPACE);
        await bulkOperationLocatorTest.bulkOperation_inputInLineEditSegmentSecondRow(8).sendKeys(values.keyCode2);
    }

    this.getInlineEditOutputQty_firstRow = async function () {
        return await bulkOperationLocatorTest.bulkOperation_getInLineEditSegmentFirstRow(4).getText();
    }

    this.clickOnDescription_campaignList = async function () {
        await bulkOperationLocatorTest.campaignList_clickOnDescription.click();
    }

    this.clickOnCancelAddSegment = async function () {
        await bulkOperationLocatorTest.selection_addSegmentCancelButton.click();
    }

    this.clickOnSaveAddSegment = async function () {
        await bulkOperationLocatorTest.selection_addSegmentSaveButton.click();
    }

    this.clickOnFooterButtons = async function (num) {
        await bulkOperationLocatorTest.bulkOperation_footerButtons(num).click();
    }
};
module.exports = Bulkoperation;