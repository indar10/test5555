const SpecReporter = require('jasmine-spec-reporter').SpecReporter;
const Jasmine2HtmlReporter = require('protractor-jasmine2-html-reporter');
const HtmlReporter = require('protractor-beautiful-reporter');
const nodemailer = require('nodemailer');
const jasmineReporters = require('jasmine-reporters');
const fs = require('fs-extra');
const HTMLReport = require('protractor-html-reporter-2');
var today = new Date();
var timeStamp = today.getMonth() + 1 + '-' + today.getDate() + '-' + today.getFullYear();
var hourSec = + '-' + today.getHours() + 'h-' + today.getMinutes() + 'm-' + today.getSeconds() + 's';
var reportPath = '\\\\stcsanisln01\\FS01\\IT\\DevTeam\\Projects\\vNext\\AutomatedTests\\Reports\\';
var summaryReportPath = 'SummaryReports ';
var emailReportPath = 'EmailReports ';
var htmlReportPath = 'HtmlReports ';
var screenshotReportPath = '\\Screenshots';
var xmlResultPath = '\\xmlresults.xml';
var consolidateReportpath = '\\IDMSConsolidateTestReport.html';
var htmlpath = '\\htmlReport.html';

exports.config = {
  seleniumAddress: 'http://localhost:4444/wd/hub',

  capabilities: {
    'browserName': 'chrome',
    shardTestFiles: true,
    maxInstances: 1,
    //acceptInsecureCerts: true
  },
  /*multiCapabilities: [
    { 'browserName': 'chrome' },
    //{ 'browserName': 'firefox' },
    { 'browserName': 'MicrosoftEdge' }
  ],*/
  params: {
    email: 'AutoTestUserA',
    password: 'Cybage@123',
    baseUrl: 'https://vnext.idms.data-axle.com/',
    wait: 100
  },

  framework: 'jasmine2',
  suites: {
    Homepage: ['./Specs/homepage.spec.js'],
    Campaign: ['./Specs/campaign.spec.js'],
    Campaignstatus: ['./Specs/campaign-status.spec.js'],
    Dashboard: ['./Specs/dashboard.spec.js'],
    Campaigncombine: ['./Specs/campaign.spec.js', './Specs/campaign-status.spec.js'],
    Newcampaigncombine: ['./Specs/new-campaign.spec.js', './Specs/campaign.spec.js'],
    Combine: ['./Specs/homepage.spec.js', './Specs/campaign.spec.js', './Specs/campaign-status.spec.js'],
    Newcampaign: ['./Specs/new-campaign.spec.js'],
    Selection: ['./Specs/selection.spec.js'],
    Bulkoperation: ['./Specs/bulk-operation.spec.js'],
  },

  jasmineNodeOpts: {
    showColors: true,
    defaultTimeoutInterval: 600000,
    allScriptsTimeout: 110000,
    getPageTimeout: 110000

  },

  onPrepare: function () {
    browser.driver.manage().window().maximize();
    browser.manage().deleteAllCookies();
    browser.waitForAngularEnabled(true);
    browser.manage().timeouts().pageLoadTimeout(40000);
    browser.manage().timeouts().implicitlyWait(25000);
    browser.ignoreSynchronization = true;

    jasmine.getEnv().addReporter(new SpecReporter({
      displayStacktrace: 'all',
      displaySuccessesSummary: false,
      displayFailuresSummary: true,
      displayPendingSummary: true,
      displaySuccessfulSpec: true,
      displayFailedSpec: true,
      displayPendingSpec: false,
      displaySpecDuration: false,
      displaySuiteNumber: false,
      colors: {
        success: 'green',
        failure: 'red',
        pending: 'yellow'
      },
      prefixes: {
        success: '✓ ',
        failure: '✗ ',
        pending: '* '
      },
      customProcessors: []
    }));

    //true: to generate report
    //false: not to generate report
    summaryReport(false);
    emailReport(false);
    htmlReport(false);

    global.Homepage = require('./Page_object/Pagepo/homepage.po.js');
    global.Dashboard = require('./Page_object/Pagepo/dashboard.po.js');
    global.Campaign = require('./Page_object/Pagepo/campaign.po.js');
    global.Campaignstatus = require('./Page_object/Pagepo/campaign-status.po.js');
    global.newCampaign = require('./Page_object/Pagepo/new-campaign.po.js');
    global.Selection = require('./Page_object/Pagepo/selection.po.js');
    global.Bulkoperation = require('./Page_object/Pagepo/bulk-operation.po.js');

    global.HomepageLocator = require('./Page_object/Locators/home.locator.js');
    global.CampaignpageLocator = require('./Page_object/Locators/campaign.locator.js');
    global.DashboardpageLocator = require('./Page_object/Locators/dashboard.locator.js');
    global.CampaignstatuspageLocator = require('./Page_object/Locators/campaign-status.locator.js');
    global.newCampaignpageLocator = require('./Page_object/Locators/new-campaign.locator.js');
    global.SelectionLocator = require('./Page_object/Locators/selection.locator.js');
    global.BulkoperationLocator = require('./Page_object/Locators/bulk-operation.locator.js');

    global.CampaignConstant = require('./Page_object/Utility/campaign.constant.js');
    global.Campaignstatusconstant = require('./Page_object/Utility/campaign-status.constant.js');
    global.Homepageconstant = require('./Page_object/Utility/homepage.constant.js');
    global.newCampaignConstant = require('./Page_object/Utility/new-campaign.constant.js');
    global.SelectionConstant = require('./Page_object/Utility/selection.constant.js');
    global.Bulkoperationconstant = require('./Page_object/Utility/bulk-operation.constant.js');

    /*return new Promise(function (fulfill, reject) {
      browser.getCapabilities().then(function (value) {
        reportName = value.get('webdriver.remote.sessionid') + '_' + value.get('browserName');
        jasmine.getEnv().addReporter(
          new Jasmine2HtmlReporter({
            savePath: reportPath + emailReportPath + timeStamp + '\\',
            screenshotsFolder: 'images',
            takeScreenshotsOnlyOnFailures: true,
            consolidate: true,
            consolidateAll: true,
            preserveDirectory: true,
            cleanDirectory: false,
            cleanDestination: false,
            fileName: "IDMSReport.html",
            fileNamePrefix: reportName + ".html"
          })
        );
        fulfill();
      })
    });*/

  },

  /*afterLaunch: function afterLaunch() {
    var fs = require('fs');
    var output = '';
    fs.readdirSync(reportPath + emailReportPath + timeStamp + '\\').forEach(function (file) {
      if (!(fs.lstatSync(reportPath + emailReportPath + timeStamp + '\\' + file).isDirectory()))
        output = output + fs.readFileSync(reportPath + emailReportPath + timeStamp + '\\' + file);
    });
    fs.writeFileSync(reportPath + emailReportPath + timeStamp + consolidateReportpath, output, 'utf8');
  },*/

  onComplete: function () {

    onCompleteHtml(false);

    /*return new Promise(function (fulfill, reject) {
      var transporter = nodemailer.createTransport({
        host: 'mail-relay.intra.infousa.com',
        secure: false,
        port: 25,
        tls: {
          rejectUnauthorized: false
        },
        auth: {
          user: 'madhur.sarangpure@infogroup.com',
          pass: 'Automation@22'
        },
      });
      var mailOptions = {
        from: 'madhur.sarangpure@infogroup.com', // sender address
        to: 'madhur.sarangpure@infogroup.com', // list of receivers
        subject: 'IDMS test report(Test mail)', // Subject line
        text: 'This is IDMS Automation test report',
        attachments: [{
          'path': reportPath + emailReportPath + timeStamp + htmlpath,
        }]
      };
      transporter.sendMail(mailOptions, function (error, info) {
        if (error) {
          reject(error);
          return console.log(error);
        }
        console.log('Mail sent: ' + info.response);
        fulfill(info);
      });
    });*/
  },
};

