var HomepageLocator = function () {
  this.username = element(by.css('[name="userNameOrEmailAddress"]'));
  this.password = element(by.css('[name="password"]'));
  this.loginPageText = element(by.css('.kt-login__title h3'));
  this.loginButton = element(by.css('[type="submit"]'));
  this.loginTextLeft = element(by.css('h3[class="kt-login__title"]'));
  this.rememberMe = element(by.css('[class="kt-checkbox"]'));
  this.forgetPassword = element(by.css('[id="forget-password"]'));
  this.checkButtonRem = element(by.css('[class="ng-tns-c1-0"]'));
  this.cookieMessage = element(by.css('[class="cc-btn cc-dismiss"]'));

};
module.exports = HomepageLocator