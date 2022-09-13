 
# Campaign UI Development Environment Set Up
## Dependencies
1.  [Dotnet Core 2.2 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
2.  [Nodejs](https://nodejs.org/en/)
3.  [Git](https://git-scm.com/)
4.  [Yarn](https://classic.yarnpkg.com/en/docs/install#windows-stable)
5.  [Angular CLI](https://angular.io/cli#installing-angular-cli)
6.  [Redis](./redis-setup-dev.md)

## Steps

 1. Create a folder name Development.
 2. Open Git bash in the same folder.
 3. Clone the repo : [IDMS_NG](https://github.com/IDMS-IG/IDMS_NG) by entering the following command.
git clone https://github.com/IDMS-IG/IDMS_NG.git
 4. Open Infogroup.IDMS.Web.sln in Visual Studio.
 5. Set Infogroup.IDMS.Web.Host project as the Startup project.
 6. Get appsetting.json file from the Team and add it to the Host Project.
 7. Run the Host Project.
 8. Open command line in the same directory as the Infogroup.IDMS.Web.Host project.
 9. Run the following command to download the node dependencies.
         `yarn` 
 10. Run the angular project.
 `yarn start`
 11. Get appconfig.json file from the Team and add it to the Host Project under src/assets/
 11. Open the web browser at http://localhost:4200/
