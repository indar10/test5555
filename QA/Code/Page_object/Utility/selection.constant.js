var SelectionConstant = function () {

    this.selection_fieldName = ['Address Type', 'Fulfillment Flag', 'Infogroup Contact', 'Company Name', 'IGID', 'Ad Size Code', 'Call Status Code', 'Affiliated Locations', 'IG Business DB Indicator', 'Location State', 'Location City', 'Expense - Accounting', 'Contact Title Role Code', 'Location County Code', 'Business Status', 'Contact Professional Title'];
    this.selection_addressTypeValues = ['FIRM', 'GENERAL DELIVERY', 'HIGH-RISE'];
    this.selection_companyNameValues = ['Microsoft,', 'Infogroup,', 'Apple'];
    this.selection_IGIDValues = ['Test 1234,', 'IGID Field Test'];
    this.selection_adSizeCodeValues = ['A : REGULAR', 'B : BOLD'];
    this.selection_callStatusCode = ['C : CONTACT/EMP VERIFIED', 'P : CONTACT VERIFIED', 'W : EMP COUNT VERIFIED'];
    this.selection_affiliatedLocations = ['Texas,', 'California,', 'Florida'];
    this.selection_locationStates = ['CA : CALIFORNIA', 'FL : FLORIDA', 'IL : ILLINOIS', 'WA : WASHINGTON'];
    this.selection_locationCity = ['New York,', 'Washigton DC,', 'Test1234'];
    this.selection_contactTitleRoleCode = ['1003 : VICE CHAIR', '1004 : BOARD MEMBER/NON-EXECUTIVE DIRECTOR', '1007 : CHIEF OFFICER(OTHER)'];
    this.selection_locationCountryCode = ['10020030,', 'Test Country Code,', 'Test Code 007'];
    this.selection_contactProfessionalTitle = ['AUD : AUDIOLOGIST', 'CFP : CERTIFIED FINANCIAL PLANNER', 'CNP : CERTIFIED NURSE PRACTITIONER'];

};
module.exports = SelectionConstant;