var emailReport = function (flag) {
  if (flag === true) {
    jasmine.getEnv().addReporter(
      new Jasmine2HtmlReporter({
        savePath: reportPath + emailReportPath + timeStamp + '\\',
        takeScreenshots: true,
        takeScreenshotsOnlyOnFailures: true,
        filePrefix: 'automationReport',
        consolidate: true,
        cleanDestination: false,
        consolidateAll: true
      })
    );
  }
};

var getSpecName = async function () {
  var config = await browser.getProcessedConfig();
  var fullName = await config.specs[0];
  var fileName = fullName.substring(fullName.lastIndexOf('\\') + 1);
  var spec = fileName.split(".", 1);
  var name = spec[0].trim();
  return name;
}

var summaryReport = async function (flag) {
  if (flag === true) {
    jasmine.getEnv().addReporter(new HtmlReporter({
      baseDirectory: reportPath + summaryReportPath + timeStamp,
      preserveDirectory: false,
      docTitle: 'IDMS Automation Execution Summary',
      docName: 'IDMS-Test-Report.html',
      screenshotsSubfolder: 'images',
      jsonsSubfolder: 'jsons',
      gatherBrowserLogs: true,
      takeScreenShotsForSkippedSpecs: false,
      takeScreenShotsOnlyForFailedSpecs: true,
      excludeSkippedSpecs: true,
      consolidate: true,
      consolidateAll: true,
      cleanDestination: false,
      disableScreenshots: false
      , clientDefaults: {
        columnSettings: {
          displayTime: true,
          displayBrowser: true,
          displaySessionId: true,
          displayOS: true,
          inlineScreenshots: false
        },
        searchSettings: {
          allselected: true,
          passed: true,
          failed: true,
          pending: true,
          withLog: true
        },
        columnSettings: {
          warningTime: 1000,
          dangerTime: 1500
        }
      }

    }).getJasmine2Reporter());
  }
}

