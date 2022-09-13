describe('IDMS homepage', function () {
  var HomepageTest = new Homepage();
  var HomepageLocatorTest = new HomepageLocator();
  var DashboardTest = new Dashboard();
  var HomepageTest = new Homepage();

  it('should greet the named user', function () {
    HomepageTest.get();
    HomepageTest.setUsername();
    HomepageTest.setPassword();
    expect(HomepageTest.getLoginPageText()).toEqual('Log in');
    expect(HomepageTest.getRememberText()).toEqual('Remember me');
    expect(HomepageTest.getForgetPasswordText()).toEqual('Forgot password?');
    expect(HomepageLocatorTest.checkButtonRem.isPresent()).toBe(true);
    expect(HomepageLocatorTest.checkButtonRem.isSelected()).toBe(false);
    expect(HomepageLocatorTest.loginButton.isPresent()).toBe(true);
    HomepageTest.checkRememberBtn();
    HomepageTest.clickLoginButton();

  });

  it('Logout from login', function () {
    HomepageTest.wait();
    DashboardTest.clickOnUser();
    DashboardTest.clickOnLogout();
  });

});
























