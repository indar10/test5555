var Homepage = function () {
  var HomepageLocatorTest = new HomepageLocator();

  this.get = function () {
    browser.manage().deleteAllCookies();
    browser.waitForAngular();
    browser.driver.get(browser.params.baseUrl);
    browser.executeScript('window.localStorage.clear();');
    browser.executeScript('window.sessionStorage.clear();');
    browser.manage().deleteAllCookies();
    browser.waitForAngular();
  };
  this.setUsername = function () {
    return HomepageLocatorTest.username.sendKeys(browser.params.email);
  };
  this.setPassword = function () {
    return HomepageLocatorTest.password.sendKeys(browser.params.password);
  };
  this.getUsername = function () {
    return HomepageLocatorTest.username.getText();
  };
  this.getPassword = function () {
    return HomepageLocatorTest.password.getText();
  };
  this.getLoginPageText = function () {
    return HomepageLocatorTest.loginPageText.getText();
  };
  this.clickLoginButton = function () {
    return HomepageLocatorTest.loginButton.click();
  };
  this.getloginTextLeft = function () {
    return HomepageLocatorTest.loginTextLeft.getText();
  };
  this.getRememberText = function () {
    return HomepageLocatorTest.rememberMe.getText();
  };
  this.getForgetPasswordText = function () {
    return HomepageLocatorTest.forgetPassword.getText();
  };
  this.checkRememberBtn = function () {
    return HomepageLocatorTest.checkButtonRem.click();
  };
  this.clickOnCookieBtn = function () {
    return HomepageLocatorTest.cookieMessage.click();
  };
  this.await = function (val) {
    var sec = val * browser.params.wait;
    return sec;
  };
  this.wait = function (val) {
    var sec = val * browser.params.wait;
    return browser.sleep(sec);
  };
  this.refresh = function () {
    browser.waitForAngular();
    browser.navigate().refresh();
  };
};
module.exports = Homepage;






