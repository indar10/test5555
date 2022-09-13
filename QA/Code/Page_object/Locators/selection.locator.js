var SelectionLocator = function () {

    this.selection_selectSearchedField = element(by.css('[class="chosen-container chosen-container-single chosen-with-drop chosen-container-active"] ul>li:nth-child(2)'));
    this.selection_ruleDropdownSearch = element.all(by.css('[class="chosen-choices"]>li>input'));
    this.selection_selectSearchedRule = element.all(by.css('[class="chosen-results"]>li:nth-child(1)'));
    this.selection_ruleTextArea = element.all(by.css('[class="form-control"]'));
    this.selection_addGroupButton = element.all(by.css('[data-add="group"]'));
    this.selection_addRuleButton = element.all(by.css('[class="btn btn-xs btn-success-addrule"]'));
    this.selection_typeOnRuleSearch = element(by.css('[class="chosen-container chosen-container-single chosen-with-drop chosen-container-active"] [class="chosen-search-input"]'));
    this.selection_adSizeCodeA_Regular = element(by.css('[value="A|||REGULAR"]'));
    this.selection_adSizeCodeB_Bold = element(by.css('[value="B|||BOLD"]'));
    this.selection_expenseAccounting_A = element(by.css('[value="A|||LESS THAN $500"]'));
    this.selection_expenseAccounting_B = element(by.css('[value="B|||$500 - $1,000"]'));
    this.selection_expenseAccounting_C = element(by.css('[value="C|||$1,000 - $2,500"]'));
    this.selection_businessStatus_Headquarter = element(by.css('[value="1|||HEADQUARTER"]'));
    this.selection_businessStatus_Branch = element(by.css('[value="2|||BRANCH"]'));
    this.selection_businessStatus_Subsidiary = element(by.css('[value="3|||SUBSIDIARY HEADQUARTER"]'));


};
module.exports = SelectionLocator;