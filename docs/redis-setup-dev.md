## Redis Setup (Dev)

 1. Download Redis msi file from this URL.
    [Redis Setup](https://github.com/MSOpenTech/redis/releases/download/win-3.2.100/Redis-x64-3.2.100.msi)
 2. The setup wizard would open clicking on the msi and we need to check follow through the wizard.
 3. Click next in the wizard to get to the agreement page and accept the agreement and click on next.
 4. Set C:\Program Files\Redis as the default location if not set already.
 5. In the next screen make sure the default port is set to 6379.
 6. Click on the checkbox to whitelist Redis on the Windows Firewall.
 7. In the next screen you can allocate the memory for Redis Storage, the default is set to 100MB and click on Next.
 8. After this click on Install to start the installation.
 9.  This will install the Redis Server as a Windows service.
 
 ## Changes in Campaign UI
 In Campaign UI application, navigate to the appsettings.json file which can be found under src\Infogroup.IDMS.Web.Host path, open the file and ensure that the Redis key is set to the following value.
 
 
        "Redis": {
        "Host": "localhost",
        "Port": 6379,
        "ExpirationInHours": 6,
        "SlidingExpirationInHours": 3,
        "DatabaseId": -1
      }