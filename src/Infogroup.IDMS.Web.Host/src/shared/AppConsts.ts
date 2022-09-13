export class AppConsts {
  static readonly tenancyNamePlaceHolderInUrl = "{TENANCY_NAME}";

  static remoteServiceBaseUrl: string;
  static remoteServiceBaseUrlFormat: string;
  static appBaseUrl: string;
  static appBaseHref: string; // returns angular's base-href parameter value if used during the publish
  static appBaseUrlFormat: string;
  static recaptchaSiteKey: string;
  static googleMapsApiKey: string;
  static subscriptionExpireNootifyDayCount: number;
  static instrumentationKey: string;
  static divisionalShipToHelpUrl: string =
    "/assets/Help/IDMS SHIP TO Ftp Server Setup.pdf";

  static localeMappings: any = [];

  static readonly userManagement = {
    defaultAdminUserName: "admin",
  };

  static readonly localization = {
    defaultLocalizationSourceName: "IDMS",
  };

  static readonly authorization = {
    encrptedAuthTokenName: "enc_auth_token",
  };

  static readonly grid = {
    defaultPageSize: 10,
  };
}
