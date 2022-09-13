var Selection = function () {

    var HomepageTest = new Homepage();
    var selectionlocatorTest = new SelectionLocator();

    this.typeOnRule = async function (field) {
        await selectionlocatorTest.selection_typeOnRuleSearch.sendKeys(field);
        await selectionlocatorTest.selection_selectSearchedField.click();
    };
    this.typeOnRuleDropdown = async function (num, values) {
        for (i = 0; i < values.length; i++) {
            HomepageTest.wait(10);
            await selectionlocatorTest.selection_ruleDropdownSearch.get(num).click();
            await selectionlocatorTest.selection_ruleDropdownSearch.get(num).sendKeys(values[i]);
            HomepageTest.wait(10);
            await selectionlocatorTest.selection_selectSearchedRule.get(num).click();
        }
    };
    this.typeOnRuleTextArea = async function (num, text) {
        for (i = 0; i < text.length; i++) {
            await selectionlocatorTest.selection_ruleTextArea.get(num).click();
            await selectionlocatorTest.selection_ruleTextArea.get(num).sendKeys(text[i]);
        }
    };
    this.clickOnAddRuleButton = async function (num) {
        await selectionlocatorTest.selection_addRuleButton.get(num).click();
    };
    this.clickOnAddGroupButton = async function (num) {
        await selectionlocatorTest.selection_addGroupButton.get(num).click();
    };
    this.checkAdSizeCodeValues = async function () {
        await selectionlocatorTest.selection_adSizeCodeA_Regular.click();
        await selectionlocatorTest.selection_adSizeCodeB_Bold.click();
    };
    this.checkExpenseAccountingValues = async function () {
        await selectionlocatorTest.selection_expenseAccounting_A.click();
        await selectionlocatorTest.selection_expenseAccounting_B.click();
        await selectionlocatorTest.selection_expenseAccounting_C.click();
    };
    this.checkBusinessStatusValues = async function () {
        await selectionlocatorTest.selection_businessStatus_Headquarter.click();
        await selectionlocatorTest.selection_businessStatus_Branch.click();
        await selectionlocatorTest.selection_businessStatus_Subsidiary.click();
    };


};
module.exports = Selection;