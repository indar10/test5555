var HomepageTest = new Homepage();
var CampaignTest = new Campaign();
var DashboardTest = new Dashboard();
var CampaignpageLocatorTest = new CampaignpageLocator();
var DashboardpageLocatorTest = new DashboardpageLocator();

describe('IDMS Dashboard',function(){
    it('Go to Dashboard and click on user',function(){
        HomepageTest.get();
        HomepageTest.setUsername();
        HomepageTest.setPassword();
        HomepageTest.clickLoginButton();
        
    });
    it('Logout from Dashboard',function(){
        HomepageTest.wait();
        DashboardTest.clickOnUser();
        DashboardTest.clickOnLogout();
    });

});