var htmlReport = async function (htmlFlag) {
  if (htmlFlag === true) {
    jasmine.getEnv().addReporter(new jasmineReporters.JUnitXmlReporter({
      consolidateAll: true,
      savePath: reportPath + htmlReportPath + timeStamp + '\\',
      filePrefix: 'xmlresults'
    }));

    fs.emptyDir(reportPath + htmlReportPath + timeStamp + screenshotReportPath, function (err) {
      console.log(err);
    });

    jasmine.getEnv().addReporter({
      specDone: function (result) {
        if (result.status == 'failed') {
          browser.getCapabilities().then(function (caps) {
            var browserName = caps.get('browserName');

            browser.takeScreenshot().then(function (png) {
              var stream = fs.createWriteStream(reportPath + htmlReportPath + timeStamp + screenshotReportPath + browserName + '-' + result.fullName + '.png');
              stream.write(new Buffer(png, 'base64'));
              stream.end();
            });
          });
        }
      }
    });
  }
}
var onCompleteHtml = async function (flag) {
  if (flag === true) {
    var browserName, browserVersion, platform;
    var capsPromise = browser.getCapabilities();

    capsPromise.then(function (caps) {
      browserName = caps.get('browserName');
      browserVersion = caps.get('version');
      platform = caps.get('platform');

      testConfig = {
        reportTitle: 'IDMS Test Execution Report',
        outputPath: reportPath + htmlReportPath + timeStamp + '\\',
        outputFilename: 'IDMSTestReport',
        screenshotPath: reportPath + htmlReportPath + timeStamp + screenshotReportPath,
        testBrowser: browserName,
        browserVersion: browserVersion,
        modifiedSuiteName: true,
        screenshotsOnlyOnFailure: true,
        testPlatform: platform
      };
      new HTMLReport().from(reportPath + htmlReportPath + timeStamp + xmlResultPath, testConfig);
    });
  }
}


