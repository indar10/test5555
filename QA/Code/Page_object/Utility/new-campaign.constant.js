var newCampaignConstant = function () {
    this.newCampaign_nonDivisionalAllLabels = ['General', 'Database *', 'Description *', 'Customer *', 'Offer *', 'Build *', 'Channel *', 'Type *', 'Postal', 'Tele-marketing', 'Email', 'Net', 'Gross', 'Max Per Field', 'Max Per Quantity'];
    this.newCampaign_divisionalAllLabels = ['General', 'Database *', 'Description *', 'Customer', 'Broker', 'Offer', 'Build *', 'Channel *', 'Type *', 'Postal', 'Tele-marketing', 'Email', 'Net', 'Gross', 'Max Per Field', 'Max Per Quantity'];
    this.campaignList_allTableLabels = ['ID', 'Description', 'Customer', '', 'Database', 'Provided Qty', 'Available Qty', 'Status', 'Status Date', 'Actions'];
    this.newCampaign_allDatabases = ['Canadian Data Warehouse : 66', 'Infogroup Consumer Database : 1267', 'Infogroup US Business Database : 992', 'mGen : 847', 'Sapphire : 71', 'US Data Warehouse : 65'];
    this.newCampaign_allDatabaseName = ['Canadian Data Warehouse', 'Infogroup Consumer Database', 'Infogroup US Business Database', 'mGen', 'Sapphire', 'US Data Warehouse '];
    this.newCampaign_allDatabaseID = ['66', '1267', '992', '847', '71', '65'];
    this.newCampaign_divisionalDatabaseID = [1267, 992, 847, 71];
    this.newCampaign_multipleOfferList = ['Select Offer', 'AutoTest Offer1 : 5857', 'AutoTest Offer2 : 5858'];
    this.newCampaign_singleOfferList = ['Select Offer', '01-ONLINE : 2568'];
    this.newCampaign_databaseCountNDM = this.newCampaign_allDatabases.length;
    this.nonDivisionalValues = {
        database: 65,
        description: "Automation Description Test",
        customer: "5778",
        offer: "5859",
        build: "12400",
    };
    this.divsionalValues = {
        database: 992,
        description: "Automation Description Test",
        customer: "18741",
        broker: "1648",
        offer: "'Offer field Test'",
        build: "12735",
    };
    this.mandatoryDivisionalFields = {
        database: 992,
        description: "Automation Description Test",
        customer: "AutoTest Customer",
        broker: "AutoTest Broker",
        build: "March 2020",
    };
    this.mandatoryNonDivisionalFields = {
        database: 65,
        description: "Automation Description Test",
        customer: "AutoTest Mailer1",
        offer: "AutoTest Offer1",
        build: "September 2017",
    };
    this.divisionalIteration = 6;
    this.nonDivisionalIteration = 5;
    this.divisionalLabelCount = 7;
    this.nonDivisionalLabelCount = 6;
    this.newCampaign_defaultBroker = 'Direct Development';
    this.newCampaign_twoChar = 'Au';
    this.newCampaign_moreThanTwoChar = 'Auto';
    this.newCampaign_divisionalCustomerNameIdTest = 'A3 Association for Advancing Automation : 13065';
    this.newCampaign_nonDivisionalCustomerId = '5777';
    this.newCampaign_nonDivisionalCustomerNameId = 'AutoTest Mailer1 : 5777';
    this.newCampaign_divisionalBrokerId = '1648';
    this.newCampaign_divisionalBrokerNameId = 'AutoTest Broker : 1648';
    this.newCampaign_nonDivisionalCustomerAdmin = 'Admin';
    this.newCampaign_nonDivisionalOffer = '01-ONLINE : 2568';
    this.newCampaign_divisionalCustomerId = '18741';
    this.newCampaign_divisionalCustomerNameId = 'AutoTest Customer : 18741';
    this.newCampaign_singleQuote = "'";

};
module.exports = newCampaignConstant;