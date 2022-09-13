var Dashboard = function () {
    var DashboardpageLocatorTest = new DashboardpageLocator();
    this.clickOnSideNavDashboardButton = function () {
        return DashboardpageLocatorTest.sideNavigation_dashboardButton.click();
    }
    this.clickOnUser = function () {
        browser.waitForAngular();
        DashboardpageLocatorTest.userButton.click();
    };
    this.clickOnLogout = function () {
        return DashboardpageLocatorTest.logoutButton.click();
    };

};
module.exports = Dashboard