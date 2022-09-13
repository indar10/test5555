var DashboardpageLocator = function () {
    this.userButton = element(by.css('[class= "kt-header__topbar-username kt-hidden-mobile"]'));
    this.logoutButton = element(by.css('[class="btn btn-label-brand btn-sm btn-bold"]'));
    this.sideNavigation_dashboardButton = element(by.css('#kt_aside_menu > ul li:nth-child(1)'));

}
module.exports = DashboardpageLocator