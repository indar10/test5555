var newCampaign = function () {

    var HomepageTest = new Homepage();
    var newCampaignLocatorTest = new newCampaignpageLocator();
    var CampaignpageLocatorTest = new CampaignpageLocator();
    var CampaignstatuspagelocatorTest = new CampaignstatuspageLocator();
    var newCampaignConstantTest = new newCampaignConstant();
    var EC = protractor.ExpectedConditions;

    this.getCampaignTitle = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.campaignList_campaignTitle), HomepageTest.await(50));
        return await CampaignpageLocatorTest.campaignList_campaignTitle.getText();
    };

    this.getAllNewCampaignLabel = async function (iteration, num) {
        var labels = [];
        labels.push(await newCampaignLocatorTest.newCampaign_NDMGeneralTabLabel.getText());
        for (i = 1; i <= iteration; i++) {
            labels.push(await newCampaignLocatorTest.newCampaign_databaseToBuildLabels(i).getText());
        }
        labels.push(await newCampaignLocatorTest.newCampaign_channelLabel(num).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_typeLabel(num).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_postalLabel(num).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_telemarketingLabel(num).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_emailLabel(num).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_netLabel(num).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_grossLabel(num).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_maxPerFieldLabel(num + 1).getText());
        labels.push(await newCampaignLocatorTest.newCampaign_maxPerQuantityLabel(num + 1).getText());
        return labels;
    };

    this.getAllCampaignListTableLabel = async function () {
        var labels = [];
        for (i = 2; i <= 11; i++) {
            labels.push(await newCampaignLocatorTest.campaignList_allTableLabels(i).getText());
        }
        return labels;
    };

    this.typeOnDatabaseSearchField = async function (database) {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_databaseSearch), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_databaseSearch.sendKeys(database);
    };

    this.typeOnDescriptionField = async function (description) {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_descriptionField), HomepageTest.await(50));
        await CampaignpageLocatorTest.newCampaign_descriptionField.sendKeys(description);
    }

    this.selectFirstSearchElement = async function () {
        browser.wait(EC.presenceOf(CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab), HomepageTest.await(50));
        return await CampaignstatuspagelocatorTest.editCampaign_SearchFieldFirstElementOutputTab.click();
    };

    this.getSelectedDatabase = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_selectedDatabase), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_selectedDatabase.getText();
    };

    this.getSelectedBroker = async function () {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_brokerField), HomepageTest.await(50));
        return await newCampaignLocatorTest.newCampaign_brokerField.getAttribute('value');
    };

    this.getSelectedCustomer = async function () {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_customerField), HomepageTest.await(50));
        return await newCampaignLocatorTest.newCampaign_customerField.getAttribute('value');
    };

    this.getSelectedOffer = async function () {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_offerField), HomepageTest.await(50));
        return await newCampaignLocatorTest.newCampaign_offerField.getText();
    };

    this.getDropdownList = async function () {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_dropdownList), HomepageTest.await(50));
        return await newCampaignLocatorTest.newCampaign_dropdownList.getText();
    };

    this.getOfferValues = async function (offerNum, value) {
        var offer = await newCampaignConstantTest.newCampaign_multipleOfferList[offerNum];
        var name = await offer.split(":");
        var offerName = await name[value].trim();
        return offerName;
    };

    this.clickOnOfferDropdown = async function () {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_offerDropdownButton), HomepageTest.await(50));
        await newCampaignLocatorTest.newCampaign_offerDropdownButton.click();
    }

    this.clearCustomerField = async function (flag) {
        if (flag === true) {
            browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_customerField), HomepageTest.await(50));
            await newCampaignLocatorTest.newCampaign_customerField.clear();
        } else {
            newCampaignLocatorTest.newCampaign_customerField.sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
            newCampaignLocatorTest.newCampaign_customerField.sendKeys(protractor.Key.BACK_SPACE);
        }
    };

    this.clearBrokerField = async function (flag) {
        if (flag === true) {
            browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_brokerField), HomepageTest.await(50));
            await newCampaignLocatorTest.newCampaign_brokerField.clear();
        } else {
            newCampaignLocatorTest.newCampaign_brokerField.sendKeys(protractor.Key.chord(protractor.Key.CONTROL, "a"));
            newCampaignLocatorTest.newCampaign_brokerField.sendKeys(protractor.Key.BACK_SPACE);
        }
    };

    this.selectDatabase = async function (database) {
        await CampaignpageLocatorTest.newCampaign_database_dropdownbutton.click();
        HomepageTest.wait(20);
        await CampaignpageLocatorTest.newCampaign_databaseSearch.sendKeys(database);
        await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
    };

    this.selectNonDivisionalOffer = async function (offer) {
        await newCampaignLocatorTest.newCampaign_SearchField_nonDivisional.sendKeys(offer);
        await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
    };

    this.clearDropdownSearchfield = async function () {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_SearchField_nonDivisional), HomepageTest.await(50));
        await newCampaignLocatorTest.newCampaign_SearchField_nonDivisional.clear();
    };

    this.getDatabaseListNDM = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_databaseList), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_databaseList.getText();
    };

    this.getSortedDatabaseIds = async function () {
        var unsortedData = []; var splitData = []; var ids = [];
        var database = await CampaignpageLocatorTest.newCampaign_databaseList.getText();
        for (i = 0; i < database.length; i++) {
            data = database[i].toLowerCase();
            unsortedData.push(data);
        }
        var sortedData = unsortedData.sort();
        for (i = 0; i < sortedData.length; i++) {
            splitData = sortedData[i].split(":");
            trimId = splitData[1].trim();
            ids.push(trimId);
        }
        return ids;
    };

    this.selectCustomer = async function (customer) {
        await newCampaignLocatorTest.newCampaign_customerSearch.sendKeys(customer);
        await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
    };

    this.typeOnCustomerField = async function (customer) {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_customerSearch), HomepageTest.await(50));
        await newCampaignLocatorTest.newCampaign_customerSearch.sendKeys(customer);
    };

    this.clickOnCustomerDropdown = async function () {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_customerDropdown), HomepageTest.await(50));
        await newCampaignLocatorTest.newCampaign_customerDropdown.click();
    };

    this.selectBroker = async function (broker) {
        await newCampaignLocatorTest.newCampaign_brokerSearch.sendKeys(broker);
        CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
    };

    this.selectBuild = async function (build) {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_buildDropdown), HomepageTest.await(50));
        await newCampaignLocatorTest.newCampaign_buildDropdown.click();
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_buildDropdownSearch), HomepageTest.await(50));
        await newCampaignLocatorTest.newCampaign_buildDropdownSearch.sendKeys(build);
        HomepageTest.wait(10);
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement), HomepageTest.await(50));
        await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
    };

    this.typeOnBrokerField = async function (broker) {
        browser.wait(EC.presenceOf(newCampaignLocatorTest.newCampaign_brokerSearch), HomepageTest.await(50));
        await newCampaignLocatorTest.newCampaign_brokerSearch.sendKeys(broker);
    };

    this.createCampaignNDM = async function (values, appendTimeStamp, save) {
        var date = new Date();
        var timeStamp = appendTimeStamp ? "' " + date.getDate() + "-" + (date.getMonth() + 1) + "-" + date.getFullYear() + " '" : '';
        await CampaignpageLocatorTest.campaignList_newCampaignButton.click();
        await CampaignpageLocatorTest.newCampaign_database_dropdownbutton.click();
        HomepageTest.wait(20);
        await CampaignpageLocatorTest.newCampaign_databaseSearch.sendKeys(values.database);
        await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
        await CampaignpageLocatorTest.newCampaign_descriptionField.clear();
        await CampaignpageLocatorTest.newCampaign_descriptionField.sendKeys(values.description + timeStamp);

        if (newCampaignConstantTest.newCampaign_divisionalDatabaseID.includes(values.database)) {
            await newCampaignLocatorTest.newCampaign_customerSearch.sendKeys(values.customer);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            await newCampaignLocatorTest.newCampaign_brokerSearch.sendKeys(values.broker);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            await CampaignpageLocatorTest.newCampaign_offerField.sendKeys(values.offer);
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_buildDropdown.click();
            await newCampaignLocatorTest.newCampaign_buildDropdownSearch.sendKeys(values.build);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_channel_postalButton.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_type_netButton.click();
        } else {
            await newCampaignLocatorTest.newCampaign_customerSearch.sendKeys(values.customer);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            await newCampaignLocatorTest.newCampaign_offerDropdown_nonDivisional.click();
            await newCampaignLocatorTest.newCampaign_SearchField_nonDivisional.sendKeys(values.offer);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            await newCampaignLocatorTest.newCampaign_buildDropdown_nonDivisional.click();
            await newCampaignLocatorTest.newCampaign_SearchField_nonDivisional.sendKeys(values.build);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            await newCampaignLocatorTest.newCampaign_channel_postalButton_nonDivisional.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_type_netButton_nonDivisional.click();
        }
        if (save === true) {
            await CampaignpageLocatorTest.newCampaign_saveButton_footer.click();
        } else {
            await CampaignpageLocatorTest.newCampaign_cancelButton_footer.click();
        }
    };

    this.newCampaignMandatoryFields = async function (values, appendTimeStamp) {
        var date = new Date();
        var timeStamp = appendTimeStamp ? "' " + date.getDate() + "-" + (date.getMonth() + 1) + "-" + date.getFullYear() + " '" : '';
        await CampaignpageLocatorTest.newCampaign_database_dropdownbutton.click();
        HomepageTest.wait(20);
        await CampaignpageLocatorTest.newCampaign_databaseSearch.sendKeys(values.database);
        await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
        await CampaignpageLocatorTest.newCampaign_descriptionField.clear();
        await CampaignpageLocatorTest.newCampaign_descriptionField.sendKeys(values.description + timeStamp);

        if (newCampaignConstantTest.newCampaign_divisionalDatabaseID.includes(values.database)) {
            await newCampaignLocatorTest.newCampaign_customerSearch.sendKeys(values.customer);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            await newCampaignLocatorTest.newCampaign_brokerSearch.sendKeys(values.broker);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_buildDropdown.click();
            await newCampaignLocatorTest.newCampaign_buildDropdownSearch.sendKeys(values.build);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_channel_postalButton.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_type_netButton.click();
        } else {
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_customerSearch.sendKeys(values.customer);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            await newCampaignLocatorTest.newCampaign_offerDropdown_nonDivisional.click();
            await newCampaignLocatorTest.newCampaign_SearchField_nonDivisional.sendKeys(values.offer);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_buildDropdown_nonDivisional.click();
            await newCampaignLocatorTest.newCampaign_SearchField_nonDivisional.sendKeys(values.build);
            HomepageTest.wait(10);
            await CampaignpageLocatorTest.newCampaign_databaseSearch_firstElement.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_channel_postalButton_nonDivisional.click();
            HomepageTest.wait(10);
            await newCampaignLocatorTest.newCampaign_type_netButton_nonDivisional.click();
        }
    };

    this.typeOnDivisionalOfferField = async function (offer) {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_offerField), HomepageTest.await(50));
        await CampaignpageLocatorTest.newCampaign_offerField.sendKeys(offer);
    };

    this.checkSaveButton = async function () {
        browser.wait(EC.presenceOf(CampaignpageLocatorTest.newCampaign_saveButton_footer), HomepageTest.await(50));
        return await CampaignpageLocatorTest.newCampaign_saveButton_footer.isEnabled();
    };
};
module.exports = newCampaign;